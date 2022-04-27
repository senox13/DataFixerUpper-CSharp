using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using DataFixerUpper.DataFixers.Util;
using DataFixerUpper.Util;

namespace DataFixerUpper.Serialization{
    public class JsonOps : DynamicOps<JToken>{
        /*
         * Fields
         */
        public static readonly JsonOps INSTANCE = new JsonOps(false);
        public static readonly JsonOps COMPRESSED = new JsonOps(true);
        private static readonly JValue JSON_NULL = JValue.CreateNull();
        private readonly bool compressed;
        

        /*
         * Constructor
         */
        protected JsonOps(bool compressedIn){
            compressed = compressedIn;
        }


        /*
         * Private methods
         */
        private bool IsNumeric(JTokenType type){
            return type == JTokenType.Integer || type == JTokenType.Float;
        }


        /*
         * DynamicOps override methods
         */
        public override JToken Empty(){
            return JSON_NULL;
        }

        public override U ConvertTo<U>(DynamicOps<U> outOps, JToken input){
            switch(input.Type){
                case JTokenType.Object:
                    return ConvertMap(outOps, input);
                case JTokenType.Array:
                    return ConvertList(outOps, input);
                case JTokenType.None:
                case JTokenType.Null:
                    return outOps.Empty();
                case JTokenType.String:
                    return outOps.CreateString((string)input);
                case JTokenType.Boolean:
                    return outOps.CreateBoolean((bool)input);
                case JTokenType.Integer:
                    long l = (long)input;
                    if((sbyte)l == l){
                        return outOps.CreateByte((sbyte)l);
                    }
                    if((short)l == l){
                        return outOps.CreateShort((short)l);
                    }
                    if((int)l == l){
                        return outOps.CreateInt((int)l);
                    }
                    return outOps.CreateLong(l);
                case JTokenType.Float:
                    double d = (double)input;
                    if((float)d == d){
                        return outOps.CreateFloat((float)d);
                    }
                    return outOps.CreateDouble(d);
                default:
                    throw new InvalidOperationException($"Unrecognized JSON token type: {input.Type}");
            }
        }

        public override DataResult<decimal> GetNumberValue(JToken input){
            if(IsNumeric(input.Type)){
                return DataResult.Success((decimal)input);
            }else if(input.Type == JTokenType.Boolean){
                return DataResult.Success((decimal)((bool)input ? 1 : 0));
            }
            if(compressed && input.Type == JTokenType.String){
                if(int.TryParse((string)input, out int result)){
                    return DataResult.Success((decimal)result);
                }
                return DataResult.Error<decimal>($"Not a number: {input}");
            }
            return DataResult.Error<decimal>($"Not a number: {input}");
        }

        public override JToken CreateNumeric(decimal value){
            return new JValue(value);
        }

        public override DataResult<bool> GetBooleanValue(JToken input){
            if(input.Type == JTokenType.Boolean){
                return DataResult.Success((bool)input);
            }
            if(IsNumeric(input.Type)){
                return DataResult.Success(decimal.ToByte((decimal)input) != 0);
            }
            return DataResult.Error<bool>($"Not a string: {input}");
        }

        public override JToken CreateBoolean(bool value){
            return new JValue(value);
        }

        public override DataResult<string> GetStringValue(JToken input){
            if(input.Type == JTokenType.String || IsNumeric(input.Type) && compressed){
                return DataResult.Success((string)input);
            }
            return DataResult.Error<string>($"Not a string: {input}");
        }

        public override JToken CreateString(string value){
            return new JValue(value);
        }

        public override DataResult<JToken> MergeToList(JToken list, JToken value){
            if(!(list is JArray) && list != Empty()){
                return DataResult.Error($"{nameof(MergeToList)} called with not a list: {list}", list);
            }
            JArray result = new JArray();
            if(list != Empty()){
                foreach(JToken entry in (JArray)list){
                    result.Add(entry);
                }
            }
            result.Add(value);
            return DataResult.Success<JToken>(result);
        }

        public override DataResult<JToken> MergeToList(JToken list, IList<JToken> values){
            if(!(list is JArray) && list != Empty()){
                return DataResult.Error($"{nameof(MergeToList)} called with not a list: {list}", list);
            }
            JArray result = new JArray();
            if(list != Empty()){
                foreach(JToken entry in (JArray)list){
                    result.Add(entry);
                }
            }
            foreach(JToken entry in values){
                result.Add(entry);
            }
            return DataResult.Success<JToken>(result);
        }

