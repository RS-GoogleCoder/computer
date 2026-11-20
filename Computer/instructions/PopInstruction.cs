namespace Computer;

public class PopInstruction : Instruction
{
    public byte Opcode;
    public byte[] Src;
    public PopInstruction(byte opcode, byte[] src) : base(InstructionCode.Jcc, opcode)
    {
        Opcode = opcode;
        Src = src;
    }

    public override InstructionReturnData Execute(ICPU icpu)
    {
        if(icpu is not CPU1 cpu) throw new Exception("CPU not supported");
        CPUMode mode = cpu.Mode;
        IRegister stackPointer = cpu.GetRegister("SP", "RSP");
        switch (Opcode)
        {
            case 0x50:
            case 0x51:
            case 0x52:
            case 0x53:
            case 0x54:
            case 0x55:
            case 0x56:
            case 0x57:
            {
                IRegister dest = cpu.GetRegisterFromNumber((byte)(Opcode - 0x50));

                if (mode == CPUMode.x16Real)
                {
                    throw new NotSupportedException();
                }
                else if (mode == CPUMode.x32Prot)
                {
                    dest.Value64 = BitConverter.ToUInt64(MemoryController.Get(stackPointer.Value64 - 4, 4));
                    stackPointer.Value64 -= 4;
                    return new InstructionReturnData("Removed " + dest.Value64 + " from the stack", 1);
                }
                else if (mode == CPUMode.x64Long)
                {
                    throw new NotSupportedException();
                }
                else throw new NotSupportedException();
            }
            default:throw new NotSupportedException();
        }
    }
}