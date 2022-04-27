using System;
using System.Collections.Generic;
using DataFixerUpper.DataFixers;
using DataFixerUpper.DataFixers.Util;
using DataFixerUpper.Serialization.Codecs;
using DataFixerUpper.Util;

namespace DataFixerUpper.Serialization{
    public interface ICodec<A> : IEncoder<A>, IDecoder<A>{}

    public static class CodecExtensions{
        /*
         * Codec extension methods
         */
        public static ICodec<A> WithLifecycle<A>(this ICodec<A> codec, Lifecycle lifecycle){
            return new LifecycleWrapper<A>(codec, lifecycle);
        }

        public static ICodec<A> Stable<A>(this ICodec<A> codec){
            return WithLifecycle(codec, Lifecycle.Stable());
        }

        public static ICodec<A> Deprecated<A>(this ICodec<A> codec, int since){
            return WithLifecycle(codec, Lifecycle.DeprecatedSince(since));
        }
        
        public static ICodec<IList<A>> ListOf<A>(this ICodec<A> codec){
            return Codec.List(codec);
        }

        public static ICodec<S> Xmap<A, S>(this ICodec<A> codec, Func<A, S> to, Func<S, A> from){
            return Codec.Of(codec.Comap(from), codec.Map(to), codec.ToString() + "[xmapped]");
        }

        public static ICodec<S> ComapFlatMap<A, S>(this ICodec<A> codec, Func<A, DataResult<S>> to, Func<S, A> from){
            return Codec.Of(codec.Comap(from), codec.FlatMap(to), codec.ToString() + "[comapFlatMapped]");
        }

        public static ICodec<S> FlatComapMap<A, S>(this ICodec<A> codec, Func<A, S> to, Func<S, DataResult<A>> from){
            return Codec.Of(codec.FlatComap(from), codec.Map(to), codec.ToString() + "[flatComapMapped]");
        }

        public static ICodec<S> FlatXmap<A, S>(this ICodec<A> codec, Func<A, DataResult<S>> to, Func<S, DataResult<A>> from){
            return Codec.Of(codec.FlatComap(from), codec.FlatMap(to), codec.ToString() + "[flatXmapped]");
        }

        public static MapCodec<A> FieldOf<A>(this ICodec<A> codec, string name){
            return MapCodec.Of(
                Encoder.FieldOfUnchecked(codec, name),
                Decoder.FieldOfUnchecked(codec, name),
                () => $"Field[{name}: {codec}]"
            );
        }

        public static MapCodec<Optional<A>> OptionalFieldOf<A>(this ICodec<A> codec, string name){
            return Codec.OptionalField(name, codec);
        }

        public static MapCodec<A> OptionalFieldOf<A>(this ICodec<A> codec, string name, A defaultValue){
            return Codec.OptionalField(name, codec).Xmap(
                o => o.OrElse(defaultValue),
                a => ObjectUtils.Equals(a, defaultValue) ? Optional<A>.Empty() : Optional<A>.Of(a)
            );
        }

        public static MapCodec<A> OptionalFieldOf<A>(this ICodec<A> codec, string name, A defaultValue, Lifecycle lifecycleOfDefault){
            return codec.OptionalFieldOf(name, Lifecycle.Experimental(), defaultValue, lifecycleOfDefault);
        }

        public static MapCodec<A> OptionalFieldOf<A>(this ICodec<A> codec, string name, Lifecycle fieldLifecycle, A defaultValue, Lifecycle lifecycleOfDefault){
            return Codec.OptionalField(name, codec).Stable().FlatXmap(
                o => o.Map(v => DataResult.Success(v, fieldLifecycle))
                    .OrElse(DataResult.Success(defaultValue, lifecycleOfDefault)),
                a => {
                    if(ObjectUtils.Equals(a, defaultValue)){
                        return DataResult.Success(Optional<A>.Empty(), lifecycleOfDefault);
                    }
                    return DataResult.Success(Optional<A>.Of(a), fieldLifecycle);
                }
            );
        }

