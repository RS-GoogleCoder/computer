using System.Buffers.Binary;

namespace Computer;

public class MovInstruction : Instruction
{
    public byte Opcode;
    public byte[] Src;
    public MovInstruction(byte opcode, byte[] src) : base(InstructionCode.MOV, opcode)
    {
        Opcode = opcode;
        Src = src;
    }

    public override InstructionReturnData Execute(ICPU icpu)
    {
        if(icpu is not CPU1 cpu) throw new Exception("CPU not supported");
        CPUMode mode = cpu.Mode;
        switch (Opcode)
        {
            case 0xb8:
            case 0xb9:
            case 0xbA:
            case 0xbB:
            case 0xbC:
            case 0xbD:
            case 0xbE:
            case 0xbF:
            {
                byte r = (byte)(Opcode - 0xb8);
                IRegister reg = cpu.GetRegisterFromNumber(r, mode.ToRegisterSize());
                
                if (mode == CPUMode.x16Real)
                {
                    ushort src2 = BinaryPrimitives.ReadUInt16LittleEndian(Src);
                    reg.Value64 = src2;
                    return new(reg.Value64.ToString(), 3);
                }
                if (mode == CPUMode.x32Prot)
                {
                    uint src2 = BinaryPrimitives.ReadUInt32LittleEndian(Src);
                    reg.Value64 = src2;
                    return new(reg.Value64.ToString(), 5);
                }

                if (mode == CPUMode.x64Long)
                {
                    ulong src2 = BinaryPrimitives.ReadUInt64LittleEndian(Src);
                    reg.Value64 = src2;
                    return new(reg.Value64.ToString(), 9);
                }
                throw new NotSupportedException($"Mode {mode} not supported");
            }
            case 0x88:
            {
                ModRM m = new ModRM(Src[0]);
                if (m.RMMode == RMMode.Memory00)
                {
                    IRegister source = cpu.GetRegisterFromNumber(m.Reg, RegisterSize.x8);
                    IRegister dstMemoryPtr = cpu.GetRegisterFromNumber(m.RM, cpu.Mode.ToRegisterSize());
                    MemoryController.Set((byte)source.Value64,  dstMemoryPtr.Value64);
                    return new InstructionReturnData(((byte)source.Value64).ToString(), 2);
                }
                else if (m.RMMode == RMMode.Register)
                {
                    IRegister result = cpu.GetRegisterFromNumber(m.RM, RegisterSize.x8);
                    IRegister source = cpu.GetRegisterFromNumber(m.Reg, RegisterSize.x8);
                    result.Value64 = source.Value64;
                    return new InstructionReturnData(result.Value64.ToString(), 2);
                }
                else throw new NotSupportedException("Mode not supported");
            }
            case 0x89:
            {
                ModRM m = new ModRM(Src[0]);
                if (m.RMMode == RMMode.Register)
                {
                    IRegister result = cpu.GetRegisterFromNumber(m.RM, mode.ToRegisterSize());
                    IRegister source = cpu.GetRegisterFromNumber(m.Reg, mode.ToRegisterSize());
                    result.Value64 = source.Value64;
                    return new InstructionReturnData(result.Value64.ToString(), 2);
                }
                else throw new NotSupportedException("Memory mode not supported");
            }
            case 0xC6:
            {
                ModRM m = new ModRM(Src[0]);
                if (m.RMMode == RMMode.Memory00)
                {
                    byte source = Src[1];
                    IRegister dstMemoryPtr = cpu.GetRegisterFromNumber(m.RM, cpu.Mode.ToRegisterSize());
                    MemoryController.Set(source,  dstMemoryPtr.Value64);
                    return new InstructionReturnData((source).ToString(), 3);
                }
                else throw new NotSupportedException("Mode not supported");
            }
        }
        throw new Exception();
    }
}