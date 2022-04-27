namespace DataFixerUpper.Util{
    /// <summary>
    /// Provides utility methods for working with objects and arrays of
    /// objects.
    /// </summary>
    public static class ObjectUtils{
        /// <summary>
        /// If the given object is null, returns <c>0</c>. Otherwise, returns
        /// the given object's hashcode.
        /// </summary>
        /// <param name="o">The object to get the hashcode of</param>
        /// <returns>The hashcode of the given object if non-<c>null</c>,
        /// othwerise <c>0</c></returns>
        public static int HashCode(object o){
            if(o == null) return 0;
            return o.GetHashCode();
        }

        /// <summary>
        /// Returns a semi-unique integer based on the contents of the given
        /// array. The chance of hash colisions is determined by the hash
        /// functions of the given objects.
        /// </summary>
        /// <param name="objects">The array of objects to hash together</param>
        /// <returns>A semi-unique integer based on the contents of the given
        /// array</returns>
        public static int Hash(params object[] objects){
            return ArrayUtils.HashCode(objects);
        }

        /// <summary>
        /// Checks if the given objects are equal. If both objects are null,
        /// returns <c>true</c>.
        /// </summary>
        /// <param name="a">The first object to compare</param>
        /// <param name="b">The second object to compare</param>
        /// <returns><c>true</c> if the given objects are equal, otherwise
        /// <c>false</c></returns>
        public static new bool Equals(object a, object b){
            if(a == b){
                return true;
            }
            return (a != null) && a.Equals(b);
        }
    }
}
