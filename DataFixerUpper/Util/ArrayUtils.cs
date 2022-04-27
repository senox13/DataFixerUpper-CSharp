﻿using System;
using System.Text;


namespace DataFixerUpper.Util{
    /// <summary>
    /// Provides utility methods for working with <see cref="Array"/>s.
    /// </summary>
    public static class ArrayUtils{
        /// <summary>
        /// Inserts the given element(s) into the given array at the given
        /// index. Note that this returns a new array instance.
        /// </summary>
        /// <typeparam name="T">The type of the array. Should usually not need
        /// to be explicitly specified</typeparam>
        /// <param name="array">The initial array</param>
        /// <param name="index">The starting index to instert values at</param>
        /// <param name="values">The value(s) to insert into the array</param>
        /// <returns>A new array with all of the given elements</returns>
        public static T[] Insert<T>(T[] array, int index, params T[] values){
            if(array == null)
                return null;
            if(values == null || values.Length == 0)
                return (T[])array.Clone();
            if(index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            T[] result = new T[array.Length + values.Length];
            Array.Copy(values, 0, result, index, values.Length);
            if(index > 0)
                Array.Copy(array, 0, result, 0, index);
            if (index < array.Length)
                Array.Copy(array, index, result, index + values.Length, array.Length - index);
            return result;
        }
        
        /// <summary>
        /// Removes the array element at the given index. Note that this
        /// returns a new array instance.
        /// </summary>
        /// <typeparam name="T">The type of the array. Should usually not need
        /// to be explicitly specified</typeparam>
        /// <param name="array">The initial array</param>
        /// <param name="index">The index of the element to remove</param>
        /// <returns>A new array with the element at the given index removed</returns>
        public static T[] Remove<T>(T[] array, int index){
            if(array == null)
                return null;
            if(index < 0 || index >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            T[] newArray = new T[array.Length - 1];
            if(index > 0)
                Array.Copy(array, 0, newArray, 0, index);
            if(index < array.Length - 1)
                Array.Copy(array, index + 1, newArray, index, array.Length - index - 1);
            return newArray;
        }
        
        /// <summary>
        /// Returns a semi-unique integer based on the contents of the given
        /// array. The chance of hash colisions is determined by the hash
        /// function of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the array. Should usually not need
        /// to be explicitly specified</typeparam>
        /// <param name="items">The array to hash</param>
        /// <returns>A semi-unique integer based on the contents of the given
        /// array</returns>
        public static int HashCode<T>(T[] items){
            return CollectionUtils.HashCode(items);
        }

        /// <summary>
        /// Compares the contents of two arrays.
        /// </summary>
        /// <param name="a">The first array to compare</param>
        /// <param name="b">The second array to compare</param>
        /// <returns><c>true</c> if the given arrays are equal,
        /// otherwise <c>false</c></returns>
        public static bool ContentsEqual<T>(T[] a, T[] b){
            if(a == b)
                return true;
            if(a==null || b==null)
                return false;
            int length = a.Length;
            if(b.Length != length)
                return false;
            for(int i=0; i<length; i++){
                if(!ObjectUtils.Equals(a[i], b[i])){
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns the string representation of the contents of the given
        /// array. If the array is null, returns <c>null</c>.
        /// </summary>
        /// <param name="objects">The array of objects to convert to a string</param>
        /// <returns>The string representation of the given array</returns>
        public static string ToString<T>(T[] objects){
            if(objects == null){
                return "null";
            }
            int iMax = objects.Length - 1;
            if(iMax == -1){
                return "[]";
            }
            StringBuilder builder = new StringBuilder("[");
            for(int i=0; i<=iMax; i++){
                builder.Append(objects[i].ToString());
                if(i != iMax){
                    builder.Append(", ");
                }
            }
            builder.Append(']');
            return builder.ToString();
        }

        /// <summary>
        /// Gets a random element from the given array using the given
        /// <see cref="Random"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the given array</typeparam>
        /// <param name="arr">The array to pick an element from</param>
        /// <param name="rand">The random generator to use to pick an element</param>
        /// <returns>A randomly selected element from the given array</returns>
        public static T GetRandom<T>(T[] arr, Random rand){
            return arr[rand.Next(arr.Length)];
        }
    }
}
