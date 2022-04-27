using System;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.DataFixers;
using DataFixerUpper.DataFixers.Util;
using DataFixerUpper.Serialization.Codecs;

namespace DataFixerUpper.Serialization {
    public abstract class MapCodec<A> : CompressorHolder, IMapDecoder<A>, IMapEncoder<A>{
        /*
         * Abstract interface implementations
         */
        public abstract DataResult<A> Decode<T>(DynamicOps<T> ops, IMapLike<T> input);

        public abstract RecordBuilder<T> Encode<T>(A input, DynamicOps<T> ops, RecordBuilder<T> prefix);


        /*
         * Concrete instance methods
         */
        public RecordCodecBuilder<O, A> ForGetter<O>(Func<O, A> getter){
            return RecordCodecBuilder.Of(getter, this);
        }

        public MapCodec<A> FieldOf(string name){
            return AsCodec().FieldOf(name);
        }

        public MapCodec<A> WithLifecycle(Lifecycle lifecycle){
            return new LifecycleWrapper(this, lifecycle);
        }

        public MapCodec<A> Stable(){
            return WithLifecycle(Lifecycle.Stable());
        }

        public MapCodec<A> Deprecated(int since){
            return WithLifecycle(Lifecycle.DeprecatedSince(since));
        }

        public ICodec<A> AsCodec(){
            return new MapCodec.MapCodecCodec<A>(this);
        }

        public MapCodec<S> Xmap<S>(Func<A, S> to, Func<S, A> from){
            return MapCodec.Of(this.Comap(from), this.Map(to), () => ToString() + "[xmapped]");
        }

        public MapCodec<S> FlatXmap<S>(Func<A, DataResult<S>> to, Func<S, DataResult<A>> from){
            return Codec.Of(this.FlatComap(from), this.FlatMap(to), () => ToString() + "[flatXmapped]");
        }

        public MapCodec<A> Dependant<E>(MapCodec<E> initialInstance, Func<A, Pair<E, MapCodec<E>>> splitter, Func<A, E, A> combiner){
            return new MapCodec.Dependent<A, E>(this, initialInstance, splitter, combiner);
        }

        public MapCodec<A> MapResult(MapCodec.IResultFunction<A> function){
            return new MapResultWrapper(this, function);
        }

        public MapCodec<A> OrElse(Action<string> onError, A value){
            return OrElse(DataFixUtils.ConsumerToFunction(onError), value);
        }

        public MapCodec<A> OrElse(Func<string, string> onError, A value){
            return MapResult(new ResultFunc(value, onError));
        }

        public MapCodec<A> OrElse(A value){
            return MapResult(new ResultFunc(value));
        }

        public MapCodec<A> OrElseGet(Action<string> onError, Func<A> value){
            return OrElseGet(DataFixUtils.ConsumerToFunction(onError), value);
        }

        public MapCodec<A> OrElseGet(Func<string, string> onError, Func<A> value){
            return MapResult(new ResultFuncDeferred(value, onError));
        }

        public MapCodec<A> OrElseGet(Func<A> value){
            return MapResult(new ResultFuncDeferred(value));
        }

        public MapCodec<A> SetPartial(Func<A> value){
            return MapResult(new ResultFuncSetPartial(value));
        }


        /*
         * Nested types
         */
        private sealed class LifecycleWrapper : MapCodec<A>{
            /*
             * Fields
             */
            private readonly MapCodec<A> wrapped;
            private readonly Lifecycle lifecycle;


            /*
             * Constructor
             */
            public LifecycleWrapper(MapCodec<A> toWrap, Lifecycle lifecycleIn){
                wrapped = toWrap;
                lifecycle = lifecycleIn;
            }


            /*
             * MapCodec override methods
             */
            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return wrapped.Keys(ops);
            }

            public override DataResult<A> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
                return wrapped.Decode(ops, input).SetLifecycle(lifecycle);
            }

