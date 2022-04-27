namespace DataFixerUpper.Serialization{
    public class Lifecycle{
        /*
         * Fields
         */
        //Static fields
        private static readonly Lifecycle STABLE = new Lifecycle("Stable");
        private static readonly Lifecycle EXPERIMENTAL = new Lifecycle("Experimental");
        //Instance fields
        private readonly string name;


        /*
         * Constructor
         */
        private Lifecycle(string nameIn){
            name = nameIn;
        }


        /*
         * Static methods
         */
        public static Lifecycle Stable(){
            return STABLE;
        }

        public static Lifecycle Experimental(){
            return EXPERIMENTAL;
        }

        public static Lifecycle DeprecatedSince(int since){
            return new Deprecated(since);
        }


        /*
         * Instance methods
         */
        public Lifecycle Add(Lifecycle other){
            if(this == EXPERIMENTAL || other == EXPERIMENTAL){
                return EXPERIMENTAL;
            }
            if(this is Deprecated thisDep){
                if(other is Deprecated otherDep && otherDep.Since() < thisDep.Since()){
                    return other;
                }
                return this;
            }
            if(other is Deprecated){
                return other;
            }
            return STABLE;
        }

        /// <inheritdoc/>
        public override string ToString(){
            return name;
        }


        /*
         * Nested types
         */
        public sealed class Deprecated : Lifecycle{
            /*
             * Fields
             */
            private readonly int since;


            /*
             * Constructor
             */
            public Deprecated(int sinceIn)
                :base("Deprecated"){
                since = sinceIn;
            }


            /*
             * Public methods
             */
            public int Since(){
                return since;
            }
        }
    }
}
