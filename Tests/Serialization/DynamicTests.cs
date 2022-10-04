using NUnit.Framework;
using Newtonsoft.Json.Linq;
using DataFixerUpper.Serialization;
using JavaUtilities;

namespace DataFixerUpperTests.Serialization{
    [TestFixture]
    public sealed class DynamicTests{
        /*
         * Fields
         */
        private static readonly ICodec<StubType> CODEC = Codec.PASSTHROUGH.ComapFlatMap(
            d => DataResult.Success(new StubType((int)d.Convert(JsonOps.INSTANCE).GetValue())),
            s => new Dynamic<JToken>(JsonOps.INSTANCE, s.value)
        );


        /*
         * Test methods
         */
        [Test]
        public void Decode(){
            JValue token = new JValue(10);
            StubType result = CODEC.Parse(JsonOps.INSTANCE, token).Result().Get();
            Assert.AreEqual(10, result.value);
        }

        [Test]
        public void Encode(){
            StubType obj = new StubType(10);
            Optional<JToken> result = CODEC.EncodeStart(JsonOps.INSTANCE, obj).Result();
            Assert.Multiple(() => {
                Assert.IsTrue(result.IsPresent());
                Assert.AreEqual(10, (int)result.Get());
            });
        }


        /*
         * Nested types
         */
        public sealed class StubType{
            public readonly int value;

            public StubType(int valueIn){
                value = valueIn;
            }
        }
    }
}
