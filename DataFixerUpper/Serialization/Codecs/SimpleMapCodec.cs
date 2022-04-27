using System.Collections.Generic;
using DataFixerUpper.Util;

namespace DataFixerUpper.Serialization.Codecs{
    public sealed class SimpleMapCodec<K, V> : MapCodec<IDictionary<K, V>>, IBaseMapCodec<K, V>{
        /*
         * Fields
         */
        private readonly ICodec<K> keyCodec;
        private readonly ICodec<V> elementCodec;
        private readonly IKeyable keys;


        /*
         * Constructor
         */
        public SimpleMapCodec(ICodec<K> keyCodecIn, ICodec<V> elementCodecIn, IKeyable keysIn) {
            keyCodec = keyCodecIn;
            elementCodec = elementCodecIn;
            keys = keysIn;
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


        /*
         * MapCodec override methods
         */
        public override DataResult<IDictionary<K, V>> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
            return BaseMapCodec.Decode(this, ops, input);
        }
        
        public override RecordBuilder<T> Encode<T>(IDictionary<K, V> input, DynamicOps<T> ops, RecordBuilder<T> prefix){
            return BaseMapCodec.Encode(this, input, ops, prefix);
        }
        
        public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
            return keys.Keys(ops);
        }


        /*
         * Object override methods
         */
        public override bool Equals(object obj){
            if(obj == this){
                return true;
            }
            if(obj is SimpleMapCodec<K, V> other){
                return ObjectUtils.Equals(keyCodec, other.keyCodec)
                    && ObjectUtils.Equals(elementCodec, other.elementCodec);
            }
            return false;
        }

        public override int GetHashCode(){
            return ObjectUtils.Hash(keyCodec, elementCodec);
        }

        public override string ToString(){
            return $"SimpleMapCodec[{keyCodec} -> {elementCodec}]";
        }

        
    }
}
