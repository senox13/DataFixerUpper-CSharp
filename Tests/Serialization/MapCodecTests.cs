using NUnit.Framework;
using Newtonsoft.Json.Linq;
using DataFixerUpper.DataFixers.Kinds;
using DataFixerUpper.Serialization;
using DataFixerUpper.Serialization.Codecs;
using System;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpperTests.Serialization{
    [TestFixture]
    public class MapCodecTests{
        /*
         * Mock types
         */
        public sealed class TestData{
            public static readonly ICodec<TestData> CODEC = RecordCodecBuilder.Create<TestData>(i => i.Group(
			    Codec.INT.FieldOf("a").ForGetter<TestData>(t => t.a),
			    Codec.INT.FieldOf("b").ForGetter<TestData>(t => t.b),
			    Codec.INT.FieldOf("c").ForGetter<TestData>(t => t.c)
		    ).Apply(i, (a, b, c) => new TestData(a, b, c)));
		    public readonly int a;
		    public readonly int b;
		    public readonly int c;
		
		
		    public TestData(int a, int b, int c){
			    this.a = a;
			    this.b = b;
			    this.c = c;
		    }
        }

        [Test]
	    public void MapCodecEncode(){
		    TestData input = new TestData(1, 2, 3);
		    DataResult<JToken> dataResult = TestData.CODEC.EncodeStart(JsonOps.INSTANCE, input);
		    JToken result = dataResult.Result().Get();
            Assert.Multiple(() => {
		        Assert.IsInstanceOf(typeof(JObject), result);
		        if(result is JObject obj){
			        Assert.AreEqual(3, obj.Count);
			        Assert.AreEqual(1, (int)obj["a"]);
			        Assert.AreEqual(2, (int)obj["b"]);
			        Assert.AreEqual(3, (int)obj["c"]);
		        }
            });
	    }
	
	    [Test]
	    public void MapCodecDecode(){
		    JObject input = new JObject();
		    input.Add("a", new JValue(1));
		    input.Add("b", new JValue(2));
		    input.Add("c", new JValue(3));
		    DataResult<TestData> dataResult = TestData.CODEC.Parse(JsonOps.INSTANCE, input);
		    TestData result = dataResult.Result().Get();
            Assert.Multiple(() => {
		        Assert.AreEqual(1, result.a);
		        Assert.AreEqual(2, result.b);
		        Assert.AreEqual(3, result.c);
            });
	    }


        /*
         * FieldDecoder tests
         */
        [Test]
        public void FieldDecoderDecode(){
            IDecoder<int> decoder = new FieldDecoder<int>("a", Codec.INT).Decoder();
            JObject input = new JObject();
            input.Add("a", new JValue(20));
            DataResult<int> dataResult = decoder.Parse(JsonOps.INSTANCE, input);
            Assert.AreEqual(20, dataResult.Result().Get());
        }


        /*
         * FieldEncoder tests
         */
        [Test]
        public void FieldEncoderEncode(){
            IEncoder<int> encoder = new FieldEncoder<int>("a", Codec.INT).Encoder();
            DataResult<JToken> dataResult = encoder.EncodeStart(JsonOps.INSTANCE, 20);
            JToken result = dataResult.Result().Get();
            Assert.Multiple(() => {
                Assert.IsInstanceOf(typeof(JObject), result);
                if(result is JObject obj){
                    Assert.AreEqual(1, obj.Count);
                    Assert.AreEqual(20, (int)obj["a"]);
                }
            });
        }


        //TODO: EitherMapCodec

        //TODO: KeyDispatchCodec
        
        //TODO: OptionalFieldCodec

        //TODO: PairMapCodec

        //TODO: SimpleMapCodec

        //TODO: UnboundedMapCodec

    }
}
