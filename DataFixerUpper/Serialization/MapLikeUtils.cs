using System.Collections;
using System.Collections.Generic;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.Serialization{
    public static class MapLikeUtils{
        /*
         * Static methods
         */
        public static IMapLike<T> ForMap<T>(IDictionary<T, T> map, DynamicOps<T> ops){
            return new MapLikeDictWrapper<T>(map, ops);
        }


        /*
         * Nested types
         */
        private class MapLikeDictWrapper<T> : IMapLike<T>{
            /*
             * Fields
             */
            private readonly IDictionary<T, T> wrapped;
            private readonly DynamicOps<T> ops;


            /*
             * Constructor
             */
            public MapLikeDictWrapper(IDictionary<T, T> toWrap, DynamicOps<T> opsIn){
                wrapped = toWrap;
                ops = opsIn;
            }

            
            /*
             * IMapLike implementation
             */
            public T Get(T key){
                if(wrapped.TryGetValue(key, out T value)){
                    return value;
                }
                return default;
            }
            
            public T Get(string key){
                return Get(ops.CreateString(key));
            }
            
            public IEnumerator<Pair<T, T>> GetEnumerator(){
                foreach(KeyValuePair<T, T> pair in wrapped){
                    yield return Pair.Of(pair.Key, pair.Value);
                }
            }
            
            IEnumerator IEnumerable.GetEnumerator(){
                return GetEnumerator();
            }
        }
    }
}
