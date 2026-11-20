using System.Buffers.Binary;

namespace Computer;

public class CallInstruction : Instruction
{
    public byte Opcode;
    public byte[] Src;
    public CallInstruction(byte opcode, byte[] src) : base(InstructionCode.Jcc, opcode)
    {
        Opcode = opcode;
        Src = src;
    }

    public override InstructionReturnData Execute(ICPU icpu)
    {
        if(icpu is not CPU1 cpu) throw new Exception("CPU not supported");
        int rel = BinaryPrimitives.ReadInt32LittleEndian(Src);
        switch (Opcode)
        {
            case 0xE8:
            { 
                return new InstructionReturnData("Calling " + rel + 5, rel + 5);
            }
            default: throw new NotSupportedException("");
        }
    }
}