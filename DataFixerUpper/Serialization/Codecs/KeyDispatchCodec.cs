using System;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.Serialization.Codecs{
    public class KeyDispatchCodec<K, V> : MapCodec<V>{
        /*
         * Fields
         */
        private readonly string typeKey;
        private readonly ICodec<K> keyCodec;
        private readonly string valueKey = "value";
        private readonly Func<V, DataResult<K>> type;
        private readonly Func<K, DataResult<IDecoder<V>>> decoder;
        private readonly Func<V, DataResult<IEncoder<V>>> encoder;
        private readonly bool assumeMap;


        /*
         * Constructor
         */
        protected KeyDispatchCodec(string typeKeyIn, ICodec<K> keyCodecIn, Func<V, DataResult<K>> typeIn, Func<K, DataResult<IDecoder<V>>> decoderIn, Func<V, DataResult<IEncoder<V>>> encoderIn, bool assumeMapIn){
            typeKey = typeKeyIn;
            keyCodec = keyCodecIn;
            type = typeIn;
            decoder = decoderIn;
            encoder=  encoderIn;
            assumeMap = assumeMapIn;
        }

        public KeyDispatchCodec(string typeKeyIn, ICodec<K> keyCodecIn, Func<V, DataResult<K>> typeIn, Func<K, DataResult<ICodec<V>>> codecIn)
            :this(typeKeyIn, keyCodecIn, typeIn, k => GetDecoder(codecIn, k), v => GetEncoder(typeIn, codecIn, v), false){}


        /*
         * Static methods
         */
        private static DataResult<IDecoder<V>> GetDecoder(Func<K, DataResult<ICodec<V>>> encoder, K input){
            return encoder.Invoke(input).Map(FuncUtils.Identity<IDecoder<V>>());
        }

        private static DataResult<IEncoder<V>> GetEncoder(Func<V, DataResult<K>> type, Func<K, DataResult<ICodec<V>>> encoder, V input){
            return type.Invoke(input).FlatMap(k => encoder.Invoke(k).Map(FuncUtils.Identity<IEncoder<V>>()));
        }

        public static KeyDispatchCodec<K, V> Unsafe(string typeKeyIn, ICodec<K> keyCodecIn, Func<V, DataResult<K>> typeIn, Func<K, DataResult<IDecoder<V>>> decoderIn, Func<V, DataResult<IEncoder<V>>> encoderIn){
            return new KeyDispatchCodec<K, V>(typeKeyIn, keyCodecIn, typeIn, decoderIn, encoderIn, true);
        }


        /*
         * MapCodec override methods
         */
        public override DataResult<V> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
            T elementName = input.Get(typeKey);
            if(elementName == null){
                return DataResult.Error<V>($"Input does not contain a key [{typeKey}]: {input}");
            }
            return keyCodec.Decode(ops, elementName).FlatMap(type => {
                DataResult<IDecoder<V>> elementDecoder = decoder.Invoke(type.GetFirst());
                return elementDecoder.FlatMap(c => {
                    if(ops.CompressMaps()){
                        T value = input.Get(ops.CreateString(valueKey));
                        if(value == null){
                            return DataResult.Error<V>($"Input does not have a \"value\" entry: {input}");
                        }
                        return c.Parse(ops, value);
                    }
                    if(c is MapCodec.MapCodecCodec<V> mapCodec){
                        return mapCodec.Codec().Decode(ops, input);
                    }
                    if(assumeMap){
                        return c.Decode(ops, ops.CreateMap(input)).Map(p => p.GetFirst());
                    }
                    return c.Decode(ops, input.Get(valueKey)).Map(p => p.GetFirst());
                });
            });
        }
        
        public override RecordBuilder<T> Encode<T>(V input, DynamicOps<T> ops, RecordBuilder<T> prefix){
            DataResult<IEncoder<V>> elementEncoder = encoder.Invoke(input);
            RecordBuilder<T> builder = prefix.WithErrorsFrom(elementEncoder);
            if(!elementEncoder.Result().IsPresent()){
                return builder;
            }
            IEncoder<V> c = elementEncoder.Result().Get();
            if(ops.CompressMaps()){
                return prefix
                    .Add(typeKey, type.Invoke(input).FlatMap(t => keyCodec.EncodeStart(ops, t)))
                    .Add(valueKey, c.EncodeStart(ops, input));
            }
            if(c is MapCodec.MapCodecCodec<V> mapCodec){
                return mapCodec.Codec().Encode(input, ops, prefix)
                    .Add(typeKey, type.Invoke(input).FlatMap(t => keyCodec.EncodeStart(ops, t)));
            }
            T typeString = ops.CreateString(typeKey);
            DataResult<T> result = c.EncodeStart(ops, input);
            if(assumeMap){
                DataResult<IMapLike<T>> element = result.FlatMap(ops.GetMap);
                return element.Map(map => {
                    prefix.Add(typeString, type.Invoke(input).FlatMap(t => keyCodec.EncodeStart(ops, t)));
                    foreach(Pair<T, T> pair in map){
                        if(!pair.GetFirst().Equals(typeString)){
                            prefix.Add(pair.GetFirst(), pair.GetSecond());
                        }
                    }
                    return prefix;
                }).Result().OrElseGet(() => prefix.WithErrorsFrom(element));
            }
            prefix.Add(typeString, type.Invoke(input).FlatMap(t => keyCodec.EncodeStart(ops, t)));
            prefix.Add(valueKey, result);
            return prefix;
        }
        
        public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
            return new List<string>(){typeKey, valueKey}.Select(ops.CreateString);
        }


        /*
         * Object override methods
         */
        public override string ToString(){
            return $"KeyDispatchCodec[{keyCodec} {type} {decoder}]";
        }
    }
}
