namespace Computer;

public class TestInstruction : Instruction
{
    public byte Opcode;
    public byte[] Src;

    public TestInstruction(byte opcode, byte[] src) : base(InstructionCode.ADD, opcode)
    {
        Opcode = opcode;
        Src = src;
    }

    public override InstructionReturnData Execute(ICPU icpu)
    {
        if (icpu is not CPU1 cpu) throw new Exception("CPU not supported");
        CPUMode mode = cpu.Mode;
        switch (Opcode)
        {
            case 0x85:
            {
                ModRM m = new ModRM(Src[0]);
                IRegister src1 = cpu.GetRegisterFromNumber(m.RM, cpu.Mode.ToRegisterSize());
                IRegister src2 = cpu.GetRegisterFromNumber(m.Reg, cpu.Mode.ToRegisterSize());
                ulong result = src1.Value64 & src2.Value64;
                cpu.Flags.ZF = result == 0;
                return new InstructionReturnData(cpu.Flags.ToString(), 2);
            }
            default: throw new Exception("invalid instruction code: " + Opcode);
        }
    }
}
