using System.Diagnostics;
using Computer.debugutil;

namespace Computer;

public class Program
{
    static void Main(string[] args)
    {
        byte[] itoa =
        [
            0xBC, 0x00, 0x90, 0x00, 0x00, // mov esp, 0x9000
            0xB8, 0x39, 0x05, 0x00, 0x00, // mov eax, 1337
            0xBF, 0x00, 0x20, 0x00, 0x00, // mov edi, 0x2000
            0xE8, 0x01, 0x00, 0x00, 0x00, // call +1 (to int_to_str)
            0xF4, // hlt
            0x55, // push ebp
            0x89, 0xE5, // mov ebp, esp
            0xBB, 0x0A, 0x00, 0x00, 0x00, // mov ebx, 10
            0x31, 0xC9, // xor ecx, ecx
            0x31, 0xD2, // xor edx, edx
            0xF7, 0xF3, // div ebx
            0x80, 0xC2, 0x30, // add dl, 0x30 ('0')
            0x66, 0x52, // push dx
            0x41, // inc ecx
            0x85, 0xC0, // test eax, eax
            0x75, 0xF1, // jnz -15 (to .divide_loop)
            0x66, 0x58, // pop ax
            0x88, 0x07, // mov [edi], al
            0x47, // inc edi
            0xE2, 0xF9, // loop -7 (to .write_loop)
            0xC6, 0x07, 0x00, // mov byte [edi], 0
            0x5D, // pop ebp
            0xF4 // hlt
        ];
        Run(itoa);
    }

    public static void Run(byte[] program)
    {
        MemoryController.Init(65536);
        MemoryController.Set(program, 0, (ulong)program.Length);


        CPU1 cpu = new CPU1();

        ulong instructionPointer = 0;
        bool running = true;
        var instructionCache = new Instruction?[1024];
        bool debuggerAttached = Debugger.IsAttached;
        bool dumpStateEachStep = debuggerAttached &&
                                 Environment.GetEnvironmentVariable("COMPUTER_DUMP_STATE") == "1";
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

            var test2 = instructionCache[instructionPointer] ??
                        (instructionCache[instructionPointer] = DecodeInstruction(currentInstruction));
            var r = test2.Execute(cpu);
            cpu.SingleInstructionModeSwitchReturn();
            instructionPointer += (ulong)r.InstrPtrAdvance;

            if (r.halt) running = false;
            if (debuggerAttached)
            {
                Console.WriteLine(test2.ToString() + ": " + r.DebugMessage);
                if (dumpStateEachStep)
                {
                    Dump.MemoryDump(MemoryController.Get());
                    Dump.RegisterDump(cpu);
                }
            }
        } while (running);

        sw.Stop();
        Console.WriteLine("Program finished in " + sw.Elapsed.TotalMilliseconds + "ms");
    }

    public static Instruction DecodeInstruction(Span<byte> bytes)
    {
        byte opcode = bytes[0];
        byte[] src = [.. bytes[1..]];
        ModRM modRM = new(bytes[1]);
        switch (opcode)
        {
            case 0xF4:
                return new HltInstruction(opcode);
            case 0x00:
            case 0x01:
                return new AddInstruction(opcode, src);
            case 0x31:
                return new XorInstruction(opcode, src);
            case 0x40:
            case 0x41:
            case 0x42:
            case 0x43:
            case 0x44:
            case 0x45:
            case 0x46:
            case 0x47:
            {
                return new IncInstruction(opcode, src);
            }
            case 0x89:
            {
                return new MovInstruction(opcode, src);
            }
            case 0x50:
            case 0x51:
            case 0x52:
            case 0x53:
            case 0x54:
            case 0x55:
            case 0x56:
            case 0x57:
                return new PushInstruction(opcode, src);
            case 0x58:
            case 0x59:
            case 0x5A:
            case 0x5B:
            case 0x5C:
            case 0x5D:
            case 0x5E:
            case 0x5F:
                return new PopInstruction(opcode, src);
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
                return new JccInstruction(opcode, src);
            }
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
                    return new AddInstruction(opcode, src);
                }

                if (modRM.Reg == 7)
                {
                    return new CmpInstruction(opcode, src);
                }

                throw new NotSupportedException("0x83 opcode with reg " + modRM.Reg + " not supported");
            }
            case 0x85:
                return new TestInstruction(opcode, src);
            case 0x88:
            case 0xb8:
            case 0xb9:
            case 0xbA:
            case 0xbB:
            case 0xbC:
            case 0xbD:
            case 0xbE:
            case 0xbF:
            case 0xC6:
            {
                return new MovInstruction(opcode, src);
            }
            case 0xE2:
            {
                return new LoopInstruction(opcode, src);
            }
            case 0xE8:
            {
                return new CallInstruction(opcode, src);
            }
            case 0xF7:
            {
                return new DivInstruction(opcode, src);
            }

            default: throw new Exception("invalid instruction code: " + opcode);
        }
    }
}