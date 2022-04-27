﻿using System;
using DataFixerUpper.DataFixers.Kinds;
using DataFixerUpper.Util;

namespace DataFixerUpper.DataFixers.Util{
    public abstract class Either<L, R> : App<Either<L, R>.Mu, L>{
        public sealed class Mu : K1{}

        /*
         * Constructor
         */
        private Either(){}


        //TODO: Move static methods to separate static class
        /*
         * Static methods
         */
        public static Either<L, R> Left(L value){
            return new LeftImpl(value);
        }

        public static Either<L, R> Right(R value){
            return new RightImpl(value);
        }


        /*
         * Abstract methods
         */
        public abstract Either<L1, R1> MapBoth<L1, R1>(Func<L, L1> leftFunc, Func<R, R1> rightFunc);

        public abstract T Map<T>(Func<L, T> leftFunc, Func<R, T> rightFunc);

        public abstract Either<L, R> IfLeft(Action<L> consumer);

        public abstract Either<L, R> IfRight(Action<R> consumer);

        public abstract Optional<L> Left();

        public abstract Optional<R> Right();


        /*
         * Instance methods
         */
        public L OrThrow(){
            return Map(
                l => l,
                r => {
                    if(r is Exception ex){
                        throw new SystemException($"{nameof(Either<L, R>)}.{nameof(OrThrow)}", ex);
                    }
                    throw new SystemException(r.ToString());
                }
            );
        }

        public Either<R, L> Swap(){
            return (Either<R, L>)Map<object>(Either<R, L>.Right, Either<R, L>.Left);
        }

        public Either<L1, R> MapLeft<L1>(Func<L, L1> function){
            return (Either<L1, R>)Map<object>(l => Either<L1, R>.Left(function(l)), Right);
        }

        public Either<L, R1> MapRight<R1>(Func<R, R1> function){
            return (Either<L, R1>)Map<object>(Left, r => Either<L, R1>.Right(function(r)));
        }


        /*
         * Nested types
         */
        private sealed class LeftImpl : Either<L, R>{
            /*
             * Fields
             */
            private readonly L value;


            /*
             * Constructor
             */
            public LeftImpl(L valueIn){
                value = valueIn;
            }

            
            /*
             * Either override methods
             */
            public override Either<L1, R1> MapBoth<L1, R1>(Func<L, L1> leftFunc, Func<R, R1> rightFunc){
                return new Either<L1, R1>.LeftImpl(leftFunc(value));
            }

            public override T Map<T>(Func<L, T> leftFunc, Func<R, T> rightFunc){
                return leftFunc(value);
            }

            public override Either<L, R> IfLeft(Action<L> consumer){
                consumer(value);
                return this;
            }

            public override Either<L, R> IfRight(Action<R> consumer){
                return this;
            }
            
            public override Optional<L> Left(){
                return Optional<L>.Of(value);
            }
            
            public override Optional<R> Right(){
                return Optional<R>.Empty();
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return $"Left[{value}]";
            }

            public override bool Equals(object obj){
                if(obj == this){
                    return true;
                }
                if(obj is LeftImpl other){
                    return value.Equals(other.value);
                }
                return false;
            }

            public override int GetHashCode(){
                return ObjectUtils.Hash(value);
            }
        }

        private sealed class RightImpl : Either<L, R>{
            /*
             * Fields
             */
            private readonly R value;


            /*
             * Constructor
             */
            public RightImpl(R valueIn){
                value = valueIn;
            }

            
            /*
             * Either override methods
             */
            public override Either<L1, R1> MapBoth<L1, R1>(Func<L, L1> leftFunc, Func<R, R1> rightFunc){
                return new Either<L1, R1>.RightImpl(rightFunc(value));
            }

            public override T Map<T>(Func<L, T> leftFunc, Func<R, T> rightFunc){
                return rightFunc(value);
            }

            public override Either<L, R> IfLeft(Action<L> consumer){
                return this;
            }

            public override Either<L, R> IfRight(Action<R> consumer){
                consumer(value);
                return this;
            }
            
            public override Optional<L> Left(){
                return Optional<L>.Empty();
            }
            
            public override Optional<R> Right(){
                return Optional<R>.Of(value);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return $"Right[{value}]";
            }

            public override bool Equals(object obj){
                if(obj == this){
                    return true;
                }
                if(obj is RightImpl other){
                    return value.Equals(other.value);
                }
                return false;
            }

            public override int GetHashCode(){
                return ObjectUtils.Hash(value);
            }
        }
    }
}
