using System;
using System.Collections.Generic;
using System.Linq;

namespace DataFixerUpper.Serialization{
    public interface IKeyable{
        IEnumerable<T> Keys<T>(DynamicOps<T> ops);
    }

    public static class Keyable{
        /*
         * Static methods
         */
        public static IKeyable ForStrings(Func<IEnumerable<string>> keys){
            return new StringKeyable(keys);
        }


        /*
         * Nested types
         */
        private sealed class StringKeyable : IKeyable{
            /*
             * Fields
             */
            private readonly Func<IEnumerable<string>> keys;


            /*
             * Constructor
             */
            public StringKeyable(Func<IEnumerable<string>> keysIn){
                keys = keysIn;
            }


            /*
             * IKeyable implementation
             */
            public IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return keys.Invoke().Select(ops.CreateString);
            }
        }
    }
}
