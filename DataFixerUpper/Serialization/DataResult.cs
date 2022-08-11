using System;
using DataFixerUpper.DataFixers.Kinds;
using DataFixerUpper.DataFixers.Util;
using JavaUtilities;

namespace DataFixerUpper.Serialization{
    public static class DataResult{
        public sealed class Mu : K1{}

        /*
         * Static methods
         */
        public static DataResult<R> Unbox<R>(App<Mu, R> box){
            return (DataResult<R>)box;
        }

        public static Instance GetInstance(){
            return Instance.INSTANCE;
        }

        public static DataResult<R> Success<R>(R result){
            return Success(result, Lifecycle.Experimental());
        }

        public static DataResult<R> Success<R>(R result, Lifecycle lifecycle){
            return new DataResult<R>(Either.Left<R, DataResult<R>.PartialResult>(result), lifecycle);
        }

        public static DataResult<R> Error<R>(string message){
            return Error<R>(message, Lifecycle.Experimental());
        }

        public static DataResult<R> Error<R>(string message, R partialResult){
            return Error(message, partialResult, Lifecycle.Experimental());
        }

        public static DataResult<R> Error<R>(string message, Lifecycle lifecycle){
            return new DataResult<R>(Either.Right<R, DataResult<R>.PartialResult>(new DataResult<R>.PartialResult(message, Optional.Empty<R>())), lifecycle);
        }

        public static DataResult<R> Error<R>(string message, R partialResult, Lifecycle lifecycle){
            return new DataResult<R>(Either.Right<R, DataResult<R>.PartialResult>(new DataResult<R>.PartialResult(message, Optional.Of(partialResult))), lifecycle);

        }

        public static Func<K, DataResult<V>> PartialGet<K, V>(Func<K, V> partialGet, Func<string> errorPrefix){
            return name => Optional.OfNullable(partialGet.Invoke(name)).Map(Success).OrElseGet(() => Error<V>(errorPrefix.Invoke() + name));
        }


        /*
         * Nested types
         */
        public sealed class Instance : Applicative<Mu, Instance.Mu>{
            public sealed class Mu : Applicative.Mu{}

            /*
             * Singleton instance
             */
            public static readonly Instance INSTANCE = new Instance();


            /*
             * Constructor
             */
            private Instance(){}


            /*
             * Applicative implementation
             */
            public override App<DataResult.Mu, R1> Map<T, R1>(Func<T, R1> func, App<DataResult.Mu, T> ts){
                return Unbox(ts).Map(func);
            }

            public override App<DataResult.Mu, A> Point<A>(A a){
                return Success(a);
            }

            public override Func<App<DataResult.Mu, A>, App<DataResult.Mu, R1>> Lift1<A, R1>(App<DataResult.Mu, Func<A, R1>> function){
                return fa => Ap(function, fa);
            }

            public override App<DataResult.Mu, R> Ap<A, R>(App<DataResult.Mu, Func<A, R>> func, App<DataResult.Mu, A> arg){
                return Unbox(arg).Ap(Unbox(func));
            }

            public override App<DataResult.Mu, R> Ap2<A, B, R>(App<DataResult.Mu, Func<A, B, R>> func, App<DataResult.Mu, A> a, App<DataResult.Mu, B> b){
                DataResult<Func<A, B, R>> fr = Unbox(func);
                DataResult<A> ra = Unbox(a);
                DataResult<B> rb = Unbox(b);

                if(fr.result.Left().IsPresent()
                    && ra.result.Left().IsPresent()
                    && rb.result.Left().IsPresent()
                ){
                    return new DataResult<R>(Either.Left<R, DataResult<R>.PartialResult>(fr.result.Left().Get().Invoke(
                        ra.result.Left().Get(),
                        rb.result.Left().Get()
                    )), fr.lifecycle.Add(ra.lifecycle).Add(rb.lifecycle));
                }

                return base.Ap2(func, a, b);
            }

