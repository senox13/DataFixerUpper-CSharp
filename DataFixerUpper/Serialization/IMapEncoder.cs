using System;
using System.Collections.Generic;

namespace DataFixerUpper.Serialization{
    public interface IMapEncoder<A> : IKeyable{
        RecordBuilder<T> Encode<T>(A input, DynamicOps<T> ops, RecordBuilder<T> prefix);

        KeyCompressor<T> Compressor<T>(DynamicOps<T> ops);
    }

    public static class MapEncoder{
        /*
         * Static methods
         */
        public static RecordBuilder<T> MakeCompressedBuilder<T>(DynamicOps<T> ops, KeyCompressor<T> compressor){
            return new CompressedRecordBuilder<T>(ops, compressor);
        }


        /*
         * Extension methods
         */
        public static RecordBuilder<T> CompressedBuilder<A, T>(this IMapEncoder<A> encoder, DynamicOps<T> ops){
            if(ops.CompressMaps()){
                return MakeCompressedBuilder(ops, encoder.Compressor(ops));
            }
            return ops.MapBuilder();
        }

        public static IMapEncoder<B> Comap<A, B>(this IMapEncoder<A> encoder, Func<B, A> function){
            return new ComapWrapper<A, B>(encoder, function);
        }

        public static IMapEncoder<B> FlatComap<A, B>(this IMapEncoder<A> encoder, Func<B, DataResult<A>> function){
            return new FlatComapWrapper<A, B>(encoder, function);
        }

        public static IEncoder<A> Encoder<A>(this IMapEncoder<A> encoder){
            return new EncoderWrapper<A>(encoder);
        }

        public static IMapEncoder<A> WithLifecycle<A>(this IMapEncoder<A> encoder, Lifecycle lifecycle){
            if(encoder is MapCodec<A> codec){
                return codec.WithLifecycle(lifecycle);
            }
            return new LifecycleWrapper<A>(encoder, lifecycle);
        }


        /*
         * Nested types
         */
        public abstract class Implementation<A> : CompressorHolder, IMapEncoder<A>{
            /*
             * Abstract IMapEncoder implementation
             */
            public abstract RecordBuilder<T> Encode<T>(A input, DynamicOps<T> ops, RecordBuilder<T> prefix);
        }

        private sealed class CompressedRecordBuilder<T> : RecordBuilder.AbstractUniversalBuilder<T, IList<T>>{
            /*
             * Fields
             */
            private readonly KeyCompressor<T> compressor;


            /*
             * Constructor
             */
            public CompressedRecordBuilder(DynamicOps<T> opsIn, KeyCompressor<T> compressorIn)
                :base(opsIn){
                compressor = compressorIn;
                builder = DataResult.Success(InitBuilder(), Lifecycle.Stable());
            }


            /*
             * AbstractUniversalBuilder override methods
             */
            protected override IList<T> InitBuilder(){
                List<T> list = new List<T>(compressor.Size());
                for(int i=0; i<compressor.Size(); i++){
                    list.Add(default);
                }
                return list;
            }

            protected override IList<T> Append(T key, T value, IList<T> builder){
                builder[compressor.Compress(key)] = value;
                return builder;
            }

            protected override DataResult<T> Build(IList<T> builder, T prefix){
                return Ops().MergeToList(prefix, builder);
            }
        }

        private sealed class ComapWrapper<A, B> : Implementation<B>{
            /*
             * Fields
             */
            private readonly IMapEncoder<A> encoder;
            private readonly Func<B, A> function;


            /*
             * Constructor
             */
            public ComapWrapper(IMapEncoder<A> encoderIn, Func<B, A> functionIn){
                encoder = encoderIn;
                function = functionIn;
            }


            /*
             * Implementation override methods
             */
            public override RecordBuilder<T> Encode<T>(B input, DynamicOps<T> ops, RecordBuilder<T> prefix){
                return encoder.Encode(function(input), ops, prefix);
            }

            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return encoder.Keys(ops);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return encoder.ToString() + "[comapped]";
            }
        }

        private sealed class FlatComapWrapper<A, B> : Implementation<B>{
            /*
             * Fields
             */
            private readonly IMapEncoder<A> encoder;
            private readonly Func<B, DataResult<A>> function;


            /*
             * Constructor
             */
            public FlatComapWrapper(IMapEncoder<A> encoderIn, Func<B, DataResult<A>> functionIn){
                encoder = encoderIn;
                function = functionIn;
            }


            /*
             * Implementation override methods
             */
            public override RecordBuilder<T> Encode<T>(B input, DynamicOps<T> ops, RecordBuilder<T> prefix){
                DataResult<A> aResult = function.Invoke(input);
                RecordBuilder<T> builder = prefix.WithErrorsFrom(aResult);
                return aResult.Map(r => encoder.Encode(r, ops, builder)).Result().OrElse(builder);
            }

            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return encoder.Keys(ops);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return encoder.ToString() + "[flatComapped]";
            }
        }

        private sealed class EncoderWrapper<A> : IEncoder<A>{
            /*
             * Fields
             */
            private readonly IMapEncoder<A> encoder;


            /*
             * Constructor
             */
            public EncoderWrapper(IMapEncoder<A> encoderIn){
                encoder = encoderIn;
            }


            /*
             * IEncoder implementation
             */
            public DataResult<T> Encode<T>(A input, DynamicOps<T> ops, T prefix){
                return encoder.Encode(input, ops, encoder.CompressedBuilder(ops)).Build(prefix);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return encoder.ToString();
            }
        }

        private sealed class LifecycleWrapper<A> : Implementation<A>{
            /*
             * Fields
             */
            private readonly IMapEncoder<A> encoder;
            private readonly Lifecycle lifecycle;


            /*
             * Constructor
             */
            public LifecycleWrapper(IMapEncoder<A> encoderIn, Lifecycle lifecycleIn){
                encoder = encoderIn;
                lifecycle = lifecycleIn;
            }


            /*
             * Implementation override methods
             */
            public override RecordBuilder<T> Encode<T>(A input, DynamicOps<T> ops, RecordBuilder<T> prefix){
                return encoder.Encode(input, ops, prefix).SetLifecycle(lifecycle);
            }

            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return encoder.Keys(ops);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return encoder.ToString();
            }
        }
    }
}