        public static ICodec<A> MapResult<A>(this ICodec<A> codec, IResultFunction<A> function){
            return new ResultFuncWrapper<A>(codec, function);
        }

        public static ICodec<A> OrElse<A>(this ICodec<A> codec, Action<string> onError, A value){
            return codec.OrElse(DataFixUtils.ConsumerToFunction(onError), value);
        }

        public static ICodec<A> OrElse<A>(this ICodec<A> codec, Func<string, string> onError, A value){
            return codec.MapResult(new OrElseResultFunc<A>(value, onError));
        }

        public static ICodec<A> OrElse<A>(this ICodec<A> codec, A value){
            return codec.MapResult(new OrElseResultFunc<A>(value));
        }

        public static ICodec<A> OrElseGet<A>(this ICodec<A> codec, Action<string> onError, Func<A> value){
            return codec.OrElseGet(DataFixUtils.ConsumerToFunction(onError), value);
        }

        public static ICodec<A> OrElseGet<A>(this ICodec<A> codec, Func<string, string> onError, Func<A> value){
            return codec.MapResult(new OrElseResultFuncDeferred<A>(value, onError));
        }

        public static ICodec<A> OrElseGet<A>(this ICodec<A> codec, Func<A> value){
            return codec.MapResult(new OrElseResultFuncDeferred<A>(value));
        }

        public static ICodec<A> PromotePartial<A>(this ICodec<A> codec, Action<string> onError){
            return Codec.Of(codec, codec.PromotePartial(onError));
        }

        public static ICodec<E> Dispatch<A, E>(this ICodec<A> thisCodec, Func<E, A> type, Func<A, ICodec<E>> codec){
            return thisCodec.Dispatch("type", type, codec);
        }

        public static ICodec<E> Dispatch<A, E>(this ICodec<A> thisCodec, string typeKey, Func<E, A> type, Func<A, ICodec<E>> codec){
            return thisCodec.PartialDispatch(typeKey, type.AndThen(DataResult.Success), codec.AndThen(DataResult.Success));
        }

        public static ICodec<E> DispatchStable<A, E>(this ICodec<A> thisCodec, Func<E, A> type, Func<A, ICodec<E>> codec){
            return thisCodec.PartialDispatch("type",
                e => DataResult.Success(type.Invoke(e), Lifecycle.Stable()),
                a => DataResult.Success(codec.Invoke(a), Lifecycle.Stable())
            );
        }

        public static ICodec<E> PartialDispatch<A, E>(this ICodec<A> thisCodec, string typeKey, Func<E, DataResult<A>> type, Func<A, DataResult<ICodec<E>>> codec){
            return new KeyDispatchCodec<A, E>(typeKey, thisCodec, type, codec).AsCodec();
        }

        public static MapCodec<E> DispatchMap<A, E>(this ICodec<A> thisCodec, Func<E, A> type, Func<A, ICodec<E>> codec){
            return thisCodec.DispatchMap("type", type, codec);
        }
        
        public static MapCodec<E> DispatchMap<A, E>(this ICodec<A> thisCodec, string typeKey, Func<E, A> type, Func<A, ICodec<E>> codec){
            return new KeyDispatchCodec<A, E>(typeKey, thisCodec, type.AndThen(DataResult.Success), codec.AndThen(DataResult.Success));
        }

        public static ICodec<E> Cast<A, E>(this ICodec<A> thisCodec) where A : E{
            return thisCodec.Xmap(a => (E)a, e => (A)e);
        }


        /*
         * Nested types
         */
        public interface IResultFunction<A>{
            DataResult<Pair<A, T>> Apply<T>(DynamicOps<T> ops, T input, DataResult<Pair<A, T>> a);

            DataResult<T> CoApply<T>(DynamicOps<T> ops, A input, DataResult<T> t);
        }

        private sealed class LifecycleWrapper<A> : ICodec<A>{
            /*
             * Fields
             */
            private readonly ICodec<A> wrapped;
            private readonly Lifecycle lifecycle;


