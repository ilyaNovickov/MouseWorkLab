namespace MouseBaseLib;

public interface IMatrix : IDisposable
{
    int Width { get; }
    int Height { get; }
    byte[] RawData { get; }
    ref byte At(int x, int y);

    ref byte At(Point point);
    ref byte AtWithCheck(int x, int y);

    ref byte AtWithCheck(Point point);
    ReadOnlySpan<byte> GetData();
    ReadOnlySpan<byte> GetRow(int y);
}
