namespace Computer;

public class DivInstruction : Instruction
    {
    public byte Opcode;
    public byte[] Src;
    public DivInstruction(byte opcode, byte[] src) : base(InstructionCode.ADD, opcode)
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
            case 0xF7:
            {
                ModRM m = new ModRM(Src[0]);
                IRegister reg = cpu.GetRegisterFromNumber(m.RM, mode.ToRegisterSize());
                if (mode == CPUMode.x32Prot)
                {
                    IRegister edx = cpu.GetRegister("DX", "EDX");
                    IRegister eax = cpu.GetRegister("AX", "EAX");
                    uint edxval = (uint)edx.Value64;
                    uint eaxval = (uint)eax.Value64;
                    ulong dividend = ((ulong)edxval << 32) | eaxval;
                    ulong divisor = reg.Value64;
                    
                    if(divisor == 0) throw new ArithmeticException("Division by zero");
                    
                    ulong quotient = dividend / divisor;
                    ulong remainder =  dividend % divisor;
                    
                    edx.Value64 = remainder;
                    eax.Value64 = quotient;
                    return new InstructionReturnData("Div", 2);
                }
                else throw new NotSupportedException();
                throw new NotImplementedException();
            }
                default: throw new Exception("Opcode not supported");
        }
    }
}