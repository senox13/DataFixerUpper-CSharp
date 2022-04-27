using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using DataFixerUpper.DataFixers.Util;
using DataFixerUpper.Serialization;
using DataFixerUpperTests.Serialization.Stubs;

namespace DataFixerUpperTests.Serialization{
    [TestFixture]
    public class CodecTests{
        /*
         * Public ICodec implementation tests
         */
        //ListCodec
        [Test]
        public void ListEncode(){
            ICodec<IList<string>> codec = Codec.STRING.ListOf();
            IList<string> input = new List<string>(){"one", "two", "three", "four", "five"};
            DataResult<JToken> result = codec.EncodeStart(JsonOps.INSTANCE, input);
            Assert.Multiple(() => {
                Assert.IsInstanceOf(typeof(JArray), result.Result().Get());
                if(result.Get().Left().Get() is JArray arr){
                    Assert.AreEqual(input.Count, arr.Count);
                    Assert.AreEqual(arr[0], JValue.CreateString("one"));
                    Assert.AreEqual(arr[4], JValue.CreateString("five"));
                }
            });
        }

        [Test]
        public void ListParse(){
            ICodec<IList<string>> codec = Codec.STRING.ListOf();
            JArray input = new JArray(){
                JValue.CreateString("one"),
                JValue.CreateString("two"),
                JValue.CreateString("three"),
                JValue.CreateString("four"),
                JValue.CreateString("five")
            };
            DataResult<IList<string>> dataResult = codec.Parse(JsonOps.INSTANCE, input);
            IList<string> result = dataResult.Result().Get();
            Assert.Multiple(() => {
                Assert.AreEqual(input.Count, result.Count);
                Assert.AreEqual("one", result[0]);
                Assert.AreEqual("five", result[4]);
            });
        }

        //CompoundListCodec
        [Test]
        public void CompoundListEncode(){
            ICodec<IList<Pair<string, int>>> codec = Codec.CompoundList(Codec.STRING, Codec.INT);
            IList<Pair<string, int>> input = new List<Pair<string, int>>(){
                Pair.Of("one", 1),
                Pair.Of("two", 2),
                Pair.Of("three", 3),
                Pair.Of("four", 4),
                Pair.Of("five", 5)
            };
            DataResult<JToken> dataResult = codec.EncodeStart(JsonOps.INSTANCE, input);
            JToken result = dataResult.Result().Get();
            Assert.Multiple(() => {
                Assert.IsInstanceOf(typeof(JObject), result);
                if(result is JObject obj){
                    Assert.AreEqual(5, obj.Count);
                    Assert.AreEqual(1, (int)obj["one"]);
                    Assert.AreEqual(5, (int)obj["five"]);
                }
            });
        }

        [Test]
        public void CompoundListParse(){
            ICodec<IList<Pair<string, int>>> codec = Codec.CompoundList(Codec.STRING, Codec.INT);
            JObject input = new JObject(){
                {"one", 1},
                {"two", 2},
                {"three", 3},
                {"four", 4},
                {"five", 5}
            };
            DataResult<IList<Pair<string, int>>> dataResult = codec.Parse(JsonOps.INSTANCE, input);
            IList<Pair<string, int>> result = dataResult.Result().Get();
            Assert.Multiple(() => {
                Assert.AreEqual(5, result.Count);
                Assert.AreEqual(result[0].GetFirst(), "one");
                Assert.AreEqual(result[0].GetSecond(), 1);
                Assert.AreEqual(result[4].GetFirst(), "five");
                Assert.AreEqual(result[4].GetSecond(), 5);
            });
        }

        //EitherCodec
        [Test]
	    public void EitherEncodeRight(){
		    ICodec<Either<int, string>> codec = Codec.Either(Codec.INT, Codec.STRING);
		    Either<int, string> input = Either<int, string>.Right("Test");
		    DataResult<JToken> dataResult = codec.EncodeStart(JsonOps.INSTANCE, input);
            Assert.Multiple(() => {
		        Assert.IsTrue(dataResult.Result().IsPresent());
		        Assert.AreEqual((string)dataResult.Result().Get(), "Test");
            });
	    }
	
