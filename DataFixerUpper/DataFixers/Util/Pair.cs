using System;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.DataFixers.Kinds;
using DataFixerUpper.Util;

namespace DataFixerUpper.DataFixers.Util{
    public sealed class Pair<F, S> : App<Pair.Mu<S>, F>{
        /*
         * Fields
         */
        private readonly F first;
        private readonly S second;

        
        /*
         * Constructor
         */
        public Pair(F firstIn, S secondIn){
            first = firstIn;
            second = secondIn;
        }


        /*
         * Instance methods
         */
        public F GetFirst(){
            return first;
        }

        public S GetSecond(){
            return second;
        }

        public Pair<S, F> Swap(){
            return Pair.Of(second, first);
        }

        public Pair<F2, S> MapFirst<F2>(Func<F, F2> function){
            return Pair.Of(function(first), second);
        }

        public Pair<F, S2> MapSecond<S2>(Func<S, S2> function){
            return Pair.Of(first, function(second));
        }


        /*
         * Object override methods
         */
        public override string ToString(){
            return $"({first}, {second})";
        }

        public override bool Equals(object obj){
            if(obj is Pair<F, S> other){
                return first.Equals(other.first) && second.Equals(other.second);
            }
            return false;
        }

        public override int GetHashCode(){
            return ObjectUtils.Hash(first, second);
        }
    }

    public static class Pair{
        public sealed class Mu<S> : K1{}


        /*
         * Static methods
         */
        public static Pair<F, S> Unbox<F, S>(App<Mu<S>, F> box){
            return (Pair<F, S>)box;
        }

        public static Pair<F, S> Of<F, S>(F first, S second){
            return new Pair<F, S>(first, second);
        }

        public static IDictionary<F, S> ToDict<F, S>(IEnumerable<Pair<F, S>> pairs){
            return pairs.ToDictionary(p => p.GetFirst(), p => p.GetSecond());
        }
    }
}
