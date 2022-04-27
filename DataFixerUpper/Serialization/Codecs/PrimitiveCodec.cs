using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.Serialization.Codecs{
    public abstract class PrimitiveCodec<A> : ICodec<A>{
        /*
         * Fields
         */
        private readonly string name;


        /*
         * Constructor
         */
        protected PrimitiveCodec(string nameIn){
            name = nameIn;
        }


        /*
         * Abstract methods
         */
        public abstract DataResult<A> Read<T>(DynamicOps<T> ops, T input);

        public abstract T Write<T>(DynamicOps<T> ops, A value);


        /*
         * ICodec implementation
         */
        public DataResult<Pair<A, T>> Decode<T>(DynamicOps<T> ops, T input){
            return Read(ops, input).Map(r => Pair.Of(r, ops.Empty()));
        }

        public DataResult<T> Encode<T>(A input, DynamicOps<T> ops, T prefix){
            return ops.MergeToPrimitive(prefix, Write(ops, input));
        }


        /*
         * Object override methods
         */
        public override string ToString(){
            return name;
        }
    }
}