            public override App<DataResult.Mu, R> Ap3<T1, T2, T3, R>(App<DataResult.Mu, Func<T1, T2, T3, R>> func, App<DataResult.Mu, T1> t1, App<DataResult.Mu, T2> t2, App<DataResult.Mu, T3> t3){
                DataResult<Func<T1, T2, T3, R>> fr = Unbox(func);
                DataResult<T1> dr1 = Unbox(t1);
                DataResult<T2> dr2 = Unbox(t2);
                DataResult<T3> dr3 = Unbox(t3);

                if(fr.result.Left().IsPresent()
                    && dr1.result.Left().IsPresent()
                    && dr2.result.Left().IsPresent()
                    && dr3.result.Left().IsPresent()
                ){
                    return new DataResult<R>(Either.Left<R, DataResult<R>.PartialResult>(fr.result.Left().Get().Invoke(
                        dr1.result.Left().Get(),
                        dr2.result.Left().Get(),
                        dr3.result.Left().Get()
                    )), fr.lifecycle.Add(dr1.lifecycle).Add(dr2.lifecycle).Add(dr3.lifecycle));
                }

                return base.Ap3(func, t1, t2, t3);
            }
        }
    }

    public class DataResult<R> : App<DataResult.Mu, R>{
        /*
         * Fields
         */
        internal readonly Either<R, PartialResult> result;
        internal readonly Lifecycle lifecycle;


        /*
         * Constructor
         */
        internal DataResult(Either<R, PartialResult> resultIn, Lifecycle lifecycleIn){
            result = resultIn;
            lifecycle = lifecycleIn;
        }


        /*
         * Static methods
         */
        private static DataResult<R> Create(Either<R, PartialResult> result, Lifecycle lifecycle){
            return new DataResult<R>(result, lifecycle);
        }

        private static string AppendMessages(string first, string second){
            return $"{first}; {second}";
        }


        /*
         * Instance methods
         */
        public Either<R, PartialResult> Get(){
            return result;
        }

        public Optional<R> Result(){
            return result.Left();
        }

        public Lifecycle Lifecycle(){
            return lifecycle;
        }

        public DataResult<R> SetLifecycle(Lifecycle newLifecycle){
            return Create(result, newLifecycle);
        }

        public DataResult<R> AddLifecycle(Lifecycle toAdd){
            return Create(result, lifecycle.Add(toAdd));
        }

        public Optional<R> ResultOrPartial(Action<string> onError){
            return result.Map(
                Optional.Of,
                r => {
                    onError.Invoke(r.message);
                    return r.partialResult;
                }
            );
        }

        public R GetOrThrow(bool allowPartial, Action<string> onError){
            return result.Map(
                l => l,
                r => {
                    onError.Invoke(r.message);
                    if(allowPartial && r.partialResult.IsPresent()){
                        return r.partialResult.Get();
                    }
                    throw new SystemException(r.message);
                }
            );
        }

        public Optional<PartialResult> Error(){
            return result.Right();
        }

        public DataResult<T> Map<T>(Func<R, T> function){
            return DataResult<T>.Create(result.MapBoth(
                function,
                r => new DataResult<T>.PartialResult(r.message, r.partialResult.Map(function))
            ), lifecycle);
        }

        public DataResult<R> PromotePartial(Action<string> onError){
            return result.Map(
                r => new DataResult<R>(Either.Left<R, PartialResult>(r), lifecycle), 
                r => {
                    onError(r.Message());
                    return r.partialResult
                        .Map(pr => new DataResult<R>(Either.Left<R, PartialResult>(pr), lifecycle))
                        .OrElseGet(() => Create(Either.Right<R, PartialResult>(r), lifecycle));
                }
            );
        }

        public DataResult<R2> FlatMap<R2>(Func<R, DataResult<R2>> function){
            return result.Map(
                l => {
                    DataResult<R2> second = function(l);
                    return DataResult<R2>.Create(second.Get(), lifecycle.Add(second.Lifecycle()));
                },
                r => r.partialResult.Map(value => {
                    DataResult<R2> second = function(value);
                    return DataResult<R2>.Create(Either.Right<R2, DataResult<R2>.PartialResult>(second.Get().Map(
                        l2 => new DataResult<R2>.PartialResult(r.message, Optional.Of(l2)),
                        r2 => new DataResult<R2>.PartialResult(AppendMessages(r.message, r2.message), r2.partialResult)
                    )), lifecycle.Add(second.Lifecycle()));
                }).OrElseGet(
                    () => DataResult<R2>.Create(Either.Right<R2, DataResult<R2>.PartialResult>(new DataResult<R2>.PartialResult(r.message, Optional.Empty<R2>())), lifecycle)
                )
            );
        }

