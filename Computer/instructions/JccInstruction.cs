namespace Computer;

public class JccInstruction : Instruction
{
    public byte Opcode;
    public byte[] Src;
    public JccInstruction(byte opcode, byte[] src) : base(InstructionCode.Jcc, opcode)
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
            case 0x74:
            { // jump zero
                if (cpu.Flags.ZF) return new InstructionReturnData("Jumping " + rel, rel);
                else return new InstructionReturnData("Not jumping", 2);
            }
                default: throw new NotSupportedException("");
        }
    }
}