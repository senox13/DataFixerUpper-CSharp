using System.Collections.Generic;
using JavaUtilities;

namespace DataFixerUpper.Serialization.Codecs{
    public class FieldEncoder<A> : MapEncoder.Implementation<A>{
        /*
         * Fields
         */
        private readonly string name;
        private readonly IEncoder<A> elementCodec;
        

        /*
         * Constructor
         */
        public FieldEncoder(string nameIn, IEncoder<A> elementCodecIn){
            name = nameIn;
            elementCodec = elementCodecIn;
        }


        /*
         * MapEncoder.Implementation override methods
         */
        public override RecordBuilder<T> Encode<T>(A input, DynamicOps<T> ops, RecordBuilder<T> prefix){
            return prefix.Add(name, elementCodec.EncodeStart(ops, input));
        }

        public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
            yield return ops.CreateString(name);
        }


        /*
         * Object override methods
         */
        public override bool Equals(object obj){
            if(obj == this){
                return true;
            }
            if(obj is FieldEncoder<A> other){
                return ObjectUtils.Equals(name, other.name)
                    && ObjectUtils.Equals(elementCodec, other.elementCodec);
            }
            return false;
        }

        public override int GetHashCode(){
            return ObjectUtils.Hash(name, elementCodec);
        }

        public override string ToString(){
            return $"FieldEncoder[{name}: {elementCodec}]";
        }
    }
}
