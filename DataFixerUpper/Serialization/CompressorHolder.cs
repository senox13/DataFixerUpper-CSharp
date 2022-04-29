using System.Collections.Generic;

namespace DataFixerUpper.Serialization{
    public abstract class CompressorHolder : ICompressable{
        /*
         * Fields
         */
        private readonly Dictionary<object, object> compressors = new Dictionary<object, object>();


        /*
         * Abstract IKeyable implementation
         */
        public abstract IEnumerable<T> Keys<T>(DynamicOps<T> ops);


        /*
         * ICompressable implementation
         */
        public virtual KeyCompressor<T> Compressor<T>(DynamicOps<T> ops){
            if(compressors.TryGetValue(ops, out object result)){
                if(result is KeyCompressor<T> compressor){
                    return compressor;
                }
            }
            KeyCompressor<T> newCompressor = new KeyCompressor<T>(ops, Keys(ops));
            compressors.Add(ops, newCompressor);
            return newCompressor;
        }
    }
}
