using System.Collections;

namespace cpuemu
{
    public enum OPCode
    {
        MovRToR = 0x00, // Move Register Value to Register
        MovIToR = 0x01, // Move Immidiate Value to Register
        SaveItoR = 0x02, // Move value (i) to memory location (in r)
        SaveRtoR = 0x03, // Move value (in r) to memory location (in r)
        SaveItoI = 0x04, // Move value (i) to memory location (i)
        SaveRtoI = 0x05, // Move value (in r) to memory location (i)
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

        public System(byte[] rom, int ramsize = 40)
        {
            Cpu = new CPU();
            Bootrom = rom;
            Ram = new byte[ramsize];
        }

        public void Run()
        {
            SaveBootRom();

            while (Cpu.ProgramCounter < (ulong)Ram.Length)
            {
                var inst = ParseInstruction();

                switch (inst.operation)
                {
                    case OPCode.MovRToR:
                        ExecuteMovRToR(inst);
                        break;
                    case OPCode.MovIToR:
                        ExecuteMovIToR(inst);
                        break;
                    case OPCode.SaveItoR:
                        ExecuteSaveIToR(inst);
                        break;
                    case OPCode.SaveRtoR:
                        ExecuteSaveRToR(inst);
                        break;
                    case OPCode.SaveRtoI:
                        ExecuteSaveRToI(inst);
                        break;
                    case OPCode.SaveItoI:
                        ExecuteSaveIToI(inst);
                        break;
                }
            }
        }

        public void ExecuteSaveIToR(Instruction inst) // Reg val to reg
        {
            var valuetomove = (inst.args[0] as Immidiate).value;
            UInt64 addresstomoveto = 0;
            switch (((Register)inst.args[1]).register)
            {
                case RegisterCode.R0:
                    addresstomoveto = Cpu.R0;
                    break;
                case RegisterCode.R1:
                    addresstomoveto = Cpu.R1;
                    break;
                case RegisterCode.R2:
                    addresstomoveto = Cpu.R2;
                    break;
                case RegisterCode.R3:
                    addresstomoveto = Cpu.R3;
                    break;
                case RegisterCode.R4:
                    addresstomoveto = Cpu.R4;
                    break;
                case RegisterCode.R5:
                    addresstomoveto = Cpu.R5;
                    break;
                case RegisterCode.R6:
                    addresstomoveto = Cpu.R6;
                    break;
                case RegisterCode.R7:
                    addresstomoveto = Cpu.R7;
                    break;
                case RegisterCode.R8:
                    addresstomoveto = Cpu.R8;
                    break;
                case RegisterCode.R9:
                    addresstomoveto = Cpu.R9;
                    break;
                case RegisterCode.Flags:
                    addresstomoveto = Cpu.Flags;
                    break;
                case RegisterCode.StackPointer:
                    addresstomoveto = Cpu.StackPointer;
                    break;
                case RegisterCode.StackStart:
                    addresstomoveto = Cpu.StackStart;
                    break;
                case RegisterCode.StackEnd:
                    addresstomoveto = Cpu.StackEnd;
                    break;
                case RegisterCode.ProgramStartLocation:
                    addresstomoveto = Cpu.ProgramStartLocation;
                    break;
                case RegisterCode.ProgramCounter:
                    addresstomoveto = Cpu.ProgramCounter;
                    break;
            }

            for (int i = 0; i < 8; i++)
            {
                Ram[addresstomoveto + (uint)i] = (byte)((valuetomove >> (i * 8)) & 0xFF);
            }
        }

