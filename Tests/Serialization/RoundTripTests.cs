using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using DataFixerUpper.DataFixers.Kinds;
using DataFixerUpper.DataFixers.Util;
using DataFixerUpper.Serialization;
using DataFixerUpper.Serialization.Codecs;
using JavaUtilities;

namespace DataFixerUpperTests.Serialization{
    [TestFixture]
    public class RoundTripTests{
        /*
         * Stub types
         */
        public sealed class Day{
            //Fields
            private static readonly Dictionary<string, Day> BY_NAME;
            public static readonly ICodec<Day> CODEC;
            public static readonly Day TUESDAY;
            public static readonly Day WEDNESDAY;
            public static readonly Day SUNDAY;
            private readonly string name;
            private readonly ICodec<DayData> codec;

            static Day(){
                BY_NAME = new Dictionary<string, Day>();
                CODEC = Codec.STRING.ComapFlatMap(DataResult.PartialGet<string, Day>(s => BY_NAME[s], () => "unknown day"), d => d.name);
                TUESDAY = new Day("tuesday", TuesdayData.CODEC.Cast<TuesdayData, DayData>());
                WEDNESDAY = new Day("wednesday", WednesdayData.CODEC.Cast<WednesdayData, DayData>());
                SUNDAY = new Day("sunday", SundayData.CODEC.Cast<SundayData, DayData>());
            }


            //Constructor
            private Day(string nameIn, ICodec<DayData> codecIn){
                name = nameIn;
                codec = codecIn;
                BY_NAME.Add(name, this);
            }


            //Instance methods
            public ICodec<DayData> GetCodec(){
                return codec;
            }
        }

        public abstract class DayData{
            //Fields
            public static readonly ICodec<DayData> CODEC = Day.CODEC.Dispatch(d => d.Type(), d => d.GetCodec());


            //Abstract methods
            public abstract Day Type();
        }

        public sealed class TuesdayData : DayData{
            //Fields
            public static readonly new ICodec<TuesdayData> CODEC = Codec.INT.Xmap(d => new TuesdayData(d), d => d.x);
            private readonly int x;


            //Constructor
            public TuesdayData(int xIn){
                x = xIn;
            }


            //DayData override methods
            public override Day Type(){
                return Day.TUESDAY;
            }


            //Object override methods
            public override bool Equals(object obj){
                if(obj == this){
                    return true;
                }
                if(obj is TuesdayData other){
                    return x == other.x;
                }
                return false;
            }

            public override int GetHashCode(){
                return ObjectUtils.Hash(x);
            }
        }

        public sealed class WednesdayData : DayData{
            //Fields
            public static readonly new ICodec<WednesdayData> CODEC = Codec.STRING.Xmap(d => new WednesdayData(d), d => d.y);
            private readonly string y;


            //Constructor
            public WednesdayData(string yIn){
                y = yIn;
            }


            //DayData override methods
            public override Day Type(){
                return Day.WEDNESDAY;
            }


            //Object override methods
            public override bool Equals(object obj){
                if(obj == this){
                    return true;
                }
                if(obj is WednesdayData other){
                    return y == other.y;
                }
                return false;
            }

            public override int GetHashCode(){
                return ObjectUtils.Hash(y);
            }
        }

        public sealed class SundayData : DayData{
            //Fields
            public static readonly new ICodec<SundayData> CODEC = Codec.FLOAT.Xmap(d => new SundayData(d), d => d.z);
            private readonly float z;


            //Constructor
            public SundayData(float zIn){
                z = zIn;
            }


            //DayData override methods
            public override Day Type(){
                return Day.TUESDAY;
            }


            //Object override methods
            public override bool Equals(object obj){
                if(obj == this){
                    return true;
                }
                if(obj is SundayData other){
                    return z == other.z;
                }
                return false;
            }

            public override int GetHashCode(){
                return ObjectUtils.Hash(z);
            }
        }