	    [Test]
	    public void EitherEncodeLeft(){
		    ICodec<Either<int, string>> codec = Codec.Either(Codec.INT, Codec.STRING);
		    Either<int, string> input = Either<int, string>.Left(20);
		    DataResult<JToken> dataResult = codec.EncodeStart(JsonOps.INSTANCE, input);
            Assert.Multiple(() => {
		        Assert.IsTrue(dataResult.Result().IsPresent());
		        Assert.AreEqual((int)dataResult.Result().Get(), 20);
            });
	    }
	
	    [Test]
	    public void EitherDecodeRight(){
		    ICodec<Either<int, string>> codec = Codec.Either(Codec.INT, Codec.STRING);
		    JValue input = JValue.CreateString("Test");
		    DataResult<Either<int, string>> dataResult = codec.Parse(JsonOps.INSTANCE, input);
		    Either<int, string> result = dataResult.Result().Get();
            Assert.Multiple(() => {
		        Assert.IsTrue(result.Right().IsPresent());
		        Assert.AreEqual(result.Right().Get(), "Test");
            });
	    }
	
	    [Test]
	    public void EitherDecodeLeft(){
		    ICodec<Either<int, string>> codec = Codec.Either(Codec.INT, Codec.STRING);
		    JValue input = new JValue(20);
		    DataResult<Either<int, string>> dataResult = codec.Parse(JsonOps.INSTANCE, input);
		    Either<int, string> result = dataResult.Result().Get();
            Assert.Multiple(() => {
		        Assert.IsTrue(result.Left().IsPresent());
		        Assert.AreEqual(result.Left().Get(), 20);
            });
	    }

        //TODO: PairCodec


        /*
         * Private ICodec implementation tests
         */
        //CodecExtensions.LifecycleWrapper
        [Test]
        public void WithLifecycleEncode(){
            ICodec<string> codec = Codec.STRING.WithLifecycle(Lifecycle.Stable());
            DataResult<JToken> dataResult = codec.EncodeStart(JsonOps.INSTANCE, "Test");
            Assert.AreSame(Lifecycle.Stable(), dataResult.Lifecycle());
        }

        [Test]
        public void WithLifecycleDecode(){
            ICodec<string> codec = Codec.STRING.WithLifecycle(Lifecycle.Stable());
            DataResult<string> dataResult = codec.Parse(JsonOps.INSTANCE, JValue.CreateString("Test"));
            Assert.AreSame(Lifecycle.Stable(), dataResult.Lifecycle());
        }

        //CodecExtensions.OrElseResultFunc
        [Test]
        public void OrElseParse(){
            bool errored = false;
            ICodec<int> codec = Codec.INT.OrElse(err => errored=true, -1);
            DataResult<int> dataResult = codec.Parse(JsonOps.INSTANCE, JValue.CreateString("Test"));
            Assert.Multiple(() => {
                Assert.AreEqual(-1, dataResult.Result().Get());
                Assert.IsTrue(errored);
            });
        }

        [Test]
        public void OrElseEncode(){
            bool errored = false;
            ICodec<string> codec = ErrorCodec.INSTANCE.OrElse(err => errored=true, "default");
            DataResult<JToken> dataResult = codec.EncodeStart(JsonOps.INSTANCE, "test");
            Assert.Multiple(() => {
                Assert.IsTrue(dataResult.Result().IsEmpty());
                Assert.IsTrue(errored);
            });
        }

        //CodecExtensions.OrElseResultFuncDeferred
        [Test]
        public void OrElseGetParse(){
            bool errored = false;
            ICodec<int> codec = Codec.INT.OrElseGet(err => errored=true, () => -1);
            DataResult<int> dataResult = codec.Parse(JsonOps.INSTANCE, JValue.CreateString("Test"));
            Assert.Multiple(() => {
                Assert.AreEqual(-1, dataResult.Result().Get());
                Assert.IsTrue(errored);
            });
        }

        [Test]
        public void OrElseGetEncode(){
            bool errored = false;
            ICodec<string> codec = ErrorCodec.INSTANCE.OrElseGet(err => errored=true, () => "default");
            DataResult<JToken> dataResult = codec.EncodeStart(JsonOps.INSTANCE, "test");
            Assert.Multiple(() => {
                Assert.IsTrue(dataResult.Result().IsEmpty());
                Assert.IsTrue(errored);
            });
        }
    }
}