        public void ExecuteSaveRToR(Instruction inst) // Reg val to reg
        {
            UInt64 valuetomove = 0;
            switch (((Register)inst.args[0]).register)
            {
                case RegisterCode.R0:
                    valuetomove = Cpu.R0;
                    break;
                case RegisterCode.R1:
                    valuetomove = Cpu.R1;
                    break;
                case RegisterCode.R2:
                    valuetomove = Cpu.R2;
                    break;
                case RegisterCode.R3:
                    valuetomove = Cpu.R3;
                    break;
                case RegisterCode.R4:
                    valuetomove = Cpu.R4;
                    break;
                case RegisterCode.R5:
                    valuetomove = Cpu.R5;
                    break;
                case RegisterCode.R6:
                    valuetomove = Cpu.R6;
                    break;
                case RegisterCode.R7:
                    valuetomove = Cpu.R7;
                    break;
                case RegisterCode.R8:
                    valuetomove = Cpu.R8;
                    break;
                case RegisterCode.R9:
                    valuetomove = Cpu.R9;
                    break;
                case RegisterCode.Flags:
                    valuetomove = Cpu.Flags;
                    break;
                case RegisterCode.StackPointer:
                    valuetomove = Cpu.StackPointer;
                    break;
                case RegisterCode.StackStart:
                    valuetomove = Cpu.StackStart;
                    break;
                case RegisterCode.StackEnd:
                    valuetomove = Cpu.StackEnd;
                    break;
                case RegisterCode.ProgramStartLocation:
                    valuetomove = Cpu.ProgramStartLocation;
                    break;
                case RegisterCode.ProgramCounter:
                    valuetomove = Cpu.ProgramCounter;
                    break;
            }
            UInt64 addresstomoveto = 0;
            switch (((Register)inst.args[1]).register)
            {
                case RegisterCode.R0:
                    addresstomoveto = Cpu.R0;
                    break;
                case RegisterCode.R1:
                    addresstomoveto = Cpu.R1;
                    break;
                case RegisterCode.R2:
                    addresstomoveto = Cpu.R2;
                    break;
                case RegisterCode.R3:
                    addresstomoveto = Cpu.R3;
                    break;
                case RegisterCode.R4:
                    addresstomoveto = Cpu.R4;
                    break;
                case RegisterCode.R5:
                    addresstomoveto = Cpu.R5;
                    break;
                case RegisterCode.R6:
                    addresstomoveto = Cpu.R6;
                    break;
                case RegisterCode.R7:
                    addresstomoveto = Cpu.R7;
                    break;
                case RegisterCode.R8:
                    addresstomoveto = Cpu.R8;
                    break;
                case RegisterCode.R9:
                    addresstomoveto = Cpu.R9;
                    break;
                case RegisterCode.Flags:
                    addresstomoveto = Cpu.Flags;
                    break;
                case RegisterCode.StackPointer:
                    addresstomoveto = Cpu.StackPointer;
                    break;
                case RegisterCode.StackStart:
                    addresstomoveto = Cpu.StackStart;
                    break;
                case RegisterCode.StackEnd:
                    addresstomoveto = Cpu.StackEnd;
                    break;
                case RegisterCode.ProgramStartLocation:
                    addresstomoveto = Cpu.ProgramStartLocation;
                    break;
                case RegisterCode.ProgramCounter:
                    addresstomoveto = Cpu.ProgramCounter;
                    break;
            }

            for (int i = 0; i < 8; i++)
            {
                Ram[addresstomoveto + (uint)i] = (byte)((valuetomove >> (i * 8)) & 0xFF);
            }
        }

        public void ExecuteSaveIToI(Instruction inst) // Reg val to reg
        {
            var valuetomove = (inst.args[0] as Immidiate).value;
            var addresstomoveto = (inst.args[1] as Immidiate).value;


            for (int i = 0; i < 8; i++)
            {
                Ram[addresstomoveto + (uint)i] = (byte)((valuetomove >> (i * 8)) & 0xFF);
            }
        }

