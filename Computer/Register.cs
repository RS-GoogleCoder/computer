namespace Computer;

public interface IRegister
{
    public ulong Value64 { get; set; }
}

public interface IReg8 : IRegister
{
    public byte Value { get; set; }
}

public interface IReg16 : IRegister
{
    public ushort Value { get; set; }
}

public interface IReg32 : IRegister
{
    public uint Value { get; set; }
}

public interface IReg64 : IRegister
{
    public ulong Value { get; set; }
}

public class Reg8Value : IReg8
{
    public byte Value { get; set; }

    public ulong Value64
    {
        get => Value;
        set
        {
            if (value > byte.MaxValue) throw new Exception();
            Value = (byte)value;
        }
    } 
}

public class Reg16Value : IReg16
{
    public ushort Value { get; set; }

    public ulong Value64
    {
        get => Value;
        set
        {
            if (value > ushort.MaxValue) throw new Exception();
            Value = (ushort)value;
        }
    } 
}

public class Reg32Value : IReg32
{
    public uint Value { get; set; }
    
    public ulong Value64
    {
        get => Value;
        set
        {
            if (value > uint.MaxValue) throw new Exception();
            Value = (uint)value;
        }
    } 
}

public class Reg64Value : IReg64
{
    public ulong Value { get; set; }
    
    public ulong Value64
    {
        get => Value;
        set
        {
            Value = value;
        }
    } 
}

public class Reg8Ref : IReg8
{
    public IRegister Reference;
    private readonly int _offset;

    public Reg8Ref(IRegister reference, int byteOffset = 0)
    {
        Reference = reference;
        _offset = byteOffset * 8;
    }

    public byte Value
    {
        get
        {
            ulong val = Reference switch
            {
                IReg16 r16 => r16.Value,
                IReg32 r32 => r32.Value,
                IReg64 r64 => r64.Value,
                _ => 0
            };
            return (byte)((val >> _offset) & 0xFF);
        }
        set
        {
            switch (Reference)
            {
                case IReg16 r16:
                    r16.Value = (ushort)((r16.Value & ~(0xFF << _offset)) | ((value & 0xFF) << _offset));
                    break;
                case IReg32 r32:
                    r32.Value = (r32.Value & ~(0xFFU << _offset)) | ((uint)(value & 0xFF) << _offset);
                    break;
                case IReg64 r64:
                    r64.Value = (r64.Value & ~(0xFFUL << _offset)) | ((ulong)(value & 0xFF) << _offset);
                    break;
            }
        }
    }
    public ulong Value64
    {
        get => Value;
        set
        {
            if (value > byte.MaxValue) throw new Exception();
            Value = (byte)value;
        }
    } 
}

public class Reg16Ref : IReg16
{
    public IRegister Reference;
    private readonly int _offset;

    public Reg16Ref(IRegister reference, int wordOffset = 0)
    {
        Reference = reference;
        _offset = wordOffset * 16;
    }

    public ushort Value
    {
        get
        {
            ulong val = Reference switch
            {
                IReg32 r32 => r32.Value,
                IReg64 r64 => r64.Value,
                _ => 0
            };
            return (ushort)((val >> _offset) & 0xFFFF);
        }
        set
        {
            switch (Reference)
            {
                case IReg32 r32:
                    r32.Value = (r32.Value & ~(0xFFFFU << _offset)) | ((uint)(value & 0xFFFF) << _offset);
                    break;
                case IReg64 r64:
                    r64.Value = (r64.Value & ~(0xFFFFUL << _offset)) | ((ulong)(value & 0xFFFF) << _offset);
                    break;
            }
        }
    }
    public ulong Value64
    {
        get => Value;
        set
        {
            if (value > ushort.MaxValue) throw new Exception();
            Value = (ushort)value;
        }
    } 
}

public class Reg32Ref : IReg32
{
    public IReg64 Reference;
    private readonly int _offset;

    public Reg32Ref(IReg64 reference, int dwordOffset = 0)
    {
        Reference = reference;
        _offset = dwordOffset * 32;
    }

    public uint Value
    {
        get
        {
            return (uint)((Reference.Value >> _offset) & 0xFFFFFFFF);
        }
        set
        {
            Reference.Value = (Reference.Value & ~(0xFFFFFFFFUL << _offset)) | ((ulong)value << _offset);
        }
    }
    public ulong Value64
    {
        get => Value;
        set
        {
            if (value > uint.MaxValue) throw new Exception();
            Value = (uint)value;
        }
    } 
}
