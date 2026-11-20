using System.Buffers.Binary;

namespace Computer;

public class AddInstruction : Instruction
{
    public byte Opcode;
    public byte[] Src;
    public AddInstruction(byte opcode, byte[] src) : base(InstructionCode.ADD, opcode)
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
            case 0x01:
            {
                ModRM m = new(Src[0]);
                if (m.RMMode == RMMode.Register)
                {
                    IRegister src1 = cpu.GetRegisterFromNumber(m.RM);
                    IRegister src2 = cpu.GetRegisterFromNumber(m.Reg);
                    src1.Value64 += src2.Value64;
                    return new(src1.Value64.ToString(), 2);
                }
                else throw new NotImplementedException("Memory mode not supported");
            }
            case 0x04:
            {
                IRegister reg = cpu.GetRegister("A", "AL");
                reg.Value64 += Src[0];
                return new(reg.Value64.ToString(), 2);
            }
            case 0x05:
            {

                if (mode == CPUMode.x16Real)
                {
                    IRegister reg = cpu.GetRegister("A", "AX");
                    ushort src2 = BinaryPrimitives.ReadUInt16BigEndian(Src);
                    reg.Value64 += src2;
                    return new(reg.Value64.ToString(), 2);
                }
                if (mode == CPUMode.x32Prot)
                {
                    IRegister reg = cpu.GetRegister("A", "EAX");
                    uint src2 = BinaryPrimitives.ReadUInt32BigEndian(Src);
                    reg.Value64 += src2;
                    return new(reg.Value64.ToString(), 2);
                }

                if (mode == CPUMode.x64Long)
                {
                    IRegister reg = cpu.GetRegister("A", "RAX");
                    ulong src2 = BinaryPrimitives.ReadUInt64BigEndian(Src);
                    reg.Value64 += src2;
                    return new(reg.Value64.ToString(), 2);
                }

                throw new NotSupportedException($"Mode {mode} not supported");
            }
            case 0x83:
            {
                ModRM m = new(Src[0]);
                if (m.RMMode == RMMode.Register)
                {
                    IRegister reg = cpu.GetRegisterFromNumber(m.Raw);
                    reg.Value64 += Src[1];
                    return new(reg.Value64.ToString(), 2);
                }
                else throw new NotImplementedException("Memory mode not supported");
            }
        }
        throw new NotImplementedException();
    }
}