using System.Collections.Generic;
using System.Linq;
using JavaUtilities;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.Serialization.Codecs{
    public sealed class PairMapCodec<F, S> : MapCodec<Pair<F, S>>{
        /*
         * Fields
         */
        private readonly MapCodec<F> first;
        private readonly MapCodec<S> second;


        /*
         * Constructor
         */
        public PairMapCodec(MapCodec<F> firstIn, MapCodec<S> secondIn){
            first = firstIn;
            second = secondIn;
        }


        /*
         * MapCodec override methods
         */
        public override DataResult<Pair<F, S>> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
            return first.Decode(ops, input).FlatMap(p1 =>
                second.Decode(ops, input).Map(p2 =>
                    Pair.Of(p1, p2)
                )
            );
        }

        public override RecordBuilder<T> Encode<T>(Pair<F, S> input, DynamicOps<T> ops, RecordBuilder<T> prefix){
            return first.Encode(input.GetFirst(), ops, second.Encode(input.GetSecond(), ops, prefix));
        }

        public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
            return Enumerable.Concat(first.Keys(ops), second.Keys(ops));
        }


        /*
         * Object override methods
         */
        public override bool Equals(object obj){
            if(obj == this)
                return true;
            if(obj is PairMapCodec<F, S> other)
                return ObjectUtils.Equals(first, other.first)
                    && ObjectUtils.Equals(second, other.second);
            return false;
        }

        public override int GetHashCode(){
            return ObjectUtils.Hash(first, second);
        }

        public override string ToString(){
            return $"PairMapCodec[{first}, {second}]";
        }
    }
}