        public override DataResult<JToken> MergeToMap(JToken map, JToken key, JToken value){
            if(!(map is JObject) && map != Empty()){
                return DataResult.Error($"{nameof(MergeToMap)} called with not a map: {map}", map);
            }
            if(!(key is JValue) || key.Type != JTokenType.String && !compressed){
                return DataResult.Error($"key is not a string: {key}", map);
            }
            JObject output = new JObject();
            if(map != Empty()){
                foreach(KeyValuePair<string, JToken> entry in (JObject)map){
                    output.Add(entry.Key, entry.Value);
                }
            }
            output.Add((string)key, value);
            return DataResult.Success<JToken>(output);
        }

        public override DataResult<JToken> MergeToMap(JToken map, IMapLike<JToken> values){
            if(!(map is JObject) && map != Empty()){
                return DataResult.Error($"{nameof(MergeToMap)} called with not a map: {map}", map);
            }
            JObject output = new JObject();
            if(map != Empty()){
                foreach(KeyValuePair<string, JToken> entry in (JObject)map){
                    output.Add(entry.Key, entry.Value);
                }
            }
            List<JToken> missed = new List<JToken>();
            foreach(Pair<JToken, JToken> entry in values){
                JToken key = entry.GetFirst();
                if(!(key is JValue) || key.Type == JTokenType.String && !compressed){
                    missed.Add(key);
                    continue;
                }
                output.Add((string)key, entry.GetSecond());
            }
            if(missed.Count != 0){
                return DataResult.Error<JToken>($"some keys are not strings: {missed}", output);
            }
            return DataResult.Success<JToken>(output);
        }

        public override DataResult<IEnumerable<Pair<JToken, JToken>>> GetMapValues(JToken input){
            if(input is JObject jsonObject){
                return DataResult.Success(((IEnumerable<KeyValuePair<string, JToken>>)jsonObject).Select(entry =>
                    Pair.Of(CreateString(entry.Key), entry.Value.Type == JTokenType.Null ? null : entry.Value)
                ));
            }
            return DataResult.Error<IEnumerable<Pair<JToken, JToken>>>("Not a JSON object: " + input);
        }

        public override DataResult<Action<Action<JToken, JToken>>> GetMapEntries(JToken input){
            if(input is JObject jsonObject){
                return DataResult.Success<Action<Action<JToken, JToken>>>(c => {
                    foreach(KeyValuePair<string, JToken>entry in jsonObject){
                        c.Invoke(CreateString(entry.Key), entry.Value.Type == JTokenType.Null ? null : entry.Value);
                    }
                });
            }
            return DataResult.Error<Action<Action<JToken, JToken>>>("Not a JSON object: " + input);
        }

        public override DataResult<IMapLike<JToken>> GetMap(JToken input){
            if(input is JObject jsonObject){
                return DataResult.Success<IMapLike<JToken>>(new JObjectMapLike(this, jsonObject));
            }
            return DataResult.Error<IMapLike<JToken>>("Not a JSON object: " + input);
        }

        public override JToken CreateMap(IEnumerable<Pair<JToken, JToken>> map){
            JObject result = new JObject();
            foreach(Pair<JToken, JToken> pair in map){
                result.Add((string)pair.GetFirst(), pair.GetSecond());
            }
            return result;
        }

        public override DataResult<IEnumerable<JToken>> GetEnumerable(JToken input){
            if(input is JArray jArray){
                return DataResult.Success(jArray.Select(e =>
                    e.Type == JTokenType.Null ? null : e));
            }
            return DataResult.Error<IEnumerable<JToken>>($"Not a json array: {input}");
        }

        public override DataResult<Action<Action<JToken>>> GetList(JToken input){
            if(input is JArray jArray){
                return DataResult.Success<Action<Action<JToken>>>(c => {
                    foreach(JToken element in jArray){
                        c.Invoke(element.Type == JTokenType.Null ? null : element);
                    }
                });
            }
            return DataResult.Error<Action<Action<JToken>>>($"Not a json array: {input}");
        }

        public override JToken CreateList(IEnumerable<JToken> input){
            JArray result = new JArray();
            foreach(JToken entry in input){
                result.Add(entry);
            }
            return result;
        }

        public override JToken Remove(JToken input, string key){
            if(input is JObject existing){
                JObject result = new JObject();
                foreach(KeyValuePair<string, JToken> entry in existing){
                    if(ObjectUtils.Equals(entry.Key, key)){
                        continue;
                    }
                    result.Add(entry.Key, entry.Value);
                }
                return result;
            }
            return input;
        }

