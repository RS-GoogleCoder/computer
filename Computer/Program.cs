using System.Diagnostics;
using Computer.debugutil;

namespace Computer;

public class Program
{
    static void Main(string[] args)
    {
        byte[] itoa = [
            0xBC, 0x00, 0x90, 0x00, 0x00, // mov esp, 0x9000
            0xB8, 0x39, 0x05, 0x00, 0x00, // mov eax, 1337
            0xBF, 0x00, 0x20, 0x00, 0x00, // mov edi, 0x2000
            0xE8, 0x01, 0x00, 0x00, 0x00, // call +1 (to int_to_str)
            0xF4,                         // hlt
            0x55,                         // push ebp
            0x89, 0xE5,                   // mov ebp, esp
            0xBB, 0x0A, 0x00, 0x00, 0x00, // mov ebx, 10
            0x31, 0xC9,                   // xor ecx, ecx
            0x31, 0xD2,                   // xor edx, edx
            0xF7, 0xF3,                   // div ebx
            0x80, 0xC2, 0x30,             // add dl, 0x30 ('0')
            0x66, 0x52,                   // push dx
            0x41,                         // inc ecx
            0x85, 0xC0,                   // test eax, eax
            0x75, 0xF1,                   // jnz -15 (to .divide_loop)
            0x66, 0x58,                   // pop ax
            0x88, 0x07,                   // mov [edi], al
            0x47,                         // inc edi
            0xE2, 0xF9,                   // loop -7 (to .write_loop)
            0xC6, 0x07, 0x00,             // mov byte [edi], 0
            0x5D,                         // pop ebp
            0xC3                          // ret
        ];
        Run(itoa);
    }

    public static void Run(byte[] program)
    {
        MemoryController.Init(65536);
        MemoryController.Set(program, 0, (ulong) program.Length);
        
        
        CPU1 cpu = new CPU1();

        ulong instructionPointer = 0;
        bool running = true;
        Stopwatch sw = Stopwatch.StartNew();
        do
        {
            //                                                         assuming all instructions require 8 bytes ahead
            Span<byte> currentInstruction = MemoryController.Get(instructionPointer, 8);
            if (currentInstruction[0] == 0x66) // mode switch
            {
                cpu.SingleInstructionModeSwitch(CPUMode.x16Real);
                instructionPointer += 1;
                continue;
            }
            
            var test2 = DecodeInstruction(currentInstruction);
            var r = test2.Execute(cpu);
            cpu.SingleInstructionModeSwitchReturn();
            instructionPointer += (ulong)r.InstrPtrAdvance;
            //                              Max program length is 1024 for now
            if (instructionPointer >= 1024)
            {
                running = false;
            }
            Console.WriteLine(test2.ToString() + ": " + r.DebugMessage);
            if(r.halt) running = false;
            if(Debugger.IsAttached)
            {
                Dump.MemoryDump(MemoryController.Get());
                Dump.RegisterDump(cpu);
            }
        } while (running);
        sw.Stop();
        Console.WriteLine("Program finished in " + sw.Elapsed.TotalMilliseconds + "ms");
    }
    
    public static Instruction DecodeInstruction(Span<byte> bytes)
    {
        byte opcode = bytes[0];
        ModRM modRM = new(bytes[1]);
        switch (opcode)
        {
            case 0xF4:
                return new HltInstruction(opcode);
            case 0x00:
            case 0x01:
                return new AddInstruction(opcode, bytes.Slice(1).ToArray());
            case 0x31:
                return new XorInstruction(opcode, bytes.Slice(1).ToArray());
            case 0x50:
            case 0x51:
            case 0x52:
            case 0x53:
            case 0x54:
            case 0x55:
            case 0x56:
            case 0x57:
                return new PushInstruction(opcode, bytes.Slice(1).ToArray());
            case 0x02:
            case 0x03:
            case 0x04:
            case 0x05:
            case 0x80:
            case 0x81:
            case 0x82:
            case 0x83:
            {
                if (modRM.Reg == 0)
                {
                    return new AddInstruction(opcode, bytes.Slice(1).ToArray());
                }

                if (modRM.Reg == 7)
                {
                    return new CmpInstruction(opcode, bytes.Slice(1).ToArray());
                }

                throw new NotSupportedException("0x83 opcode with reg " + modRM.Reg + " not supported");
            }
            case 0xb8:
            case 0xb9:
            case 0xbA:
            case 0xbB:
            case 0xbC:
            case 0xbD:
            case 0xbE:
            case 0xbF:
            {
                return new MovInstruction(opcode, bytes.Slice(1).ToArray());
            }
            case 0x41:
            {
                return new IncInstruction(opcode, bytes.Slice(1).ToArray());
            }
            case 0x70:
            case 0x71:
            case 0x72:
            case 0x73:
            case 0x74:
            case 0x75:
            case 0x76:
            case 0x77:
            case 0x78:
            case 0x79:
            case 0x7A:
            case 0x7B:
            case 0x7C:
            case 0x7D:
            case 0x7E:
            case 0x7F:
            {
                return new JccInstruction(opcode, bytes.Slice(1).ToArray());
            }
            case 0x89:
            {
                return new MovInstruction(opcode, bytes.Slice(1).ToArray());
            }
            case 0xE2:
            {
                return new LoopInstruction(opcode, bytes.Slice(1).ToArray());
            }
            case 0xE8:
            {
                return new CallInstruction(opcode, bytes.Slice(1).ToArray());
            }
            case 0xF7:
            {
                return new DivInstruction(opcode, bytes.Slice(1).ToArray());
            }

        default: throw new Exception("invalid instruction code: " + opcode);
        }
    }
}