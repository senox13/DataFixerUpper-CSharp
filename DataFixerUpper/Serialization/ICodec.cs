namespace DataFixerUpper.Serialization{
    public interface ICodec<A> : IEncoder<A>, IDecoder<A>{}
}
