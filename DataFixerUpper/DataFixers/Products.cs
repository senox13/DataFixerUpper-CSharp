using System;
using DataFixerUpper.DataFixers.Kinds;

namespace DataFixerUpper.DataFixers{
    public static class Products{
        /*
         * The classes below were generated with the following code. It's an
         * absolute mess and will probably remain so until I bother with
         * source generators.
using System;
using System.IO;
using System.Text;

class MainClass{
  public static void AddClassDef(StringBuilder b, int productNum){
    b.Append("\t\tpublic sealed class P");
    b.Append(productNum);
    b.Append("<F, ");
    for(int i=1; i<=productNum; i++){
      b.Append("T");
      b.Append(i);
      if(i < productNum){
        b.Append(", ");
      }
    }
    b.Append("> where F : K1{\n");
  }

  public static void AddFields(StringBuilder b, int productNum){
    b.Append("\t\t\t//Fields\n");
    for(int i=1; i<=productNum; i++){
      b.Append("\t\t\tinternal readonly App<F, T");
      b.Append(i);
      b.Append("> t");
      b.Append(i);
      b.Append(";\n");
    }
    b.Append("\n\n");
  }

  public static void AddConstructor(StringBuilder b, int productNum){
    b.Append("\t\t\t//Constructor\n");
    b.Append("\t\t\tpublic P");
    b.Append(productNum);
    b.Append("(");
    for(int i=1; i<=productNum; i++){
      b.Append("App<F, T");
      b.Append(i);
      b.Append("> t");
      b.Append(i);
      b.Append("In");
      if(i < productNum){
        b.Append(", ");
      }
    }
    b.Append("){\n");
    for(int i=1; i<=productNum; i++){
      b.Append("\t\t\t\tt");
      b.Append(i);
      b.Append(" = t");
      b.Append(i);
      b.Append("In;\n");
    }
    b.Append("\t\t\t}\n\n\n");
  }
  
  public static void AddGetters(StringBuilder b, int productNum){
    b.Append("\t\t\t//Instance methods\n");
    for(int i=1; i<=productNum; i++){
      b.Append("\t\t\tpublic App<F, T");
      b.Append(i);
      b.Append("> GetT");
      b.Append(i);
      b.Append("(){\n");
      b.Append("\t\t\t\treturn t");
      b.Append(i);
      b.Append(";\n\t\t\t}\n\n");
    }
  }
  
  public static void AddAndMethods(StringBuilder b, int productNum){
    int startIndex = productNum + 1;
    for(int pReturnCount=startIndex; pReturnCount<=8; pReturnCount++){
      b.Append("\t\t\tpublic P");
      b.Append(pReturnCount);
      b.Append("<F, ");
      for(int genArg=1; genArg<=pReturnCount; genArg++){
        b.Append("T");
        b.Append(genArg);
        if(genArg < pReturnCount){
          b.Append(", ");
        }
      }
      b.Append("> And<");
      for(int genArg=startIndex; genArg<=pReturnCount; genArg++){
        b.Append("T");
        b.Append(genArg);
        if(genArg < pReturnCount){
          b.Append(", ");
        }
      }
      b.Append(">(");
      if(pReturnCount == startIndex){
        b.Append("App<F, ");
      }else{
        b.Append("P");
        b.Append(pReturnCount - productNum);
        b.Append("<F, ");
      }
      for(int argParam=startIndex; argParam<=pReturnCount; argParam++){
        b.Append("T");
        b.Append(argParam);
        if(argParam < pReturnCount){
          b.Append(", ");
        }
      }
      b.Append("> ");
      if(pReturnCount == startIndex){
        b.Append("t");
        b.Append(pReturnCount);
      }else{
        b.Append("p");
      }
      b.Append("){\n\t\t\t\treturn new P");
      b.Append(pReturnCount);
      b.Append("<F, ");
      for(int genArg=1; genArg<=pReturnCount; genArg++){
        b.Append("T");
        b.Append(genArg);
        if(genArg < pReturnCount){
          b.Append(", ");
        }
      }
      b.Append(">(");
      for(int localI=1; localI<=productNum; localI++){
        b.Append("t");
        b.Append(localI);
        b.Append(", ");
      }
      if(pReturnCount == startIndex){
        b.Append("t");
        b.Append(pReturnCount);
      }else{
        int pFieldCount = pReturnCount-startIndex+1;
        for(int argI=1; argI<=pFieldCount; argI++){
          b.Append("p.t");
          b.Append(argI);
          if(argI < pFieldCount){
            b.Append(", ");
          }
        }
      }
      b.Append(");\n\t\t\t}\n\n");
    }
  }

  public static void AddApplyMethods(StringBuilder b, int productNum){
    b.Append("\t\t\tpublic App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<");
    for(int genArg=1; genArg<=productNum; genArg++){
      b.Append("T");
      b.Append(genArg);
      b.Append(", ");
    }
    b.Append("R> function) where V : Applicative.Mu{\n\t\t\t\treturn Apply(instance, instance.Point(function));\n\t\t\t}\n\n");
    b.Append("\t\t\tpublic App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<");
    for(int genArg=1; genArg<=productNum; genArg++){
      b.Append("T");
      b.Append(genArg);
      b.Append(", ");
    }
    b.Append("R>> function) where V : Applicative.Mu{\n\t\t\t\treturn instance.Ap");
    if(productNum > 1){
      b.Append(productNum);
    }
    b.Append("(function, ");
    for(int apArg=1; apArg<=productNum; apArg++){
      b.Append("t");
      b.Append(apArg);
      if(apArg < productNum){
        b.Append(", ");
      }
    }
    b.Append(");\n\t\t\t}\n");
  }
  
  public static void Main(string[] args){
    StringBuilder b = new StringBuilder();
    for(int i=1; i<=8; i++){
      AddClassDef(b, i);
      AddFields(b, i);
      AddConstructor(b, i);
      AddGetters(b, i);
      AddAndMethods(b, i);
      AddApplyMethods(b, i);
      b.Append("\t\t}\n\n");
    }
    for(int i=9; i<=16; i++){
      AddClassDef(b, i);
      AddFields(b, i);
      AddConstructor(b, i);
      AddApplyMethods(b, i);
      b.Append("\t\t}\n\n");
    }
    Console.WriteLine(b);
  }
}*/

