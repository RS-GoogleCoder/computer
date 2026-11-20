namespace Computer;

public class XorInstruction : Instruction
{
    public byte Opcode;
    public byte[] Src;
    public XorInstruction(byte opcode, byte[] src) : base(InstructionCode.ADD, opcode)
    {
        Opcode = opcode;
        Src = src;
    }

    public override InstructionReturnData Execute(ICPU icpu)
    {
        if (icpu is not CPU1 cpu) throw new Exception("CPU not supported");
        CPUMode mode = cpu.Mode;
        ModRM m = new ModRM(Src[0]);
        
        if (mode == CPUMode.x16Real)
        {
            throw new NotSupportedException();
        }
        else if (mode == CPUMode.x32Prot)
        {
            IRegister src1 = cpu.GetRegisterFromNumber(m.RM);
            IRegister src2 = cpu.GetRegisterFromNumber(m.Reg);
            
            ulong result = src1.Value64 ^ src2.Value64;
            src1.Value64 ^= result;
            return new InstructionReturnData("XOR", 2);
        }
        else if (mode == CPUMode.x64Long)
        {
            throw new NotSupportedException();
        }
        else throw new NotSupportedException();
    }
}