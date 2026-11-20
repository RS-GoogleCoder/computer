using System.Buffers.Binary;

namespace Computer;

public class CmpInstruction : Instruction
{
    public byte Opcode;
    public byte[] Src;
    public CmpInstruction(byte opcode, byte[] src) : base(InstructionCode.CMP, opcode)
    {
        Opcode = opcode;
        Src = src;
    }

    public override InstructionReturnData Execute(ICPU icpu)
    {
        if(icpu is not CPU1 cpu) throw new Exception("CPU not supported");
        ulong result;
        byte instrPtrAdvance;
        CPUMode mode = cpu.Mode;
        switch (Opcode)
        {
            case 0x83:
            {
                ModRM b = new ModRM(Src[0]);
                IRegister reg = cpu.GetRegisterFromNumber(b.RM, mode.ToRegisterSize());
                byte imm = Src[1];
                result = reg.Value64 - imm;
                instrPtrAdvance = 3;
                break;
            }
            case 0x81:
            {
                if (mode == CPUMode.x16Real)
                {
                    ushort imm = BinaryPrimitives.ReadUInt16LittleEndian(Src);
                    ModRM b = new ModRM(Src[1]);
                    IRegister reg = cpu.GetRegisterFromNumber(b.Reg, mode.ToRegisterSize());
                    result = reg.Value64 - imm;
                    instrPtrAdvance = 4;
                }
                else if (mode == CPUMode.x32Prot)
                {
                    uint imm = BinaryPrimitives.ReadUInt32LittleEndian(Src);
                    ModRM b = new ModRM(Src[1]);
                    IRegister reg = cpu.GetRegisterFromNumber(b.Reg, mode.ToRegisterSize());
                    result = reg.Value64 - imm;
                    instrPtrAdvance = 6;
                }
                else if (mode == CPUMode.x64Long)
                {
                    ulong imm = BinaryPrimitives.ReadUInt64LittleEndian(Src);
                    ModRM b = new ModRM(Src[1]);
                    IRegister reg = cpu.GetRegisterFromNumber(b.Reg, mode.ToRegisterSize());
                    result = reg.Value64 - imm;
                    instrPtrAdvance = 10;
                }
                else throw new NotSupportedException($"Mode {mode} not supported");

                break;
            }
            default:throw new NotSupportedException($"Opcode {Opcode} not supported");
        }
        //cpu.Flags.CF
        //cpu.Flags.OF
        //cpu.Flags.SF
        cpu.Flags.ZF = result == 0;
        return new InstructionReturnData(cpu.Flags.ToString(), instrPtrAdvance);
    }
}