        public sealed class P1<F, T1> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;


            //Constructor
            public P1(App<F, T1> t1In){
                t1 = t1In;
            }


            //Instance methods
            public App<F, T1> GetT1(){
                return t1;
            }

            public P2<F, T1, T2> And<T2>(App<F, T2> t2){
                return new P2<F, T1, T2>(t1, t2);
            }

            public P3<F, T1, T2, T3> And<T2, T3>(P2<F, T2, T3> p){
                return new P3<F, T1, T2, T3>(t1, p.t1, p.t2);
            }

            public P4<F, T1, T2, T3, T4> And<T2, T3, T4>(P3<F, T2, T3, T4> p){
                return new P4<F, T1, T2, T3, T4>(t1, p.t1, p.t2, p.t3);
            }

            public P5<F, T1, T2, T3, T4, T5> And<T2, T3, T4, T5>(P4<F, T2, T3, T4, T5> p){
                return new P5<F, T1, T2, T3, T4, T5>(t1, p.t1, p.t2, p.t3, p.t4);
            }

            public P6<F, T1, T2, T3, T4, T5, T6> And<T2, T3, T4, T5, T6>(P5<F, T2, T3, T4, T5, T6> p){
                return new P6<F, T1, T2, T3, T4, T5, T6>(t1, p.t1, p.t2, p.t3, p.t4, p.t5);
            }

            public P7<F, T1, T2, T3, T4, T5, T6, T7> And<T2, T3, T4, T5, T6, T7>(P6<F, T2, T3, T4, T5, T6, T7> p){
                return new P7<F, T1, T2, T3, T4, T5, T6, T7>(t1, p.t1, p.t2, p.t3, p.t4, p.t5, p.t6);
            }

