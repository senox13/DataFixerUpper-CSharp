using System.Collections.Generic;
using DataFixerUpper.Util;

namespace DataFixerUpper.Serialization.Codecs{
    public sealed class FieldDecoder<A> : MapDecoder.Implementation<A>{
        /*
         * Fields
         */
        private readonly string name;
        private readonly IDecoder<A> elementCodec;
        

        /*
         * Constructor
         */
        public FieldDecoder(string nameIn, IDecoder<A> elementCodecIn){
            name = nameIn;
            elementCodec = elementCodecIn;
        }

        
        /*
         * MapDecoder.Implementation override methods
         */
        public override DataResult<A> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
            T value = input.Get(name);
            if(value == null){
                return DataResult.Error<A>($"No key {name} in {input}");
            }
            return elementCodec.Parse(ops, value);
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
            if(obj is FieldDecoder<A> other){
                return ObjectUtils.Equals(name, other.name)
                    && ObjectUtils.Equals(elementCodec, other.elementCodec);
            }
            return false;
        }

        public override int GetHashCode(){
            return ObjectUtils.Hash(name, elementCodec);
        }

        public override string ToString(){
            return $"FieldDecoder[{name}: {elementCodec}]";
        }
    }
}
