namespace MouseBaseLib;

public interface IMatrix
{
    int Width { get; }
    int Height { get; }
    byte[] RawData { get; }
    ref byte At(int x, int y);
    ref byte AtWithCheck(int x, int y);
    ReadOnlySpan<byte> GetData();
    ReadOnlySpan<byte> GetRow(int y);
}
