using System.Collections.Generic;
using DataFixerUpper.DataFixers.Util;

namespace DataFixerUpper.Serialization{
    public interface IMapLike<T> : IEnumerable<Pair<T, T>>{
        T Get(T key);

        T Get(string key);
    }
}
