namespace Computer;

public interface ICPU
{
    public abstract IRegister GetRegisterFromCode(byte code);
    public abstract IRegister GetRegisterFromModRM(byte code);
}

public class CPU1 : ICPU
{
    //                Group              // Name
    public Dictionary<string, Dictionary<string, IRegister>> Registers = [];

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
        
        Registers.Add("FLAGS", Make16To64("FLAGS"));
    }
    
    
    public Dictionary<string, IRegister> Make16To64(string name)
    {
        Reg64Value reg64 = new Reg64Value();
        Reg32Ref reg32 = new Reg32Ref(reg64);
        Reg16Ref reg16 = new Reg16Ref(reg64);
        return new()
        {
            {"R"+name, reg64},
            {"E"+name, reg32},
            {name, reg16},
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

    
    public IRegister GetRegisterFromModRM(byte b)
    {
        byte code =(byte) (b & 0b00111000);
        return code switch
        {
            0b00000000 => Registers["AX"]["RAX"],
            0b00001000 => Registers["CX"]["RCX"],
            0b00010000 => Registers["DX"]["RDX"],
            0b00011000 => Registers["BX"]["RBX"],
            _ => throw new NotSupportedException()
        };
    }
    
    public IRegister GetRegisterFromCode(byte code)
    {
        return code switch
        {
            0b000 => Registers["AX"]["RAX"],
            0b001 => Registers["CX"]["RCX"],
            0b010 => Registers["DX"]["RDX"],
            0b011 => Registers["BX"]["RBX"],
            _ => throw new NotSupportedException()
        };
    }
    
}

public struct ModRM
{
    public byte b;
    public readonly Mode m;

    public ModRM(byte b)
    {
        this.b = b;
        byte firstBits =(byte)(b & 0b11000000);
        m = firstBits switch
        {
            0b00000000 => Mode.Memory00,
            0b01000000 => Mode.Memory01,
            0b10000000 => Mode.Memory10,
            0b11000000 => Mode.Register,
            _ => throw new NotSupportedException()
        };
    }

    public enum Mode
    {
        Memory00,
        Memory01,
        Memory10,
        Register
    }
}