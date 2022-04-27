using System.Collections.Generic;
using DataFixerUpper.Util;

namespace DataFixerUpper.Serialization.Codecs{
    public class OptionalFieldCodec<A> : MapCodec<Optional<A>>{
        /*
         * Fields
         */
        private readonly string name;
        private readonly ICodec<A> elementCodec;


        /*
         * Constructor
         */
        public OptionalFieldCodec(string nameIn, ICodec<A> elementCodecIn){
            name = nameIn;
            elementCodec = elementCodecIn;
        }


        /*
         * MapCodec override methods
         */
        public override DataResult<Optional<A>> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
            T value = input.Get(name);
            if(value == null){
                return DataResult.Success(Optional<A>.Empty());
            }
            DataResult<A> parsed = elementCodec.Parse(ops, value);
            if(parsed.Result().IsPresent()){
                return parsed.Map(Optional<A>.Of);
            }
            return DataResult.Success(Optional<A>.Empty());
        }

        public override RecordBuilder<T> Encode<T>(Optional<A> input, DynamicOps<T> ops, RecordBuilder<T> prefix){
            if(input.IsPresent()){
                return prefix.Add(name, elementCodec.EncodeStart(ops, input.Get()));
            }
            return prefix;
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
            if(obj is OptionalFieldCodec<A> other){
                return ObjectUtils.Equals(name, other.name)
                    && ObjectUtils.Equals(elementCodec, other.elementCodec);
            }
            return false;
        }

        public override int GetHashCode(){
            return ObjectUtils.Hash(name, elementCodec);
        }

        public override string ToString(){
            return $"OptionalFieldCodec[{name}: {elementCodec}]";
        }
    }
}
