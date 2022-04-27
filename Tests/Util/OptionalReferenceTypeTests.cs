using System;
using NUnit.Framework;
using DataFixerUpper.Util;


namespace Tests.Util{
    [TestFixture]
    public class OptionalReferenceTypeTests{
        private static readonly string TEST_STRING = "Hello, world!";
        private Optional<string> testOptional;


        [SetUp]
        public void SetUp(){
            testOptional = Optional<string>.Of(TEST_STRING);
        }


        [Test]
        public void OfNullableNonNull(){
            Optional<string> result = Optional<string>.OfNullable(TEST_STRING);
            Assert.AreSame(TEST_STRING, result.Get());
        }

        [Test]
        public void OfNullableNull(){
            Optional<string> result = Optional<string>.OfNullable(null);
            Assert.AreEqual(Optional<string>.Empty(), result);
        }

        [Test]
        public void OfNull(){
            Assert.Throws<ArgumentNullException>(() => Optional<string>.Of(null));
        }

        [Test]
        public void IsPresent(){
            Assert.IsTrue(testOptional.IsPresent());
        }

        [Test]
        public void IsPresentEmpty(){
            Assert.IsTrue(Optional<string>.Empty().IsEmpty());
        }

        [Test]
        public void IfPresent(){
            bool called = false;
            testOptional.IfPresent(s => called=true);
            Assert.IsTrue(called);
        }

        [Test]
        public void IfPresentEmpty(){
            bool called = false;
            Optional<string>.Empty().IfPresent(s => called=true);
            Assert.IsFalse(called);
        }

        [Test]
        public void IfPresentOrElse(){
            bool presentCalled = false;
            bool emptyCalled = false;
            testOptional.IfPresentOrElse(
                s => presentCalled=true,
                () => emptyCalled=true
            );
            Assert.Multiple(() => {
                Assert.IsTrue(presentCalled);
                Assert.IsFalse(emptyCalled);
            });
        }

        [Test]
        public void IfPresentOrElseEmpty(){
            bool presentCalled = false;
            bool emptyCalled = false;
            Optional<string>.Empty().IfPresentOrElse(
                s => presentCalled=true,
                () => emptyCalled=true
            );
            Assert.Multiple(() => {
                Assert.IsFalse(presentCalled);
                Assert.IsTrue(emptyCalled);
            });
        }

        [Test]
        public void FilterTrue(){
            Optional<string> result = testOptional.Filter(s => true);
            Assert.AreEqual(testOptional, result);
        }

        [Test]
        public void FilterFalse(){
            Optional<string> result = testOptional.Filter(s => false);
            Assert.AreEqual(Optional<string>.Empty(), result);
        }

        [Test]
        public void Map(){
            Optional<int> result = testOptional.Map(s => s.Length);
            Assert.AreEqual(TEST_STRING.Length, result.Get());
        }

        [Test]
        public void FlatMap(){
            Optional<int> result = testOptional.FlatMap(s => Optional<int>.Of(s.Length));
            Assert.AreEqual(TEST_STRING.Length, result.Get());
        }

        [Test]
        public void Or(){
            Optional<string> result = testOptional.Or(() => Optional<string>.Of("Goodbye, world!"));
            Assert.AreSame(TEST_STRING, result.Get());
        }

        [Test]
        public void OrEmpty(){
            string emptyString = "Goodbye, world!";
            Optional<string> result = Optional<string>.Empty().Or(() => Optional<string>.Of(emptyString));
            Assert.AreSame(emptyString, result.Get());
        }

        [Test]
        public void OrElse(){
            string result = testOptional.OrElse("Goodbye, world!");
            Assert.AreSame(TEST_STRING, result);
        }

        [Test]
        public void OrElseEmpty(){
            string emptyString = "Goodbye, world!";
            string result = Optional<string>.Empty().OrElse(emptyString);
            Assert.AreSame(emptyString, result);
        }

        [Test]
        public void OrElseGet(){
            string result = testOptional.OrElseGet(() => "Goodbye, world!");
            Assert.AreSame(TEST_STRING, result);
        }

        [Test]
        public void OrElseGetEmpty(){
            string emptyString = "Goodbye, world!";
            string result = Optional<string>.Empty().OrElseGet(() => emptyString);
            Assert.AreSame(emptyString, result);
        }

        [Test]
        public void OrElseThrow(){
            string result = testOptional.OrElseThrow();
            Assert.AreSame(TEST_STRING, result);
        }

        [Test]
        public void OrElseThrowEmpty(){
            Assert.Throws<InvalidOperationException>(() => Optional<string>.Empty().OrElseThrow());
        }

        [Test]
        public void OrElseThrowSupplier(){
            string result = testOptional.OrElseThrow(() => new Exception("supplier"));
            Assert.AreSame(TEST_STRING, result);
        }

        [Test]
        public void OrElseThrowSupplierEmpty(){
            string exceptionMessage = "supplier";
            Assert.Throws<Exception>(
                () => Optional<string>.Empty().OrElseThrow(() => new Exception(exceptionMessage)),
                exceptionMessage
            );
        }

        [Test]
        public void Equals(){
            Optional<string> other = Optional<string>.Of(TEST_STRING);
            Assert.IsTrue(testOptional.Equals(other));
        }

        [Test]
        public void EqualsNotEqual(){
            Assert.IsFalse(testOptional.Equals(Optional<string>.Empty()));
        }

        [Test]
        public void TestToString(){
            string result = testOptional.ToString();
            Assert.AreEqual("Optional<String>[Hello, world!]", result);
        }

        [Test]
        public void TestToStringEmpty(){
            string result = Optional<string>.Empty().ToString();
            Assert.AreEqual("Optional<String>.empty", result);
        }
    }
}
