using DataFixerUpper.DataFixers.Util;
using DataFixerUpper.Util;

namespace DataFixerUpper.Serialization.Codecs{
    public sealed class EitherCodec<F, S> : ICodec<Either<F, S>>{
        /*
         * Fields
         */
        private readonly ICodec<F> first;
        private readonly ICodec<S> second;


        /*
         * Constructor
         */
        public EitherCodec(ICodec<F> firstIn, ICodec<S> secondIn){
            first = firstIn;
            second = secondIn;
        }


        /*
         * ICodec implementation
         */
        public DataResult<Pair<Either<F, S>, T>> Decode<T>(DynamicOps<T> ops, T input){
            DataResult<Pair<Either<F, S>, T>> firstRead = first.Decode(ops, input).Map(vo => vo.MapFirst(Either.Left<F, S>));
            if(firstRead.Result().IsPresent()){
                return firstRead;
            }
            return second.Decode(ops, input).Map(vo => vo.MapFirst(Either.Right<F, S>));
        }

        public DataResult<T> Encode<T>(Either<F, S> input, DynamicOps<T> ops, T prefix){
            return input.Map(
                f => first.Encode(f, ops, prefix),
                s => second.Encode(s, ops, prefix)
            );
        }


        /*
         * Object override methods
         */
        public override bool Equals(object obj){
            if(obj == this)
                return true;
            if(obj is EitherCodec<F, S> other)
                return first.Equals(other.first) && second.Equals(other.second);
            return false;
        }

        public override int GetHashCode(){
            return ObjectUtils.Hash(first, second);
        }

        public override string ToString(){
            return $"EitherCodec[{first}, {second}]";
        }
    }
}