        public void ExecuteSaveRToI(Instruction inst) // Reg val to reg
        {
            UInt64 valuetomove = 0;
            switch (((Register)inst.args[0]).register)
            {
                case RegisterCode.R0:
                    valuetomove = Cpu.R0;
                    break;
                case RegisterCode.R1:
                    valuetomove = Cpu.R1;
                    break;
                case RegisterCode.R2:
                    valuetomove = Cpu.R2;
                    break;
                case RegisterCode.R3:
                    valuetomove = Cpu.R3;
                    break;
                case RegisterCode.R4:
                    valuetomove = Cpu.R4;
                    break;
                case RegisterCode.R5:
                    valuetomove = Cpu.R5;
                    break;
                case RegisterCode.R6:
                    valuetomove = Cpu.R6;
                    break;
                case RegisterCode.R7:
                    valuetomove = Cpu.R7;
                    break;
                case RegisterCode.R8:
                    valuetomove = Cpu.R8;
                    break;
                case RegisterCode.R9:
                    valuetomove = Cpu.R9;
                    break;
                case RegisterCode.Flags:
                    valuetomove = Cpu.Flags;
                    break;
                case RegisterCode.StackPointer:
                    valuetomove = Cpu.StackPointer;
                    break;
                case RegisterCode.StackStart:
                    valuetomove = Cpu.StackStart;
                    break;
                case RegisterCode.StackEnd:
                    valuetomove = Cpu.StackEnd;
                    break;
                case RegisterCode.ProgramStartLocation:
                    valuetomove = Cpu.ProgramStartLocation;
                    break;
                case RegisterCode.ProgramCounter:
                    valuetomove = Cpu.ProgramCounter;
                    break;
            }

            var addresstomoveto = (inst.args[1] as Immidiate).value;

            for (int i = 0; i < 8; i++)
            {
                Ram[addresstomoveto + (uint)i] = (byte)((valuetomove >> (i * 8)) & 0xFF);
            }
        }

        public void ExecuteMovRToR(Instruction inst) // Reg val to reg
        {
            UInt64 valuetomove = 0;
            switch (((Register)inst.args[0]).register)
            {
                case RegisterCode.R0:
                    valuetomove = Cpu.R0;
                    break;
                case RegisterCode.R1:
                    valuetomove = Cpu.R1;
                    break;
                case RegisterCode.R2:
                    valuetomove = Cpu.R2;
                    break;
                case RegisterCode.R3:
                    valuetomove = Cpu.R3;
                    break;
                case RegisterCode.R4:
                    valuetomove = Cpu.R4;
                    break;
                case RegisterCode.R5:
                    valuetomove = Cpu.R5;
                    break;
                case RegisterCode.R6:
                    valuetomove = Cpu.R6;
                    break;
                case RegisterCode.R7:
                    valuetomove = Cpu.R7;
                    break;
                case RegisterCode.R8:
                    valuetomove = Cpu.R8;
                    break;
                case RegisterCode.R9:
                    valuetomove = Cpu.R9;
                    break;
                case RegisterCode.Flags:
                    valuetomove = Cpu.Flags;
                    break;
                case RegisterCode.StackPointer:
                    valuetomove = Cpu.StackPointer;
                    break;
                case RegisterCode.StackStart:
                    valuetomove = Cpu.StackStart;
                    break;
                case RegisterCode.StackEnd:
                    valuetomove = Cpu.StackEnd;
                    break;
                case RegisterCode.ProgramStartLocation:
                    valuetomove = Cpu.ProgramStartLocation;
                    break;
                case RegisterCode.ProgramCounter:
                    valuetomove = Cpu.ProgramCounter;
                    break;
            }
            switch (((Register)inst.args[1]).register)
            {
                case RegisterCode.R0:
                    Cpu.R0 = valuetomove;
                    break;
                case RegisterCode.R1:
                    Cpu.R1 = valuetomove;
                    break;
                case RegisterCode.R2:
                    Cpu.R2 = valuetomove;
                    break;
                case RegisterCode.R3:
                    Cpu.R3 = valuetomove;
                    break;
                case RegisterCode.R4:
                    Cpu.R4 = valuetomove;
                    break;
                case RegisterCode.R5:
                    Cpu.R5 = valuetomove;
                    break;
                case RegisterCode.R6:
                    Cpu.R6 = valuetomove;
                    break;
                case RegisterCode.R7:
                    Cpu.R7 = valuetomove;
                    break;
                case RegisterCode.R8:
                    Cpu.R8 = valuetomove;
                    break;
                case RegisterCode.R9:
                    Cpu.R9 = valuetomove;
                    break;
                case RegisterCode.Flags:
                    Cpu.Flags = valuetomove;
                    break;
                case RegisterCode.StackPointer:
                    Cpu.StackPointer = valuetomove;
                    break;
                case RegisterCode.StackStart:
                    Cpu.StackStart = valuetomove;
                    break;
                case RegisterCode.StackEnd:
                    Cpu.StackEnd = valuetomove;
                    break;
                case RegisterCode.ProgramStartLocation:
                    Cpu.ProgramStartLocation = valuetomove;
                    break;
                case RegisterCode.ProgramCounter:
                    Cpu.ProgramCounter = valuetomove;
                    break;
            }
        }