        public sealed class TestData{
            //Fields
            public static readonly ICodec<TestData> CODEC = RecordCodecBuilder.Create<TestData>(i => i.Group(
                Codec.FLOAT.FieldOf("a").ForGetter<TestData>(d => d.a),
                Codec.DOUBLE.FieldOf("b").ForGetter<TestData>(d => d.b),
                Codec.BYTE.FieldOf("c").ForGetter<TestData>(d => d.c),
                Codec.SHORT.FieldOf("d").ForGetter<TestData>(d => d.d),
                Codec.INT.FieldOf("e").ForGetter<TestData>(d => d.e),
                Codec.LONG.FieldOf("f").ForGetter<TestData>(d => d.f),
                Codec.BOOL.FieldOf("g").ForGetter<TestData>(d => d.g),
                Codec.STRING.FieldOf("h").ForGetter<TestData>(d => d.h),
                Codec.STRING.ListOf().FieldOf("i").ForGetter<TestData>(d => d.i),
                Codec.UnboundedMap(Codec.STRING, Codec.STRING).FieldOf("j").ForGetter<TestData>(d => d.j),
                Codec.CompoundList(Codec.STRING, Codec.STRING).FieldOf("k").ForGetter<TestData>(d => d.k),
                DayData.CODEC.FieldOf("day_data").ForGetter<TestData>(d => d.dayData)
            ).Apply(i, (a, b, c, d, e, f, g, h, i, j, k, day) => new TestData(a, b, c, d, e, f, g, h, i, j, k, day)));
            private readonly float a;
            private readonly double b;
            private readonly sbyte c;
            private readonly short d;
            private readonly int e;
            private readonly long f;
            private readonly bool g;
            private readonly string h;
            private readonly IList<string> i;
            private readonly IDictionary<string, string> j;
            private readonly IList<Pair<string, string>> k;
            private readonly DayData dayData;


            //Constructor
            public TestData(float a, double b, sbyte c, short d, int e, long f, bool g, string h, IList<string> i, IDictionary<string, string> j, IList<Pair<string, string>> k, DayData dayData) {
                this.a = a;
                this.b = b;
                this.c = c;
                this.d = d;
                this.e = e;
                this.f = f;
                this.g = g;
                this.h = h;
                this.i = i;
                this.j = j;
                this.k = k;
                this.dayData = dayData;
            }


            //Object override methods
            public override bool Equals(object obj){
                if(obj == this){
                    return true;
                }
                if(obj is TestData other){
                    return MathF.Abs(a) - MathF.Abs(other.a) < 0.0000001f
                        && Math.Abs(b) - Math.Abs(other.b) < 0.00000000000001f
                        && c == other.c
                        && d == other.d
                        && e == other.e
                        && f == other.f
                        && g == other.g
                        && h.Equals(other.h)
                        && CollectionUtils.ContentsEqual(i, other.i)
                        && j.Count == other.j.Count && j.Except(other.j).Count() == 0
                        && CollectionUtils.ContentsEqual(k, other.k)
                        && dayData.Equals(other.dayData);
                }
                return false;
            }

            public override int GetHashCode(){
                return ObjectUtils.Hash(a, b, c, d, e, f, g, h, i, j, k, dayData);
            }
        }


        /*
         * Private methods
         */
        private static TestData MakeRandomTestData(){
            Random random = new Random(4);
            return new TestData(
                (float)random.NextDouble(),
                random.NextDouble(),
                (sbyte)random.Next(),
                (short)random.Next(),
                random.Next(),
                random.Next(),
                random.Next(0, 2) != 0,
                ((float)random.NextDouble()).ToString(),
                Enumerable.Range(0, random.Next(100))
                    .Select(i => ((float)random.NextDouble()).ToString())
                    .ToList(),
                Enumerable.Range(0, random.Next(100))
                    .ToDictionary(
                        i => ((float)random.NextDouble()).ToString(),
                        i => ((float)random.NextDouble()).ToString()
                    ),
                Enumerable.Range(0, random.Next(100))
                    .Select(i => Pair.Of(((float)random.NextDouble()).ToString(), ((float)random.NextDouble()).ToString()))
                    .ToList(),
                new WednesdayData("meetings lol")
            );
        }

        private void TestWriteRead(JsonOps ops){
            TestData data = MakeRandomTestData();

            DataResult<JToken> encoded = TestData.CODEC.EncodeStart(ops, data);
            DataResult<TestData> decoded = encoded.FlatMap(r => TestData.CODEC.Parse(ops, r));

            Assert.AreEqual(DataResult.Success(data), decoded);
        }

        private void TestReadWrite(JsonOps ops){
            TestData data = MakeRandomTestData();

            DataResult<JToken> encoded = TestData.CODEC.EncodeStart(ops, data);
            DataResult<TestData> decoded = encoded.FlatMap(r => TestData.CODEC.Parse(ops, r));
            DataResult<JToken> reEncoded = decoded.FlatMap(r => TestData.CODEC.EncodeStart(ops, r));

            Assert.IsTrue(JToken.DeepEquals(encoded.Result().Get(), reEncoded.Result().Get()));
        }


        /*
         * Tests
         */
        [Test]
        public void TestWriteReadNormal(){
            TestWriteRead(JsonOps.INSTANCE);
        }

        [Test]
        public void TestReadWriteNormal(){
            TestReadWrite(JsonOps.INSTANCE);
        }

        [Test]
        public void TestWriteReadCompressed(){
            TestWriteRead(JsonOps.COMPRESSED);
        }

        [Test]
        public void TestReadWriteCompressed(){
            TestReadWrite(JsonOps.COMPRESSED);
        }
    }
}
