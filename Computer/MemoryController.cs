namespace Computer;

public static class MemoryController
{
    private static byte[] _memory;

    public static void Init(uint size)
    {
        _memory = new byte[size];
    }

    public static void Set(byte[] data, ulong startIndex, ulong length)
    {
        Array.Copy(data, 0, _memory, (long)startIndex, (long)length);
    }

    public static void Set(byte data, ulong index)
    {
        _memory[index] = data;
    }

    public static Span<byte> Get(ulong startIndex, ulong length)
    {
        return _memory.AsSpan((int)startIndex, (int)length);
    }

    public static byte Get(ulong index) => _memory[index];

    public static byte[] Get() => _memory;
}