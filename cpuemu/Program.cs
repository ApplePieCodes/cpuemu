using System.Collections;

namespace cpuemu
{
    public enum OPCode
    {
        MovRToR = 0x00, // Move Register Value to Register
        MovIToR = 0x01, // Move Immidiate Value to Register
        MovRToI = 0x02, // Move Regster Value to Immidiate(Memory)
        MovIToI = 0x03, // Move Immidiate Value to Immidiate(Memory)s
    }

    public enum RegisterCode // One Byte
    {
        R0 = 0x00,
        R1 = 0x01,
        R2 = 0x02,
        R3 = 0x03,
        R4 = 0x04,
        R5 = 0x05,
        R6 = 0x06,
        R7 = 0x07,
        R8 = 0x08,
        R9 = 0x09,
        Flags = 0x0A,
        StackPointer = 0x0B,
        StackStart = 0x0C,
        StackEnd = 0x0D,
        ProgramStartLocation = 0x0E,
        ProgramCounter = 0x0F
    }

    public class Instruction
    {
        public OPCode operation;
        public List<Arguement> args = [];
    }

    public abstract class Arguement { }

    public class Register : Arguement
    {
        public RegisterCode register;
    }

    public class Immidiate : Arguement
    {
        public UInt64 value;
    }

    public class CPU
    {
        public UInt64 R0 = 0;
        public UInt64 R1 = 0;
        public UInt64 R2 = 0;
        public UInt64 R3 = 0;
        public UInt64 R4 = 0;
        public UInt64 R5 = 0;
        public UInt64 R6 = 0;
        public UInt64 R7 = 0;
        public UInt64 R8 = 0;
        public UInt64 R9 = 0;

        public UInt64 Flags = 0;
        public UInt64 StackPointer = 0;
        public UInt64 StackStart = 0;
        public UInt64 StackEnd = 0;
        public UInt64 ProgramStartLocation = 0;
        public UInt64 ProgramCounter = 0;

        public const UInt64 ZeroFlag = 1 << 0;      // 0000 0000 0000 0000 0000 0000 0000 0001
        public const UInt64 CarryFlag = 1 << 1;     // 0000 0000 0000 0000 0000 0000 0000 0010
        public const UInt64 OverflowFlag = 1 << 2;  // 0000 0000 0000 0000 0000 0000 0000 0100
        public const UInt64 NegativeFlag = 1 << 3;  // 0000 0000 0000 0000 0000 0000 0000 1000

        public void SetFlag(byte flag)
        {
            Flags |= flag;
        }

        public void ClearFlag(byte flag)
        {
            Flags &= (byte)~flag;
        }

        public bool IsFlagSet(byte flag)
        {
            return (Flags & flag) != 0;
        }

    }

    public class System
    {
        public CPU Cpu;
        public byte[] Bootrom;
        public byte[] Ram;

        public System(byte[] rom, int ramsize)
        {
            Cpu = new CPU();
            Bootrom = rom;
            Ram = new byte[ramsize];
        }

        public void Run()
        {
            while (true)
            {
                var inst = ParseInstruction();


            }
        }

        public Instruction ParseInstruction()
        {
            OPCode opcode = (OPCode)Ram[Cpu.ProgramCounter];  // Cast to OPCode

            Instruction inst = new()
            {
                operation = opcode 
            };

            Cpu.ProgramCounter++;

            switch (inst.operation)
            {
                case OPCode.MovRToR:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.MovIToR:
                    inst.args.Add(ParseImmediate());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.MovRToI:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseImmediate());
                    break;
                 case OPCode.MovIToI:
                    inst.args.Add(ParseImmediate());
                    inst.args.Add(ParseImmediate());
                    break;
            }

            return inst;
        }

        public Immidiate ParseImmediate()
        {
            Immidiate imm = new();

            imm.value = BitConverter.ToUInt64(Ram, (int)Cpu.ProgramCounter);

            // Increment ProgramCounter by 8 after reading the immediate
            Cpu.ProgramCounter += 8;

            return imm;
        }

        public Register ParseRegister()
        {
            RegisterCode regcode = (RegisterCode)Ram[Cpu.ProgramCounter];

            Register reg = new();

            reg.register = regcode;

            Cpu.ProgramCounter++;

            return reg;
        }


        public bool LoadBootRom()
        {
            if (Bootrom.Length > Ram.Length)
            {
                Console.WriteLine("Cannot load bootrom because ROM is larger than RAM Size.");
                return false;
            }

            int pos = Ram.Length - Bootrom.Length;
            Array.Copy(Bootrom, 0, Ram, pos, Bootrom.Length);

            // Explicitly cast 'pos' to UInt64
            Cpu.ProgramStartLocation = (UInt64)pos;
            Cpu.ProgramCounter = Cpu.ProgramStartLocation;

            return true;
        }

    }
}