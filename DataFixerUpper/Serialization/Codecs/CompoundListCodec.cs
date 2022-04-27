using System.Collections.Generic;
using System.Collections.Immutable;
using DataFixerUpper.DataFixers.Util;
using DataFixerUpper.Util;

namespace DataFixerUpper.Serialization.Codecs{
    public sealed class CompoundListCodec<K, V> : ICodec<IList<Pair<K, V>>>{
        /*
         * Fields
         */
        private readonly ICodec<K> keyCodec;
        private readonly ICodec<V> elementCodec;


        /*
         * Constructor
         */
        public CompoundListCodec(ICodec<K> keyCodecIn, ICodec<V> elementCodecIn){
            keyCodec = keyCodecIn;
            elementCodec = elementCodecIn;
        }


        /*
         * ICodec implementation
         */
        public DataResult<Pair<IList<Pair<K, V>>, T>> Decode<T>(DynamicOps<T> ops, T input){
            return ops.GetMapEntries(input).FlatMap(map => {
                ImmutableList<Pair<K, V>>.Builder read = ImmutableList.CreateBuilder<Pair<K, V>>();
                ImmutableDictionary<T, T>.Builder failed = ImmutableDictionary.CreateBuilder<T, T>();
                DataResult<Unit> result = DataResult.Success(Unit.INSTANCE, Lifecycle.Experimental());

                map.Invoke((key, value) => {
                    DataResult<K> k = keyCodec.Parse(ops, key);
                    DataResult<V> v = elementCodec.Parse(ops, value);
                    DataResult<Pair<K, V>> readEntry = k.Apply2Stable(Pair.Of, v);
                    readEntry.Error().IfPresent(e => failed.Add(key, value));
                    result = result.Apply2Stable((u, e) => {
                        read.Add(e);
                        return u;
                    }, readEntry);
                });

                IList<Pair<K, V>> elements = read.ToImmutable();
                T errors = ops.CreateMap(failed.ToImmutable());
                Pair<IList<Pair<K, V>>, T> pair = Pair.Of(elements, errors);
                return result.Map(unit => pair).SetPartial(pair);
            });
        }

        public DataResult<T> Encode<T>(IList<Pair<K, V>> input, DynamicOps<T> ops, T prefix){
            RecordBuilder<T> builder = ops.MapBuilder();
            foreach(Pair<K, V> pair in input){
                builder.Add(
                    keyCodec.EncodeStart(ops, pair.GetFirst()),
                    elementCodec.EncodeStart(ops, pair.GetSecond())
                );
            }
            return builder.Build(prefix);
        }


        /*
         * Object override methods
         */
        public override bool Equals(object obj){
            if(obj == this){
                return true;
            }
            if(obj is CompoundListCodec<K, V> other){
                return ObjectUtils.Equals(keyCodec, other.keyCodec)
                    && ObjectUtils.Equals(elementCodec, other.elementCodec);
            }
            return false;
        }

        public override int GetHashCode(){
            return ObjectUtils.Hash(keyCodec, elementCodec);
        }

        public override string ToString(){
            return $"CompoundListCodec[{keyCodec} -> {elementCodec}]";
        }
    }
}
