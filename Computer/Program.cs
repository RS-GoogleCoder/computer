using System.Diagnostics;
using Computer.debugutil;

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
        
        byte[] fibbonaci = [
            0xB9, 0x0A, 0x00, 0x00, 0x00, // mov ecx, 10
            0xB8, 0x00, 0x00, 0x00, 0x00, // mov eax, 0
            0xBB, 0x01, 0x00, 0x00, 0x00, // mov ebx, 1
            0x83, 0xF9, 0x00,             // cmp ecx, 0
            0x74, 0x0A,                   // je +10 (to 0x1E)
            0x89, 0xC2,                   // mov edx, eax
            0x01, 0xDA,                   // add edx, ebx
            0x89, 0xD8,                   // mov eax, ebx
            0x89, 0xD3,                   // mov ebx, edx
            0xE2, 0xF6,                   // loop -10 (to 0x14)
            0xF4                          // hlt
        ];

        
        Array.Copy(fibbonaci, Memory, fibbonaci.Length);
        
        
        CPU1 cpu = new CPU1();

        int instructionPointer = 0;
        bool running = true;
        Stopwatch sw = Stopwatch.StartNew();
        do
        {
            Span<byte> currentInstruction = Memory;
            //                                                                           assuming all instructions require 8 bytes ahead
            currentInstruction = currentInstruction.Slice(instructionPointer, 8);
            
            var test2 = DecodeInstruction(currentInstruction);
            var r = test2.Execute(cpu);
            instructionPointer += r.InstrPtrAdvance;
            //                              Max program length is 1024 for now
            if (instructionPointer >= 1024)
            {
                running = false;
            }
            Console.WriteLine(test2.ToString() + ": " + r.DebugMessage);
            if(r.halt) running = false;
            if(Debugger.IsAttached)
            {
                Dump.MemoryDump(Memory);
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
            

        default: throw new Exception("invalid instruction code: " + opcode);
        }
    }
}