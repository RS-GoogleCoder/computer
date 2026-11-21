using System.Text;

namespace Computer.debugutil;

public static class Dump
{
    public static void RegisterDump(ICPU icpu)
    {
        if (icpu is not CPU1 cpu) throw new NotSupportedException();
        StringBuilder sb = new();
        foreach (var keyValuePair in cpu.Registers)
        {
            sb.AppendLine(keyValuePair.Key);
            foreach (var valuePair in keyValuePair.Value)
            {
                sb.AppendLine(valuePair.Key + " : " + valuePair.Value.Value64);
            }

            sb.AppendLine();
        }
        File.WriteAllText("regdump.txt", sb.ToString());
    }
    
    public static void MemoryDump(byte[] memory)
    {
        StringBuilder sb = new();
        sb.Append("    00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F");
        sb.AppendLine();
        for (int i = 0; i < memory.Length; i+=16)
        {
            sb.AppendLine();
            sb.Append((i/16).ToString("X3") + " ");
            
            for (int j = 0; j < 16; j++)
            {
                sb.Append(memory[i + j].ToString("X2") + " ");
            }
        }
        File.WriteAllText("memdump.txt", sb.ToString());
    }
}