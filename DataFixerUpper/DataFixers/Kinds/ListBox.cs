using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.DataFixers.Kinds{
    public static class ListBox{
        public sealed class Mu : K1{}


        /*
         * Static methods
         */
        public static IList<T> Unbox<T>(App<Mu, T> box){
            return ((ListBox<T>)box).value;
        }

        public static ListBox<T> Create<T>(IList<T> value){
            return new ListBox<T>(value);
        }

        public static App<F, IList<B>> Traverse<F, M, A, B>(Applicative<F, M> applicative, Func<A, App<F, B>> function, IList<A> input) where F : K1 where M : Applicative.Mu{
            return applicative.Map(Unbox, Instance.INSTANCE.Traverse(applicative, function, Create(input)));
        }

        public static App<F, IList<A>> Flip<F, M, A>(Applicative<F, M> applicative, IList<App<F, A>> input) where F : K1 where M : Applicative.Mu{
            return applicative.Map(Unbox, Instance.INSTANCE.Flip(applicative, Create(input)));
        }


        /*
         * Nested types
         */
        public sealed class Instance : Traversable<Mu, Instance.Mu>{
            public sealed class Mu : Traversable.Mu{}


            /*
             * Fields
             */
            public static readonly Instance INSTANCE = new Instance();


            /*
             * Constructor
             */
            private Instance(){}


            /*
             * Functor implementation
             */
            public App<ListBox.Mu, R> Map<T, R>(Func<T, R> func, App<ListBox.Mu, T> ts){
                return Create(Unbox(ts).Select(func).ToList());
            }


            /*
             * Traversable implementation
             */
            public App<F, App<ListBox.Mu, B>> Traverse<F, M, A, B>(Applicative<F, M> applicative, Func<A, App<F, B>> function, App<ListBox.Mu, A> input) where F : K1 where M : Applicative.Mu{
                IList<A> list = Unbox(input);
                App<F, ImmutableList<B>> result = applicative.Point(ImmutableList.Create<B>());
                foreach(A a in list){
                    App<F, B> fb = function.Invoke(a);
                    result = applicative.Ap2(applicative.Point<Func<ImmutableList<B>, B, ImmutableList<B>>>((l, v) => l.Add(v)), result, fb);
                }
                return (App<F, App<ListBox.Mu, B>>)applicative.Map(b => Create(b), result);
            }

            public App<F, App<ListBox.Mu, A>> Flip<F, M, A>(Applicative<F, M> applicative, App<ListBox.Mu, App<F, A>> input) where F : K1 where M : Applicative.Mu{
                return Traverse(applicative, FuncUtils.Identity<App<F, A>>(), input);
            }
        }
    }

    public sealed class ListBox<T> : App<ListBox.Mu, T>{
        /*
         * Fields
         */
        internal readonly IList<T> value;


        /*
         * Constructor
         */
        internal ListBox(IList<T> valueIn){
            value = valueIn;
        }
    }
}
