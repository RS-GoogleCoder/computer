using System.Buffers.Binary;

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
        if (icpu is not CPU1 cpu) throw new Exception("CPU not supported");
        CPUMode mode = cpu.Mode;
        IRegister stackPointer = cpu.GetRegister("SP", "RSP");
        switch (Opcode)
        {
            case 0x58:
            case 0x59:
            case 0x5A:
            case 0x5B:
            case 0x5C:
            case 0x5D:
            case 0x5E:
            case 0x5F:
            {
                IRegister dest = cpu.GetRegisterFromNumber((byte)(Opcode - 0x58), mode.ToRegisterSize());

                if (mode == CPUMode.x16Real)
                {
                    dest.Value64 =
                        BinaryPrimitives.ReadUInt16LittleEndian(MemoryController.Get(stackPointer.Value64 - 2, 2));
                    stackPointer.Value64 -= 2;
                    return new InstructionReturnData("Removed " + dest.Value64 + " from the stack", 1);
                }
                else if (mode == CPUMode.x32Prot)
                {
                    dest.Value64 =
                        BinaryPrimitives.ReadUInt32LittleEndian(MemoryController.Get(stackPointer.Value64 - 4, 4));
                    stackPointer.Value64 -= 4;
                    return new InstructionReturnData("Removed " + dest.Value64 + " from the stack", 1);
                }
                else if (mode == CPUMode.x64Long)
                {
                    throw new NotSupportedException();
                }
                else throw new NotSupportedException();
            }
            default: throw new NotSupportedException();
        }
    }
}