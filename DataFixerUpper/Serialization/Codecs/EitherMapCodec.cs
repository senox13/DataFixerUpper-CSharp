using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.DataFixers.Util;
using JavaUtilities;

namespace DataFixerUpper.Serialization.Codecs{
    public sealed class EitherMapCodec<F, S> : MapCodec<Either<F, S>>{
        /*
         * Fields
         */
        private readonly MapCodec<F> first;
        private readonly MapCodec<S> second;


        /*
         * Constructor
         */
        public EitherMapCodec(MapCodec<F> firstIn, MapCodec<S> secondIn){
            first = firstIn;
            second = secondIn;
        }


        /*
         * MapCodec override methods
         */
        public override DataResult<Either<F, S>> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
            DataResult<Either<F, S>> firstRead = first.Decode(ops, input).Map(Either.Left<F, S>);
            if(firstRead.Result().IsPresent()){
                return firstRead;
            }
            return second.Decode(ops, input).Map(Either.Right<F, S>);
        }

        public override RecordBuilder<T> Encode<T>(Either<F, S> input, DynamicOps<T> ops, RecordBuilder<T> prefix){
            return input.Map(
                value1 => first.Encode(value1, ops, prefix),
                value2 => second.Encode(value2, ops, prefix)
            );
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
            if(obj is EitherMapCodec<F, S> other)
                return ObjectUtils.Equals(first, other.first)
                    && ObjectUtils.Equals(second, other.second);
            return false;
        }

        public override int GetHashCode(){
            return ObjectUtils.Hash(first, second);
        }

        public override string ToString(){
            return $"EitherMapCodec[{first}, {second}]";
        }
    }
}