        public override ListBuilder<JToken> ListBuilder(){
            return new ArrayBuilder();
        }

        public override bool CompressMaps(){
            return compressed;
        }

        public override RecordBuilder<JToken> MapBuilder(){
            return new JsonRecordBuilder(this);
        }


        /*
         * Object override methods
         */
        public override string ToString(){
            return "JSON";
        }


        /*
         * Nested types
         */
        private sealed class JObjectMapLike : IMapLike<JToken>{
            /*
             * Fields
             */
            private readonly JsonOps ops;
            private readonly JObject obj;


            /*
             * Constructor
             */
            public JObjectMapLike(JsonOps opsIn, JObject objIn){
                ops = opsIn;
                obj = objIn;
            }


            /*
             * IMapLike implementation
             */
            public JToken Get(JToken key){
                if(obj.TryGetValue((string)key, out JToken result)){
                    return result;
                }
                return null;
            }
            
            public JToken Get(string key){
                if(obj.TryGetValue(key, out JToken result)){
                    return result;
                }
                return null;
            }
            
            public IEnumerator<Pair<JToken, JToken>> GetEnumerator(){
                return ((IEnumerable<KeyValuePair<string, JToken>>)obj).Select(entry =>
                    Pair.Of(ops.CreateString(entry.Key), entry.Value)).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator(){
                return GetEnumerator();
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return $"MapLike[{obj}]";
            }
        }

        protected sealed class ArrayBuilder : ListBuilder<JToken>{
            /*
             * Fields
             */
            private DataResult<JArray> builder = DataResult.Success(new JArray(), Lifecycle.Stable());


            /*
             * ListBuilder override methods
             */
            public override DynamicOps<JToken> Ops(){
                return INSTANCE;
            }

            public override ListBuilder<JToken> Add(JToken value){
                builder = builder.Map(b => {
                    b.Add(value);
                    return b;
                });
                return this;
            }

            public override ListBuilder<JToken> Add(DataResult<JToken> value){
                builder = builder.Apply2Stable((b, element) => {
                    b.Add(element);
                    return b;
                }, value);
                return this;
            }

            public override ListBuilder<JToken> WithErrorsFrom<R>(DataResult<R> result){
                builder = builder.FlatMap(r => result.Map(v => r));
                return this;
            }

            public override ListBuilder<JToken> MapError(Func<string, string> onError){
                builder = builder.MapError(onError);
                return this;
            }

            public override DataResult<JToken> Build(JToken prefix){
                DataResult<JToken> result = builder.FlatMap(b => {
                    if(!(prefix is JArray) && prefix != Ops().Empty()){
                        return DataResult.Error($"Cannot append a list to not a list: {prefix}", prefix);
                    }
                    JArray array = new JArray();
                    if(prefix != Ops().Empty()){
                        foreach(JToken entry in (JArray)prefix){
                            array.Add(entry);
                        }
                    }
                    foreach(JToken entry in b){
                        array.Add(entry);
                    }
                    return DataResult.Success<JToken>(array, Lifecycle.Stable());
                });
                builder = DataResult.Success(new JArray(), Lifecycle.Stable());
                return result;
            }
        }

        protected class JsonRecordBuilder : RecordBuilder.AbstractStringBuilder<JToken, JObject>{
            /*
             * Constructor
             */
            public JsonRecordBuilder(JsonOps opsIn)
                :base(opsIn){
                builder = DataResult.Success(InitBuilder(), Lifecycle.Stable());
            }


            /*
             * RecordBuilder.AbstractStringBuilder override
             */
            protected override JObject InitBuilder(){
                return new JObject();
            }

            public override JObject Append(string key, JToken value, JObject builder){
                builder.Add(key, value);
                return builder;
            }

            protected override DataResult<JToken> Build(JObject builder, JToken prefix){
                if(prefix == null || prefix.Type == JTokenType.Null){
                    return DataResult.Success<JToken>(builder);
                }
                if(prefix is JObject existingObj){
                    JObject result = new JObject();
                    foreach(KeyValuePair<string, JToken> entry in existingObj){
                        result.Add(entry.Key, entry.Value);
                    }
                    foreach(KeyValuePair<string, JToken> entry in builder){
                        result.Add(entry.Key, entry.Value);
                    }
                    return DataResult.Success<JToken>(result);
                }
                return DataResult.Error($"{nameof(Build)} called with not a map: {prefix}", prefix);
            }
        }
    }
}
