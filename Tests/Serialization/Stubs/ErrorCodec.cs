using DataFixerUpper.DataFixers.Util;
using DataFixerUpper.Serialization;

namespace DataFixerUpperTests.Serialization.Stubs{
    public sealed class ErrorCodec : ICodec<string>{
        /*
         * Singleton instance
         */
        public static readonly ErrorCodec INSTANCE = new ErrorCodec();


        /*
         * Constructor
         */
        private ErrorCodec(){}

        
        /*
         * ICodec implementation
         */
        public DataResult<Pair<string, T>> Decode<T>(DynamicOps<T> ops, T input){
            return DataResult.Error<Pair<string, T>>("Error intentionally generated for testing");
        }

        public DataResult<T> Encode<T>(string input, DynamicOps<T> ops, T prefix){
            return DataResult.Error<T>("Error intentionally generated for testing");
        }
    }
}
