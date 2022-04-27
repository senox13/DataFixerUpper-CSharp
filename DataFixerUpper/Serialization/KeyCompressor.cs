using System.Collections.Generic;

namespace DataFixerUpper.Serialization{
    public sealed class KeyCompressor<T>{
        /*
         * Fields
         */
        private readonly Dictionary<int, T> decompress = new Dictionary<int, T>();
        private readonly Dictionary<T, int> compress = new Dictionary<T, int>();
        private readonly Dictionary<string, int> compressString = new Dictionary<string, int>();
        private readonly int size;
        private readonly DynamicOps<T> ops;


        /*
         * Constructor
         */
        public KeyCompressor(DynamicOps<T> opsIn, IEnumerable<T> keyStream){
            ops = opsIn;
            foreach(T key in keyStream){
                if(compress.ContainsKey(key)){
                    continue;
                }
                int next = compress.Count;
                compress.Add(key, next);
                ops.GetStringValue(key).Result().IfPresent(k =>
                    compressString.Add(k, next)
                );
                decompress.Add(next, key);
            }
            size = compress.Count;
        }


        /*
         * Instance methods
         */
        public T Decompress(int key){
            if(decompress.TryGetValue(key, out T result)){
                return result;
            }
            return default;
        }

        public int Compress(string key){
            if(compressString.TryGetValue(key, out int id)){
                return id;
            }
            return Compress(ops.CreateString(key));
        }

        public int Compress(T key){
            if(compress.TryGetValue(key, out int result)){
                return result;
            }
            return default;
        }

        public int Size(){
            return size;
        }
    }
}
