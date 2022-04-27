using System;

namespace DataFixerUpper.DataFixers.Util{
    public static class FuncUtils{
        /*
         * Static methods
         */
        public static Func<T, T> Identity<T>(){
            return t => t;
        }


        /*
         * Func<T, R> extension methods
         */
        public static Func<T, V> AndThen<T, V, R>(this Func<T, R> func, Func<R, V> after){
            if(after == null){
                throw new ArgumentNullException(nameof(after));
            }
            return t => after.Invoke(func.Invoke(t));
        }


        /*
         * The following code was used to generate the below curry extension
         * methods. Should replace this with a source generator at some point.
        private const string TAB = "    ";

        public static void AppendSectionComment(StringBuilder b, int thisArgCount){
            string funcType = "Func<" + string.Join(", ", Enumerable.Range(1, thisArgCount).Select(i => $"T{i}")) + ", R>";
            b.Append($"\n{TAB}{TAB}/*");
            b.Append($"\n{TAB}{TAB} * {funcType} extension methods");
            b.Append($"\n{TAB}{TAB} * /\n");
        }

        public static void AppendCurryFunc(StringBuilder b, int thisArgCount, int returnArgCount){
            //Generic args
            string genericArgs = "<" + string.Join(", ", Enumerable.Range(1, thisArgCount).Select(i => $"T{i}")) + ", R>";
            //Return type
            string returnType = "Func<";
            for(int arg=1; arg<=returnArgCount; arg++){
                if(arg > 1){
                    returnType += ", ";
                }
                returnType += $"T{arg}";
            }
            returnType += ", Func<";
            for(int arg=returnArgCount+1; arg<=thisArgCount; arg++){
                returnType += $"T{arg}";
                if(arg < thisArgCount){
                    returnType += ", ";
                }
            }
            returnType += ", R>>";
            //Function args
            string arguments = "this Func<";
            for(int arg=1; arg<=thisArgCount; arg++){
                if(arg > 1){
                    arguments += ", ";
                }
                arguments += $"T{arg}";
            }
            arguments += ", R> func";
            //Function body
            string body = "return ";
            if(returnArgCount > 1){
                body += "(";
            }
            for(int arg=1; arg<=returnArgCount; arg++){
                if(arg > 1){
                    body += ", ";
                }
                body += $"T{arg}";
            }
            if(returnArgCount > 1){
                body += ")";
            }
            body += " => ";
            if(thisArgCount - returnArgCount > 1){
                body += "(";
            }
            for(int arg=returnArgCount+1; arg<=thisArgCount; arg++){
                body += $"T{arg}";
                if(arg < thisArgCount){
                    body += ", ";
                }
            }
            if(thisArgCount - returnArgCount > 1){
                body += ")";
            }
            body += " => func.Invoke(";
            for(int arg=1; arg<=thisArgCount; arg++){
                if(arg > 1){
                    body += ", ";
                }
                body += $"T{arg}";
            }
            body += ");";
            //Append to builder
            b.Append($"{TAB}{TAB}public static ");
            b.Append(returnType);
            b.Append(" Curry");
            if(returnArgCount > 1){
                b.Append(returnArgCount);
            }
            b.Append(genericArgs);
            b.Append("(");
            b.Append(arguments);
            b.Append(")");
            b.Append($"{{\n{TAB}{TAB}{TAB}");
            b.Append(body);
            b.Append($"\n{TAB}{TAB}}}\n\n");
        }
  
        public static void Main(string[] args){
            StringBuilder b = new StringBuilder();
            for(int argCount=2; argCount<=10; argCount++){
                AppendSectionComment(b, argCount);
                for(int curryCount=1; curryCount<argCount; curryCount++){
                    AppendCurryFunc(b, argCount, curryCount);
                }
            }
            Console.WriteLine(b);
        }*/

        /*
         * Func<T1, T2, R> extension methods
         */
        public static Func<T1, Func<T2, R>> Curry<T1, T2, R>(this Func<T1, T2, R> func){
            return T1 => T2 => func.Invoke(T1, T2);
        }


        /*
         * Func<T1, T2, T3, R> extension methods
         */
        public static Func<T1, Func<T2, T3, R>> Curry<T1, T2, T3, R>(this Func<T1, T2, T3, R> func){
            return T1 => (T2, T3) => func.Invoke(T1, T2, T3);
        }

