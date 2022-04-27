using System;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.DataFixers.Kinds{
    public abstract class Applicative<F, Mu> : Functor<F, Mu> where F : K1 where Mu : Applicative.Mu{
        /*
         * Abstract methods
         */
        public abstract App<F, A> Point<A>(A a);

        public abstract Func<App<F, A>, App<F, R>> Lift1<A, R>(App<F, Func<A, R>> function);

        public abstract App<F, R> Map<T, R>(Func<T, R> func, App<F, T> ts);


        /*
         * Virtual methods
         */
        public virtual Func<App<F, A>, App<F, B>, App<F, R>> Lift2<A, B, R>(App<F, Func<A, B,R>> function){
            return (fa, fb) => Ap2(function, fa, fb);
        }

        public virtual Func<App<F, T1>, App<F, T2>, App<F, T3>, App<F, R>> Lift3<T1, T2, T3, R>(App<F, Func<T1, T2, T3, R>> function){
            return (f1, f2, f3) => Ap3(function, f1, f2, f3);
        }

        public virtual Func<App<F, T1>, App<F, T2>, App<F, T3>, App<F, T4>, App<F, R>> Lift4<T1, T2, T3, T4, R>(App<F, Func<T1, T2, T3, T4, R>> function){
            return (ft1, ft2, ft3, ft4) => Ap4(function, ft1, ft2, ft3, ft4);
        }

        public virtual Func<App<F, T1>, App<F, T2>, App<F, T3>, App<F, T4>, App<F, T5>, App<F, R>> Lift5<T1, T2, T3, T4, T5, R>(App<F, Func<T1, T2, T3, T4, T5, R>> function){
            return (ft1, ft2, ft3, ft4, ft5) => Ap5(function, ft1, ft2, ft3, ft4, ft5);
        }

        public virtual Func<App<F, T1>, App<F, T2>, App<F, T3>, App<F, T4>, App<F, T5>, App<F, T6>, App<F, R>> Lift6<T1, T2, T3, T4, T5, T6, R>(App<F, Func<T1, T2, T3, T4, T5, T6, R>> function){
            return (ft1, ft2, ft3, ft4, ft5, ft6) => Ap6(function, ft1, ft2, ft3, ft4, ft5, ft6);
        }

        public virtual Func<App<F, T1>, App<F, T2>, App<F, T3>, App<F, T4>, App<F, T5>, App<F, T6>, App<F, T7>, App<F, R>> Lift7<T1, T2, T3, T4, T5, T6, T7, R>(App<F, Func<T1, T2, T3, T4, T5, T6, T7, R>> function){
            return (ft1, ft2, ft3, ft4, ft5, ft6, ft7) => Ap7(function, ft1, ft2, ft3, ft4, ft5, ft6, ft7);
        }

        public virtual Func<App<F, T1>, App<F, T2>, App<F, T3>, App<F, T4>, App<F, T5>, App<F, T6>, App<F, T7>, App<F, T8>, App<F, R>> Lift8<T1, T2, T3, T4, T5, T6, T7, T8, R>(App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, R>> function){
            return (ft1, ft2, ft3, ft4, ft5, ft6, ft7, ft8) => Ap8(function, ft1, ft2, ft3, ft4, ft5, ft6, ft7, ft8);
        }

        public virtual Func<App<F, T1>, App<F, T2>, App<F, T3>, App<F, T4>, App<F, T5>, App<F, T6>, App<F, T7>, App<F, T8>, App<F, T9>, App<F, R>> Lift9<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>> function){
            return (ft1, ft2, ft3, ft4, ft5, ft6, ft7, ft8, ft9) => Ap9(function, ft1, ft2, ft3, ft4, ft5, ft6, ft7, ft8, ft9);
        }

        public virtual App<F, R> Ap<A, R>(App<F, Func<A, R>> func, App<F, A> arg){
            return Lift1(func).Invoke(arg);
        }

        public virtual App<F, R> Ap<A, R>(Func<A, R> func, App<F, A> arg){
            return Map(func, arg);
        }

        public virtual App<F, R> Ap2<A, B, R>(App<F, Func<A, B, R>> func, App<F, A> a, App<F, B> b){
            Func<Func<A, B, R>, Func<A, Func<B, R>>> curry = f => a1 => b1 => f.Invoke(a1, b1);
            return Ap(Ap(Map(curry, func), a), b);
        }

        public virtual App<F, R> Ap3<T1, T2, T3, R>(App<F, Func<T1, T2, T3, R>> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3){
            return Ap2(Ap(Map(FuncUtils.Curry, func), t1), t2, t3);
        }

        public virtual App<F, R> Ap4<T1, T2, T3, T4, R>(App<F, Func<T1, T2, T3, T4, R>> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4){
            return Ap2(Ap2(Map(FuncUtils.Curry2, func), t1, t2), t3, t4);
        }

        public virtual App<F, R> Ap5<T1, T2, T3, T4, T5, R>(App<F, Func<T1, T2, T3, T4, T5, R>> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5){
            return Ap3(Ap2(Map(FuncUtils.Curry2, func), t1, t2), t3, t4, t5);
        }

        public virtual App<F, R> Ap6<T1, T2, T3, T4, T5, T6, R>(App<F, Func<T1, T2, T3, T4, T5, T6, R>> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6){
            return Ap3(Ap3(Map(FuncUtils.Curry3, func), t1, t2, t3), t4, t5, t6);
        }

        public virtual App<F, R> Ap7<T1, T2, T3, T4, T5, T6, T7, R>(App<F, Func<T1, T2, T3, T4, T5, T6, T7, R>> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6, App<F, T7> t7){
            return Ap4(Ap3(Map(FuncUtils.Curry3, func), t1, t2, t3), t4, t5, t6, t7);
        }

        public virtual App<F, R> Ap8<T1, T2, T3, T4, T5, T6, T7, T8, R>(App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, R>> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6, App<F, T7> t7, App<F, T8> t8){
            return Ap4(Ap4(Map(FuncUtils.Curry4, func), t1, t2, t3, t4), t5, t6, t7, t8);
        }

        public virtual App<F, R> Ap9<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6, App<F, T7> t7, App<F, T8> t8, App<F, T9> t9){
            return Ap5(Ap4(Map(FuncUtils.Curry4, func), t1, t2, t3, t4), t5, t6, t7, t8, t9);
        }

        public virtual App<F, R> Ap10<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6, App<F, T7> t7, App<F, T8> t8, App<F, T9> t9, App<F, T10> t10){
            return Ap5(Ap5(Map(FuncUtils.Curry5, func), t1, t2, t3, t4, t5), t6, t7, t8, t9, t10);
        }

        public virtual App<F, R> Ap11<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6, App<F, T7> t7, App<F, T8> t8, App<F, T9> t9, App<F, T10> t10, App<F, T11> t11){
            return Ap6(Ap5(Map(FuncUtils.Curry5, func), t1, t2, t3, t4, t5), t6, t7, t8, t9, t10, t11);
        }

        public virtual App<F, R> Ap12<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6, App<F, T7> t7, App<F, T8> t8, App<F, T9> t9, App<F, T10> t10, App<F, T11> t11, App<F, T12> t12){
            return Ap6(Ap6(Map(FuncUtils.Curry6, func), t1, t2, t3, t4, t5, t6), t7, t8, t9, t10, t11, t12);
        }

        public virtual App<F, R> Ap13<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6, App<F, T7> t7, App<F, T8> t8, App<F, T9> t9, App<F, T10> t10, App<F, T11> t11, App<F, T12> t12, App<F, T13> t13){
            return Ap7(Ap6(Map(FuncUtils.Curry6, func), t1, t2, t3, t4, t5, t6), t7, t8, t9, t10, t11, t12, t13);
        }

        public virtual App<F, R> Ap14<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6, App<F, T7> t7, App<F, T8> t8, App<F, T9> t9, App<F, T10> t10, App<F, T11> t11, App<F, T12> t12, App<F, T13> t13, App<F, T14> t14){
            return Ap7(Ap7(Map(FuncUtils.Curry7, func), t1, t2, t3, t4, t5, t6, t7), t8, t9, t10, t11, t12, t13, t14);
        }

        public virtual App<F, R> Ap15<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6, App<F, T7> t7, App<F, T8> t8, App<F, T9> t9, App<F, T10> t10, App<F, T11> t11, App<F, T12> t12, App<F, T13> t13, App<F, T14> t14, App<F, T15> t15){
            return Ap8(Ap7(Map(FuncUtils.Curry7, func), t1, t2, t3, t4, t5, t6, t7), t8, t9, t10, t11, t12, t13, t14, t15);
        }

        public virtual App<F, R> Ap16<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6, App<F, T7> t7, App<F, T8> t8, App<F, T9> t9, App<F, T10> t10, App<F, T11> t11, App<F, T12> t12, App<F, T13> t13, App<F, T14> t14, App<F, T15> t15, App<F, T16> t16){
            return Ap8(Ap8(Map(FuncUtils.Curry8, func), t1, t2, t3, t4, t5, t6, t7, t8), t9, t10, t11, t12, t13, t14, t15, t16);
        }

        public virtual App<F, R> Apply2<A, B, R>(Func<A, B, R> func, App<F, A> a, App<F, B> b){
            return Ap2(Point(func), a, b);
        }

        public virtual App<F, R> Apply3<T1, T2, T3, R>(Func<T1, T2, T3, R> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3){
            return Ap3(Point(func), t1, t2, t3);
        }

        public virtual App<F, R> Apply4<T1, T2, T3, T4, R>(Func<T1, T2, T3, T4, R> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4){
            return Ap4(Point(func), t1, t2, t3, t4);
        }

        public virtual App<F, R> Apply5<T1, T2, T3, T4, T5, R>(Func<T1, T2, T3, T4, T5, R> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5){
            return Ap5(Point(func), t1, t2, t3, t4, t5);
        }

        public virtual App<F, R> Apply6<T1, T2, T3, T4, T5, T6, R>(Func<T1, T2, T3, T4, T5, T6, R> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6){
            return Ap6(Point(func), t1, t2, t3, t4, t5, t6);
        }

        public virtual App<F, R> Apply7<T1, T2, T3, T4, T5, T6, T7, R>(Func<T1, T2, T3, T4, T5, T6, T7, R> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6, App<F, T7> t7){
            return Ap7(Point(func), t1, t2, t3, t4, t5, t6, t7);
        }

        public virtual App<F, R> Apply8<T1, T2, T3, T4, T5, T6, T7, T8, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6, App<F, T7> t7, App<F, T8> t8){
            return Ap8(Point(func), t1, t2, t3, t4, t5, t6, t7, t8);
        }

        public virtual App<F, R> Apply9<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func, App<F, T1> t1, App<F, T2> t2, App<F, T3> t3, App<F, T4> t4, App<F, T5> t5, App<F, T6> t6, App<F, T7> t7, App<F, T8> t8, App<F, T9> t9){
            return Ap9(Point(func), t1, t2, t3, t4, t5, t6, t7, t8, t9);
        }
    }
    
    public static class Applicative{
        public interface Mu : Functor.Mu{}


        /*
         * Static methods
         */
        public static Applicative<F, Mu> Unbox<F, Mu>(App<Mu, F> proofBox) where F : K1 where Mu : Applicative.Mu{
            return (Applicative<F, Mu>)proofBox;
        }
    }
}
