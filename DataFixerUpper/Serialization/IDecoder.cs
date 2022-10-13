using System;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.DataFixers.Util;
using DataFixerUpper.Serialization.Codecs;

namespace DataFixerUpper.Serialization{
    public interface IDecoder<A>{
        DataResult<Pair<A, T>> Decode<T>(DynamicOps<T> ops, T input);
    }

    public static class Decoder{
        /*
         * Static methods
         */
        //OfTerminal

        //OfBoxed

        //OfSimple

        public static IMapDecoder<A> Unit<A>(A instance){
            return Unit(() => instance);
        }

        public static IMapDecoder<A> Unit<A>(Func<A> instance){
            return new UnitDecoder<A>(instance);
        }

        public static IDecoder<A> Error<A>(string error){
            return new ErrorDecoder<A>(error);
        }

        internal static IMapDecoder<A> FieldOfUnchecked<A>(IDecoder<A> decoder, string name){
            return new FieldDecoder<A>(name, decoder);
        }


        /*
         * IDecoder extension methods
         */
        public static DataResult<A> Parse<A, T>(this IDecoder<A> decoder, DynamicOps<T> ops, T input){
            return decoder.Decode(ops, input).Map(p => p.GetFirst());
        }

        public static DataResult<Pair<A, T>> Decode<A, T>(this IDecoder<A> decoder, Dynamic<T> input){
            return decoder.Decode(input.GetOps(), input.GetValue());
        }

        public static DataResult<A> Parse<A, T>(this IDecoder<A> decoder, Dynamic<T> input){
            return decoder.Decode(input).Map(p => p.GetFirst());
        }

        //Terminal

        //Boxed

        //Simple

        public static IMapDecoder<A> FieldOf<A>(this IDecoder<A> decoder, string name){
            if(decoder is ICodec<A> codec){
                return codec.FieldOf(name);
            }
            return FieldOfUnchecked(decoder, name);
        }

        public static IDecoder<B> FlatMap<A, B>(this IDecoder<A> decoder, Func<A, DataResult<B>> function){
            return new FlatMapWrapper<A, B>(decoder, function);
        }

        public static IDecoder<B> Map<A, B>(this IDecoder<A> decoder, Func<A, B> function){
            return new MapWrapper<A, B>(decoder, function);
        }

        public static IDecoder<A> PromotePartial<A>(this IDecoder<A> decoder, Action<string> onError){
            if(decoder is ICodec<A> codec){
                return codec.PromotePartial(onError);
            }
            return new PromotePartialWrapper<A>(decoder, onError);
        }

        public static IDecoder<A> WithLifecycle<A>(this IDecoder<A> decoder, Lifecycle lifecycle){
            if(decoder is ICodec<A> codec){
                return codec.WithLifecycle(lifecycle);
            }
            return new LifecycleWrapper<A>(decoder, lifecycle);
        }


        /*
         * Nested types
         */
        private sealed class UnitDecoder<A> : MapDecoder.Implementation<A>{
            /*
             * Fields
             */
            private readonly Func<A> instance;


            /*
             * Constructor
             */
            public UnitDecoder(Func<A> instanceIn){
                instance = instanceIn;
            }


            /*
             * MapDecoder.Implementation override methods
             */
            public override DataResult<A> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
                return DataResult.Success(instance.Invoke());
            }
            
            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return Enumerable.Empty<T>();
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return $"UnitDecoder[{instance.Invoke()}]";
            }
        }

        private sealed class ErrorDecoder<A> : IDecoder<A>{
            /*
             * Fields
             */
            private readonly string error;


            /*
             * Constructor
             */
            public ErrorDecoder(string errorIn){
                error = errorIn;
            }


            /*
             * IDecoder implementation
             */
            public DataResult<Pair<A, T>> Decode<T>(DynamicOps<T> ops, T input){
                return DataResult.Error<Pair<A, T>>(error);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return $"ErrorDecoder[{error}]";
            }
        }

        private sealed class FlatMapWrapper<A, B> : IDecoder<B>{
            /*
             * Fields
             */
            public readonly IDecoder<A> wrapped;
            public readonly Func<A, DataResult<B>> function;


            /*
             * Constructor
             */
            public FlatMapWrapper(IDecoder<A> toWrap, Func<A, DataResult<B>> functionIn){
                wrapped = toWrap;
                function = functionIn;
            }


            /*
             * IDecoder implementation
             */
            public DataResult<Pair<B, T>> Decode<T>(DynamicOps<T> ops, T input){
                return wrapped.Decode(ops, input).FlatMap(p => function(p.GetFirst()).Map(r => Pair.Of(r, p.GetSecond())));
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return wrapped.ToString() + "[flatMapped]";
            }
        }

        private sealed class MapWrapper<A, B> : IDecoder<B>{
            /*
             * Fields
             */
            public readonly IDecoder<A> wrapped;
            public readonly Func<A, B> function;


            /*
             * Constructor
             */
            public MapWrapper(IDecoder<A> toWrap, Func<A, B> functionIn){
                wrapped = toWrap;
                function = functionIn;
            }


            /*
             * IDecoder implementation
             */
            public DataResult<Pair<B, T>> Decode<T>(DynamicOps<T> ops, T input){
                return wrapped.Decode(ops, input).Map(p => p.MapFirst(function));
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return wrapped.ToString() + "[flatMapped]";
            }
        }

        private sealed class PromotePartialWrapper<A> : IDecoder<A>{
            /*
             * Fields
             */
            public readonly IDecoder<A> wrapped;
            public readonly Action<string> onError;


            /*
             * Constructor
             */
            public PromotePartialWrapper(IDecoder<A> toWrap, Action<string> onErrorIn){
                wrapped = toWrap;
                onError = onErrorIn;
            }


            /*
             * IDecoder implementation
             */
            public DataResult<Pair<A, T>> Decode<T>(DynamicOps<T> ops, T input){
                return wrapped.Decode(ops, input).PromotePartial(onError);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return wrapped.ToString() + "[promotePartial]";
            }
        }

        private sealed class LifecycleWrapper<A> : IDecoder<A>{
            /*
             * Fields
             */
            public readonly IDecoder<A> wrapped;
            public readonly Lifecycle lifecycle;


            /*
             * Constructor
             */
            public LifecycleWrapper(IDecoder<A> toWrap, Lifecycle lifecycleIn){
                wrapped = toWrap;
                lifecycle = lifecycleIn;
            }


            /*
             * IDecoder implementation
             */
            public DataResult<Pair<A, T>> Decode<T>(DynamicOps<T> ops, T input){
                return wrapped.Decode(ops, input).SetLifecycle(lifecycle);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return wrapped.ToString() + "[flatMapped]";
            }
        }
    }
}
