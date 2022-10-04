using System;
using System.Collections.Generic;
using System.IO;
using JavaUtilities;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.Serialization{
    public class OptionalDynamic<T> : DynamicLike<T>{
        /*
         * Fields
         */
        private readonly DataResult<Dynamic<T>> delegateDynamic;

        
        /*
         * Constructor
         */
        public OptionalDynamic(DynamicOps<T> opsIn, DataResult<Dynamic<T>> delegateDynamicIn)
            :base(opsIn){
            delegateDynamic = delegateDynamicIn;
        }


        /*
         * Public methods
         */
        public DataResult<Dynamic<T>> Get(){
            return delegateDynamic;
        }

        public Optional<Dynamic<T>> Result(){
            return delegateDynamic.Result();
        }

        public DataResult<U> Map<U>(Func<Dynamic<T>, U> mapper){
            return delegateDynamic.Map(mapper);
        }

        public DataResult<U> FlatMap<U>(Func<Dynamic<T>, DataResult<U>> mapper){
            return delegateDynamic.FlatMap(mapper);
        }

        public Dynamic<T> OrElseEmptyMap(){
            return Result().OrElseGet(EmptyMap);
        }

        public Dynamic<T> OrElseEmptyList(){
            return Result().OrElseGet(EmptyList);
        }

        public DataResult<V> Into<V>(Func<Dynamic<T>, V> action){
            return delegateDynamic.Map(action);
        }


        /*
         * DynamicLike override methods
         */
        public override DataResult<decimal> AsNumber(){
            return FlatMap(d => d.AsNumber());
        }
        
        public override DataResult<string> AsString(){
            return FlatMap(d => d.AsString());
        }
        
        public override DataResult<IEnumerable<Dynamic<T>>> AsEnumerableOpt(){
            return FlatMap(d => d.AsEnumerableOpt());
        }
        
        public override DataResult<IEnumerable<Pair<Dynamic<T>, Dynamic<T>>>> AsMapOpt(){
            return FlatMap(d => d.AsMapOpt());
        }
        
        public override DataResult<MemoryStream> AsMemoryStreamOpt(){
            return FlatMap(d => d.AsMemoryStreamOpt());
        }
        
        public override DataResult<IEnumerable<int>> AsIntEnumerableOpt(){
            return FlatMap(d => d.AsIntEnumerableOpt());
        }
        
        public override DataResult<IEnumerable<long>> AsLongEnumerableOpt(){
            return FlatMap(d => d.AsLongEnumerableOpt());
        }
        
        public override OptionalDynamic<T> Get(string key){
            return new OptionalDynamic<T>(ops, delegateDynamic.FlatMap(k => k.Get(key).delegateDynamic));
        }
        
        public override DataResult<T> GetGeneric(T key){
            return FlatMap(v => v.GetGeneric(key));
        }
        
        public override DataResult<T> GetElement(string key){
            return FlatMap(v => v.GetElement(key));
        }
        
        public override DataResult<T> GetElementGeneric(T key){
            return FlatMap(v => v.GetElementGeneric(key));
        }
        
        public override DataResult<Pair<A, T>> Decode<A>(IDecoder<A> decoder){
            return delegateDynamic.FlatMap(t => t.Decode(decoder));
        }
    }
}
