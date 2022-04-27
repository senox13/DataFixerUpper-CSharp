namespace DataFixerUpper.Serialization{
    public interface ICompressable : IKeyable{
        KeyCompressor<T> Compressor<T>(DynamicOps<T> ops);
    }
}
