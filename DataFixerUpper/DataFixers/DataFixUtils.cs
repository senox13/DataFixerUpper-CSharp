using System;

namespace DataFixerUpper.DataFixers{
    public static class DataFixUtils{
        /*
         * Static methods
         */
        public static Func<T, T> ConsumerToFunction<T>(Action<T> consumer){
            return s => {
                consumer.Invoke(s);
                return s;
            };
        }
    }
}
