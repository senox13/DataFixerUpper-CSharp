using System.Collections.Generic;
using System.Collections.Immutable;
using JavaUtilities;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.Serialization.Codecs{
    public sealed class ListCodec<A> : ICodec<IList<A>>{
        /*
         * Fields
         */
        private readonly ICodec<A> elementCodec;


        /*
         * Constructor
         */
        public ListCodec(ICodec<A> elementCodecIn){
            elementCodec = elementCodecIn;
        }


        /*
         * ICodec implementation
         */
        public DataResult<Pair<IList<A>, T>> Decode<T>(DynamicOps<T> ops, T input){
            return ops.GetList(input).SetLifecycle(Lifecycle.Stable()).FlatMap(stream => {
                ImmutableList<A>.Builder read = ImmutableList.CreateBuilder<A>();
                List<T> failed = new List<T>();
                DataResult<Unit> result = DataResult.Success(Unit.INSTANCE, Lifecycle.Stable());

                stream.Invoke(t => {
                    DataResult<Pair<A, T>> element = elementCodec.Decode(ops, t);
                    element.Error().IfPresent(e => failed.Add(t));
                    result = result.Apply2Stable((r, v) => {
                        read.Add(v.GetFirst());
                        return r;
                    }, element);
                });

                IList<A> elements = read.ToImmutable();
                T errors = ops.CreateList(failed);
                Pair<IList<A>, T> pair = Pair.Of(elements, errors);
                
                return result.Map(unit => pair).SetPartial(pair);
            });
        }

        public DataResult<T> Encode<T>(IList<A> input, DynamicOps<T> ops, T prefix){
            ListBuilder<T> builder = ops.ListBuilder();
            foreach(A element in input){
                builder.Add(elementCodec.EncodeStart(ops, element));
            }
            return builder.Build(prefix);
        }


        /*
         * Object override methods
         */
        public override bool Equals(object obj){
            if(obj == this)
                return true;
            if(obj is ListCodec<A> other)
                return elementCodec.Equals(other.elementCodec);
            return false;
        }

        public override int GetHashCode(){
            return ObjectUtils.Hash(elementCodec);
        }

        public override string ToString(){
            return $"ListCodec[{elementCodec}]";
        }
    }
}