        public DataResult<R2> Ap<R2>(DataResult<Func<R, R2>> functionResult){
            return DataResult<R2>.Create(result.Map(
                arg => functionResult.result.MapBoth(
                    func => func.Invoke(arg),
                    funcError => new DataResult<R2>.PartialResult(funcError.message, funcError.partialResult.Map(f => f.Invoke(arg)))
                ),
                argError => Either.Right<R2, DataResult<R2>.PartialResult>(functionResult.result.Map(
                    func => new DataResult<R2>.PartialResult(argError.message, argError.partialResult.Map(func)),
                    funcError => new DataResult<R2>.PartialResult(
                        AppendMessages(argError.message, funcError.message),
                        argError.partialResult.FlatMap(a => funcError.partialResult.Map(f => f.Invoke(a)))
                    )
                ))
            ), lifecycle.Add(functionResult.lifecycle));
        }

        public DataResult<S> Apply2<R2, S>(Func<R, R2, S> function, DataResult<R2> second){
            return DataResult.Unbox(DataResult.GetInstance().Apply2(function, this, second));
        }

        public DataResult<S> Apply2Stable<R2, S>(Func<R, R2, S> function, DataResult<R2> second){
            Applicative<DataResult.Mu, DataResult.Instance.Mu> instance = DataResult.GetInstance();
            DataResult<Func<R, R2, S>> f = DataResult.Unbox(instance.Point(function)).SetLifecycle(Serialization.Lifecycle.Stable());
            return DataResult.Unbox(instance.Ap2(f, this, second));
        }

        public DataResult<S> Apply3<R2, R3, S>(Func<R, R2, R3, S> function, DataResult<R2> second, DataResult<R3> third){
            return DataResult.Unbox(DataResult.GetInstance().Apply3(function, this, second, third));
        }

        public DataResult<R> SetPartial(Func<R> partial){
            return Create(result.MapRight(r => new PartialResult(r.message, Optional.Of(partial.Invoke()))), lifecycle);
        }

        public DataResult<R> SetPartial(R partial){
            return Create(result.MapRight(r => new PartialResult(r.message, Optional.Of(partial))), lifecycle);
        }

        public DataResult<R> MapError(Func<string, string> function){
            return Create(result.MapRight(r => new PartialResult(function.Invoke(r.message), r.partialResult)), lifecycle);
        }


        /*
         * Object override methods
         */
        public override bool Equals(object obj){
            if(obj == this){
                return true;
            }
            if(obj is DataResult<R> other){
                return ObjectUtils.Equals(result, other.result);
            }
            return false;
        }

        public override int GetHashCode(){
            return ObjectUtils.Hash(result);
        }

        public override string ToString(){
            return $"DataResult[{result}]";
        }


        /*
         * Nested types
         */
        public class PartialResult{
            /*
             * Fields
             */
            internal readonly string message;
            internal readonly Optional<R> partialResult;


            /*
             * Constructor
             */
            public PartialResult(string messageIn, Optional<R> partialResultIn){
                message = messageIn;
                partialResult = partialResultIn;
            }


            /*
             * Instance methods
             */
            public DataResult<R2>.PartialResult Map<R2>(Func<R, R2> function){
                return new DataResult<R2>.PartialResult(message, partialResult.Map(function));
            }

            public DataResult<R2>.PartialResult FlatMap<R2>(Func<R, DataResult<R2>.PartialResult> function){
                if(partialResult.IsPresent()){
                    DataResult<R2>.PartialResult result = function(partialResult.Get());
                    return new DataResult<R2>.PartialResult(AppendMessages(message, result.message), result.partialResult);
                }
                return new DataResult<R2>.PartialResult(message, Optional.Empty<R2>());
            }

            public string Message(){
                return message;
            }

            
            /*
             * Object override methods
             */
            public override bool Equals(object obj){
                if(obj == this){
                    return true;
                }
                if(obj is PartialResult other){
                    return message.Equals(other.message)
                        && partialResult.Equals(other.partialResult);
                }
                return false;
            }

            public override int GetHashCode(){
                return ObjectUtils.Hash(message, partialResult);
            }

            public override string ToString(){
                return $"DynamicException[{message} {partialResult}]";
            }
        }

        
    }
}