            public P8<F, T1, T2, T3, T4, T5, T6, T7, T8> And<T2, T3, T4, T5, T6, T7, T8>(P7<F, T2, T3, T4, T5, T6, T7, T8> p){
                return new P8<F, T1, T2, T3, T4, T5, T6, T7, T8>(t1, p.t1, p.t2, p.t3, p.t4, p.t5, p.t6, p.t7);
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, R>> function) where V : Applicative.Mu{
                return instance.Ap(function, t1);
            }
        }

        public sealed class P2<F, T1, T2> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;


            //Constructor
            public P2(App<F, T1> t1In, App<F, T2> t2In){
                t1 = t1In;
                t2 = t2In;
            }


            //Instance methods
            public App<F, T1> GetT1(){
                return t1;
            }

            public App<F, T2> GetT2(){
                return t2;
            }

            public P3<F, T1, T2, T3> And<T3>(App<F, T3> t3){
                return new P3<F, T1, T2, T3>(t1, t2, t3);
            }

            public P4<F, T1, T2, T3, T4> And<T3, T4>(P2<F, T3, T4> p){
                return new P4<F, T1, T2, T3, T4>(t1, t2, p.t1, p.t2);
            }

            public P5<F, T1, T2, T3, T4, T5> And<T3, T4, T5>(P3<F, T3, T4, T5> p){
                return new P5<F, T1, T2, T3, T4, T5>(t1, t2, p.t1, p.t2, p.t3);
            }

            public P6<F, T1, T2, T3, T4, T5, T6> And<T3, T4, T5, T6>(P4<F, T3, T4, T5, T6> p){
                return new P6<F, T1, T2, T3, T4, T5, T6>(t1, t2, p.t1, p.t2, p.t3, p.t4);
            }

            public P7<F, T1, T2, T3, T4, T5, T6, T7> And<T3, T4, T5, T6, T7>(P5<F, T3, T4, T5, T6, T7> p){
                return new P7<F, T1, T2, T3, T4, T5, T6, T7>(t1, t2, p.t1, p.t2, p.t3, p.t4, p.t5);
            }

