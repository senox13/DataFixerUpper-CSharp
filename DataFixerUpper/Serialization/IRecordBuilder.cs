using System;
using System.Collections.Immutable;

namespace DataFixerUpper.Serialization{
    public abstract class RecordBuilder<T>{
        /*
         * Abstract methods
         */
        public abstract DynamicOps<T> Ops();

        public abstract RecordBuilder<T> Add(T key, T value);

        public abstract RecordBuilder<T> Add(T key, DataResult<T> value);

        public abstract RecordBuilder<T> Add(DataResult<T> key, DataResult<T> value);

        public abstract RecordBuilder<T> WithErrorsFrom<O>(DataResult<O> result);

        public abstract RecordBuilder<T> SetLifecycle(Lifecycle lifecycle);

        public abstract RecordBuilder<T> MapError(Func<string, string> onError);

        public abstract DataResult<T> Build(T prefix);


        /*
         * Virtual methods
         */
        public virtual DataResult<T> Build(DataResult<T> prefix){
            return prefix.FlatMap(Build);
        }

        public virtual RecordBuilder<T> Add(string key, T value){
            return Add(Ops().CreateString(key), value);
        }

        public virtual RecordBuilder<T> Add(string key, DataResult<T> value){
            return Add(Ops().CreateString(key), value);
        }

        public virtual RecordBuilder<T> Add<E>(string key, E value, IEncoder<E> encoder){
            return Add(key, encoder.EncodeStart(Ops(), value));
        }
    }

    public static class RecordBuilder{
        /*
         * Nested types
         */
        //Note that subclasses of this must assign a value to builder in their constructor
        //This feels fragile and necessitates code duplication, could stand to be refactored
        public abstract class AbstractBuilder<T, R> : RecordBuilder<T>{
            /*
             * Fields
             */
            private readonly DynamicOps<T> ops;
            protected DataResult<R> builder;
            

            /*
             * Constructor
             */
            protected AbstractBuilder(DynamicOps<T> opsIn){
                ops = opsIn;
            }


            /*
             * Abstract methods
             */
            protected abstract R InitBuilder();

            protected abstract DataResult<T> Build(R builder, T prefix);


            /*
             * RecordBuilder override methods
             */
            public override DynamicOps<T> Ops(){
                return ops;
            }

            public override DataResult<T> Build(T prefix){
                DataResult<T> result = builder.FlatMap(b => Build(b, prefix));
                builder = DataResult.Success(InitBuilder(), Lifecycle.Stable());
                return result;
            }

            public override RecordBuilder<T> WithErrorsFrom<O>(DataResult<O> result){
                builder = builder.FlatMap(v => result.Map(r => v));
                return this;
            }

            public override RecordBuilder<T> SetLifecycle(Lifecycle lifecycle){
                builder = builder.SetLifecycle(lifecycle);
                return this;
            }

            public override RecordBuilder<T> MapError(Func<string, string> onError){
                builder = builder.MapError(onError);
                return this;
            }
        }

        public abstract class AbstractStringBuilder<T, R> : AbstractBuilder<T, R>{
            /*
             * Constructor
             */
            protected AbstractStringBuilder(DynamicOps<T> opsIn)
                :base(opsIn){}


            /*
             * Abstract methods
             */
            public abstract R Append(string key, T value, R builder);


            /*
             * AbstractBuilder override methods
             */
            public override RecordBuilder<T> Add(string key, T value){
                builder = builder.Map(b => Append(key, value, b));
                return this;
            }

            public override RecordBuilder<T> Add(string key, DataResult<T> value){
                builder = builder.Apply2Stable((b, v) => Append(key, v, b), value);
                return this;
            }

            public override RecordBuilder<T> Add(T key, T value){
                builder = Ops().GetStringValue(key).FlatMap(k => {
                    Add(k, value);
                    return builder;
                });
                return this;
            }

            public override RecordBuilder<T> Add(T key, DataResult<T> value){
                builder = Ops().GetStringValue(key).FlatMap(k => {
                    Add(k, value);
                    return builder;
                });
                return this;
            }

            public override RecordBuilder<T> Add(DataResult<T> key, DataResult<T> value){
                builder = key.FlatMap(Ops().GetStringValue).FlatMap(k => {
                    Add(k, value);
                    return builder;
                });
                return this;
            }
        }

        public abstract class AbstractUniversalBuilder<T, R> : AbstractBuilder<T, R>{
            /*
             * Constructor
             */
            protected AbstractUniversalBuilder(DynamicOps<T> opsIn)
                :base(opsIn){}


            /*
             * Abstract methods
             */
            protected abstract R Append(T key, T value, R builder);


            /*
             * AbstractBuilder override methods
             */
            public override RecordBuilder<T> Add(T key, T value){
                builder = builder.Map(b => Append(key, value, b));
                return this;
            }

            public override RecordBuilder<T> Add(T key, DataResult<T> value){
                builder = builder.Apply2Stable((b, v) => Append(key, v, b), value);
                return this;
            }

            public override RecordBuilder<T> Add(DataResult<T> key, DataResult<T> value){
                builder = builder.Ap(key.Apply2Stable<T, Func<R, R>>((k, v) => b => Append(k, v, b), value));
                return this;
            }
        }
        
        public sealed class MapBuilder<T> : AbstractUniversalBuilder<T, ImmutableDictionary<T, T>.Builder>{
            /*
             * Constructor
             */
            public MapBuilder(DynamicOps<T> opsIn)
                :base(opsIn){
                builder = DataResult.Success(InitBuilder(), Lifecycle.Stable());
            }


            /*
             * AbstractUniversalBuilder override methods
             */
            protected override ImmutableDictionary<T, T>.Builder InitBuilder(){
                return ImmutableDictionary.CreateBuilder<T, T>();
            }

            protected override ImmutableDictionary<T, T>.Builder Append(T key, T value, ImmutableDictionary<T, T>.Builder builder){
                builder.Add(key, value);
                return builder;
            }

            protected override DataResult<T> Build(ImmutableDictionary<T, T>.Builder builder, T prefix){
                return Ops().MergeToMap(prefix, builder.ToImmutable());
            }
        }
    }
}
