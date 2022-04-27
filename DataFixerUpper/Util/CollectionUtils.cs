using System;
using System.Collections.Generic;

namespace DataFixerUpper.Util{
    /// <summary>
    /// Provides utility methods for working with <see cref="IEnumerable{T}"/>
    /// instances and collections.
    /// </summary>
    public static class CollectionUtils{
        /// <summary>
        /// Calls the given <see cref="Action{T}"/> with the given <typeparamref name="T"/>
        /// instance, then returns the given <typeparamref name="T"/> instance.
        /// This method is primarily intended to allow for static initialization
        /// without a static block.
        /// </summary>
        /// <typeparam name="T">The type of the object being initialized</typeparam>
        /// <param name="instance">The instance to initialize then return</param>
        /// <param name="initializer">The method to initilized the given instance</param>
        /// <returns>The given <typeparamref name="T"/> instance</returns>
        public static T Make<T>(T instance, Action<T> initializer) where T : class{
            initializer(instance);
            return instance;
        }

        /// <summary>
        /// Returns a semi-unique integer based on the contents of the given
        /// <see cref="IEnumerable{T}"/>. The chance of hash collisions is
        /// determined by the hash function of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable. Should usually not
        /// need to be explicitly specified</typeparam>
        /// <param name="items">The array to hash</param>
        /// <returns>Returns a semi-unique integer based on the contents of
        /// the given enumerable</returns>
        public static int HashCode<T>(IEnumerable<T> items){
            if(items == null)
                return 0;
            int retVal = 1;
            foreach(T elem in items){
                retVal = 31 * retVal + ((elem == null) ? 0 : elem.GetHashCode());
            }
            return retVal;
        }

        /// <summary>
        /// Gets a random element from the given collection using the given
        /// <see cref="Random"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the given
        /// collection</typeparam>
        /// <param name="collection">The collection to pick an element from</param>
        /// <param name="rand">The random generator to use to pick an element</param>
        /// <returns>A randomly selected element from the given collection</returns>
        public static T GetRandom<T>(IList<T> collection, Random rand){
            return collection[rand.Next(collection.Count)];
        }

        public static bool ContentsEqual<T>(IEnumerable<T> a, IEnumerable<T> b){
            if(a == b){
                return true;
            }
            if(a == null || b == null){
                return false;
            }
            using(IEnumerator<T> enumA = a.GetEnumerator())
            using(IEnumerator<T> enumB = b.GetEnumerator())
            while(true){
                bool nextA = enumA.MoveNext();
                bool nextB = enumB.MoveNext();
                if(!nextA && !nextB){
                    return true;
                }
                if(!nextA || !nextB){
                    return false;
                }
                if(!ObjectUtils.Equals(enumA.Current, enumB.Current)){
                    return false;
                }
            }
        }
    }
}
