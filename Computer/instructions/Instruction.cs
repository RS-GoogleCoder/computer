namespace Computer;

public abstract class Instruction
{
    public InstructionCode Code;
    public byte Opcode;

    protected Instruction(InstructionCode code, byte opcode)
    {
        Code = code;
        Opcode = opcode;
    }

    public abstract InstructionReturnData Execute(ICPU icpu);
}



public enum InstructionCode
{
    ADD,
    MOV,
    CMP,
    Jcc,
    LOOP,
    INT
}

public record struct InstructionReturnData(string DebugMessage, int InstrPtrAdvance)
{
    public string DebugMessage = DebugMessage;
    public int InstrPtrAdvance = InstrPtrAdvance;

    public override string ToString() => DebugMessage;
}