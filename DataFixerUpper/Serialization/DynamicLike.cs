using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using DataFixerUpper.DataFixers.Kinds;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.Serialization{
    public abstract class DynamicLike{
        /*
         * Public methods
         */
        public DynamicLike<T> Disambiguate<T>(){
            if(this is DynamicLike<T> cast)
                return cast;
            throw new InvalidCastException($"Cannot cast {this.GetType()} to {typeof(DynamicLike<T>)}");
        }
    }


    public abstract class DynamicLike<T> : DynamicLike{
        /*
         * Fields
         */
        protected readonly DynamicOps<T> ops;


        /*
         * Constructor
         */
        public DynamicLike(DynamicOps<T> opsIn){
            if(opsIn == null)
                throw new ArgumentNullException(nameof(opsIn), "DynamicOps cannot be null");
            ops = opsIn;
        }


        /*
         * Abstract methods
         */
        public abstract DataResult<decimal> AsNumber();

        public abstract DataResult<string> AsString();

        public abstract DataResult<IEnumerable<Dynamic<T>>> AsEnumerableOpt();

        public abstract DataResult<IEnumerable<Pair<Dynamic<T>, Dynamic<T>>>> AsMapOpt();

        public abstract DataResult<MemoryStream> AsMemoryStreamOpt();

        public abstract DataResult<IEnumerable<int>> AsIntEnumerableOpt();

        public abstract DataResult<IEnumerable<long>> AsLongEnumerableOpt();

        public abstract OptionalDynamic<T> Get(string key);

        public abstract DataResult<T> GetGeneric(T key);

        public abstract DataResult<T> GetElement(string key);

        public abstract DataResult<T> GetElementGeneric(T key);

        public abstract DataResult<Pair<A, T>> Decode<A>(IDecoder<A> decoder);


        /*
         * Public methods
         */
        public DynamicOps<T> GetOps(){
            return ops;
        }

        public DataResult<IList<U>> AsListOpt<U>(Func<Dynamic<T>, U> deserializer){
            return AsEnumerableOpt().Map(stream => (IList<U>)stream.Select(deserializer).ToList());
        }

        public DataResult<IDictionary<K, V>> AsMapOpt<K, V>(Func<Dynamic<T>, K> keyDeserializer, Func<Dynamic<T>, V> valueDeserializer){
            return AsMapOpt().Map(map => {
                ImmutableDictionary<K, V>.Builder builder = ImmutableDictionary.CreateBuilder<K, V>();
                foreach(Pair<Dynamic<T>, Dynamic<T>> entry in map){
                    builder.Add(keyDeserializer.Invoke(entry.GetFirst()), valueDeserializer.Invoke(entry.GetSecond()));
                }
                return (IDictionary<K, V>)builder.ToImmutable();
            });
        }

        public DataResult<A> Read<A>(IDecoder<A> decoder){
            return Decode(decoder).Map(p => p.GetFirst());
        }

        public DataResult<IList<E>> ReadList<E>(IDecoder<E> decoder){
            return AsEnumerableOpt()
                .Map(s => s.Select(d => d.Read(decoder)).ToList<App<DataResult.Mu, E>>())
                .FlatMap(l => DataResult.Unbox(ListBox.Flip(DataResult.GetInstance(), l)));
        }

        public DataResult<IList<E>> ReadList<E>(Func<Dynamic<T>, DataResult<E>> decoder){
            return AsEnumerableOpt()
                .Map(s => s.Select(decoder).ToList<App<DataResult.Mu, E>>())
                .FlatMap(l => DataResult.Unbox(ListBox.Flip(DataResult.GetInstance(), l)));
        }

        public DataResult<IList<Pair<K, V>>> ReadMap<K, V>(IDecoder<K> keyDecoder, IDecoder<V> valueDecoder){
            return AsMapOpt()
                .Map(e => e.Select(p => p.GetFirst().Read(keyDecoder).FlatMap(f => p.GetSecond().Read(valueDecoder).Map(s => Pair.Of(f, s)))).ToList<App<DataResult.Mu, Pair<K, V>>>())
                .FlatMap(l => DataResult.Unbox(ListBox.Flip(DataResult.GetInstance(), l)));
        }

        public DataResult<IList<Pair<K, V>>> ReadMap<K, V>(IDecoder<K> keyDecoder, Func<K, IDecoder<V>> valueDecoder){
            return AsMapOpt()
                .Map(e => e.Select(p => p.GetFirst().Read(keyDecoder).FlatMap(f => p.GetSecond().Read(valueDecoder.Invoke(f)).Map(s => Pair.Of(f, s)))).ToList<App<DataResult.Mu, Pair<K, V>>>())
                .FlatMap(l => DataResult.Unbox(ListBox.Flip(DataResult.GetInstance(), l)));
        }

        public DataResult<R> ReadMap<R>(DataResult<R> empty, Func<R, Dynamic<T>, Dynamic<T>, DataResult<R>> combiner){
            return AsMapOpt().FlatMap(e => {
                DataResult<R> result = empty;
                foreach(Pair<Dynamic<T>, Dynamic<T>> p in e){
                    result = result.FlatMap(r => combiner.Invoke(r, p.GetFirst(), p.GetSecond()));
                }
                return result;
            });
        }

        public decimal AsNumber(decimal defaultValue){
            return AsNumber().Result().OrElse(defaultValue);
        }

        public int AsInt(int defaultValue){
            return (int)AsNumber(defaultValue);
        }

        public long AsLong(long defaultValue){
            return (long)AsNumber(defaultValue);
        }

        public float AsFloat(float defaultValue){
            return (float)AsNumber((decimal)defaultValue);
        }

        public double AsDouble(double defaultValue){
            return (double)AsNumber((decimal)defaultValue);
        }

        public sbyte AsByte(sbyte defaultValue){
            return (sbyte)AsNumber(defaultValue);
        }

        public short AsShort(short defaultValue){
            return (short)AsNumber(defaultValue);
        }

        public bool AsBoolean(bool defaultValue){
            return (int)AsNumber(defaultValue ? 1 : 0) != 0;
        }

        public string AsString(string defaultValue){
            return AsString().Result().OrElse(defaultValue);
        }

        public IEnumerable<Dynamic<T>> AsEnumerable(){
            return AsEnumerableOpt().Result().OrElseGet(Enumerable.Empty<Dynamic<T>>);
        }

        public MemoryStream AsMemoryStream(){
            return AsMemoryStreamOpt().Result().OrElseGet(() => new MemoryStream(new byte[0]));
        }

        public IEnumerable<int> AsIntEnumerable(){
            return AsIntEnumerableOpt().Result().OrElseGet(Enumerable.Empty<int>);
        }

        public IEnumerable<long> AsLongEnumerable(){
            return AsLongEnumerableOpt().Result().OrElseGet(Enumerable.Empty<long>);
        }

        public IList<U> AsList<U>(Func<Dynamic<T>, U> deserializer){
            return AsListOpt(deserializer).Result().OrElseGet(ImmutableList.Create<U>);
        }

        public IDictionary<K, V> AsMap<K, V>(Func<Dynamic<T>, K> keyDeserializer, Func<Dynamic<T>, V> valueDeserializer){
            return AsMapOpt(keyDeserializer, valueDeserializer).Result().OrElseGet(ImmutableDictionary.Create<K, V>);
        }

        public T GetElement(string key, T defaultValue){
            return GetElement(key).Result().OrElse(defaultValue);
        }

        public T GetElementGeneric(T key, T defaultValue){
            return GetElementGeneric(key).Result().OrElse(defaultValue);
        }

        public Dynamic<T> EmptyList(){
            return new Dynamic<T>(ops, ops.EmptyList());
        }

        public Dynamic<T> EmptyMap(){
            return new Dynamic<T>(ops, ops.EmptyMap());
        }

        public Dynamic<T> CreateNumeric(decimal i){
            return new Dynamic<T>(ops, ops.CreateNumeric(i));
        }

        public Dynamic<T> CreateByte(sbyte value){
            return new Dynamic<T>(ops, ops.CreateByte(value));
        }

        public Dynamic<T> CreateShort(short value){
            return new Dynamic<T>(ops, ops.CreateShort(value));
        }

        public Dynamic<T> CreateInt(int value){
            return new Dynamic<T>(ops, ops.CreateInt(value));
        }

        public Dynamic<T> CreateLong(long value){
            return new Dynamic<T>(ops, ops.CreateLong(value));
        }

        public Dynamic<T> CreateFloat(float value){
            return new Dynamic<T>(ops, ops.CreateFloat(value));
        }

        public Dynamic<T> CreateDouble(double value){
            return new Dynamic<T>(ops, ops.CreateDouble(value));
        }

        public Dynamic<T> CreateBool(bool value){
            return new Dynamic<T>(ops, ops.CreateBoolean(value));
        }

        public Dynamic<T> CreateString(string value){
            return new Dynamic<T>(ops, ops.CreateString(value));
        }

        public Dynamic<T> CreateList(IEnumerable<Dynamic<T>> input){
            return new Dynamic<T>(ops, ops.CreateList(input.Select(e => e.Cast(ops))));
        }

        public Dynamic<T> CreateMap(IDictionary<Dynamic<T>, Dynamic<T>> map){
            ImmutableDictionary<T, T>.Builder builder = ImmutableDictionary.CreateBuilder<T, T>();
            foreach(KeyValuePair<Dynamic<T>, Dynamic<T>> entry in map){
                builder.Add(entry.Key.Cast(ops), entry.Value.Cast(ops));
            }
            return new Dynamic<T>(ops, ops.CreateMap(builder.ToImmutable()));
        }

        public Dynamic<T> CreateByteList(MemoryStream input){
            return new Dynamic<T>(ops, ops.CreateByteList(input));
        }

        public Dynamic<T> CreateIntList(IEnumerable<int> input){
            return new Dynamic<T>(ops, ops.CreateIntList(input));
        }

        public Dynamic<T> CreateLongList(IEnumerable<long> input){
            return new Dynamic<T>(ops, ops.CreateLongList(input));
        }
    }
}