            public override RecordBuilder<T> Encode<T>(A input, DynamicOps<T> ops, RecordBuilder<T> prefix){
                return wrapped.Encode(input, ops, prefix).SetLifecycle(lifecycle);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return wrapped.ToString();
            }
        }

        private sealed class MapResultWrapper : MapCodec<A>{
            /*
             * Fields
             */
            private readonly MapCodec<A> wrapped;
            private readonly MapCodec.IResultFunction<A> function;


            /*
             * Constructor
             */
            public MapResultWrapper(MapCodec<A> toWrap, MapCodec.IResultFunction<A> functionIn){
                wrapped = toWrap;
                function = functionIn;
            }


            /*
             * MapCodec override methods
             */
            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return wrapped.Keys(ops);
            }

            public override RecordBuilder<T> Encode<T>(A input, DynamicOps<T> ops, RecordBuilder<T> prefix){
                return function.CoApply(ops, input, wrapped.Encode(input, ops, prefix));
            }

            public override DataResult<A> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
                return function.Apply(ops, input, wrapped.Decode(ops, input));
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return $"{wrapped}[mapResult {function}]";
            }
        }

        private sealed class ResultFunc : MapCodec.IResultFunction<A>{
            /*
             * Fields
             */
            private readonly A value;
            private readonly Func<string, string> onError;


            /*
             * Constructors
             */
            public ResultFunc(A valueIn, Func<string, string> onErrorIn){
                value = valueIn;
                onError = onErrorIn;
            }

            public ResultFunc(A valueIn)
                :this(valueIn, null){}


            /*
             * IResultFunction implementation
             */
            public DataResult<A> Apply<T>(DynamicOps<T> ops, IMapLike<T> input, DataResult<A> a){
                if(onError != null){
                    return DataResult.Success(a.MapError(onError).Result().OrElse(value));
                }
                return DataResult.Success(a.Result().OrElse(value));
            }

            public RecordBuilder<T> CoApply<T>(DynamicOps<T> ops, A input, RecordBuilder<T> t){
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

        private sealed class ResultFuncDeferred : MapCodec.IResultFunction<A>{
            /*
             * Fields
             */
            private readonly Func<A> value;
            private readonly Func<string, string> onError;


            /*
             * Constructors
             */
            public ResultFuncDeferred(Func<A> valueIn, Func<string, string> onErrorIn){
                value = valueIn;
                onError = onErrorIn;
            }

            public ResultFuncDeferred(Func<A> valueIn)
                :this(valueIn, null){}


            /*
             * IResultFunction implementation
             */
            public DataResult<A> Apply<T>(DynamicOps<T> ops, IMapLike<T> input, DataResult<A> a){
                if(onError != null){
                    return DataResult.Success(a.MapError(onError).Result().OrElseGet(value));
                }
                return DataResult.Success(a.Result().OrElseGet(value));
            }

            public RecordBuilder<T> CoApply<T>(DynamicOps<T> ops, A input, RecordBuilder<T> t){
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
                    return $"OrElseGet[{onError} {value.Invoke()}]";
                }
                return $"OrElseGet[{value.Invoke()}]";
            }
        }

        private sealed class ResultFuncSetPartial : MapCodec.IResultFunction<A>{
            /*
             * Fields
             */
            private readonly Func<A> value;


            /*
             * Constructor
             */
            public ResultFuncSetPartial(Func<A> valueIn){
                value = valueIn;
            }


            /*
             * IResultFunction implementation
             */
            public DataResult<A> Apply<T>(DynamicOps<T> ops, IMapLike<T> input, DataResult<A> a){
                return a.SetPartial(value);
            }

            public RecordBuilder<T> CoApply<T>(DynamicOps<T> ops, A input, RecordBuilder<T> t){
                return t;
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return $"SetPartial[{value}]";
            }
        }
    }

    public static class MapCodec{
        /*
         * Static methods
         */
        public static MapCodec<A> Of<A>(IMapEncoder<A> encoder, IMapDecoder<A> decoder){
            return Of(encoder, decoder, () => $"MapCodec[{encoder} {decoder}]");
        }

        public static MapCodec<A> Of<A>(IMapEncoder<A> encoder, IMapDecoder<A> decoder, Func<string> name){
            return new EncoderDecoderCodecWrapper<A>(encoder, decoder, name);
        }

        public static MapCodec<A> Unit<A>(A defaultValue){
            return Unit(() => defaultValue);
        }

        public static MapCodec<A> Unit<A>(Func<A> defaultValue){
            return Of(Encoder.Empty<A>(), Decoder.Unit(defaultValue));
        }


        /*
         * Nested types
         */
        public sealed class MapCodecCodec<A> : ICodec<A>{
            /*
             * Fields
             */
            private readonly MapCodec<A> codec;


            /*
             * Constructor
             */
            public MapCodecCodec(MapCodec<A> codecIn){
                codec = codecIn;
            }


            /*
             * Instance methods
             */
            public MapCodec<A> Codec(){
                return codec;
            }


            /*
             * ICodec implementation
             */
            public DataResult<Pair<A, T>> Decode<T>(DynamicOps<T> ops, T input){
                return codec.CompressedDecode(ops, input).Map(r => Pair.Of(r, input));
            }

            public DataResult<T> Encode<T>(A input, DynamicOps<T> ops, T prefix){
                return codec.Encode(input, ops, codec.CompressedBuilder(ops)).Build(prefix);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return codec.ToString();
            }
        }

        public class Dependent<O, E> : MapCodec<O>{
            /*
             * Fields
             */
            private readonly MapCodec<E> initialInstance;
            private readonly Func<O, Pair<E, MapCodec<E>>> splitter;
            private readonly MapCodec<O> codec;
            private readonly Func<O, E, O> combiner;


            /*
             * Constructor
             */
            public Dependent(MapCodec<O> codecIn, MapCodec<E> initialInstanceIn, Func<O, Pair<E, MapCodec<E>>> splitterIn, Func<O, E, O> combinerIn){
                initialInstance = initialInstanceIn;
                splitter = splitterIn;
                codec = codecIn;
                combiner = combinerIn;
            }


            /*
             * MapCodec override methods
             */
            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return Enumerable.Concat(codec.Keys(ops), initialInstance.Keys(ops));
            }

            public override DataResult<O> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
                return codec.Decode(ops, input).FlatMap(o =>
                    splitter.Invoke(o).GetSecond().Decode(ops, input).Map(e => combiner.Invoke(o, e)).SetLifecycle(Lifecycle.Experimental())
                );
            }

            public override RecordBuilder<T> Encode<T>(O input, DynamicOps<T> ops, RecordBuilder<T> prefix){
                codec.Encode(input, ops, prefix);
                Pair<E, MapCodec<E>> e = splitter.Invoke(input);
                e.GetSecond().Encode(e.GetFirst(), ops, prefix);
                return prefix.SetLifecycle(Lifecycle.Experimental());
            }
        }

        public interface IResultFunction<A>{
            DataResult<A> Apply<T>(DynamicOps<T> ops, IMapLike<T> input, DataResult<A> a);

            RecordBuilder<T> CoApply<T>(DynamicOps<T> ops, A input, RecordBuilder<T> t);
        }

        private sealed class EncoderDecoderCodecWrapper<A> : MapCodec<A>{
            /*
             * Fields
             */
            private readonly IMapEncoder<A> encoder;
            private readonly IMapDecoder<A> decoder;
            private readonly Func<string> name;


            /*
             * Constructor
             */
            public EncoderDecoderCodecWrapper(IMapEncoder<A> encoderIn, IMapDecoder<A> decoderIn, Func<string> nameIn){
                encoder = encoderIn;
                decoder = decoderIn;
                name = nameIn;
            }


            /*
             * MapCodec override methods
             */
            public override DataResult<A> Decode<T>(DynamicOps<T> ops, IMapLike<T> input) {
                return decoder.Decode(ops, input);
            }

            public override RecordBuilder<T> Encode<T>(A input, DynamicOps<T> ops, RecordBuilder<T> prefix) {
                return encoder.Encode(input, ops, prefix);
            }

            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return Enumerable.Concat(encoder.Keys(ops), decoder.Keys(ops));
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return name.Invoke();
            }
        }
    }
}
