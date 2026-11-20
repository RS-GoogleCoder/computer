namespace Computer;

public class IncInstruction : Instruction
{
    public byte Opcode;
    public byte[] Src;

    public IncInstruction(byte opcode, byte[] src) : base(InstructionCode.ADD, opcode)
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
            case 0x40:
            case 0x41:
            case 0x42:
            case 0x43:
            case 0x44:
            case 0x45:
            case 0x46:
            case 0x47:
            {
                IRegister reg = cpu.GetRegisterFromNumber((byte)(Opcode - 0x40), cpu.Mode.ToRegisterSize());
                reg.Value64++;
                return new InstructionReturnData(reg.Value64.ToString(), 1);
            }
            default: throw new Exception("invalid instruction code: " + Opcode);
        }
    }
}
    