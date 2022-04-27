using System;

namespace DataFixerUpper.DataFixers.Kinds{
    public interface Functor<F, Mu> : Kind1<F, Mu> where F : K1 where Mu : Functor.Mu{
        App<F, R> Map<T, R>(Func<T, R> func, App<F, T> ts);
    }

    public static class Functor{
        public interface Mu : Kind1.Mu{}


        /*
         * Static methods
         */
        public static Functor<F, M> Unbox<F, M>(App<M, F> proofBox) where F : K1 where M : Mu{
            return (Functor<F, M>)proofBox;
        }
    }
}
