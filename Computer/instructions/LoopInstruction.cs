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
            {
                CPUMode mode = cpu.Mode;
                if (mode == CPUMode.x32Prot)
                {
                    ulong count = cpu.GetRegister("CX", "ECX").Value64 - 1;
                    bool takeBranch = count > 0;
                    cpu.GetRegister("CX", "ECX").Value64 = count;
                    return  new InstructionReturnData("Looping " + rel, takeBranch ? rel + 2 : 2);
                }
                else throw new NotSupportedException();
            }
            default: throw new NotSupportedException("");
        }
    }
}