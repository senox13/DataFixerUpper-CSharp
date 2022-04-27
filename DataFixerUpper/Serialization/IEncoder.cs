using System;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Serialization.Codecs;

namespace DataFixerUpper.Serialization{
    public interface IEncoder<A>{
        DataResult<T> Encode<T>(A input, DynamicOps<T> ops, T prefix);
    }

    public static class Encoder{
        /*
         * Static methods
         */
        public static IMapEncoder<A> Empty<A>(){
            return new EmptyEncoder<A>();
        }

        public static IEncoder<A> Error<A>(string error){
            return new ErrorEncoder<A>(error);
        }

        internal static IMapEncoder<A> FieldOfUnchecked<A>(IEncoder<A> encoder, string name){
            return new FieldEncoder<A>(name, encoder);
        }


        /*
         * IEncoder extension methods
         */
        public static DataResult<T> EncodeStart<A, T>(this IEncoder<A> encoder, DynamicOps<T> ops, A input){
            return encoder.Encode(input, ops, ops.Empty());
        }

        public static IMapEncoder<A> FieldOf<A>(this IEncoder<A> encoder, string name){
            if(encoder is ICodec<A> codec){
                return codec.FieldOf(name);
            }
            return FieldOfUnchecked(encoder, name);
        }

        public static IEncoder<B> Comap<A, B>(this IEncoder<A> encoder, Func<B, A> func){
            return new ComapWrapper<A, B>(encoder, func);
        }

        public static IEncoder<B> FlatComap<A, B>(this IEncoder<A> encoder, Func<B, DataResult<A>> func){
            return new FlatComapWrapper<A, B>(encoder, func);
        }

        public static IEncoder<A> WithLifecycle<A>(this IEncoder<A> encoder, Lifecycle lifecycle){
            if(encoder is ICodec<A> codec){
                return codec.WithLifecycle(lifecycle);
            }
            return new LifecycleWrapper<A>(encoder, lifecycle);
        }


        /*
         * Nested types
         */
        private sealed class ComapWrapper<A, B> : IEncoder<B>{
            /*
             * Fields
             */
            public readonly IEncoder<A> wrapped;
            public readonly Func<B, A> function;


            /*
             * Constructor
             */
            public ComapWrapper(IEncoder<A> toWrap, Func<B, A> functionIn){
                wrapped = toWrap;
                function = functionIn;
            }


            /*
             * IEncoder implementation
             */
            public DataResult<T> Encode<T>(B input, DynamicOps<T> ops, T prefix){
                return wrapped.Encode(function(input), ops, prefix);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return wrapped.ToString() + "[comapped]";
            }
        }

        private sealed class FlatComapWrapper<A, B> : IEncoder<B>{
            /*
             * Fields
             */
            public readonly IEncoder<A> wrapped;
            public readonly Func<B, DataResult<A>> function;


            /*
             * Constructor
             */
            public FlatComapWrapper(IEncoder<A> toWrap, Func<B, DataResult<A>> functionIn){
                wrapped = toWrap;
                function = functionIn;
            }


            /*
             * IEncoder implementation
             */
            public DataResult<T> Encode<T>(B input, DynamicOps<T> ops, T prefix){
                return function(input).FlatMap(a => wrapped.Encode(a, ops, prefix));
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return wrapped.ToString();
            }
        }

        private sealed class LifecycleWrapper<A> : IEncoder<A>{
            /*
             * Fields
             */
            public readonly IEncoder<A> wrapped;
            public readonly Lifecycle lifecycle;


            /*
             * Constructor
             */
            public LifecycleWrapper(IEncoder<A> toWrap, Lifecycle lifecycleIn){
                wrapped = toWrap;
                lifecycle = lifecycleIn;
            }


            /*
             * IEncoder implementation
             */
            public DataResult<T> Encode<T>(A input, DynamicOps<T> ops, T prefix){
                return wrapped.Encode(input, ops, prefix).SetLifecycle(lifecycle);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return wrapped.ToString();
            }
        }

        private sealed class EmptyEncoder<A> : MapEncoder.Implementation<A>{
            /*
             * MapEncoder.Implementation override methods
             */
            public override RecordBuilder<T> Encode<T>(A input, DynamicOps<T> ops, RecordBuilder<T> prefix){
                return prefix;
            }

            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return Enumerable.Empty<T>();
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return "EmptyEncoder";
            }
        }

        private sealed class ErrorEncoder<A> : IEncoder<A>{
            /*
             * Fields
             */
            private readonly string error;


            /*
             * Constructor
             */
            public ErrorEncoder(string errorIn){
                error = errorIn;
            }


            /*
             * IEncoder implementation
             */
            public DataResult<T> Encode<T>(A input, DynamicOps<T> ops, T prefix){
                return DataResult.Error<T>($"{error} {input}");
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return $"ErrorEncoder[{error}]";
            }
        }
    }
}
