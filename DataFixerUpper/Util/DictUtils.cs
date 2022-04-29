using System;
using System.Collections.Generic;


namespace MCEngine.Util{
    /// <summary>
    /// Provides utilities for creating and populating dictionaries from collections.
    /// </summary>
    public static class DictUtils{
        /// <summary>
        /// Returns a new dictionary populated with the given values. If one
        /// enumerable returns more values than the other, only elements up to
        /// the end of the shorter enumerable will be used.
        /// </summary>
        /// <typeparam name="K">The type of the keys in the dictionary to be created</typeparam>
        /// <typeparam name="V">The type of the values in the dictionary to be created</typeparam>
        /// <param name="keys">An <see cref="IEnumerable{K}"/> to use values from as keys in the new dictionary</param>
        /// <param name="values">An <see cref="IEnumerable{V}"/> to use values from as keys in the new dictionary</param>
        /// <returns>A new dictionary containing values from the two given enumerables</returns>
        public static Dictionary<K, V> CreateDict<K, V>(IEnumerable<K> keys, IEnumerable<V> values){
            return PopulateDict(keys, values, new Dictionary<K, V>());
        }
        
        /// <summary>
        /// Returns the given dictionary populated with key, value pairs in
        /// the respective order they are returned by the the two given
        /// enumerables. If one enumerable returns more values than the other,
        /// only elements up to the end of the shorter enumerable will be used.
        /// If the given dictionary already contains any of the given keys, an
        /// <see cref="ArgumentException"/> will be thrown.
        /// </summary>
        /// <typeparam name="K">The type of the keys in the dictionary to be populated</typeparam>
        /// <typeparam name="V">The type of the values in the dictionary to be populated</typeparam>
        /// <param name="keys">An <see cref="IEnumerable{K}"/> to use values from as keys in the new dictionary</param>
        /// <param name="values">An <see cref="IEnumerable{V}"/> to use values from as keys in the new dictionary</param>
        /// <param name="dict">The dictionary to populate with values</param>
        /// <returns>The same dictionary that was given as an argument, populated with the given values</returns>
        public static Dictionary<K, V> PopulateDict<K, V>(IEnumerable<K> keys, IEnumerable<V> values, Dictionary<K, V> dict){
            IEnumerator<K> keyEnum = keys.GetEnumerator();
            IEnumerator<V> valEnum = values.GetEnumerator();
            while(keyEnum.MoveNext() && valEnum.MoveNext()){
                dict.Add(keyEnum.Current, valEnum.Current);
            }
            return dict;
        }

        /// <summary>
        /// Gets a semi-unique hashcode based on the unordered contents of
        /// the given <see cref="Dictionary{K, V}"/>. The actual likelihood
        /// of a hash collision depends on the hash functions of the generic
        /// types of the given dictionary.
        /// </summary>
        /// <typeparam name="K">The type of the keys in the given dictionary</typeparam>
        /// <typeparam name="V">The type of the values in the given dictionary</typeparam>
        /// <param name="dict">The dictionary to generate a hashcode for</param>
        /// <returns>A hash code for the given dictionary</returns>
        public static int ContentsHashCode<K, V>(Dictionary<K, V> dict){
            int hash = 0;
            if(dict == null){
                return hash;
            }
            foreach(KeyValuePair<K, V> pair in dict){
                int keyHash = pair.Key==null ? 0 : pair.Key.GetHashCode();
                int valHash = pair.Value==null ? 0 : pair.Value.GetHashCode();
                hash += keyHash ^ valHash;
            }
            return hash;
        }
    }
}