        public static Func<T1, T2, Func<T3, R>> Curry2<T1, T2, T3, R>(this Func<T1, T2, T3, R> func){
            return (T1, T2) => T3 => func.Invoke(T1, T2, T3);
        }


        /*
         * Func<T1, T2, T3, T4, R> extension methods
         */
        public static Func<T1, Func<T2, T3, T4, R>> Curry<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> func){
            return T1 => (T2, T3, T4) => func.Invoke(T1, T2, T3, T4);
        }

        public static Func<T1, T2, Func<T3, T4, R>> Curry2<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> func){
            return (T1, T2) => (T3, T4) => func.Invoke(T1, T2, T3, T4);
        }

        public static Func<T1, T2, T3, Func<T4, R>> Curry3<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4, R> func){
            return (T1, T2, T3) => T4 => func.Invoke(T1, T2, T3, T4);
        }


        /*
         * Func<T1, T2, T3, T4, T5, R> extension methods
         */
        public static Func<T1, Func<T2, T3, T4, T5, R>> Curry<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> func){
            return T1 => (T2, T3, T4, T5) => func.Invoke(T1, T2, T3, T4, T5);
        }

        public static Func<T1, T2, Func<T3, T4, T5, R>> Curry2<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> func){
            return (T1, T2) => (T3, T4, T5) => func.Invoke(T1, T2, T3, T4, T5);
        }

        public static Func<T1, T2, T3, Func<T4, T5, R>> Curry3<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> func){
            return (T1, T2, T3) => (T4, T5) => func.Invoke(T1, T2, T3, T4, T5);
        }

        public static Func<T1, T2, T3, T4, Func<T5, R>> Curry4<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5, R> func){
            return (T1, T2, T3, T4) => T5 => func.Invoke(T1, T2, T3, T4, T5);
        }


        /*
         * Func<T1, T2, T3, T4, T5, T6, R> extension methods
         */
        public static Func<T1, Func<T2, T3, T4, T5, T6, R>> Curry<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> func){
            return T1 => (T2, T3, T4, T5, T6) => func.Invoke(T1, T2, T3, T4, T5, T6);
        }

        public static Func<T1, T2, Func<T3, T4, T5, T6, R>> Curry2<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> func){
            return (T1, T2) => (T3, T4, T5, T6) => func.Invoke(T1, T2, T3, T4, T5, T6);
        }

        public static Func<T1, T2, T3, Func<T4, T5, T6, R>> Curry3<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> func){
            return (T1, T2, T3) => (T4, T5, T6) => func.Invoke(T1, T2, T3, T4, T5, T6);
        }

        public static Func<T1, T2, T3, T4, Func<T5, T6, R>> Curry4<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> func){
            return (T1, T2, T3, T4) => (T5, T6) => func.Invoke(T1, T2, T3, T4, T5, T6);
        }

        public static Func<T1, T2, T3, T4, T5, Func<T6, R>> Curry5<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6, R> func){
            return (T1, T2, T3, T4, T5) => T6 => func.Invoke(T1, T2, T3, T4, T5, T6);
        }


        /*
         * Func<T1, T2, T3, T4, T5, T6, T7, R> extension methods
         */
        public static Func<T1, Func<T2, T3, T4, T5, T6, T7, R>> Curry<T1, T2, T3, T4, T5, T6, T7, R>(this Func<T1, T2, T3, T4, T5, T6, T7, R> func){
            return T1 => (T2, T3, T4, T5, T6, T7) => func.Invoke(T1, T2, T3, T4, T5, T6, T7);
        }

        public static Func<T1, T2, Func<T3, T4, T5, T6, T7, R>> Curry2<T1, T2, T3, T4, T5, T6, T7, R>(this Func<T1, T2, T3, T4, T5, T6, T7, R> func){
            return (T1, T2) => (T3, T4, T5, T6, T7) => func.Invoke(T1, T2, T3, T4, T5, T6, T7);
        }

        public static Func<T1, T2, T3, Func<T4, T5, T6, T7, R>> Curry3<T1, T2, T3, T4, T5, T6, T7, R>(this Func<T1, T2, T3, T4, T5, T6, T7, R> func){
            return (T1, T2, T3) => (T4, T5, T6, T7) => func.Invoke(T1, T2, T3, T4, T5, T6, T7);
        }

        public static Func<T1, T2, T3, T4, Func<T5, T6, T7, R>> Curry4<T1, T2, T3, T4, T5, T6, T7, R>(this Func<T1, T2, T3, T4, T5, T6, T7, R> func){
            return (T1, T2, T3, T4) => (T5, T6, T7) => func.Invoke(T1, T2, T3, T4, T5, T6, T7);
        }

        public static Func<T1, T2, T3, T4, T5, Func<T6, T7, R>> Curry5<T1, T2, T3, T4, T5, T6, T7, R>(this Func<T1, T2, T3, T4, T5, T6, T7, R> func){
            return (T1, T2, T3, T4, T5) => (T6, T7) => func.Invoke(T1, T2, T3, T4, T5, T6, T7);
        }

        public static Func<T1, T2, T3, T4, T5, T6, Func<T7, R>> Curry6<T1, T2, T3, T4, T5, T6, T7, R>(this Func<T1, T2, T3, T4, T5, T6, T7, R> func){
            return (T1, T2, T3, T4, T5, T6) => T7 => func.Invoke(T1, T2, T3, T4, T5, T6, T7);
        }


        /*
         * Func<T1, T2, T3, T4, T5, T6, T7, T8, R> extension methods
         */
        public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8, R>> Curry<T1, T2, T3, T4, T5, T6, T7, T8, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func){
            return T1 => (T2, T3, T4, T5, T6, T7, T8) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8);
        }

        public static Func<T1, T2, Func<T3, T4, T5, T6, T7, T8, R>> Curry2<T1, T2, T3, T4, T5, T6, T7, T8, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func){
            return (T1, T2) => (T3, T4, T5, T6, T7, T8) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8);
        }

        public static Func<T1, T2, T3, Func<T4, T5, T6, T7, T8, R>> Curry3<T1, T2, T3, T4, T5, T6, T7, T8, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func){
            return (T1, T2, T3) => (T4, T5, T6, T7, T8) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8);
        }

        public static Func<T1, T2, T3, T4, Func<T5, T6, T7, T8, R>> Curry4<T1, T2, T3, T4, T5, T6, T7, T8, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func){
            return (T1, T2, T3, T4) => (T5, T6, T7, T8) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8);
        }

        public static Func<T1, T2, T3, T4, T5, Func<T6, T7, T8, R>> Curry5<T1, T2, T3, T4, T5, T6, T7, T8, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func){
            return (T1, T2, T3, T4, T5) => (T6, T7, T8) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8);
        }

        public static Func<T1, T2, T3, T4, T5, T6, Func<T7, T8, R>> Curry6<T1, T2, T3, T4, T5, T6, T7, T8, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func){
            return (T1, T2, T3, T4, T5, T6) => (T7, T8) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, Func<T8, R>> Curry7<T1, T2, T3, T4, T5, T6, T7, T8, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, R> func){
            return (T1, T2, T3, T4, T5, T6, T7) => T8 => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8);
        }


        /*
         * Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> extension methods
         */
        public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8, T9, R>> Curry<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func){
            return T1 => (T2, T3, T4, T5, T6, T7, T8, T9) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9);
        }

        public static Func<T1, T2, Func<T3, T4, T5, T6, T7, T8, T9, R>> Curry2<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func){
            return (T1, T2) => (T3, T4, T5, T6, T7, T8, T9) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9);
        }

        public static Func<T1, T2, T3, Func<T4, T5, T6, T7, T8, T9, R>> Curry3<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func){
            return (T1, T2, T3) => (T4, T5, T6, T7, T8, T9) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9);
        }

        public static Func<T1, T2, T3, T4, Func<T5, T6, T7, T8, T9, R>> Curry4<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func){
            return (T1, T2, T3, T4) => (T5, T6, T7, T8, T9) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9);
        }

        public static Func<T1, T2, T3, T4, T5, Func<T6, T7, T8, T9, R>> Curry5<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func){
            return (T1, T2, T3, T4, T5) => (T6, T7, T8, T9) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9);
        }

        public static Func<T1, T2, T3, T4, T5, T6, Func<T7, T8, T9, R>> Curry6<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func){
            return (T1, T2, T3, T4, T5, T6) => (T7, T8, T9) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, Func<T8, T9, R>> Curry7<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func){
            return (T1, T2, T3, T4, T5, T6, T7) => (T8, T9) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Func<T9, R>> Curry8<T1, T2, T3, T4, T5, T6, T7, T8, T9, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8) => T9 => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9);
        }


        /*
         * Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> extension methods
         */
        public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, R>> Curry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func){
            return T1 => (T2, T3, T4, T5, T6, T7, T8, T9, T10) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10);
        }

        public static Func<T1, T2, Func<T3, T4, T5, T6, T7, T8, T9, T10, R>> Curry2<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func){
            return (T1, T2) => (T3, T4, T5, T6, T7, T8, T9, T10) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10);
        }

        public static Func<T1, T2, T3, Func<T4, T5, T6, T7, T8, T9, T10, R>> Curry3<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func){
            return (T1, T2, T3) => (T4, T5, T6, T7, T8, T9, T10) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10);
        }

        public static Func<T1, T2, T3, T4, Func<T5, T6, T7, T8, T9, T10, R>> Curry4<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func){
            return (T1, T2, T3, T4) => (T5, T6, T7, T8, T9, T10) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10);
        }

        public static Func<T1, T2, T3, T4, T5, Func<T6, T7, T8, T9, T10, R>> Curry5<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func){
            return (T1, T2, T3, T4, T5) => (T6, T7, T8, T9, T10) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10);
        }

        public static Func<T1, T2, T3, T4, T5, T6, Func<T7, T8, T9, T10, R>> Curry6<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func){
            return (T1, T2, T3, T4, T5, T6) => (T7, T8, T9, T10) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, Func<T8, T9, T10, R>> Curry7<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func){
            return (T1, T2, T3, T4, T5, T6, T7) => (T8, T9, T10) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Func<T9, T10, R>> Curry8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8) => (T9, T10) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Func<T10, R>> Curry9<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9) => T10 => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10);
        }


        /*
         * Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> extension methods
         */
        public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>> Curry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> func){
            return T1 => (T2, T3, T4, T5, T6, T7, T8, T9, T10, T11) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11);
        }

        public static Func<T1, T2, Func<T3, T4, T5, T6, T7, T8, T9, T10, T11, R>> Curry2<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> func){
            return (T1, T2) => (T3, T4, T5, T6, T7, T8, T9, T10, T11) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11);
        }

        public static Func<T1, T2, T3, Func<T4, T5, T6, T7, T8, T9, T10, T11, R>> Curry3<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> func){
            return (T1, T2, T3) => (T4, T5, T6, T7, T8, T9, T10, T11) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11);
        }

        public static Func<T1, T2, T3, T4, Func<T5, T6, T7, T8, T9, T10, T11, R>> Curry4<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> func){
            return (T1, T2, T3, T4) => (T5, T6, T7, T8, T9, T10, T11) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11);
        }

        public static Func<T1, T2, T3, T4, T5, Func<T6, T7, T8, T9, T10, T11, R>> Curry5<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> func){
            return (T1, T2, T3, T4, T5) => (T6, T7, T8, T9, T10, T11) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11);
        }

        public static Func<T1, T2, T3, T4, T5, T6, Func<T7, T8, T9, T10, T11, R>> Curry6<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> func){
            return (T1, T2, T3, T4, T5, T6) => (T7, T8, T9, T10, T11) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, Func<T8, T9, T10, T11, R>> Curry7<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> func){
            return (T1, T2, T3, T4, T5, T6, T7) => (T8, T9, T10, T11) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Func<T9, T10, T11, R>> Curry8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8) => (T9, T10, T11) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Func<T10, T11, R>> Curry9<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9) => (T10, T11) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Func<T11, R>> Curry10<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) => T11 => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11);
        }


        /*
         * Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> extension methods
         */
        public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>> Curry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> func){
            return T1 => (T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12);
        }

        public static Func<T1, T2, Func<T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>> Curry2<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> func){
            return (T1, T2) => (T3, T4, T5, T6, T7, T8, T9, T10, T11, T12) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12);
        }

        public static Func<T1, T2, T3, Func<T4, T5, T6, T7, T8, T9, T10, T11, T12, R>> Curry3<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> func){
            return (T1, T2, T3) => (T4, T5, T6, T7, T8, T9, T10, T11, T12) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12);
        }

        public static Func<T1, T2, T3, T4, Func<T5, T6, T7, T8, T9, T10, T11, T12, R>> Curry4<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> func){
            return (T1, T2, T3, T4) => (T5, T6, T7, T8, T9, T10, T11, T12) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12);
        }

        public static Func<T1, T2, T3, T4, T5, Func<T6, T7, T8, T9, T10, T11, T12, R>> Curry5<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> func){
            return (T1, T2, T3, T4, T5) => (T6, T7, T8, T9, T10, T11, T12) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12);
        }

        public static Func<T1, T2, T3, T4, T5, T6, Func<T7, T8, T9, T10, T11, T12, R>> Curry6<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> func){
            return (T1, T2, T3, T4, T5, T6) => (T7, T8, T9, T10, T11, T12) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, Func<T8, T9, T10, T11, T12, R>> Curry7<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> func){
            return (T1, T2, T3, T4, T5, T6, T7) => (T8, T9, T10, T11, T12) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Func<T9, T10, T11, T12, R>> Curry8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8) => (T9, T10, T11, T12) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Func<T10, T11, T12, R>> Curry9<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9) => (T10, T11, T12) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Func<T11, T12, R>> Curry10<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) => (T11, T12) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Func<T12, R>> Curry11<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11) => T12 => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12);
        }


        /*
         * Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> extension methods
         */
        public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>> Curry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> func){
            return T1 => (T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13);
        }

        public static Func<T1, T2, Func<T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>> Curry2<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> func){
            return (T1, T2) => (T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13);
        }

        public static Func<T1, T2, T3, Func<T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>> Curry3<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> func){
            return (T1, T2, T3) => (T4, T5, T6, T7, T8, T9, T10, T11, T12, T13) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13);
        }

        public static Func<T1, T2, T3, T4, Func<T5, T6, T7, T8, T9, T10, T11, T12, T13, R>> Curry4<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> func){
            return (T1, T2, T3, T4) => (T5, T6, T7, T8, T9, T10, T11, T12, T13) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13);
        }

        public static Func<T1, T2, T3, T4, T5, Func<T6, T7, T8, T9, T10, T11, T12, T13, R>> Curry5<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> func){
            return (T1, T2, T3, T4, T5) => (T6, T7, T8, T9, T10, T11, T12, T13) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13);
        }

        public static Func<T1, T2, T3, T4, T5, T6, Func<T7, T8, T9, T10, T11, T12, T13, R>> Curry6<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> func){
            return (T1, T2, T3, T4, T5, T6) => (T7, T8, T9, T10, T11, T12, T13) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, Func<T8, T9, T10, T11, T12, T13, R>> Curry7<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> func){
            return (T1, T2, T3, T4, T5, T6, T7) => (T8, T9, T10, T11, T12, T13) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Func<T9, T10, T11, T12, T13, R>> Curry8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8) => (T9, T10, T11, T12, T13) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Func<T10, T11, T12, T13, R>> Curry9<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9) => (T10, T11, T12, T13) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Func<T11, T12, T13, R>> Curry10<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) => (T11, T12, T13) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Func<T12, T13, R>> Curry11<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11) => (T12, T13) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Func<T13, R>> Curry12<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12) => T13 => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13);
        }


        /*
         * Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> extension methods
         */
        public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>> Curry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> func){
            return T1 => (T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14);
        }

        public static Func<T1, T2, Func<T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>> Curry2<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> func){
            return (T1, T2) => (T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14);
        }

        public static Func<T1, T2, T3, Func<T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>> Curry3<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> func){
            return (T1, T2, T3) => (T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14);
        }

        public static Func<T1, T2, T3, T4, Func<T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>> Curry4<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> func){
            return (T1, T2, T3, T4) => (T5, T6, T7, T8, T9, T10, T11, T12, T13, T14) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14);
        }

        public static Func<T1, T2, T3, T4, T5, Func<T6, T7, T8, T9, T10, T11, T12, T13, T14, R>> Curry5<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> func){
            return (T1, T2, T3, T4, T5) => (T6, T7, T8, T9, T10, T11, T12, T13, T14) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14);
        }

        public static Func<T1, T2, T3, T4, T5, T6, Func<T7, T8, T9, T10, T11, T12, T13, T14, R>> Curry6<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> func){
            return (T1, T2, T3, T4, T5, T6) => (T7, T8, T9, T10, T11, T12, T13, T14) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, Func<T8, T9, T10, T11, T12, T13, T14, R>> Curry7<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> func){
            return (T1, T2, T3, T4, T5, T6, T7) => (T8, T9, T10, T11, T12, T13, T14) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Func<T9, T10, T11, T12, T13, T14, R>> Curry8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8) => (T9, T10, T11, T12, T13, T14) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Func<T10, T11, T12, T13, T14, R>> Curry9<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9) => (T10, T11, T12, T13, T14) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Func<T11, T12, T13, T14, R>> Curry10<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) => (T11, T12, T13, T14) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Func<T12, T13, T14, R>> Curry11<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11) => (T12, T13, T14) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Func<T13, T14, R>> Curry12<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12) => (T13, T14) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Func<T14, R>> Curry13<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13) => T14 => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14);
        }


        /*
         * Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> extension methods
         */
        public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>> Curry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> func){
            return T1 => (T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15);
        }

        public static Func<T1, T2, Func<T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>> Curry2<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> func){
            return (T1, T2) => (T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15);
        }

        public static Func<T1, T2, T3, Func<T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>> Curry3<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> func){
            return (T1, T2, T3) => (T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15);
        }

        public static Func<T1, T2, T3, T4, Func<T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>> Curry4<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> func){
            return (T1, T2, T3, T4) => (T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15);
        }

        public static Func<T1, T2, T3, T4, T5, Func<T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>> Curry5<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> func){
            return (T1, T2, T3, T4, T5) => (T6, T7, T8, T9, T10, T11, T12, T13, T14, T15) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15);
        }

        public static Func<T1, T2, T3, T4, T5, T6, Func<T7, T8, T9, T10, T11, T12, T13, T14, T15, R>> Curry6<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> func){
            return (T1, T2, T3, T4, T5, T6) => (T7, T8, T9, T10, T11, T12, T13, T14, T15) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, Func<T8, T9, T10, T11, T12, T13, T14, T15, R>> Curry7<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> func){
            return (T1, T2, T3, T4, T5, T6, T7) => (T8, T9, T10, T11, T12, T13, T14, T15) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Func<T9, T10, T11, T12, T13, T14, T15, R>> Curry8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8) => (T9, T10, T11, T12, T13, T14, T15) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Func<T10, T11, T12, T13, T14, T15, R>> Curry9<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9) => (T10, T11, T12, T13, T14, T15) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Func<T11, T12, T13, T14, T15, R>> Curry10<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) => (T11, T12, T13, T14, T15) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Func<T12, T13, T14, T15, R>> Curry11<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11) => (T12, T13, T14, T15) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Func<T13, T14, T15, R>> Curry12<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12) => (T13, T14, T15) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Func<T14, T15, R>> Curry13<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13) => (T14, T15) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Func<T15, R>> Curry14<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14) => T15 => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15);
        }


        /*
         * Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> extension methods
         */
        public static Func<T1, Func<T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>> Curry<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return T1 => (T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }

        public static Func<T1, T2, Func<T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>> Curry2<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return (T1, T2) => (T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }

        public static Func<T1, T2, T3, Func<T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>> Curry3<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return (T1, T2, T3) => (T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }

        public static Func<T1, T2, T3, T4, Func<T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>> Curry4<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return (T1, T2, T3, T4) => (T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }

        public static Func<T1, T2, T3, T4, T5, Func<T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>> Curry5<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return (T1, T2, T3, T4, T5) => (T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }

        public static Func<T1, T2, T3, T4, T5, T6, Func<T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>> Curry6<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return (T1, T2, T3, T4, T5, T6) => (T7, T8, T9, T10, T11, T12, T13, T14, T15, T16) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, Func<T8, T9, T10, T11, T12, T13, T14, T15, T16, R>> Curry7<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return (T1, T2, T3, T4, T5, T6, T7) => (T8, T9, T10, T11, T12, T13, T14, T15, T16) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Func<T9, T10, T11, T12, T13, T14, T15, T16, R>> Curry8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8) => (T9, T10, T11, T12, T13, T14, T15, T16) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Func<T10, T11, T12, T13, T14, T15, T16, R>> Curry9<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9) => (T10, T11, T12, T13, T14, T15, T16) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Func<T11, T12, T13, T14, T15, T16, R>> Curry10<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) => (T11, T12, T13, T14, T15, T16) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Func<T12, T13, T14, T15, T16, R>> Curry11<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11) => (T12, T13, T14, T15, T16) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Func<T13, T14, T15, T16, R>> Curry12<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12) => (T13, T14, T15, T16) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Func<T14, T15, T16, R>> Curry13<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13) => (T14, T15, T16) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Func<T15, T16, R>> Curry14<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14) => (T15, T16) => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }

        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Func<T16, R>> Curry15<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R>(this Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, R> func){
            return (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15) => T16 => func.Invoke(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16);
        }
    }
}
