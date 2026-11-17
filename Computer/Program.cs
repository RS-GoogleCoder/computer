namespace Computer;

class Program
{
    public static byte[] Memory = new byte[65536];
    
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        /*
         * • 01: ADD a Register to a Register or Memory (add [mem]/reg, reg)
            • 03: ADD a Register or Memory to a Register (add reg, [mem]/reg)
            • 81 / 83: ADD an Immediate value (constant) to a Register or Memory (add [mem]/reg, imm)
         */
        
        byte[] test1 =
        [
            0x83, 0xC0, 0x05, //add 5 to EAX
            //0x01,0b11_000_011 // Add register RAX to RBX
        ];
        
        
        CPU1 cpu = new CPU1();
        ((Reg32Ref)cpu.Registers["AX"]["EAX"]).Value = 50; 
        BinaryReader reader = new BinaryReader(new MemoryStream(test1));
        var test2 = DecodeInstruction(reader);
        Console.WriteLine(test2.Execute(cpu));
    }

    public static Instruction DecodeInstruction(BinaryReader br)
    {
        byte opcode = br.ReadByte();
        InstructionCode code;
        switch (opcode)
        {
            case 0x83: code = InstructionCode.ADD; break;
            default: throw new Exception("invalid instruction code: " + opcode);
        }

        Instruction instruction;
        if (code == InstructionCode.ADD)
        {
            switch (opcode)
            {
                case 0x83:
                {
                    byte modRM = br.ReadByte();
                    byte imm = br.ReadByte();
                    
                    instruction = new AddInstruction(opcode, modRM, imm);
                    break;
                }
                default: throw new NotSupportedException();
            }
        }
        else throw new NotSupportedException();
        return instruction;
    }
}

public enum InstructionCode
{
    ADD 
}

public abstract class Instruction
{
    public InstructionCode Code;
    public byte Opcode;

    protected Instruction(InstructionCode code, byte opcode)
    {
        Code = code;
        Opcode = opcode;
    }

    public abstract string Execute(ICPU cpu);
}

public class AddInstruction : Instruction
{
    public byte Opcode, Src1, Src2;
    public AddInstruction(byte opcode, byte src1, byte src2) : base(InstructionCode.ADD, opcode)
    {
        Opcode = opcode;
        Src1 = src1;
        Src2 = src2;
    }

    public override string Execute(ICPU cpu)
    {
        switch (Opcode)
        {
            case 0x83:
            {
                ModRM m = new(Src1);
                if (m.m == ModRM.Mode.Register)
                {
                    IRegister reg = cpu.GetRegisterFromModRM(m.b);
                    reg.Value64 += Src2;
                }
                else throw new NotImplementedException();
                break;
            }
                default: throw new NotSupportedException();
        }

        return "";
    }
}