            /*
             * Constructor
             */
            public LifecycleWrapper(ICodec<A> toWrap, Lifecycle lifecycleIn){
                wrapped = toWrap;
                lifecycle = lifecycleIn;
            }


            /*
             * ICodec implementation
             */
            public DataResult<Pair<A, T>> Decode<T>(DynamicOps<T> ops, T input){
                return wrapped.Decode(ops, input).SetLifecycle(lifecycle);
            }

            public DataResult<T> Encode<T>(A input, DynamicOps<T> ops, T prefix){
                return wrapped.Encode(input, ops, prefix).SetLifecycle(lifecycle);
            }
        }

        private sealed class ResultFuncWrapper<A> : ICodec<A>{
            /*
             * Fields
             */
            private readonly ICodec<A> wrapped;
            private readonly IResultFunction<A> function;


            /*
             * Constructor
             */
            public ResultFuncWrapper(ICodec<A> toWrap, IResultFunction<A> functionIn){
                wrapped = toWrap;
                function = functionIn;
            }


            /*
             * ICodec implementation
             */
            public DataResult<T> Encode<T>(A input, DynamicOps<T> ops, T prefix){
                return function.CoApply(ops, input, wrapped.Encode(input, ops, prefix));
            }

            public DataResult<Pair<A, T>> Decode<T>(DynamicOps<T> ops, T input){
                return function.Apply(ops, input, wrapped.Decode(ops, input));
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return $"{wrapped}[mapResult {function}]";
            }
        }

        private sealed class OrElseResultFunc<A> : IResultFunction<A>{
            /*
             * Fields
             */
            private readonly A value;
            private readonly Func<string, string> onError;


            /*
             * Constructor
             */
            public OrElseResultFunc(A valueIn, Func<string, string> onErrorIn){
                value = valueIn;
                onError = onErrorIn;
            }

            public OrElseResultFunc(A valueIn)
                :this(valueIn, null){}


            /*
             * IResultFunction implementation
             */
            public DataResult<Pair<A, T>> Apply<T>(DynamicOps<T> ops, T input, DataResult<Pair<A, T>> a){
                if(onError != null){
                    return DataResult.Success(a.MapError(onError).Result().OrElseGet(() => Pair.Of(value, input)));
                }
                return DataResult.Success(a.Result().OrElseGet(() => Pair.Of(value, input)));
            }

            public DataResult<T> CoApply<T>(DynamicOps<T> ops, A input, DataResult<T> t){
                if(onError != null){
                    return t.MapError(onError);
                }
                return t;
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                if(onError != null){
                    return $"OrElse[{onError} {value}]";
                }
                return $"OrElse[{value}]";
            }
        }

        private sealed class OrElseResultFuncDeferred<A> : IResultFunction<A>{
            /*
             * Fields
             */
            private readonly Func<A> value;
            private readonly Func<string, string> onError;


            /*
             * Constructor
             */
            public OrElseResultFuncDeferred(Func<A> valueIn, Func<string, string> onErrorIn){
                value = valueIn;
                onError = onErrorIn;
            }

            public OrElseResultFuncDeferred(Func<A> valueIn)
                :this(valueIn, null){}


            /*
             * IResultFunction implementation
             */
            public DataResult<Pair<A, T>> Apply<T>(DynamicOps<T> ops, T input, DataResult<Pair<A, T>> a){
                if(onError != null){
                    return DataResult.Success(a.MapError(onError).Result().OrElseGet(() => Pair.Of(value.Invoke(), input)));
                }
                return DataResult.Success(a.Result().OrElseGet(() => Pair.Of(value.Invoke(), input)));
            }

            public DataResult<T> CoApply<T>(DynamicOps<T> ops, A input, DataResult<T> t){
                if(onError != null){
                    return t.MapError(onError);
                }
                return t;
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                if(onError != null){
                    return $"OrElse[{onError} {value.Invoke()}]";
                }
                return $"OrElse[{value.Invoke()}]";
            }
        }
    }
}
