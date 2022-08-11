using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JavaUtilities;
using DataFixerUpper.Serialization.Codecs;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.Serialization{
    public static class Codec{
        /*
         * Fields
         */
        public static readonly PrimitiveCodec<bool> BOOL = new BooleanCodec();
        public static readonly PrimitiveCodec<sbyte> BYTE = new ByteCodec();
        public static readonly PrimitiveCodec<short> SHORT = new ShortCodec();
        public static readonly PrimitiveCodec<int> INT = new IntCodec();
        public static readonly PrimitiveCodec<long> LONG = new LongCodec();
        public static readonly PrimitiveCodec<float> FLOAT = new FloatCodec();
        public static readonly PrimitiveCodec<double> DOUBLE = new DoubleCodec();
        public static readonly PrimitiveCodec<string> STRING = new StringCodec();
        public static readonly PrimitiveCodec<MemoryStream> MEMORY_STREAM = new MemoryStreamCodec();
        public static readonly PrimitiveCodec<IEnumerable<int>> INT_ENUMERABLE = new IntStreamCodec();
        public static readonly PrimitiveCodec<IEnumerable<long>> LONG_ENUMERABLE = new LongStreamCodec();
        //PENDINGIMPL: PASSTHROUGH omitted until Dynamic is added
        public static readonly MapCodec<Unit> EMPTY = MapCodec.Of(Encoder.Empty<Unit>(), Decoder.Unit(DataFixers.Util.Unit.INSTANCE));


        /*
         * Static methods
         */
        public static ICodec<A> Of<A>(IEncoder<A> encoder, IDecoder<A> decoder){
            return Of(encoder, decoder, $"Codec[{encoder} {decoder}]");
        }

        public static ICodec<A> Of<A>(IEncoder<A> encoder, IDecoder<A> decoder, string name){
            return new EncoderDecoderWrapper<A>(encoder, decoder, name);
        }

        public static MapCodec<A> Of<A>(IMapEncoder<A> encoder, IMapDecoder<A> decoder){
            return Of(encoder, decoder, () => $"MapCodec[{encoder} {decoder}]");
        }

        public static MapCodec<A> Of<A>(IMapEncoder<A> encoder, IMapDecoder<A> decoder, Func<string> name){
            return new MapEncoderDecoderWrapper<A>(encoder, decoder, name);
        }

        public static ICodec<Pair<F, S>> Pair<F, S>(ICodec<F> first, ICodec<S> second){
            return new PairCodec<F, S>(first, second);
        }

        public static ICodec<Either<F, S>> Either<F, S>(ICodec<F> first, ICodec<S> second){
            return new EitherCodec<F, S>(first, second);
        }

        public static MapCodec<Pair<F, S>> MapPair<F, S>(MapCodec<F> first, MapCodec<S> second){
            return new PairMapCodec<F, S>(first, second);
        }

        public static MapCodec<Either<F, S>> MapEither<F, S>(MapCodec<F> first, MapCodec<S> second){
            return new EitherMapCodec<F, S>(first, second);
        }

        public static ICodec<IList<A>> List<A>(ICodec<A> elementCodec){
            return new ListCodec<A>(elementCodec);
        }

        public static ICodec<IList<Pair<K, V>>> CompoundList<K, V>(ICodec<K> keyCodec, ICodec<V> elementCodec){
            return new CompoundListCodec<K, V>(keyCodec, elementCodec);
        }

        public static SimpleMapCodec<K, V> SimpleMap<K, V>(ICodec<K> keyCodec, ICodec<V> elementCodec, IKeyable keys){
            return new SimpleMapCodec<K, V>(keyCodec, elementCodec, keys);
        }

        public static UnboundedMapCodec<K, V> UnboundedMap<K, V>(ICodec<K> keyCodec, ICodec<V> elementCodec){
            return new UnboundedMapCodec<K, V>(keyCodec, elementCodec);
        }

        public static MapCodec<Optional<F>> OptionalField<F>(string name, ICodec<F> elementCodec){
            return new OptionalFieldCodec<F>(name, elementCodec);
        }

        private static Func<T, DataResult<T>> CheckRange<T>(T minInclusive, T maxInclusive) where T : IComparable<T>{
            return value => {
                if(value.CompareTo(minInclusive) >= 0 && value.CompareTo(maxInclusive) <= 0){
                    return DataResult.Success(value);
                }
                return DataResult.Error($"Value {value} outside of range [{minInclusive}:{maxInclusive}]", value);
            };
        }

        public static ICodec<A> Unit<A>(A defaultValue){
            return Unit(() => defaultValue);
        }

        public static ICodec<A> Unit<A>(Func<A> defaultValue){
            return MapCodec.Unit(defaultValue).AsCodec();
        }

        public static ICodec<int> IntRange(int minInclusive, int maxInclusive){
            Func<int, DataResult<int>> checker = CheckRange(minInclusive, maxInclusive);
            return INT.FlatXmap(checker, checker);
        }

        public static ICodec<float> FloatRange(float minInclusive, float maxInclusive){
            Func<float, DataResult<float>> checker = CheckRange(minInclusive, maxInclusive);
            return FLOAT.FlatXmap(checker, checker);
        }
        
        public static ICodec<double> DoubleRange(double minInclusive, double maxInclusive){
            Func<double, DataResult<double>> checker = CheckRange(minInclusive, maxInclusive);
            return DOUBLE.FlatXmap(checker, checker);
        }


        /*
         * Nested types
         */
        private sealed class BooleanCodec : PrimitiveCodec<bool>{
            /*
             * Constructor
             */
            public BooleanCodec() : base("Bool"){}


            /*
             * PrimitiveCodec overrides
             */
            public override DataResult<bool> Read<T>(DynamicOps<T> ops, T input){
                return ops.GetBooleanValue(input);
            }

            public override T Write<T>(DynamicOps<T> ops, bool value){
                return ops.CreateBoolean(value);
            }
        }

        private sealed class ByteCodec : PrimitiveCodec<sbyte>{
            /*
             * Constructor
             */
            public ByteCodec() : base("Byte"){}

            
            /*
             * PrimitiveCodec overrides
             */
            public override DataResult<sbyte> Read<T>(DynamicOps<T> ops, T input){
                return ops.GetNumberValue(input).Map(decimal.ToSByte);
            }
            
            public override T Write<T>(DynamicOps<T> ops, sbyte value){
                return ops.CreateByte(value);
            }
        }

        private sealed class ShortCodec : PrimitiveCodec<short>{
            /*
             * Constructor
             */
            public ShortCodec() : base("Short"){}


            /*
             * PrimitiveCodec overrides
             */
            public override DataResult<short> Read<T>(DynamicOps<T> ops, T input){
                return ops.GetNumberValue(input).Map(decimal.ToInt16);
            }
            
            public override T Write<T>(DynamicOps<T> ops, short value){
                return ops.CreateShort(value);
            }
        }

        private sealed class IntCodec : PrimitiveCodec<int>{
            /*
             * Constructor
             */
            public IntCodec() : base("Int"){}


            /*
             * PrimitiveCodec overrides
             */
            public override DataResult<int> Read<T>(DynamicOps<T> ops, T input){
                return ops.GetNumberValue(input).Map(decimal.ToInt32);
            }
            
            public override T Write<T>(DynamicOps<T> ops, int value){
                return ops.CreateInt(value);
            }
        }

        private sealed class LongCodec : PrimitiveCodec<long>{
            /*
             * Constructor
             */
            public LongCodec() : base("Long"){}


            /*
             * PrimitiveCodec overrides
             */
            public override DataResult<long> Read<T>(DynamicOps<T> ops, T input){
                return ops.GetNumberValue(input).Map(decimal.ToInt64);
            }
            
            public override T Write<T>(DynamicOps<T> ops, long value){
                return ops.CreateLong(value);
            }
        }

        private sealed class FloatCodec : PrimitiveCodec<float>{
            /*
             * Constructor
             */
            public FloatCodec() : base("Float"){}


            /*
             * PrimitiveCodec overrides
             */
            public override DataResult<float> Read<T>(DynamicOps<T> ops, T input){
                return ops.GetNumberValue(input).Map(decimal.ToSingle);
            }
            
            public override T Write<T>(DynamicOps<T> ops, float value){
                return ops.CreateFloat(value);
            }
        }

        private sealed class DoubleCodec : PrimitiveCodec<double>{
            /*
             * Constructor
             */
            public DoubleCodec() : base("Double"){}


            /*
             * PrimitiveCodec overrides
             */
            public override DataResult<double> Read<T>(DynamicOps<T> ops, T input){
                return ops.GetNumberValue(input).Map(decimal.ToDouble);
            }
            
            public override T Write<T>(DynamicOps<T> ops, double value){
                return ops.CreateDouble(value);
            }
        }

        private sealed class StringCodec : PrimitiveCodec<string>{
            /*
             * Constructor
             */
            public StringCodec() : base("String"){}


            /*
             * PrimitiveCodec overrides
             */
            public override DataResult<string> Read<T>(DynamicOps<T> ops, T input){
                return ops.GetStringValue(input);
            }
            
            public override T Write<T>(DynamicOps<T> ops, string value){
                return ops.CreateString(value);
            }
        }

        private sealed class MemoryStreamCodec : PrimitiveCodec<MemoryStream>{
            /*
             * Constructor
             */
            public MemoryStreamCodec() : base("MemoryStream"){}


            /*
             * PrimitiveCodec overrides
             */
            public override DataResult<MemoryStream> Read<T>(DynamicOps<T> ops, T input){
                return ops.GetByteBuffer(input);
            }

            public override T Write<T>(DynamicOps<T> ops, MemoryStream value){
                return ops.CreateByteList(value);
            }
        }

        private sealed class IntStreamCodec : PrimitiveCodec<IEnumerable<int>>{
            /*
             * Constructor
             */
            public IntStreamCodec() : base("IntStream"){}


            /*
             * PrimitiveCodec overrides
             */
            public override DataResult<IEnumerable<int>> Read<T>(DynamicOps<T> ops, T input){
                return ops.GetIntStream(input);
            }

            public override T Write<T>(DynamicOps<T> ops, IEnumerable<int> value){
                return ops.CreateIntList(value);
            }
        }
        
        private sealed class LongStreamCodec : PrimitiveCodec<IEnumerable<long>>{
            /*
             * Constructor
             */
            public LongStreamCodec() : base("LongStream"){}


            /*
             * PrimitiveCodec overrides
             */
            public override DataResult<IEnumerable<long>> Read<T>(DynamicOps<T> ops, T input){
                return ops.GetLongStream(input);
            }

            public override T Write<T>(DynamicOps<T> ops, IEnumerable<long> value){
                return ops.CreateLongList(value);
            }
        }

        private sealed class EncoderDecoderWrapper<A> : ICodec<A>{
            /*
             * Fields
             */
            private readonly IEncoder<A> encoder;
            private readonly IDecoder<A> decoder;
            private readonly string name;

            
            /*
             * Constructor
             */
            public EncoderDecoderWrapper(IEncoder<A> encoderIn, IDecoder<A> decoderIn, string nameIn){
                encoder = encoderIn;
                decoder = decoderIn;
                name = nameIn;
            }


            /*
             * ICodec implementation
             */
            public DataResult<Pair<A, T>> Decode<T>(DynamicOps<T> ops, T input){
                return decoder.Decode(ops, input);
            }

            public DataResult<T> Encode<T>(A input, DynamicOps<T> ops, T prefix){
                return encoder.Encode(input, ops, prefix);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return name;
            }
        }

        private sealed class MapEncoderDecoderWrapper<A> : MapCodec<A>{
            /*
             * Fields
             */
            private readonly IMapEncoder<A> encoder;
            private readonly IMapDecoder<A> decoder;
            private readonly Func<string> name;

            
            /*
             * Constructor
             */
            public MapEncoderDecoderWrapper(IMapEncoder<A> encoderIn, IMapDecoder<A> decoderIn, Func<string> nameIn){
                encoder = encoderIn;
                decoder = decoderIn;
                name = nameIn;
            }


            /*
             * MapCodec override methods
             */
            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return Enumerable.Concat(encoder.Keys(ops), decoder.Keys(ops));
            }

            public override DataResult<A> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
                return decoder.Decode(ops, input);
            }

            public override RecordBuilder<T> Encode<T>(A input, DynamicOps<T> ops, RecordBuilder<T> prefix){
                return encoder.Encode(input, ops, prefix);
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
