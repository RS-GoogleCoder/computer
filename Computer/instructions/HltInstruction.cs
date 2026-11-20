namespace Computer;

public class HltInstruction : Instruction
{
    public HltInstruction(byte opcode) : base(InstructionCode.Hlt, opcode)
    {
    }

    public override InstructionReturnData Execute(ICPU icpu)
    {
        return new InstructionReturnData("Halting", 0, true);
    }
}