            public P8<F, T1, T2, T3, T4, T5, T6, T7, T8> And<T3, T4, T5, T6, T7, T8>(P6<F, T3, T4, T5, T6, T7, T8> p){
                return new P8<F, T1, T2, T3, T4, T5, T6, T7, T8>(t1, t2, p.t1, p.t2, p.t3, p.t4, p.t5, p.t6);
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, R>> function) where V : Applicative.Mu{
                return instance.Ap2(function, t1, t2);
            }
        }

        public sealed class P3<F, T1, T2, T3> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;
            internal readonly App<F, T3> t3;


            //Constructor
            public P3(App<F, T1> t1In, App<F, T2> t2In, App<F, T3> t3In){
                t1 = t1In;
                t2 = t2In;
                t3 = t3In;
            }


            //Instance methods
            public App<F, T1> GetT1(){
                return t1;
            }

            public App<F, T2> GetT2(){
                return t2;
            }

            public App<F, T3> GetT3(){
                return t3;
            }

            public P4<F, T1, T2, T3, T4> And<T4>(App<F, T4> t4){
                return new P4<F, T1, T2, T3, T4>(t1, t2, t3, t4);
            }

            public P5<F, T1, T2, T3, T4, T5> And<T4, T5>(P2<F, T4, T5> p){
                return new P5<F, T1, T2, T3, T4, T5>(t1, t2, t3, p.t1, p.t2);
            }

            public P6<F, T1, T2, T3, T4, T5, T6> And<T4, T5, T6>(P3<F, T4, T5, T6> p){
                return new P6<F, T1, T2, T3, T4, T5, T6>(t1, t2, t3, p.t1, p.t2, p.t3);
            }

            public P7<F, T1, T2, T3, T4, T5, T6, T7> And<T4, T5, T6, T7>(P4<F, T4, T5, T6, T7> p){
                return new P7<F, T1, T2, T3, T4, T5, T6, T7>(t1, t2, t3, p.t1, p.t2, p.t3, p.t4);
            }

            public P8<F, T1, T2, T3, T4, T5, T6, T7, T8> And<T4, T5, T6, T7, T8>(P5<F, T4, T5, T6, T7, T8> p){
                return new P8<F, T1, T2, T3, T4, T5, T6, T7, T8>(t1, t2, t3, p.t1, p.t2, p.t3, p.t4, p.t5);
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, T3, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, T3, R>> function) where V : Applicative.Mu{
                return instance.Ap3(function, t1, t2, t3);
            }
        }

        public sealed class P4<F, T1, T2, T3, T4> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;
            internal readonly App<F, T3> t3;
            internal readonly App<F, T4> t4;


            //Constructor
            public P4(App<F, T1> t1In, App<F, T2> t2In, App<F, T3> t3In, App<F, T4> t4In){
                t1 = t1In;
                t2 = t2In;
                t3 = t3In;
                t4 = t4In;
            }


            //Instance methods
            public App<F, T1> GetT1(){
                return t1;
            }

            public App<F, T2> GetT2(){
                return t2;
            }

            public App<F, T3> GetT3(){
                return t3;
            }

            public App<F, T4> GetT4(){
                return t4;
            }

            public P5<F, T1, T2, T3, T4, T5> And<T5>(App<F, T5> t5){
                return new P5<F, T1, T2, T3, T4, T5>(t1, t2, t3, t4, t5);
            }

            public P6<F, T1, T2, T3, T4, T5, T6> And<T5, T6>(P2<F, T5, T6> p){
                return new P6<F, T1, T2, T3, T4, T5, T6>(t1, t2, t3, t4, p.t1, p.t2);
            }

            public P7<F, T1, T2, T3, T4, T5, T6, T7> And<T5, T6, T7>(P3<F, T5, T6, T7> p){
                return new P7<F, T1, T2, T3, T4, T5, T6, T7>(t1, t2, t3, t4, p.t1, p.t2, p.t3);
            }

            public P8<F, T1, T2, T3, T4, T5, T6, T7, T8> And<T5, T6, T7, T8>(P4<F, T5, T6, T7, T8> p){
                return new P8<F, T1, T2, T3, T4, T5, T6, T7, T8>(t1, t2, t3, t4, p.t1, p.t2, p.t3, p.t4);
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, T3, T4, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, T3, T4, R>> function) where V : Applicative.Mu{
                return instance.Ap4(function, t1, t2, t3, t4);
            }
        }

        public sealed class P5<F, T1, T2, T3, T4, T5> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;
            internal readonly App<F, T3> t3;
            internal readonly App<F, T4> t4;
            internal readonly App<F, T5> t5;


            //Constructor
            public P5(App<F, T1> t1In, App<F, T2> t2In, App<F, T3> t3In, App<F, T4> t4In, App<F, T5> t5In){
                t1 = t1In;
                t2 = t2In;
                t3 = t3In;
                t4 = t4In;
                t5 = t5In;
            }


            //Instance methods
            public App<F, T1> GetT1(){
                return t1;
            }

            public App<F, T2> GetT2(){
                return t2;
            }

            public App<F, T3> GetT3(){
                return t3;
            }

            public App<F, T4> GetT4(){
                return t4;
            }

            public App<F, T5> GetT5(){
                return t5;
            }

            public P6<F, T1, T2, T3, T4, T5, T6> And<T6>(App<F, T6> t6){
                return new P6<F, T1, T2, T3, T4, T5, T6>(t1, t2, t3, t4, t5, t6);
            }

            public P7<F, T1, T2, T3, T4, T5, T6, T7> And<T6, T7>(P2<F, T6, T7> p){
                return new P7<F, T1, T2, T3, T4, T5, T6, T7>(t1, t2, t3, t4, t5, p.t1, p.t2);
            }

            public P8<F, T1, T2, T3, T4, T5, T6, T7, T8> And<T6, T7, T8>(P3<F, T6, T7, T8> p){
                return new P8<F, T1, T2, T3, T4, T5, T6, T7, T8>(t1, t2, t3, t4, t5, p.t1, p.t2, p.t3);
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, T3, T4, T5, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, T3, T4, T5, R>> function) where V : Applicative.Mu{
                return instance.Ap5(function, t1, t2, t3, t4, t5);
            }
        }

        public sealed class P6<F, T1, T2, T3, T4, T5, T6> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;
            internal readonly App<F, T3> t3;
            internal readonly App<F, T4> t4;
            internal readonly App<F, T5> t5;
            internal readonly App<F, T6> t6;


            //Constructor
            public P6(App<F, T1> t1In, App<F, T2> t2In, App<F, T3> t3In, App<F, T4> t4In, App<F, T5> t5In, App<F, T6> t6In){
                t1 = t1In;
                t2 = t2In;
                t3 = t3In;
                t4 = t4In;
                t5 = t5In;
                t6 = t6In;
            }


            //Instance methods
            public App<F, T1> GetT1(){
                return t1;
            }

            public App<F, T2> GetT2(){
                return t2;
            }

            public App<F, T3> GetT3(){
                return t3;
            }

            public App<F, T4> GetT4(){
                return t4;
            }

            public App<F, T5> GetT5(){
                return t5;
            }

            public App<F, T6> GetT6(){
                return t6;
            }

            public P7<F, T1, T2, T3, T4, T5, T6, T7> And<T7>(App<F, T7> t7){
                return new P7<F, T1, T2, T3, T4, T5, T6, T7>(t1, t2, t3, t4, t5, t6, t7);
            }

            public P8<F, T1, T2, T3, T4, T5, T6, T7, T8> And<T7, T8>(P2<F, T7, T8> p){
                return new P8<F, T1, T2, T3, T4, T5, T6, T7, T8>(t1, t2, t3, t4, t5, t6, p.t1, p.t2);
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, T3, T4, T5, T6, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, T3, T4, T5, T6, R>> function) where V : Applicative.Mu{
                return instance.Ap6(function, t1, t2, t3, t4, t5, t6);
            }
        }

        public sealed class P7<F, T1, T2, T3, T4, T5, T6, T7> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;
            internal readonly App<F, T3> t3;
            internal readonly App<F, T4> t4;
            internal readonly App<F, T5> t5;
            internal readonly App<F, T6> t6;
            internal readonly App<F, T7> t7;


            //Constructor
            public P7(App<F, T1> t1In, App<F, T2> t2In, App<F, T3> t3In, App<F, T4> t4In, App<F, T5> t5In, App<F, T6> t6In, App<F, T7> t7In){
                t1 = t1In;
                t2 = t2In;
                t3 = t3In;
                t4 = t4In;
                t5 = t5In;
                t6 = t6In;
                t7 = t7In;
            }


            //Instance methods
            public App<F, T1> GetT1(){
                return t1;
            }

            public App<F, T2> GetT2(){
                return t2;
            }

            public App<F, T3> GetT3(){
                return t3;
            }

            public App<F, T4> GetT4(){
                return t4;
            }

            public App<F, T5> GetT5(){
                return t5;
            }

            public App<F, T6> GetT6(){
                return t6;
            }

            public App<F, T7> GetT7(){
                return t7;
            }

            public P8<F, T1, T2, T3, T4, T5, T6, T7, T8> And<T8>(App<F, T8> t8){
                return new P8<F, T1, T2, T3, T4, T5, T6, T7, T8>(t1, t2, t3, t4, t5, t6, t7, t8);
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, T3, T4, T5, T6, T7, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, T3, T4, T5, T6, T7, R>> function) where V : Applicative.Mu{
                return instance.Ap7(function, t1, t2, t3, t4, t5, t6, t7);
            }
        }

        public sealed class P8<F, T1, T2, T3, T4, T5, T6, T7, T8> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;
            internal readonly App<F, T3> t3;
            internal readonly App<F, T4> t4;
            internal readonly App<F, T5> t5;
            internal readonly App<F, T6> t6;
            internal readonly App<F, T7> t7;
            internal readonly App<F, T8> t8;


            //Constructor
            public P8(App<F, T1> t1In, App<F, T2> t2In, App<F, T3> t3In, App<F, T4> t4In, App<F, T5> t5In, App<F, T6> t6In, App<F, T7> t7In, App<F, T8> t8In){
                t1 = t1In;
                t2 = t2In;
                t3 = t3In;
                t4 = t4In;
                t5 = t5In;
                t6 = t6In;
                t7 = t7In;
                t8 = t8In;
            }


            //Instance methods
            public App<F, T1> GetT1(){
                return t1;
            }

            public App<F, T2> GetT2(){
                return t2;
            }

            public App<F, T3> GetT3(){
                return t3;
            }

            public App<F, T4> GetT4(){
                return t4;
            }

            public App<F, T5> GetT5(){
                return t5;
            }

            public App<F, T6> GetT6(){
                return t6;
            }

            public App<F, T7> GetT7(){
                return t7;
            }

            public App<F, T8> GetT8(){
                return t8;
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, R>> function) where V : Applicative.Mu{
                return instance.Ap8(function, t1, t2, t3, t4, t5, t6, t7, t8);
            }
        }

        public sealed class P9<F, T1, T2, T3, T4, T5, T6, T7, T8, T9> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;
            internal readonly App<F, T3> t3;
            internal readonly App<F, T4> t4;
            internal readonly App<F, T5> t5;
            internal readonly App<F, T6> t6;
            internal readonly App<F, T7> t7;
            internal readonly App<F, T8> t8;
            internal readonly App<F, T9> t9;


            //Constructor
            public P9(App<F, T1> t1In, App<F, T2> t2In, App<F, T3> t3In, App<F, T4> t4In, App<F, T5> t5In, App<F, T6> t6In, App<F, T7> t7In, App<F, T8> t8In, App<F, T9> t9In){
                t1 = t1In;
                t2 = t2In;
                t3 = t3In;
                t4 = t4In;
                t5 = t5In;
                t6 = t6In;
                t7 = t7In;
                t8 = t8In;
                t9 = t9In;
            }


            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>> function) where V : Applicative.Mu{
                return instance.Ap9(function, t1, t2, t3, t4, t5, t6, t7, t8, t9);
            }
        }

        public sealed class P10<F, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;
            internal readonly App<F, T3> t3;
            internal readonly App<F, T4> t4;
            internal readonly App<F, T5> t5;
            internal readonly App<F, T6> t6;
            internal readonly App<F, T7> t7;
            internal readonly App<F, T8> t8;
            internal readonly App<F, T9> t9;
            internal readonly App<F, T10> t10;


            //Constructor
            public P10(App<F, T1> t1In, App<F, T2> t2In, App<F, T3> t3In, App<F, T4> t4In, App<F, T5> t5In, App<F, T6> t6In, App<F, T7> t7In, App<F, T8> t8In, App<F, T9> t9In, App<F, T10> t10In){
                t1 = t1In;
                t2 = t2In;
                t3 = t3In;
                t4 = t4In;
                t5 = t5In;
                t6 = t6In;
                t7 = t7In;
                t8 = t8In;
                t9 = t9In;
                t10 = t10In;
            }


            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>> function) where V : Applicative.Mu{
                return instance.Ap10(function, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10);
            }
        }

        public sealed class P11<F, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;
            internal readonly App<F, T3> t3;
            internal readonly App<F, T4> t4;
            internal readonly App<F, T5> t5;
            internal readonly App<F, T6> t6;
            internal readonly App<F, T7> t7;
            internal readonly App<F, T8> t8;
            internal readonly App<F, T9> t9;
            internal readonly App<F, T10> t10;
            internal readonly App<F, T11> t11;


            //Constructor
            public P11(App<F, T1> t1In, App<F, T2> t2In, App<F, T3> t3In, App<F, T4> t4In, App<F, T5> t5In, App<F, T6> t6In, App<F, T7> t7In, App<F, T8> t8In, App<F, T9> t9In, App<F, T10> t10In, App<F, T11> t11In){
                t1 = t1In;
                t2 = t2In;
                t3 = t3In;
                t4 = t4In;
                t5 = t5In;
                t6 = t6In;
                t7 = t7In;
                t8 = t8In;
                t9 = t9In;
                t10 = t10In;
                t11 = t11In;
            }


            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>> function) where V : Applicative.Mu{
                return instance.Ap11(function, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
            }
        }

        public sealed class P12<F, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;
            internal readonly App<F, T3> t3;
            internal readonly App<F, T4> t4;
            internal readonly App<F, T5> t5;
            internal readonly App<F, T6> t6;
            internal readonly App<F, T7> t7;
            internal readonly App<F, T8> t8;
            internal readonly App<F, T9> t9;
            internal readonly App<F, T10> t10;
            internal readonly App<F, T11> t11;
            internal readonly App<F, T12> t12;


            //Constructor
            public P12(App<F, T1> t1In, App<F, T2> t2In, App<F, T3> t3In, App<F, T4> t4In, App<F, T5> t5In, App<F, T6> t6In, App<F, T7> t7In, App<F, T8> t8In, App<F, T9> t9In, App<F, T10> t10In, App<F, T11> t11In, App<F, T12> t12In){
                t1 = t1In;
                t2 = t2In;
                t3 = t3In;
                t4 = t4In;
                t5 = t5In;
                t6 = t6In;
                t7 = t7In;
                t8 = t8In;
                t9 = t9In;
                t10 = t10In;
                t11 = t11In;
                t12 = t12In;
            }


            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>> function) where V : Applicative.Mu{
                return instance.Ap12(function, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12);
            }
        }

        public sealed class P13<F, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;
            internal readonly App<F, T3> t3;
            internal readonly App<F, T4> t4;
            internal readonly App<F, T5> t5;
            internal readonly App<F, T6> t6;
            internal readonly App<F, T7> t7;
            internal readonly App<F, T8> t8;
            internal readonly App<F, T9> t9;
            internal readonly App<F, T10> t10;
            internal readonly App<F, T11> t11;
            internal readonly App<F, T12> t12;
            internal readonly App<F, T13> t13;


            //Constructor
            public P13(App<F, T1> t1In, App<F, T2> t2In, App<F, T3> t3In, App<F, T4> t4In, App<F, T5> t5In, App<F, T6> t6In, App<F, T7> t7In, App<F, T8> t8In, App<F, T9> t9In, App<F, T10> t10In, App<F, T11> t11In, App<F, T12> t12In, App<F, T13> t13In){
                t1 = t1In;
                t2 = t2In;
                t3 = t3In;
                t4 = t4In;
                t5 = t5In;
                t6 = t6In;
                t7 = t7In;
                t8 = t8In;
                t9 = t9In;
                t10 = t10In;
                t11 = t11In;
                t12 = t12In;
                t13 = t13In;
            }


            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>> function) where V : Applicative.Mu{
                return instance.Ap13(function, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13);
            }
        }

        public sealed class P14<F, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;
            internal readonly App<F, T3> t3;
            internal readonly App<F, T4> t4;
            internal readonly App<F, T5> t5;
            internal readonly App<F, T6> t6;
            internal readonly App<F, T7> t7;
            internal readonly App<F, T8> t8;
            internal readonly App<F, T9> t9;
            internal readonly App<F, T10> t10;
            internal readonly App<F, T11> t11;
            internal readonly App<F, T12> t12;
            internal readonly App<F, T13> t13;
            internal readonly App<F, T14> t14;


            //Constructor
            public P14(App<F, T1> t1In, App<F, T2> t2In, App<F, T3> t3In, App<F, T4> t4In, App<F, T5> t5In, App<F, T6> t6In, App<F, T7> t7In, App<F, T8> t8In, App<F, T9> t9In, App<F, T10> t10In, App<F, T11> t11In, App<F, T12> t12In, App<F, T13> t13In, App<F, T14> t14In){
                t1 = t1In;
                t2 = t2In;
                t3 = t3In;
                t4 = t4In;
                t5 = t5In;
                t6 = t6In;
                t7 = t7In;
                t8 = t8In;
                t9 = t9In;
                t10 = t10In;
                t11 = t11In;
                t12 = t12In;
                t13 = t13In;
                t14 = t14In;
            }


            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>> function) where V : Applicative.Mu{
                return instance.Ap14(function, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
            }
        }

        public sealed class P15<F, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;
            internal readonly App<F, T3> t3;
            internal readonly App<F, T4> t4;
            internal readonly App<F, T5> t5;
            internal readonly App<F, T6> t6;
            internal readonly App<F, T7> t7;
            internal readonly App<F, T8> t8;
            internal readonly App<F, T9> t9;
            internal readonly App<F, T10> t10;
            internal readonly App<F, T11> t11;
            internal readonly App<F, T12> t12;
            internal readonly App<F, T13> t13;
            internal readonly App<F, T14> t14;
            internal readonly App<F, T15> t15;


            //Constructor
            public P15(App<F, T1> t1In, App<F, T2> t2In, App<F, T3> t3In, App<F, T4> t4In, App<F, T5> t5In, App<F, T6> t6In, App<F, T7> t7In, App<F, T8> t8In, App<F, T9> t9In, App<F, T10> t10In, App<F, T11> t11In, App<F, T12> t12In, App<F, T13> t13In, App<F, T14> t14In, App<F, T15> t15In){
                t1 = t1In;
                t2 = t2In;
                t3 = t3In;
                t4 = t4In;
                t5 = t5In;
                t6 = t6In;
                t7 = t7In;
                t8 = t8In;
                t9 = t9In;
                t10 = t10In;
                t11 = t11In;
                t12 = t12In;
                t13 = t13In;
                t14 = t14In;
                t15 = t15In;
            }


            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>> function) where V : Applicative.Mu{
                return instance.Ap15(function, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15);
            }
        }

        public sealed class P16<F, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> where F : K1{
            //Fields
            internal readonly App<F, T1> t1;
            internal readonly App<F, T2> t2;
            internal readonly App<F, T3> t3;
            internal readonly App<F, T4> t4;
            internal readonly App<F, T5> t5;
            internal readonly App<F, T6> t6;
            internal readonly App<F, T7> t7;
            internal readonly App<F, T8> t8;
            internal readonly App<F, T9> t9;
            internal readonly App<F, T10> t10;
            internal readonly App<F, T11> t11;
            internal readonly App<F, T12> t12;
            internal readonly App<F, T13> t13;
            internal readonly App<F, T14> t14;
            internal readonly App<F, T15> t15;
            internal readonly App<F, T16> t16;


            //Constructor
            public P16(App<F, T1> t1In, App<F, T2> t2In, App<F, T3> t3In, App<F, T4> t4In, App<F, T5> t5In, App<F, T6> t6In, App<F, T7> t7In, App<F, T8> t8In, App<F, T9> t9In, App<F, T10> t10In, App<F, T11> t11In, App<F, T12> t12In, App<F, T13> t13In, App<F, T14> t14In, App<F, T15> t15In, App<F, T16> t16In){
                t1 = t1In;
                t2 = t2In;
                t3 = t3In;
                t4 = t4In;
                t5 = t5In;
                t6 = t6In;
                t7 = t7In;
                t8 = t8In;
                t9 = t9In;
                t10 = t10In;
                t11 = t11In;
                t12 = t12In;
                t13 = t13In;
                t14 = t14In;
                t15 = t15In;
                t16 = t16In;
            }


            public App<F, R> Apply<R, V>(Applicative<F, V> instance, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> function) where V : Applicative.Mu{
                return Apply(instance, instance.Point(function));
            }

            public App<F, R> Apply<R, V>(Applicative<F, V> instance, App<F, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>> function) where V : Applicative.Mu{
                return instance.Ap16(function, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16);
            }
        }
    }
}
