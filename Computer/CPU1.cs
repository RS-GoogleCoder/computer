namespace Computer;

public interface ICPU
{
    public abstract IRegister GetRegisterFromNumber(byte code);
    public abstract IRegister GetRegister(string group, string name);
}

public class CPU1 : ICPU
{
    //                Group              // Name
    public Dictionary<string, Dictionary<string, IRegister>> Registers = [];
    public FlagsRegister Flags { get; } = new();
    
    public CPUMode Mode = CPUMode.x32Prot;

    public CPU1()
    {
        Registers.Add("AX", Make8To64("AX"));
        Registers.Add("BX", Make8To64("BX"));
        Registers.Add("CX", Make8To64("CX"));
        Registers.Add("DX", Make8To64("DX"));
        
        Registers.Add("BP", Make8To64("BP"));
        Registers.Add("DI", Make8To64("DI"));
        Registers.Add("IP", Make8To64("IP"));
        Registers.Add("SI", Make8To64("SI"));
        Registers.Add("SP", Make8To64("SP"));
        
        Registers.Add("FLAGS", MakeFlags());
    }
    
    public Dictionary<string, IRegister> MakeFlags()
    {
        Reg32Ref reg32 = new Reg32Ref(Flags);
        Reg16Ref reg16 = new Reg16Ref(Flags);
        return new()
        {
            { "RFLAGS", Flags },
            { "EFLAGS", reg32 },
            { "FLAGS",  reg16 },
        };
    }

    public Dictionary<string, IRegister> Make8To64(string name)
    {
        Reg64Value reg64 = new Reg64Value();
        Reg32Ref reg32 = new Reg32Ref(reg64);
        Reg16Ref reg16 = new Reg16Ref(reg64);
        Reg8Ref reg8Low = new Reg8Ref(reg64);
        Reg8Ref reg8High = new Reg8Ref(reg64,1);
        return new()
        {
            {"R"+name, reg64},
            {"E"+name, reg32},
            {name, reg16},
            {name+"H", reg8High},
            {name+"L", reg8Low},
        };
    }

    
    public IRegister GetRegisterFromNumber(byte code)
    {
        if (Mode != CPUMode.x32Prot) throw new NotSupportedException();
        return code switch
        {
            0b000 => Registers["AX"]["EAX"],
            0b001 => Registers["CX"]["ECX"],
            0b010 => Registers["DX"]["EDX"],
            0b011 => Registers["BX"]["EBX"],
            0b100 => Registers["SP"]["ESP"],
            0b101 => Registers["BP"]["EBP"],
            0b110 => Registers["SI"]["ESI"],
            0b111 => Registers["DI"]["EDI"],
            _ => throw new NotSupportedException()
        };
    }

    public IRegister GetRegister(string group, string name)
    {
        return Registers[group][name];
    }
}

public struct ModRM
{
    public byte Raw;
    public readonly RMMode RMMode;
    public readonly byte Reg;
    public readonly byte RM;

    public ModRM(byte raw)
    {
        this.Raw = raw;
        byte firstBits =(byte)(raw & 0b11000000);
        byte middleBits = (byte)(raw & 0b00111000);
        byte endBits = (byte)(raw & 0b00000111);
        RMMode = firstBits switch
        {
            0b00000000 => RMMode.Memory00,
            0b01000000 => RMMode.Memory01,
            0b10000000 => RMMode.Memory10,
            0b11000000 => RMMode.Register,
            _ => throw new NotSupportedException()
        };
        Reg = middleBits switch
        {
            0b00000000 => 0,
            0b00001000 => 1,
            0b00010000 => 2,
            0b00011000 => 3,
            0b00100000 => 4,
            0b00101000 => 5,
            0b00110000 => 6,
            0b00111000 => 7,
            _ => throw new NotSupportedException()
        };
        RM =endBits switch
        {
            0b00000000 => 0,
            0b00000001 => 1,
            0b00000010 => 2,
            0b00000011 => 3,
            0b00000100 => 4,
            0b00000101 => 5,
            0b00000110 => 6,
            0b00000111 => 7,
            _ => throw new NotSupportedException()
        };
    }

}
public enum RMMode
{
    Memory00,
    Memory01,
    Memory10,
    Register
}

public enum CPUMode
{
    x16Real = 2,
    x32Prot = 4,
    x64Long = 8,
}

[Flags]
public enum CpuFlags : ulong
{
    None = 0,
    CF   = 1UL << 0,  // Carry
    // Bit 1 is always 1 in x86
    PF   = 1UL << 2,  // Parity
    AF   = 1UL << 4,  // Auxiliary Carry
    ZF   = 1UL << 6,  // Zero
    SF   = 1UL << 7,  // Sign
    TF   = 1UL << 8,  // Trap
    IF   = 1UL << 9,  // Interrupt Enable
    DF   = 1UL << 10, // Direction
    OF   = 1UL << 11, // Overflow
}
