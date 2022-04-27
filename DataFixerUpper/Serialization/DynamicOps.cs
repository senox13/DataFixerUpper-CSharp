using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataFixerUpper.DataFixers.Util;
using DataFixerUpper.Util;

namespace DataFixerUpper.Serialization{
    public abstract class DynamicOps<T>{
        /*
         * Abstract methods
         */
        public abstract T Empty();

        public abstract U ConvertTo<U>(DynamicOps<U> outOps, T input);

        public abstract DataResult<decimal> GetNumberValue(T input);

        public abstract T CreateNumeric(decimal value);

        public abstract DataResult<string> GetStringValue(T input);

        public abstract T CreateString(string value);

        public abstract DataResult<T> MergeToList(T list, T value);

        public abstract DataResult<T> MergeToMap(T map, T key, T value);

        public abstract DataResult<IEnumerable<Pair<T, T>>> GetMapValues(T input);

        public abstract T CreateMap(IEnumerable<Pair<T, T>> map);

        public abstract DataResult<IEnumerable<T>> GetEnumerable(T input);

        public abstract T CreateList(IEnumerable<T> input);

        public abstract T Remove(T input, string key);


        /*
         * Virtual methods
         */
        public virtual T EmptyMap(){
            return CreateMap(Enumerable.Empty<Pair<T, T>>());
        }

        public virtual T EmptyList(){
            return CreateList(Enumerable.Empty<T>());
        }

        public virtual decimal GetNumberValue(T input, decimal defaultValue){
            return GetNumberValue(input).Result().OrElse(defaultValue);
        }

        public virtual T CreateByte(sbyte value){
            return CreateNumeric(value);
        }

        public virtual T CreateShort(short value){
            return CreateNumeric(value);
        }

        public virtual T CreateInt(int value){
            return CreateNumeric(value);
        }

        public virtual T CreateLong(long value){
            return CreateNumeric(value);
        }

        public virtual T CreateFloat(float value){
            return CreateNumeric((decimal)value);
        }

        public virtual T CreateDouble(double value){
            return CreateNumeric((decimal)value);
        }

        public virtual DataResult<bool> GetBooleanValue(T input){
            return GetNumberValue(input).Map(number => decimal.ToByte(number) != 0);
        }

        public virtual T CreateBoolean(bool value){
            return CreateByte((sbyte)(value ? 0 : 1));
        }

        public virtual DataResult<T> MergeToList(T list, IList<T> values){
            DataResult<T> result = DataResult.Success(list);
            foreach(T value in values){
                result = result.FlatMap(r => MergeToList(r, value));
            }
            return result;
        }

        public virtual DataResult<T> MergeToMap(T map, IDictionary<T, T> values){
            return MergeToMap(map, MapLikeUtils.ForMap(values, this));
        }

        public virtual DataResult<T> MergeToMap(T map, IMapLike<T> values){
            DataResult<T> result = DataResult.Success(map);
            foreach(Pair<T, T> entry in values){
                result = result.FlatMap(r => MergeToMap(r, entry.GetFirst(), entry.GetSecond()));
            }
            return result;
        }

        public virtual DataResult<T> MergeToPrimitive(T prefix, T value){
            if(!prefix.Equals(Empty())){
                return DataResult.Error($"Do not know how to append a primitive value {value} to {prefix}", value);
            }
            return DataResult.Success(value);
        }

        public virtual DataResult<Action<Action<T, T>>> GetMapEntries(T input){
            return GetMapValues(input).Map<Action<Action<T, T>>>(s => c => {
                foreach(Pair<T, T> pair in s){
                    c(pair.GetFirst(), pair.GetSecond());
                }
            });
        }

        public virtual DataResult<IMapLike<T>> GetMap(T input){
            return GetMapValues(input).FlatMap(s => {
                try{
                    return DataResult.Success(MapLikeUtils.ForMap(Pair.ToDict(s), this));
                }catch(InvalidOperationException ex){
                    return DataResult.Error<IMapLike<T>>($"Error while building map: {ex.Message}");
                }
            });
        }

        public virtual T CreateMap(IDictionary<T, T> map){
            return CreateMap(map.Select(e => Pair.Of(e.Key, e.Value)));
        }

        public virtual DataResult<Action<Action<T>>> GetList(T input){
            return GetEnumerable(input).Map<Action<Action<T>>>(s => c => {
                foreach(T val in s){
                    c(val);
                }
            });
        }

