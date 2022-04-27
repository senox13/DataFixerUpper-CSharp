using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.Serialization.Codecs{
    public interface IBaseMapCodec<K, V>{
        ICodec<K> KeyCodec();

        ICodec<V> ElementCodec();

        DataResult<IDictionary<K, V>> Decode<T>(DynamicOps<T> ops, IMapLike<T> input);

        RecordBuilder<T> Encode<T>(IDictionary<K, V> input, DynamicOps<T> ops, RecordBuilder<T> prefix);
    }

    public static class BaseMapCodec{
        public static DataResult<IDictionary<K, V>> Decode<K, V, T>(IBaseMapCodec<K, V> mapCodec, DynamicOps<T> ops, IMapLike<T> input){
            ImmutableDictionary<K, V>.Builder read = ImmutableDictionary.CreateBuilder<K, V>();
            ImmutableList<Pair<T, T>>.Builder failed = ImmutableList.CreateBuilder<Pair<T, T>>();

            DataResult<Unit> result = input.Aggregate(
                DataResult.Success(Unit.INSTANCE, Lifecycle.Stable()),
                (r, pair) => {
                    DataResult<K> k = mapCodec.KeyCodec().Parse(ops, pair.GetFirst());
                    DataResult<V> v = mapCodec.ElementCodec().Parse(ops, pair.GetSecond());
                    DataResult<Pair<K, V>> entry = k.Apply2Stable(Pair.Of, v);
                    entry.Error().IfPresent(e => failed.Add(pair));
                    return r.Apply2Stable((u, p) => {
                        read.Add(p.GetFirst(), p.GetSecond());
                        return u;
                    }, entry);
                }
            );

            IDictionary<K, V> elements = read.ToImmutable();
            T errors = ops.CreateMap(failed.ToImmutable());
            return result.Map(unit => elements).SetPartial(elements).MapError(e => e + " missed input: " + errors);
        }

        public static RecordBuilder<T> Encode<K, V, T>(IBaseMapCodec<K, V> mapCodec, IDictionary<K, V> input, DynamicOps<T> ops, RecordBuilder<T> prefix){
            foreach(KeyValuePair<K, V> entry in input){
                prefix.Add(
                    mapCodec.KeyCodec().EncodeStart(ops, entry.Key),
                    mapCodec.ElementCodec().EncodeStart(ops, entry.Value)
                );
            }
            return prefix;
        }
    }
}
