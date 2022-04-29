using NUnit.Framework;
using MCEngine.Util;
using System.Collections.Generic;
using System;


namespace Tests.Util{
    [TestFixture]
    public class DictUtilsTests{
        [Test]
        public void CreateDict(){
            Dictionary<int, string> result = DictUtils.CreateDict(
                new int[]{1, 2, 3},
                new string[]{"one", "two", "three"}
            );
            Assert.Multiple(() => {
                Assert.AreEqual(3, result.Count);
                Assert.Contains(new KeyValuePair<int, string>(1, "one"), result);
                Assert.Contains(new KeyValuePair<int, string>(2, "two"), result);
                Assert.Contains(new KeyValuePair<int, string>(3, "three"), result);
            });
        }

        [Test]
        public void CreateDictDifferentLengths(){
            Dictionary<int, string> result = DictUtils.CreateDict(
                new int[]{1, 2, 3, 4, 5},
                new string[]{"one", "two", "three"}
            );
            Assert.Multiple(() => {
                Assert.AreEqual(3, result.Count);
                Assert.Contains(new KeyValuePair<int, string>(1, "one"), result);
                Assert.Contains(new KeyValuePair<int, string>(2, "two"), result);
                Assert.Contains(new KeyValuePair<int, string>(3, "three"), result);
            });
        }

        //There is no need to test PopulateDict's standard behaviour, as it is always invoked by CreateDict
        [Test]
        public void PopulateDictDuplicateKey(){
            Dictionary<int, string> dict = new Dictionary<int, string>{
                {1, "one"},
                {2, "two"},
                {3, "three"}
            };
            Assert.Throws<ArgumentException>(() => DictUtils.PopulateDict(new int[]{3, 4}, new string[]{"three", "four"}, dict));
        }

        [Test]
        public void ContentsHashCode(){
            Dictionary<int, string> dict = new Dictionary<int, string>(){
                {1, "one"},
                {2, "two"},
                {3, "three"}
            };
            Dictionary<int, string> otherDict = new Dictionary<int, string>(){
                {1, "one"},
                {2, "two"},
                {3, "three"}
            };
            int expected = DictUtils.ContentsHashCode(dict);
            int result = DictUtils.ContentsHashCode(otherDict);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ContentsHashCodeNull(){
            int result = DictUtils.ContentsHashCode<int, string>(null);
            Assert.Zero(result);
        }
    }
}