        public virtual DataResult<MemoryStream> GetByteBuffer(T input){
            return GetEnumerable(input).FlatMap(e => {
                List<T> elements = e.ToList();
                if(!elements.All(elem => GetNumberValue(elem).Result().IsPresent())){
                    return DataResult.Error<MemoryStream>($"Some elements are not bytes: {input}");
                }
                MemoryStream result = new MemoryStream(elements.Count);
                foreach(T elem in elements){
                    result.WriteByte(decimal.ToByte(GetNumberValue(elem).Result().Get()));
                }
                return DataResult.Success(result);
            });
        }

        public virtual T CreateByteList(MemoryStream input){
            List<T> bytes = new List<T>();
            for(int i=0; i<input.Length; i++){
                bytes.Insert(i, CreateByte((sbyte)input.ReadByte()));
            }
            return CreateList(bytes);
        }

        public virtual DataResult<IEnumerable<int>> GetIntStream(T input){
            return GetEnumerable(input).FlatMap(e => {
                List<T> elements = e.ToList();
                if(!elements.All(elem => GetNumberValue(elem).Result().IsPresent())){
                    return DataResult.Error<IEnumerable<int>>($"Some elements are not ints: {input}");
                }
                return DataResult.Success(elements.Select(elem => 
                    decimal.ToInt32(GetNumberValue(elem).Result().Get())
                ));
            });
        }

        public virtual T CreateIntList(IEnumerable<int> input){
            return CreateList(input.Select(i => CreateInt(i)));
        }

        public virtual DataResult<IEnumerable<long>> GetLongStream(T input){
            return GetEnumerable(input).FlatMap(e => {
                List<T> elements = e.ToList();
                if(!elements.All(elem => GetNumberValue(elem).Result().IsPresent())){
                    return DataResult.Error<IEnumerable<long>>($"Some elements are not longs: {input}");
                }
                return DataResult.Success(elements.Select(elem => 
                    decimal.ToInt64(GetNumberValue(elem).Result().Get())
                ));
            });
        }

        public virtual T CreateLongList(IEnumerable<long> input){
            return CreateList(input.Select(i => CreateLong(i)));
        }

        public virtual bool CompressMaps(){
            return false;
        }

        public virtual DataResult<T> Get(T input, string key){
            return GetGeneric(input, CreateString(key));
        }

        public virtual DataResult<T> GetGeneric(T input, T key){
            return GetMap(input).FlatMap(map => Optional<T>.OfNullable(map.Get(key))
                .Map(t => DataResult.Success(t))
                .OrElseGet(() => DataResult.Error<T>($"No element {key} in the map {input}"))
            );
        }

        public virtual T Set(T input, string key, T value){
            return MergeToMap(input, CreateString(key), value).Result().OrElse(input);
        }

        public virtual T Update(T input, string key, Func<T, T> function){
            return Get(input, key).Map(value => Set(input, key, function(value))).Result().OrElse(input);
        }
        
        public virtual T UpdateGeneric(T input, T key, Func<T, T> function){
            return GetGeneric(input, key).FlatMap(value => MergeToMap(input, key, function(value))).Result().OrElse(input);
        }

        public virtual ListBuilder<T> ListBuilder(){
            return new ListBuilder<T>.Builder(this);
        }

        public virtual RecordBuilder<T> MapBuilder(){
            return new RecordBuilder.MapBuilder<T>(this);
        }

        public virtual Func<E, DataResult<T>> WithEncoder<E>(IEncoder<E> encoder){
            return e => encoder.EncodeStart(this, e);
        }

        public virtual Func<T, DataResult<Pair<E, T>>> WithDecoder<E>(IDecoder<E> decoder){
            return t => decoder.Decode(this, t);
        }

        public virtual Func<T, DataResult<E>> WithParser<E>(IDecoder<E> decoder){
            return t => decoder.Parse(this, t);
        }

        public virtual U ConvertList<U>(DynamicOps<U> outOps, T input){
            return outOps.CreateList(GetEnumerable(input).Result().OrElse(Enumerable.Empty<T>()).Select(e => ConvertTo(outOps, e)));
        }

        public virtual U ConvertMap<U>(DynamicOps<U> outOps, T input){
            return outOps.CreateMap(GetMapValues(input).Result().OrElse(Enumerable.Empty<Pair<T, T>>())
                .Select(e => Pair.Of(ConvertTo(outOps, e.GetFirst()), ConvertTo(outOps, e.GetSecond()))));
        }
    }
}
