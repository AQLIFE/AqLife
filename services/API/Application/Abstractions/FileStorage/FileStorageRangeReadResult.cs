namespace AqLife.Application.Abstractions.FileStorage
{
    /// <summary>
    /// A storage stream for an inclusive byte range and the full object's length.
    /// </summary>
    public sealed record FileStorageRangeReadResult(
        Stream Stream,
        long TotalLength,
        long Start,
        long End)
    {
        public long ContentLength => End - Start + 1;
    }
}