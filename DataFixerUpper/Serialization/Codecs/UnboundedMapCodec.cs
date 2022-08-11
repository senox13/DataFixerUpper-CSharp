using System.Collections.Generic;
using JavaUtilities;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.Serialization.Codecs{
    public sealed class UnboundedMapCodec<K, V> : IBaseMapCodec<K, V>, ICodec<IDictionary<K, V>>{
        /*
         * Fields
         */
        private readonly ICodec<K> keyCodec;
        private readonly ICodec<V> elementCodec;


        /*
         * Constructor
         */
        public UnboundedMapCodec(ICodec<K> keyCodecIn, ICodec<V> elementCodecIn){
            keyCodec = keyCodecIn;
            elementCodec = elementCodecIn;
        }


        /*
         * ICodec implementation
         */
        public DataResult<Pair<IDictionary<K, V>, T>> Decode<T>(DynamicOps<T> ops, T input){
            return ops.GetMap(input).SetLifecycle(Lifecycle.Stable())
                .FlatMap(map => Decode(ops, map))
                .Map(r => Pair.Of(r, input));
        }

        public DataResult<T> Encode<T>(IDictionary<K, V> input, DynamicOps<T> ops, T prefix){
            return Encode(input, ops, ops.MapBuilder()).Build(prefix);
        }


        /*
         * IBaseMapCodec implementation
         */
        public ICodec<K> KeyCodec(){
            return keyCodec;
        }

        public ICodec<V> ElementCodec(){
            return elementCodec;
        }

        public DataResult<IDictionary<K, V>> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
            return BaseMapCodec.Decode(this, ops, input);
        }

        public RecordBuilder<T> Encode<T>(IDictionary<K, V> input, DynamicOps<T> ops, RecordBuilder<T> prefix){
            return BaseMapCodec.Encode(this, input, ops, prefix);
        }


        /*
         * Object override methods
         */
        public override bool Equals(object obj){
            if(obj == this){
                return true;
            }
            if(obj is UnboundedMapCodec<K, V> other){
                return ObjectUtils.Equals(keyCodec, other.keyCodec)
                    && ObjectUtils.Equals(elementCodec, other.elementCodec);
            }
            return false;
        }

        public override int GetHashCode(){
            return ObjectUtils.Hash(keyCodec, elementCodec);
        }

        public override string ToString(){
            return $"UnboundedMapCodec[{keyCodec} -> {elementCodec}]";
        }
    }
}
