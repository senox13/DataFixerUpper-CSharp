namespace DataFixerUpper.DataFixers.Util{
    public sealed class Unit{
        /*
         * Fields
         */
        public static readonly Unit INSTANCE = new Unit();


        /*
         * 
         */
        private Unit(){}


        /*
         * Object override methods
         */
        public override string ToString(){
            return "Unit";
        }
    }
}
