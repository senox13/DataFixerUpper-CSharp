using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace DataFixerUpper.Serialization{
    public abstract class ListBuilder<T>{
        /*
         * Abstract methods
         */
        public abstract DynamicOps<T> Ops();

        public abstract DataResult<T> Build(T prefix);

        public abstract ListBuilder<T> Add(T value);

        public abstract ListBuilder<T> Add(DataResult<T> value);

        public abstract ListBuilder<T> WithErrorsFrom<R>(DataResult<R> result);

        public abstract ListBuilder<T> MapError(Func<string, string> onError);


        /*
         * Concrete methods
         */
        public virtual DataResult<T> Build(DataResult<T> prefix){
            return prefix.FlatMap(t => Build(t));
        }

        public virtual ListBuilder<T> Add<E>(E value, IEncoder<E> encoder){
            return Add(encoder.EncodeStart(Ops(), value));
        }

        public virtual ListBuilder<T> AddAll<E>(IEnumerable<E> values, IEncoder<E> encoder){
            foreach(E value in values){
                encoder.Encode(value, Ops(), Ops().Empty());
            }
            return this;
        }


        /*
         * Nested types
         */
        public sealed class Builder : ListBuilder<T>{
            /*
             * Fields
             */
            private readonly DynamicOps<T> ops;
            private DataResult<ImmutableList<T>.Builder> builder = DataResult.Success(ImmutableList.CreateBuilder<T>(), Lifecycle.Stable());


            /*
             * Constructor
             */
            public Builder(DynamicOps<T> opsIn){
                ops = opsIn;
            }


            /*
             * ListBuilder override methods
             */
            public override DynamicOps<T> Ops(){
                return ops;
            }
            
            public override ListBuilder<T> Add(T value){
                builder = builder.Map(b => {
                    b.Add(value);
                    return b;
                });
                return this;
            }
            
            public override ListBuilder<T> Add(DataResult<T> value){
                builder = builder.Apply2Stable((builder, t) => {
                    builder.Add(t);
                    return builder;
                }, value);
                return this;
            }
            
            public override ListBuilder<T> WithErrorsFrom<R>(DataResult<R> result){
                builder = builder.FlatMap(r => result.Map(v => r));
                return this;
            }
            
            public override ListBuilder<T> MapError(Func<string, string> onError){
                builder = builder.MapError(onError);
                return this;
            }
            
            public override DataResult<T> Build(T prefix){
                DataResult<T> result = builder.FlatMap(b => ops.MergeToList(prefix, b.ToImmutable()));
                builder = DataResult.Success(ImmutableList.CreateBuilder<T>(), Lifecycle.Stable());
                return result;
            }
        }
    }
}
