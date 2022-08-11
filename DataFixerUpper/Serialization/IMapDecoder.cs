using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.DataFixers.Util;
using JavaUtilities;

namespace DataFixerUpper.Serialization{
    public interface IMapDecoder<A> : IKeyable{
        DataResult<A> Decode<T>(DynamicOps<T> ops, IMapLike<T> input);

        KeyCompressor<T> Compressor<T>(DynamicOps<T> ops);
    }

    public static class MapDecoder{
        public static DataResult<A> CompressedDecode<A, T>(this IMapDecoder<A> decoder, DynamicOps<T> ops, T input){
            if(ops.CompressMaps()){
                Optional<Action<Action<T>>> inputList = ops.GetList(input).Result();
                if(!inputList.IsPresent()){
                    return DataResult.Error<A>("Input is not a list");
                }
                KeyCompressor<T> compressor = decoder.Compressor(ops);
                List<T> entries = new List<T>();
                inputList.Get().Invoke(entries.Add);
                return decoder.Decode(ops, new CompressedDecodeWrapper<T>(compressor, entries));
            }
            return ops.GetMap(input).SetLifecycle(Lifecycle.Stable())
                .FlatMap(map => decoder.Decode(ops, map));
        }

        public static IDecoder<A> Decoder<A>(this IMapDecoder<A> decoder){
            return new DecoderWrapper<A>(decoder);
        }

        public static IMapDecoder<B> FlatMap<A, B>(this IMapDecoder<A> decoder, Func<A, DataResult<B>> function){
            return new FlatMapWrapper<A, B>(decoder, function);
        }

        public static IMapDecoder<B> Map<A, B>(this IMapDecoder<A> decoder, Func<A, B> function){
            return new MapWrapper<A, B>(decoder, function);
        }

        public static IMapDecoder<E> Ap<A, E>(this IMapDecoder<A> decoder, IMapDecoder<Func<A, E>> decodeFunc){
            return new ApWrapper<A, E>(decoder, decodeFunc);
        }

        public static IMapDecoder<A> WithLifecycle<A>(this IMapDecoder<A> decoder, Lifecycle lifecycle){
            if(decoder is MapCodec<A> codec){
                return codec.WithLifecycle(lifecycle);
            }
            return new LifecycleWrapper<A>(decoder, lifecycle);
        }


        /*
         * Nested types
         */
        public abstract class Implementation<A> : CompressorHolder, IMapDecoder<A>{
            /*
             * Abstract IMapDecoder implementation
             */
            public abstract DataResult<A> Decode<T>(DynamicOps<T> ops, IMapLike<T> input);
        }

        private sealed class CompressedDecodeWrapper<T> : IMapLike<T>{
            /*
             * Fields
             */
            private readonly KeyCompressor<T> compressor;
            private readonly IList<T> entries;


            /*
             * Constructor
             */
            public CompressedDecodeWrapper(KeyCompressor<T> compressorIn, IList<T> entriesIn){
                compressor = compressorIn;
                entries = entriesIn;
            }


            /*
             * IMapLike implementation
             */
            public T Get(T key){
                int index = compressor.Compress(key);
                return index < entries.Count ? entries[index] : default;
            }
            
            public T Get(string key){
                int index = compressor.Compress(key);
                return index < entries.Count ? entries[index] : default;
            }

            public IEnumerator<Pair<T, T>> GetEnumerator(){
                return Enumerable.Range(0, entries.Count)
                    .Select(i => Pair.Of(compressor.Decompress(i), entries[i]))
                    .Where(p => p.GetSecond() != null).GetEnumerator();
            }
            
            IEnumerator IEnumerable.GetEnumerator(){
                return GetEnumerator();
            }
        }

        private sealed class DecoderWrapper<A> : IDecoder<A>{
            /*
             * Fields
             */
            private readonly IMapDecoder<A> decoder;


            /*
             * Constructor
             */
            public DecoderWrapper(IMapDecoder<A> decoderIn){
                decoder = decoderIn;
            }


            /*
             * IDecoder implementation
             */
            public DataResult<Pair<A, T>> Decode<T>(DynamicOps<T> ops, T input){
                return decoder.CompressedDecode(ops, input).Map(r => Pair.Of(r, input));
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return decoder.ToString();
            }
        }

        private sealed class FlatMapWrapper<A, B> : Implementation<B>{
            /*
             * Fields
             */
            private readonly IMapDecoder<A> decoder;
            private readonly Func<A, DataResult<B>> function;


            /*
             * Constructor
             */
            public FlatMapWrapper(IMapDecoder<A> decoderIn, Func<A, DataResult<B>> functionIn){
                decoder = decoderIn;
                function = functionIn;
            }


            /*
             * Implementation override methods
             */
            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return decoder.Keys(ops);
            }

            public override DataResult<B> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
                return decoder.Decode(ops, input).FlatMap(b => function.Invoke(b).Map(t => t));
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return decoder.ToString() + "[flatMapped]";
            }
        }

        private sealed class MapWrapper<A, B> : Implementation<B>{
            /*
             * Fields
             */
            private readonly IMapDecoder<A> decoder;
            private readonly Func<A, B> function;


            /*
             * Constructor
             */
            public MapWrapper(IMapDecoder<A> decoderIn, Func<A, B> functionIn){
                decoder = decoderIn;
                function = functionIn;
            }


            /*
             * Implementation override methods
             */
            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return decoder.Keys(ops);
            }

            public override DataResult<B> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
                return decoder.Decode(ops, input).Map(function);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return decoder.ToString() + "[mapped]";
            }
        }

        private sealed class ApWrapper<A, E> : Implementation<E>{
            /*
             * Fields
             */
            private readonly IMapDecoder<A> wrapped;
            private readonly IMapDecoder<Func<A, E>> decoder;


            /*
             * Constructor
             */
            public ApWrapper(IMapDecoder<A> wrappedIn, IMapDecoder<Func<A, E>> decoderIn){
                wrapped = wrappedIn;
                decoder = decoderIn;
            }


            /*
             * Implementation override methods
             */
            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return Enumerable.Concat(wrapped.Keys(ops), decoder.Keys(ops));
            }

            public override DataResult<E> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
                return wrapped.Decode(ops, input).FlatMap(f =>
                    decoder.Decode(ops, input).Map(e => e.Invoke(f))
                );
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return wrapped.ToString() + " * " + decoder.ToString();
            }
        }

        private sealed class LifecycleWrapper<A> : Implementation<A>{
            /*
             * Fields
             */
            private readonly IMapDecoder<A> decoder;
            private readonly Lifecycle lifecycle;


            /*
             * Constructor
             */
            public LifecycleWrapper(IMapDecoder<A> decoderIn, Lifecycle lifecycleIn){
                decoder = decoderIn;
                lifecycle = lifecycleIn;
            }


            /*
             * Implementation override methods
             */
            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return decoder.Keys(ops);
            }

            public override DataResult<A> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
                return decoder.Decode(ops, input).SetLifecycle(lifecycle);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return decoder.ToString();
            }
        }
    }
}
