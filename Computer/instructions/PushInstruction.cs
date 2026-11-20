namespace Computer;

public class PushInstruction : Instruction
{
    public byte Opcode;
    public byte[] Src;
    public PushInstruction(byte opcode, byte[] src) : base(InstructionCode.Jcc, opcode)
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
                IRegister src = cpu.GetRegisterFromNumber((byte)(Opcode - 0x50), mode.ToRegisterSize());

                if (mode == CPUMode.x16Real)
                {
                    ushort s = (ushort)src.Value64;
                    byte[] bytes = BitConverter.GetBytes(s);
                    MemoryController.Set(bytes, stackPointer.Value64, 2);
                    stackPointer.Value64 += 2;
                    return new InstructionReturnData("Added " + s + " to the stack", 1);
                }
                else if (mode == CPUMode.x32Prot)
                {
                    uint s = (uint)src.Value64;
                    byte[] bytes = BitConverter.GetBytes(s);
                    MemoryController.Set(bytes, stackPointer.Value64, 4);
                    stackPointer.Value64 += 4;
                    return new InstructionReturnData("Added " + s + " to the stack", 1);
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