        public void ExecuteMovIToR(Instruction inst) // Immediate val to reg
        {
            UInt64 valuetomove = 0;
            valuetomove = (inst.args[0] as Immidiate).value;
            switch (((Register)inst.args[1]).register)
            {
                case RegisterCode.R0:
                    Cpu.R0 = valuetomove;
                    break;
                case RegisterCode.R1:
                    Cpu.R1 = valuetomove;
                    break;
                case RegisterCode.R2:
                    Cpu.R2 = valuetomove;
                    break;
                case RegisterCode.R3:
                    Cpu.R3 = valuetomove;
                    break;
                case RegisterCode.R4:
                    Cpu.R4 = valuetomove;
                    break;
                case RegisterCode.R5:
                    Cpu.R5 = valuetomove;
                    break;
                case RegisterCode.R6:
                    Cpu.R6 = valuetomove;
                    break;
                case RegisterCode.R7:
                    Cpu.R7 = valuetomove;
                    break;
                case RegisterCode.R8:
                    Cpu.R8 = valuetomove;
                    break;
                case RegisterCode.R9:
                    Cpu.R9 = valuetomove;
                    break;
                case RegisterCode.Flags:
                    Cpu.Flags = valuetomove;
                    break;
                case RegisterCode.StackPointer:
                    Cpu.StackPointer = valuetomove;
                    break;
                case RegisterCode.StackStart:
                    Cpu.StackStart = valuetomove;
                    break;
                case RegisterCode.StackEnd:
                    Cpu.StackEnd = valuetomove;
                    break;
                case RegisterCode.ProgramStartLocation:
                    Cpu.ProgramStartLocation = valuetomove;
                    break;
                case RegisterCode.ProgramCounter:
                    Cpu.ProgramCounter = valuetomove;
                    break;
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
                case OPCode.SaveItoR:
                    inst.args.Add(ParseImmediate());
                    inst.args.Add(ParseRegister());
                    break;
                 case OPCode.SaveRtoR:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.SaveRtoI:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseImmediate());
                    break;
                case OPCode.SaveItoI:
                    inst.args.Add(ParseImmediate());
                    inst.args.Add(ParseImmediate());
                    break;
            }

            return inst;
        }

        public Immidiate ParseImmediate()
        {
            Immidiate imm = new();

            // Read 8 bytes starting from ProgramCounter
            byte[] immediateBytes = new byte[8];
            Array.Copy(Ram, (int)Cpu.ProgramCounter, immediateBytes, 0, 8);

            // Reverse for big-endian interpretation
            Array.Reverse(immediateBytes);

            // Convert to UInt64
            imm.value = BitConverter.ToUInt64(immediateBytes, 0);

            // Increment ProgramCounter by 8 after reading the immediate
            Cpu.ProgramCounter += 8;

            return imm;
        }


        public Register ParseRegister()
        {
            RegisterCode regcode = (RegisterCode)Ram[Cpu.ProgramCounter];

            Register reg = new()
            {
                register = regcode
            };

            Cpu.ProgramCounter++;

            return reg;
        }



        public bool SaveBootRom()
        {
            if (Bootrom.Length > Ram.Length)
            {
                Console.WriteLine("Cannot Save bootrom because ROM is larger than RAM Size.");
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

    public class Test()
    {
        public static void Main()
        {
            System sys = new(File.ReadAllBytes(@"C:\Users\Liam Greenway\Downloads\Romtest1.xer"));
            sys.Run();

            Console.WriteLine("TraceLog");

            foreach (byte b in sys.Ram) {
                Console.Write(b + " ");
            }
        }
    }
}