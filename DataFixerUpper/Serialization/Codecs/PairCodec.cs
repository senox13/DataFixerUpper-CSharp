using DataFixerUpper.DataFixers.Util;
using DataFixerUpper.Util;

namespace DataFixerUpper.Serialization.Codecs{
    public sealed class PairCodec<F, S> : ICodec<Pair<F, S>>{
        /*
         * Fields
         */
        private readonly ICodec<F> first;
        private readonly ICodec<S> second;


        /*
         * Constructor
         */
        public PairCodec(ICodec<F> firstIn, ICodec<S> secondIn){
            first = firstIn;
            second = secondIn;
        }

        
        /*
         * ICodec implementation
         */
        public DataResult<Pair<Pair<F, S>, T>> Decode<T>(DynamicOps<T> ops, T input){
            return first.Decode(ops, input).FlatMap(p1 =>
                second.Decode(ops, p1.GetSecond()).Map(p2 =>
                    Pair.Of(Pair.Of(p1.GetFirst(), p2.GetFirst()), p2.GetSecond())
                )
            );
        }

        public DataResult<T> Encode<T>(Pair<F, S> input, DynamicOps<T> ops, T prefix){
            return second.Encode(input.GetSecond(), ops, prefix).FlatMap(f => first.Encode(input.GetFirst(), ops, f));
        }


        /*
         * Object override methods
         */
        public override bool Equals(object obj){
            if(obj == this)
                return true;
            if(obj is PairCodec<F, S> other)
                return first.Equals(other.first) && second.Equals(other.second);
            return false;
        }

        public override int GetHashCode(){
            return ObjectUtils.Hash(first, second);
        }

        public override string ToString(){
            return $"PairCodec[{first}, {second}]";
        }
    }
}
