using System;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.DataFixers.Kinds;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.Serialization.Codecs{
    public sealed class RecordCodecBuilder<O, F> : App<RecordCodecBuilder.Mu<O>, F>{
        /*
         * Fields
         */
        internal readonly Func<O, F> getter;
        internal readonly Func<O, IMapEncoder<F>> encoder;
        internal readonly IMapDecoder<F> decoder;


        /*
         * Constructor
         */
        internal RecordCodecBuilder(Func<O, F> getterIn, Func<O, IMapEncoder<F>> encoderIn, IMapDecoder<F> decoderIn){
            getter = getterIn;
            encoder = encoderIn;
            decoder = decoderIn;
        }


        /*
         * Instance methods
         */
        public RecordCodecBuilder<O, E> Dependant<E>(Func<O, E> getter, IMapEncoder<E> encoder, Func<F, IMapDecoder<E>> decoderGetter){
            return new RecordCodecBuilder<O, E>(
                getter,
                o => encoder,
                new MapDecoderDependantWrapper<E>(encoder, decoder, decoderGetter)
            );
        }


        /*
         * Nested types
         */
        private sealed class MapDecoderDependantWrapper<E> : MapDecoder.Implementation<E>{
            /*
             * Fields
             */
            private readonly IMapEncoder<E> encoder;
            private readonly IMapDecoder<F> decoder;
            private readonly Func<F, IMapDecoder<E>> decoderGetter;


            /*
             * Constructor
             */
            public MapDecoderDependantWrapper(IMapEncoder<E> encoderIn, IMapDecoder<F> decoderIn, Func<F, IMapDecoder<E>> decoderGetterIn){
                encoder = encoderIn;
                decoder = decoderIn;
                decoderGetter = decoderGetterIn;
            }


            /*
             * MapDecoder.Implementation override methods
             */
            public override DataResult<E> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
                return decoder.Decode(ops, input).Map(decoderGetter).FlatMap(decoder1 => decoder1.Decode(ops, input));
            }

            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return encoder.Keys(ops);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return $"Dependent[{encoder}]";
            }
        }
    }

    public static class RecordCodecBuilder{
        public sealed class Mu<O> : K1{}


        /*
         * Static methods
         */
        public static RecordCodecBuilder<O, F> Unbox<O, F>(App<Mu<O>, F> box){
            return (RecordCodecBuilder<O, F>)box;
        }

        public static Instance<O> GetInstance<O>(){
            return new Instance<O>();
        }

        public static RecordCodecBuilder<O, F> Of<O, F>(Func<O, F> getter, string name, ICodec<F> fieldCodec){
            return Of(getter, fieldCodec.FieldOf(name));
        }
        
        public static RecordCodecBuilder<O, F> Of<O, F>(Func<O, F> getter, MapCodec<F> codec){
            return new RecordCodecBuilder<O, F>(getter, o => codec, codec);
        }

        public static RecordCodecBuilder<O, F> Point<O, F>(F instance){
            return new RecordCodecBuilder<O, F>(o => instance, o => Encoder.Empty<F>(), Decoder.Unit(instance));
        }

        public static RecordCodecBuilder<O, F> Point<O, F>(F instance, Lifecycle lifecycle){
            return new RecordCodecBuilder<O, F>(o => instance, o => Encoder.Empty<F>().WithLifecycle(lifecycle), Decoder.Unit(instance).WithLifecycle(lifecycle));
        }

        public static RecordCodecBuilder<O, F> Stable<O, F>(F instance){
            return Point<O, F>(instance, Lifecycle.Stable());
        }

        public static RecordCodecBuilder<O, F> Deprecated<O, F>(F instance, int since){
            return Point<O, F>(instance, Lifecycle.DeprecatedSince(since));
        }

        public static ICodec<O> Create<O>(Func<Instance<O>, App<Mu<O>, O>> builder){
            return Build(builder.Invoke(GetInstance<O>())).AsCodec();
        }

        public static MapCodec<O> MapCodec<O>(Func<Instance<O>, App<Mu<O>, O>> builder){
            return Build(builder.Invoke(GetInstance<O>()));
        }

        public static MapCodec<O> Build<O>(App<Mu<O>, O> builderBox){
            RecordCodecBuilder<O, O> builder = Unbox(builderBox);
            return new MapCodecWrapper<O>(builder.encoder, builder.decoder);
        }


        /*
         * Nested types
         */
        public sealed class Instance<O> : Applicative<Mu<O>, Instance<O>.Mu>{
            public sealed class Mu : Applicative.Mu{}


            /*
             * Instance methods
             */
            public App<Mu<O>, A> Stable<A>(A a){
                return Stable(a);
            }

            public App<Mu<O>, A> Deprecated<A>(A a, int since){
                return Deprecated(a, since);
            }

            public App<Mu<O>, A> Point<A>(A a, Lifecycle lifecycle){
                return Point(a, lifecycle);
            }


            /*
             * Applicative override methods
             */
            public override App<Mu<O>, A> Point<A>(A a){
                return RecordCodecBuilder.Point<O, A>(a);
            }

            public override Func<App<Mu<O>, A>, App<Mu<O>, R>> Lift1<A, R>(App<Mu<O>, Func<A, R>> function){
                return fa => {
                    RecordCodecBuilder<O, Func<A, R>> f = Unbox(function);
                    RecordCodecBuilder<O, A> a = Unbox(fa);

                    return new RecordCodecBuilder<O, R>(
                        o => f.getter.Invoke(o).Invoke(a.getter.Invoke(o)),
                        o => new MapEncoderLift1<A, R>(f, a, o),
                        new MapDecoderLift1<A, R>(f, a)
                    );
                };
            }

            public override App<Mu<O>, R> Ap2<A, B, R>(App<Mu<O>, Func<A, B, R>> func, App<Mu<O>, A> a, App<Mu<O>, B> b){
                RecordCodecBuilder<O, Func<A, B, R>> function = Unbox(func);
                RecordCodecBuilder<O, A> fa = Unbox(a);
                RecordCodecBuilder<O, B> fb = Unbox(b);

                return new RecordCodecBuilder<O, R>(
                    o => function.getter.Invoke(o).Invoke(fa.getter.Invoke(o), fb.getter.Invoke(o)),
                    o => new MapEncoderAp2<A, B, R>(function, fa, fb, o),
                    new MapDecoderAp2<A, B, R>(function, fa, fb)
                );
            }

            public override App<Mu<O>, R> Ap3<T1, T2, T3, R>(App<Mu<O>, Func<T1, T2, T3, R>> func, App<Mu<O>, T1> t1, App<Mu<O>, T2> t2, App<Mu<O>, T3> t3){
                RecordCodecBuilder<O, Func<T1, T2, T3, R>> function = Unbox(func);
                RecordCodecBuilder<O, T1> f1 = Unbox(t1);
                RecordCodecBuilder<O, T2> f2 = Unbox(t2);
                RecordCodecBuilder<O, T3> f3 = Unbox(t3);

                return new RecordCodecBuilder<O, R>(
                    o => function.getter.Invoke(o).Invoke(
                        f1.getter.Invoke(o),
                        f2.getter.Invoke(o),
                        f3.getter.Invoke(o)
                    ),
                    o => new MapEncoderAp3<T1, T2, T3, R>(function, f1, f2, f3, o),
                    new MapDecoderAp3<T1, T2, T3, R>(function, f1, f2, f3)
                );
            }

            /*public override App<Mu<O>, R> Ap4<T1, T2, T3, T4, R>(App<Mu<O>, Func<T1, T2, T3, T4, R>> func, App<Mu<O>, T1> t1, App<Mu<O>, T2> t2, App<Mu<O>, T3> t3, App<Mu<O>, T4> t4){
                //TODO: Ap4
            }*/

            public override App<Mu<O>, R> Map<T, R>(Func<T, R> func, App<Mu<O>, T> ts){
                RecordCodecBuilder<O, T> unbox = Unbox(ts);
                return new RecordCodecBuilder<O, R>(
                    unbox.getter.AndThen(func),
                    o => new MapEncoderMapped<T, R>(unbox, o),
                    unbox.decoder.Map(func)
                );
            }


            /*
             * Nested types
             */
            private sealed class MapEncoderLift1<A, R> : MapEncoder.Implementation<R>{
                /*
                 * Fields
                 */
                private readonly IMapEncoder<Func<A, R>> fEnc;
                private readonly IMapEncoder<A> aEnc;
                private readonly A aFromO;


                /*
                 * Constructors
                 */
                public MapEncoderLift1(RecordCodecBuilder<O, Func<A, R>> f, RecordCodecBuilder<O, A> a, O o){
                    fEnc = f.encoder.Invoke(o);
                    aEnc = a.encoder.Invoke(o);
                    aFromO = a.getter.Invoke(o);
                }


                /*
                 * MapEncoder.Implementation override methods
                 */
                public override RecordBuilder<T> Encode<T>(R input, DynamicOps<T> ops, RecordBuilder<T> prefix){
                    aEnc.Encode(aFromO, ops, prefix);
                    fEnc.Encode(a1 => input, ops, prefix);
                    return prefix;
                }

                public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                    return Enumerable.Concat(aEnc.Keys(ops), fEnc.Keys(ops));
                }


                /*
                 * Object override methods
                 */
                public override string ToString(){
                    return $"{fEnc} * {aEnc}";
                }
            }

            private sealed class MapDecoderLift1<A, R> : MapDecoder.Implementation<R>{
                /*
                 * Fields
                 */
                private readonly RecordCodecBuilder<O, Func<A, R>> f;
                private readonly RecordCodecBuilder<O, A> a;


                /*
                 * Constructor
                 */
                public MapDecoderLift1(RecordCodecBuilder<O, Func<A, R>> fIn, RecordCodecBuilder<O, A> aIn){
                    f = fIn;
                    a = aIn;
                }


                /*
                 * MapDecoder.Implementation override methods
                 */
                public override DataResult<R> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
                    return a.decoder.Decode(ops, input).FlatMap(ar =>
                        f.decoder.Decode(ops, input).Map(fr =>
                            fr.Invoke(ar)
                        )
                    );
                }
                
                public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                    return Enumerable.Concat(a.decoder.Keys(ops), f.decoder.Keys(ops));
                }


                /*
                 * Object override methods
                 */
                public override string ToString(){
                    return $"{f.decoder} * {a.decoder}";
                }
            }

            private sealed class MapEncoderAp2<A, B, R> : MapEncoder.Implementation<R>{
                /*
                 * Fields
                 */
                private readonly IMapEncoder<Func<A, B, R>> fEncoder;
                private readonly IMapEncoder<A> aEncoder;
                private readonly IMapEncoder<B> bEncoder;
                private readonly A aFromO;
                private readonly B bFromO;


                /*
                 * Constructor
                 */
                public MapEncoderAp2(RecordCodecBuilder<O, Func<A, B, R>> function, RecordCodecBuilder<O, A> fa, RecordCodecBuilder<O, B> fb, O o){
                    fEncoder = function.encoder.Invoke(o);
                    aEncoder = fa.encoder.Invoke(o);
                    bEncoder = fb.encoder.Invoke(o);
                    aFromO = fa.getter.Invoke(o);
                    bFromO = fb.getter.Invoke(o);
                }


                /*
                 * MapEncoder.Implementation override methods
                 */
                public override RecordBuilder<T> Encode<T>(R input, DynamicOps<T> ops, RecordBuilder<T> prefix){
                    aEncoder.Encode(aFromO, ops, prefix);
                    bEncoder.Encode(bFromO, ops, prefix);
                    fEncoder.Encode((a1, b1) => input, ops, prefix);
                    return prefix;
                }

                public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                    return Enumerable.Concat(fEncoder.Keys(ops),
                        Enumerable.Concat(aEncoder.Keys(ops), bEncoder.Keys(ops)));
                }


                /*
                 * Object override methods
                 */
                public override string ToString(){
                    return $"{fEncoder} * {aEncoder} * {bEncoder}";
                }
            }

            private sealed class MapDecoderAp2<A, B, R> : MapDecoder.Implementation<R>{
                /*
                 * Fields
                 */
                private readonly RecordCodecBuilder<O, Func<A, B, R>> function;
                private readonly RecordCodecBuilder<O, A> fa;
                private readonly RecordCodecBuilder<O, B> fb;


                /*
                 * Constructor
                 */
                public MapDecoderAp2(RecordCodecBuilder<O, Func<A, B, R>> functionIn, RecordCodecBuilder<O, A> faIn, RecordCodecBuilder<O, B> fbIn){
                    function = functionIn;
                    fa = faIn;
                    fb = fbIn;
                }


                /*
                 * MapDecoder.Implementation override methods
                 */
                public override DataResult<R> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
                    return DataResult.Unbox(DataResult.GetInstance().Ap2(
                        function.decoder.Decode(ops, input),
                        fa.decoder.Decode(ops, input),
                        fb.decoder.Decode(ops, input)
                    ));
                }

                public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                    return Enumerable.Concat(function.decoder.Keys(ops),
                        Enumerable.Concat(fa.decoder.Keys(ops), fb.decoder.Keys(ops)));
                }


                /*
                 * Object override methods
                 */
                public override string ToString(){
                    return $"{function.decoder} * {fa.decoder} * {fb.decoder}";
                }
            }

            private sealed class MapEncoderAp3<T1, T2, T3, R> : MapEncoder.Implementation<R>{
                /*
                 * Fields
                 */
                private readonly IMapEncoder<Func<T1, T2, T3, R>> fEncoder;
                private readonly IMapEncoder<T1> e1;
                private readonly IMapEncoder<T2> e2;
                private readonly IMapEncoder<T3> e3;
                private readonly T1 v1;
                private readonly T2 v2;
                private readonly T3 v3;


                /*
                 * Constructor
                 */
                public MapEncoderAp3(RecordCodecBuilder<O, Func<T1, T2, T3, R>> function, RecordCodecBuilder<O, T1> f1, RecordCodecBuilder<O, T2> f2, RecordCodecBuilder<O, T3> f3, O o){
                    fEncoder = function.encoder.Invoke(o);
                    e1 = f1.encoder.Invoke(o);
                    e2 = f2.encoder.Invoke(o);
                    e3 = f3.encoder.Invoke(o);
                    v1 = f1.getter.Invoke(o);
                    v2 = f2.getter.Invoke(o);
                    v3 = f3.getter.Invoke(o);
                }


                /*
                 * MapEncoder.Implementation override methods
                 */
                public override RecordBuilder<T> Encode<T>(R input, DynamicOps<T> ops, RecordBuilder<T> prefix){
                    e1.Encode(v1, ops, prefix);
                    e2.Encode(v2, ops, prefix);
                    e3.Encode(v3, ops, prefix);
                    fEncoder.Encode((t1, t2, t3) => input, ops, prefix);
                    return prefix;
                }

                public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                    return Enumerable.Concat(fEncoder.Keys(ops),
                        Enumerable.Concat(e1.Keys(ops),
                        Enumerable.Concat(e2.Keys(ops), e3.Keys(ops))));
                }


                /*
                 * Object override methods
                 */
                public override string ToString(){
                    return $"{fEncoder} * {e1} * {e2} * {e3}";
                }
            }

            private sealed class MapDecoderAp3<T1, T2, T3, R> : MapDecoder.Implementation<R>{
                /*
                 * Fields
                 */
                private readonly RecordCodecBuilder<O, Func<T1, T2, T3, R>> function;
                private readonly RecordCodecBuilder<O, T1> f1;
                private readonly RecordCodecBuilder<O, T2> f2;
                private readonly RecordCodecBuilder<O, T3> f3;


                /*
                 * Constructor
                 */
                public MapDecoderAp3(RecordCodecBuilder<O, Func<T1, T2, T3, R>> functionIn, RecordCodecBuilder<O, T1> f1In, RecordCodecBuilder<O, T2> f2In, RecordCodecBuilder<O, T3> f3In){
                    function = functionIn;
                    f1 = f1In;
                    f2 = f2In;
                    f3 = f3In;
                }


                /*
                 * MapDecoder.Implementation override methods
                 */
                public override DataResult<R> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
                    return DataResult.Unbox(DataResult.GetInstance().Ap3(
                        function.decoder.Decode(ops, input),
                        f1.decoder.Decode(ops, input),
                        f2.decoder.Decode(ops, input),
                        f3.decoder.Decode(ops, input)
                    ));
                }
                
                public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                    return Enumerable.Concat(function.decoder.Keys(ops),
                        Enumerable.Concat(f1.decoder.Keys(ops),
                        Enumerable.Concat(f2.decoder.Keys(ops), f3.decoder.Keys(ops))));
                }


                /*
                 * Object override methods
                 */
                public override string ToString(){
                    return $"{function.decoder} * {f1.decoder} * {f2.decoder} * {f3.decoder}";
                }
            }

            //TODO: MapEncoderAp4

            //TODO: MapDecoderAp4

            private sealed class MapEncoderMapped<T, R> : MapEncoder.Implementation<R>{
                /*
                 * Fields
                 */
                private readonly IMapEncoder<T> encoder;
                private readonly Func<O, T> getter;
                private readonly O o;


                /*
                 * Constructor
                 */
                public MapEncoderMapped(RecordCodecBuilder<O, T> unbox, O oIn){
                    o = oIn;
                    encoder = unbox.encoder.Invoke(o);
                    getter = unbox.getter;
                }


                /*
                 * MapEncoder.Implementation override methods
                 */
                public override RecordBuilder<U> Encode<U>(R input, DynamicOps<U> ops, RecordBuilder<U> prefix){
                    return encoder.Encode(getter.Invoke(o), ops, prefix);
                }

                public override IEnumerable<U> Keys<U>(DynamicOps<U> ops){
                    return encoder.Keys(ops);
                }


                /*
                 * Object override methods
                 */
                public override string ToString(){
                    return encoder + "[mapped]";
                }
            }
        }

        private sealed class MapCodecWrapper<O> : MapCodec<O>{
            /*
             * Fields
             */
            private readonly Func<O, IMapEncoder<O>> encoder;
            private readonly IMapDecoder<O> decoder;


            /*
             * Constructor
             */
            public MapCodecWrapper(Func<O, IMapEncoder<O>> encoderIn, IMapDecoder<O> decoderIn){
                encoder = encoderIn;
                decoder = decoderIn;
            }


            /*
             * MapCodec override methods
             */
            public override DataResult<O> Decode<T>(DynamicOps<T> ops, IMapLike<T> input){
                return decoder.Decode(ops, input);
            }

            public override RecordBuilder<T> Encode<T>(O input, DynamicOps<T> ops, RecordBuilder<T> prefix){
                return encoder.Invoke(input).Encode(input, ops, prefix);
            }

            public override IEnumerable<T> Keys<T>(DynamicOps<T> ops){
                return decoder.Keys(ops);
            }


            /*
             * Object override methods
             */
            public override string ToString(){
                return $"RecordCodec[{decoder}]";
            }
        }
    }
}
