namespace Computer;

public class LoopInstruction : Instruction
{
    public byte Opcode;
    public byte[] Src;
    public LoopInstruction(byte opcode, byte[] src) : base(InstructionCode.LOOP, opcode)
    {
        Opcode = opcode;
        Src = src;
    }

    public override InstructionReturnData Execute(ICPU icpu)
    {
        if(icpu is not CPU1 cpu) throw new Exception("CPU not supported");
        sbyte rel = (sbyte) Src[0];
        switch (Opcode)
        {
            case 0xE2:
            { // jump zero
                if (cpu.Flags.ZF) return new InstructionReturnData("Jumping", rel);
                else return new InstructionReturnData("Not jumping", 0);
            }
            default: throw new NotSupportedException("");
        }
    }
}