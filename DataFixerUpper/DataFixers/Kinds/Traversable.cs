using System;

namespace DataFixerUpper.DataFixers.Kinds{
    public static class Traversable{
        public interface Mu : Functor.Mu{}


        /*
         * Static methods
         */
        public static Traversable<F, Mu> Unbox<F, Mu>(App<Mu, F> proofBox) where F : K1 where Mu : Traversable.Mu{
            return (Traversable<F, Mu>)proofBox;
        }
    }

    public interface Traversable<T, Mu> : Functor<T, Mu> where T : K1 where Mu : Traversable.Mu{
        App<F, App<T, B>> Traverse<F, M, A, B>(Applicative<F, M> applicative, Func<A, App<F, B>> function, App<T, A> input) where F : K1 where M : Applicative.Mu;

        App<F, App<T, A>> Flip<F, M, A>(Applicative<F, M> applicative, App<T, App<F, A>> input) where F : K1 where M : Applicative.Mu;
    }
}
