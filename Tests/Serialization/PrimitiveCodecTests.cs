using NUnit.Framework;
using Newtonsoft.Json.Linq;
using DataFixerUpper.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpperTests.Serialization{
    [TestFixture]
    public class PrimitiveCodecTests{
        //Codec.BOOL
        [Test]
        public void BooleanEncode(){
            DataResult<JToken> result = Codec.BOOL.EncodeStart(JsonOps.INSTANCE, true);
		    Assert.AreEqual((bool)result.Result().Get(), true);
        }

        [Test]
        public void BooleanDecode(){
            DataResult<bool> result = Codec.BOOL.Parse(JsonOps.INSTANCE, new JValue(true));
		    Assert.AreEqual(result.Result().Get(), true);
        }

        //Codec.BYTE
        [Test]
        public void ByteEncode(){
            DataResult<JToken> result = Codec.BYTE.EncodeStart(JsonOps.INSTANCE, (sbyte)20);
		    Assert.AreEqual((sbyte)result.Result().Get(), 20);
        }

        [Test]
        public void ByteDecode(){
            DataResult<sbyte> result = Codec.BYTE.Parse(JsonOps.INSTANCE, new JValue(20));
		    Assert.AreEqual(result.Result().Get(), (sbyte)20);
        }

        //Codec.SHORT
        [Test]
        public void ShortEncode(){
            DataResult<JToken> result = Codec.SHORT.EncodeStart(JsonOps.INSTANCE, (short)20);
		    Assert.AreEqual((short)result.Result().Get(), 20);
        }

        [Test]
        public void ShortDecode(){
            DataResult<short> result = Codec.SHORT.Parse(JsonOps.INSTANCE, new JValue(20));
		    Assert.AreEqual(result.Result().Get(), (short)20);
        }

        //Codec.INT
        [Test]
        public void IntEncode(){
            DataResult<JToken> result = Codec.INT.EncodeStart(JsonOps.INSTANCE, 20);
		    Assert.AreEqual((int)result.Result().Get(), 20);
        }

        [Test]
        public void IntDecode(){
            DataResult<int> result = Codec.INT.Parse(JsonOps.INSTANCE, new JValue(20));
		    Assert.AreEqual(result.Result().Get(), 20);
        }

        //Codec.LONG
        [Test]
        public void LongEncode(){
            DataResult<JToken> result = Codec.LONG.EncodeStart(JsonOps.INSTANCE, 20L);
		    Assert.AreEqual((long)result.Result().Get(), 20L);
        }

        [Test]
        public void LongDecode(){
            DataResult<long> result = Codec.LONG.Parse(JsonOps.INSTANCE, new JValue(20));
		    Assert.AreEqual(result.Result().Get(), 20L);
        }

        //Codec.FLOAT
        [Test]
        public void FloatEncode(){
            DataResult<JToken> result = Codec.FLOAT.EncodeStart(JsonOps.INSTANCE, 20f);
		    Assert.AreEqual((float)result.Result().Get(), 20f);
        }

        [Test]
        public void FloatDecode(){
            DataResult<float> result = Codec.FLOAT.Parse(JsonOps.INSTANCE, new JValue(20f));
		    Assert.AreEqual(result.Result().Get(), 20f);
        }

        //Codec.DOUBLE
        [Test]
        public void DoubleEncode(){
            DataResult<JToken> result = Codec.DOUBLE.EncodeStart(JsonOps.INSTANCE, 20D);
		    Assert.AreEqual((double)result.Result().Get(), 20D);
        }

        [Test]
        public void DoubleDecode(){
            DataResult<double> result = Codec.DOUBLE.Parse(JsonOps.INSTANCE, new JValue(20D));
		    Assert.AreEqual(result.Result().Get(), 20D);
        }

        //Codec.STRING
        [Test]
        public void StringEncode(){
            DataResult<JToken> result = Codec.STRING.EncodeStart(JsonOps.INSTANCE, "Hello, World!");
		    Assert.AreEqual((string)result.Result().Get(), "Hello, World!");
        }

        [Test]
        public void StringDecode(){
            DataResult<string> result = Codec.STRING.Parse(JsonOps.INSTANCE, JValue.CreateString("Hello, World!"));
		    Assert.AreEqual(result.Result().Get(), "Hello, World!");
        }

        //Codec.MEMORY_STREAM
        [Test]
        public void MemoryStreamEncode(){
            MemoryStream input = new MemoryStream(new byte[]{1, 2, 3, 4, 5});
            DataResult<JToken> dataResult = Codec.MEMORY_STREAM.EncodeStart(JsonOps.INSTANCE, input);
            JToken result = dataResult.Result().Get();
            Assert.Multiple(() => {
                Assert.IsInstanceOf(typeof(JArray), result);
                if(result is JArray arr){
                    Assert.AreEqual(5, arr.Count);
                    Assert.AreEqual(1, (int)arr[0]);
                    Assert.AreEqual(5, (int)arr[4]);
                }
            });
        }

        [Test]
        public void MemoryStreamDecode(){
            JArray input = new JArray();
            input.Add(new JValue(1));
            input.Add(new JValue(2));
            input.Add(new JValue(3));
            input.Add(new JValue(4));
            input.Add(new JValue(5));
            DataResult<MemoryStream> dataResult = Codec.MEMORY_STREAM.Parse(JsonOps.INSTANCE, input);
		    MemoryStream result = dataResult.Result().Get();
            Assert.Multiple(() => {
                Assert.AreEqual(5, result.Length);
                Assert.AreEqual(1, result.ToArray()[0]);
                Assert.AreEqual(5, result.ToArray()[4]);
            });
        }
        
        //Codec.INT_ENUMERABLE
        [Test]
        public void IntEnumerableEncode(){
            List<int> input = new List<int>(){1, 2, 3, 4, 5};
            DataResult<JToken> dataResult = Codec.INT_ENUMERABLE.EncodeStart(JsonOps.INSTANCE, input);
            JToken result = dataResult.Result().Get();
            Assert.Multiple(() => {
                Assert.IsInstanceOf(typeof(JArray), result);
                if(result is JArray arr){
                    Assert.AreEqual(5, arr.Count);
                    Assert.AreEqual(1, (int)arr[0]);
                    Assert.AreEqual(5, (int)arr[4]);
                }
            });
        }

        [Test]
        public void IntEnumerableDecode(){
            JArray input = new JArray();
            input.Add(new JValue(1));
            input.Add(new JValue(2));
            input.Add(new JValue(3));
            input.Add(new JValue(4));
            input.Add(new JValue(5));
            DataResult<IEnumerable<int>> dataResult = Codec.INT_ENUMERABLE.Parse(JsonOps.INSTANCE, input);
            int[] result = dataResult.Result().Get().ToArray();
            Assert.Multiple(() => {
                Assert.AreEqual(5, result.Length);
                Assert.AreEqual(1, result[0]);
                Assert.AreEqual(5, result[4]);
            });
        }

        //Codec.LONG_ENUMERABLE
        [Test]
        public void LongEnumerableEncode(){
            List<long> input = new List<long>(){1L, 2L, 3L, 4L, 5L};
            DataResult<JToken> dataResult = Codec.LONG_ENUMERABLE.EncodeStart(JsonOps.INSTANCE, input);
            JToken result = dataResult.Result().Get();
            Assert.Multiple(() => {
                Assert.IsInstanceOf(typeof(JArray), result);
                if(result is JArray arr){
                    Assert.AreEqual(5, arr.Count);
                    Assert.AreEqual(1L, (long)arr[0]);
                    Assert.AreEqual(5L, (long)arr[4]);
                }
            });
        }

        [Test]
        public void LongEnumerableDecode(){
            JArray input = new JArray();
            input.Add(new JValue(1));
            input.Add(new JValue(2));
            input.Add(new JValue(3));
            input.Add(new JValue(4));
            input.Add(new JValue(5));
            DataResult<IEnumerable<long>> dataResult = Codec.LONG_ENUMERABLE.Parse(JsonOps.INSTANCE, input);
            long[] result = dataResult.Result().Get().ToArray();
            Assert.Multiple(() => {
                Assert.AreEqual(5, result.Length);
                Assert.AreEqual(1L, result[0]);
                Assert.AreEqual(5L, result[4]);
            });
        }
    }
}
