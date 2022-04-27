using System;
using NUnit.Framework;
using DataFixerUpper.Util;


namespace Tests.Util{
    [TestFixture]
    public class ArrayUtilsTests{
        private int[] array;
        
        
        [SetUp]
        public void SetUp(){
            array = new int[]{1, 2, 3, 4, 5};
        }
        
        [Test]
        public void Insert(){
            array = ArrayUtils.Insert(array, 2, 0);
            Assert.Multiple(() => {
                Assert.AreEqual(0, array[2]);
                Assert.AreEqual(6, array.Length);
            });
        }
        
        [Test]
        public void InsertNullArray(){
            int[] result = ArrayUtils.Insert(null, 0, 0);
            Assert.AreEqual(null, result);
        }
        
        [Test]
        public void InsertIndexOutOfRange(){
            Assert.Throws<ArgumentOutOfRangeException>(() => ArrayUtils.Insert(array, 6, 0));
        }
        
        [Test]
        public void Remove(){
            array = ArrayUtils.Remove(array, 2);
            Assert.Multiple(() => {
                Assert.AreEqual(4, array.Length);
                Assert.AreEqual(-1, Array.IndexOf(array, 3));
            });
        }
        
        [Test]
        public void RemoveNullArray(){
            int[] result = ArrayUtils.Remove<int>(null, 0);
            Assert.AreEqual(null, result);
        }
        
        [Test]
        public void RemoveIndexOutOfRange(){
            Assert.Throws<ArgumentOutOfRangeException>(() => ArrayUtils.Remove(array, 6));
        }
        
        [Test]
        public void HashCode(){
            int result = ArrayUtils.HashCode(array);
            Assert.AreEqual(29615266, result);
        }
        
        [Test]
        public void HashCodeNull(){
            int result = ArrayUtils.HashCode<int>(null);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void ContentsEqual(){
            int[] other = {1, 2, 3, 4, 5};
            Assert.IsTrue(ArrayUtils.ContentsEqual(array, other));
        }

        [Test]
        public void ContentsEqualNotEqual(){
            int[] other = {0, 1, 2, 3};
            Assert.IsFalse(ArrayUtils.ContentsEqual(array, other));
        }

        [Test]
        public void TestToString(){
            string result = ArrayUtils.ToString(array);
            Assert.AreEqual("[1, 2, 3, 4, 5]", result);
        }
        
        [Test]
        public void TestToStringNull(){
            string result = ArrayUtils.ToString<int>(null);
            Assert.AreEqual("null", result);
        }

        [Test]
        public void TestToStringEmpty(){
            string result = ArrayUtils.ToString(new int[]{});
            Assert.AreEqual("[]", result);
        }
    }
}
