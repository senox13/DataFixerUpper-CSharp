using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;
using JavaUtilities;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.Serialization{
    public static class Dynamic{
        /*
         * Static methods
         */
        public static T Convert<S, T>(DynamicOps<S> inOps, DynamicOps<T> outOps, S input){
            if(ObjectUtils.Equals(inOps, outOps))
                return (T)(object)input;
            return inOps.ConvertTo(outOps, input);
        }
    }

    public interface IDynamic{
        Dynamic<R> Convert<R>(DynamicOps<R> outOps);
    }

    public class Dynamic<T> : DynamicLike<T>, IDynamic{
        /*
         * Fields
         */
        private readonly T value;


        /*
         * Constructors
         */
        public Dynamic(DynamicOps<T> opsIn)
            :this(opsIn, opsIn.Empty()){}

        public Dynamic(DynamicOps<T> opsIn, T valueIn)
            :base(opsIn){
            value = valueIn==null ? opsIn.Empty() : valueIn;
        }


        /*
         * Public methods
         */
        public T GetValue(){
            return value;
        }

        public Dynamic<T> Map(Func<T, T> function){
            return new Dynamic<T>(ops, function.Invoke(value));
        }

        //HACK: These two methods may be unnecessary due to reified generics in C#
        public Dynamic<U> CastTyped<U>(DynamicOps<U> ops){
            if(!ObjectUtils.Equals(this.ops, ops))
                throw new InvalidOperationException("Dynamic type doesn't match");
            return (Dynamic<U>)(object)this;
        }

        public U Cast<U>(DynamicOps<U> ops){
            return CastTyped(ops).GetValue();
        }

        public OptionalDynamic<T> Merge(Dynamic<T> value){
            DataResult<T> merged = ops.MergeToList(this.value, value.Cast(ops));
            return new OptionalDynamic<T>(ops, merged.Map(m => new Dynamic<T>(ops, m)));
        }

        public OptionalDynamic<T> Merge(Dynamic<T> key, Dynamic<T> value){
            DataResult<T> merged = ops.MergeToMap(this.value, key.Cast(ops), value.Cast(ops));
            return new OptionalDynamic<T>(ops, merged.Map(m => new Dynamic<T>(ops, m)));
        }

        public DataResult<IDictionary<Dynamic<T>, Dynamic<T>>> GetMapValues(){
            return ops.GetMapValues(value).Map(map => {
                ImmutableDictionary<Dynamic<T>, Dynamic<T>>.Builder builder = ImmutableDictionary.CreateBuilder<Dynamic<T>, Dynamic<T>>();
                foreach(Pair<T, T> entry in map){
                    builder.Add(new Dynamic<T>(ops, entry.GetFirst()), new Dynamic<T>(ops, entry.GetSecond()));
                }
                return (IDictionary<Dynamic<T>, Dynamic<T>>)builder.ToImmutable();
            });
        }

        public Dynamic<T> UpdateMapValues(Func<Pair<Dynamic<T>, Dynamic<T>>, Pair<Dynamic<T>, Dynamic<T>>> updater){
            return GetMapValues().Map(map => Pair.ToDict(map.Select(e => {
                Pair<Dynamic<T>, Dynamic<T>> pair = updater.Invoke(Pair.Of(e.Key, e.Value));
                return Pair.Of(pair.GetFirst().CastTyped(ops), pair.GetSecond().CastTyped(ops));
            }))).Map(CreateMap).Result().OrElse(this);
        }

        public Dynamic<T> Remove(string key){
            return Map(v => ops.Remove(v, key));
        }

        public Dynamic<T> Set(string key, Dynamic<T> value){
            return Map(v => ops.Set(v, key, value.Cast(ops)));
        }

        public Dynamic<T> Update(string key, Func<Dynamic<T>, Dynamic<T>> function){
            return Map(v => ops.Update(v, key, value => function.Invoke(new Dynamic<T>(ops, value)).Cast(ops)));
        }

        public Dynamic<T> UpdateGeneric(T key, Func<T, T> function){
            return Map(v => ops.UpdateGeneric(v, key, function));
        }

        public V Into<V>(Func<Dynamic<T>, V> action){
            return action.Invoke(this);
        }

        
        /*
         * IDynamic implementation
         */
        public Dynamic<R> Convert<R>(DynamicOps<R> outOps){
            return new Dynamic<R>(outOps, Dynamic.Convert(ops, outOps, value));
        }


        /*
         * DynamicLike override methods
         */
        public override DataResult<decimal> AsNumber(){
            return ops.GetNumberValue(value);
        }

        public override DataResult<string> AsString(){
            return ops.GetStringValue(value);
        }

        public override DataResult<IEnumerable<Dynamic<T>>> AsEnumerableOpt(){
            return ops.GetEnumerable(value).Map(s => s.Select(e => new Dynamic<T>(ops, e)));
        }

        public override DataResult<IEnumerable<Pair<Dynamic<T>, Dynamic<T>>>> AsMapOpt(){
            return ops.GetMapValues(value).Map(s => s.Select(p => Pair.Of(new Dynamic<T>(ops, p.GetFirst()), new Dynamic<T>(ops, p.GetSecond()))));
        }
        
        public override DataResult<MemoryStream> AsMemoryStreamOpt(){
            return ops.GetMemoryStream(value);
        }
        
        public override DataResult<IEnumerable<int>> AsIntEnumerableOpt(){
            return ops.GetIntEnumerable(value);
        }
        
        public override DataResult<IEnumerable<long>> AsLongEnumerableOpt(){
            return ops.GetLongEnumerable(value);
        }
        
        public override OptionalDynamic<T> Get(string key){
            return new OptionalDynamic<T>(ops, ops.GetMap(value).FlatMap(m => {
                T value = m.Get(key);
                if(value == null)
                    return DataResult.Error<Dynamic<T>>($"key missing: {key} in {this.value}");
                return DataResult.Success(new Dynamic<T>(ops, value));
            }));
        }
        
        public override DataResult<T> GetGeneric(T key){
            return ops.GetGeneric(value, key);
        }
        
        public override DataResult<T> GetElement(string key){
            return GetElementGeneric(ops.CreateString(key));
        }
        
        public override DataResult<T> GetElementGeneric(T key){
            return ops.GetGeneric(value, key);
        }
        
        public override DataResult<Pair<A, T>> Decode<A>(IDecoder<A> decoder){
            return decoder.Decode(ops, value);
        }


        /*
         * Object override methods
         */
        public override bool Equals(object obj){
            if(this == obj)
                return true;
            if(obj is Dynamic<T> other)
                return ops.Equals(other.ops) && value.Equals(other.value);
            return false;
        }

        public override int GetHashCode(){
            return ObjectUtils.Hash(ops, value);
        }

        public override string ToString(){
            return $"{ops}[{value}]";
        }
    }
}
