using System.Collections;
using System.IO.MemoryMappedFiles;

namespace cpuemu
{
    public enum OPCode
    {
        MovBIToR = 0x00, // Move I (Byte) to First Byte in R
        MovBRToR = 0x01, // Move First Byte in R to First Byte in R
        MovWIToR = 0x02, // Move I (Word) to First Word in R
        MovWRToR = 0x03, // Move First Word in R to First Word in R
        MovDRToR = 0x04, // Move First DWord in R to First DWord in R
        MovDIToR = 0x05, // Move I (DWord) to First DWord in R
        MovRToR = 0x06, // Move R to R
        MovIToR = 0x07, // Move I (QWord) to R
        SaveBRToR = 0x08, // Save First Byte in R to Address in R
        SaveBIToR = 0x09, // Save I (Byte) to Address in R
        SaveWRToR = 0x0A, // Save First Word in R to Address in R
        SaveWIToR = 0x0B, // Save I (Word) to Address in R
        SaveDRToR = 0x0C, // Save First DWord in R to Address in R
        SaveDIToR = 0x0D, // Save I (DWord) to Address in R
        SaveRToR = 0x0E, // Save Value in R to Address in R
        SaveIToR = 0x0F, // Save I (QWord) to Address in R
        SaveBRToI = 0x10, // Save First Byte in R to Address I
        SaveBIToI = 0x11, // Save I (Byte) to Address I
        SaveWRToI = 0x12, // Save First Word in R to Address I
        SaveWIToI = 0x13, // Save I (Word) to Address I
        SaveDRToI = 0x14, // Save First DWord in R to Address I
        SaveDIToI = 0x15, // Save I (DWord) to Address I
        SaveRToI = 0x16, // Save Value in R to Address I
        SaveIToI = 0x17, // Save I (QWord) to Address I
        LoadIToR = 0x18, // Load QWord from address I to R
        LoadRToR = 0x19, // Load QWord from address in R to R
        LoadDIToR = 0x1A, // Load DWord from address I to R
        LoadDRToR = 0x1B, // Load DWord from address in R to R
        LoadWIToR = 0x1C, // Load Word from address I to R
        LoadWRToR = 0x1D, // Load Word from address in R to R
        LoadBIToR = 0x1E, // Load Byte from address I to R
        LoadBRToR = 0x1F, // Load Byte from address in R to R
        Inc = 0x20, // Increment the RCOUNT Register by 1
        Dec = 0x21, // Decrement the RCOUNT Register by 1
    }

    public enum RegisterCode : byte // One Byte
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
        RCOUNT = 0x0A,
        Flags = 0x0B,
        StackPointer = 0x0C,
        StackStart = 0x0D,
        StackEnd = 0x0E,
        ProgramStartLocation = 0x0F,
        ProgramCounter = 0x10
    }

    public class Instruction
    {
        public OPCode operation;
        public List<Arguement> args = [];

        public Instruction(OPCode code, Arguement[] args)
        {
            operation = code;
            foreach (var arg in args)
            {
                this.args.Add(arg);
            }
        }

        public Instruction()
        {
            
        }
    }

    public abstract class Arguement { }

    public class Register : Arguement
    {
        public RegisterCode Code;
    }

    public abstract class Immidiate : Arguement { }

    public class ImmByte : Immidiate
    {
        public byte Value = 0x00;

        public ImmByte(byte value)
        {
            Value = value;
        }
    }

    public class ImmWord : Immidiate
    {
        public ushort Value = 0x00;

        public ImmWord(ushort value)
        {
            Value = value;
        }
    }

    public class ImmDWord : Immidiate
    {
        public uint Value = 0x00;

        public ImmDWord(uint value)
        {
            this.Value = value;
        }
    }

    public class ImmQWord : Immidiate
    {
        public ulong Value = 0x00;

        public ImmQWord(ulong value)
        {
            Value = value;
        }
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
        public UInt64 RCOUNT = 0;

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
        public MemoryMappedFile RamMMF;
        public MemoryMappedViewAccessor Ram;

        public override string ToString()
        {
            string retstr = "";

            retstr += "System Snapshot:\n";

            retstr += $"    R0: {Cpu.R0}\n";
            retstr += $"    R1: {Cpu.R1}\n";
            retstr += $"    R2: {Cpu.R2}\n";
            retstr += $"    R3: {Cpu.R3}\n";
            retstr += $"    R4: {Cpu.R4}\n";
            retstr += $"    R5: {Cpu.R5}\n";
            retstr += $"    R6: {Cpu.R6}\n";
            retstr += $"    R7: {Cpu.R7}\n";
            retstr += $"    R8: {Cpu.R8}\n";
            retstr += $"    R9: {Cpu.R9}\n";
            retstr += $"    RCOUNT: {Cpu.RCOUNT}\n";
            retstr += $"    FLAGS: {Cpu.Flags}\n";
            retstr += $"    STACKPOINTER: {Cpu.StackPointer}\n";
            retstr += $"    STACKSTART: {Cpu.StackStart}\n";
            retstr += $"    STACKEND: {Cpu.StackEnd}\n";
            retstr += $"    PROGRAMCOUNTER: {Cpu.ProgramCounter}\n";
            retstr += $"    PROGRAMSTARTLOCATION: {Cpu.ProgramStartLocation}\n";

            retstr += $"    Ram SnapShot:";

            int bytes = 0;

            for (int i = 0; i < Ram.Capacity; i++)
            {
                if (bytes == 0)
                {
                    retstr += "\n        ";
                }
                retstr += Ram.ReadByte(i);
                bytes++;
                if (bytes == 8)
                {
                    bytes = 0;
                }
            }

            return retstr;
        }

        public System(byte[] rom, int ramsize = 0)
        {
            if (ramsize == 0)
            {
                ramsize = rom.Length + 20;
            }
            Cpu = new CPU();
            Bootrom = rom;

            RamMMF = MemoryMappedFile.CreateNew("RamFile.bin", ramsize);
            Ram = RamMMF.CreateViewAccessor(0, ramsize, MemoryMappedFileAccess.ReadWrite);
        }

        public void Run()
        {
            LoadBootRom();

            while (Cpu.ProgramCounter < (ulong)Ram.Capacity)
            {
                var inst = ParseInstruction();

                switch (inst.operation)
                {
                    case OPCode.MovBRToR:
                        ExecuteMovBRToR(inst);
                        break;
                    case OPCode.MovBIToR:
                        ExecuteMovBIToR(inst);
                        break;
                    case OPCode.MovWIToR:
                        ExecuteMovWIToR(inst);
                        break;
                    case OPCode.MovWRToR:
                        ExecuteMovWRToR(inst);
                        break;
                    case OPCode.MovDRToR:
                        ExecuteMovDRToR(inst);
                        break;
                    case OPCode.MovDIToR:
                        ExecuteMovDIToR(inst);
                        break;
                    case OPCode.MovRToR:
                        ExecuteMovQRToR(inst);
                        break;
                    case OPCode.MovIToR:
                        ExecuteMovQIToR(inst);
                        break;
                    case OPCode.SaveIToR:
                        ExecuteSaveQIToR(inst);
                        break;
                    case OPCode.SaveRToR:
                        ExecuteSaveQRToR(inst);
                        break;
                    case OPCode.SaveDIToR:
                        ExecuteSaveDIToR(inst);
                        break;
                    case OPCode.SaveDRToR:
                        ExecuteSaveDRToR(inst);
                        break;
                    case OPCode.SaveWIToR:
                        ExecuteSaveWIToR(inst);
                        break;
                    case OPCode.SaveWRToR:
                        ExecuteSaveWRToR(inst);
                        break;
                    case OPCode.SaveBIToR:
                        ExecuteSaveBIToR(inst);
                        break;
                    case OPCode.SaveBRToR:
                        ExecuteSaveBRToR(inst);
                        break;
                    case OPCode.SaveIToI:
                        ExecuteSaveQIToI(inst);
                        break;
                    case OPCode.SaveRToI:
                        ExecuteSaveQRToI(inst);
                        break;
                    case OPCode.SaveDIToI:
                        ExecuteSaveDIToI(inst);
                        break;
                    case OPCode.SaveDRToI:
                        ExecuteSaveDRToI(inst);
                        break;
                    case OPCode.SaveWIToI:
                        ExecuteSaveWIToI(inst);
                        break;
                    case OPCode.SaveWRToI:
                        ExecuteSaveWRToI(inst);
                        break;
                    case OPCode.SaveBIToI:
                        ExecuteSaveBIToI(inst);
                        break;
                    case OPCode.SaveBRToI:
                        ExecuteSaveBRToI(inst);
                        break;
                    case OPCode.LoadIToR:
                        ExecuteLoadQIToR(inst);
                        break;
                    case OPCode.LoadRToR:
                        ExecuteLoadQRToR(inst);
                        break;
                    case OPCode.LoadDIToR:
                        ExecuteLoadDIToR(inst);
                        break;
                    case OPCode.LoadDRToR:
                        ExecuteLoadDRToR(inst);
                        break;
                    case OPCode.LoadWIToR:
                        ExecuteLoadWIToR(inst);
                        break;
                    case OPCode.LoadWRToR:
                        ExecuteLoadWRToR(inst);
                        break;
                    case OPCode.LoadBIToR:
                        ExecuteLoadBIToR(inst);
                        break;
                    case OPCode.LoadBRToR:
                        ExecuteLoadBRToR(inst);
                        break;
                    case OPCode.Inc:
                        Cpu.RCOUNT++;
                        break;
                    case OPCode.Dec:
                        Cpu.RCOUNT--;
                        break;
                }
            }
        }

        #region LoadInst
        public void ExecuteLoadQIToR(Instruction inst)
        {
            ImmQWord source = inst.args[0] as ImmQWord;
            Register destination = inst.args[1] as Register;

            ulong address = source.Value;

            ulong value = Ram.ReadUInt64((long)address);

            ExecuteMovQIToR(new Instruction(OPCode.LoadIToR, [new ImmQWord(value), destination]));

            SetRegisterValue(destination.Code, value);
        }
        public void ExecuteLoadQRToR(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            Register destination = inst.args[1] as Register;

            ulong address = GetRegisterValue(source.Code);

            ulong value = Ram.ReadUInt64((long)address);

            ExecuteMovQIToR(new Instruction(OPCode.LoadIToR, [new ImmQWord(value), destination]));

            SetRegisterValue(destination.Code, value);
        }
        public void ExecuteLoadDIToR(Instruction inst)
        {
            ImmDWord source = inst.args[0] as ImmDWord;
            Register destination = inst.args[1] as Register;

            ulong address = source.Value;

            uint value = Ram.ReadUInt32((long)address);

            ExecuteMovQIToR(new Instruction(OPCode.LoadDIToR, [new ImmDWord(value), destination]));

            SetRegisterValue(destination.Code, value);
        }
        public void ExecuteLoadDRToR(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            Register destination = inst.args[1] as Register;

            ulong address = GetRegisterValue(source.Code);

            uint value = Ram.ReadUInt32((long)address);

            ExecuteMovQIToR(new Instruction(OPCode.LoadDIToR, [new ImmDWord(value), destination]));

            SetRegisterValue(destination.Code, value);
        }
        public void ExecuteLoadWIToR(Instruction inst)
        {
            ImmWord source = inst.args[0] as ImmWord;
            Register destination = inst.args[1] as Register;

            ulong address = source.Value;

            ushort value = Ram.ReadUInt16((long)address);

            ExecuteMovWIToR(new Instruction(OPCode.LoadWIToR, [new ImmWord(value), destination]));

            SetRegisterValue(destination.Code, value);
        }
        public void ExecuteLoadWRToR(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            Register destination = inst.args[1] as Register;

            ulong address = GetRegisterValue(source.Code);

            ushort value = Ram.ReadUInt16((long)address);

            ExecuteMovQIToR(new Instruction(OPCode.LoadWIToR, [new ImmWord(value), destination]));

            SetRegisterValue(destination.Code, value);
        }
        public void ExecuteLoadBIToR(Instruction inst)
        {
            ImmByte source = inst.args[0] as ImmByte;
            Register destination = inst.args[1] as Register;

            ulong address = source.Value;

            byte value = Ram.ReadByte((long)address);

            ExecuteMovQIToR(new Instruction(OPCode.LoadBIToR, [new ImmByte(value), destination]));

            SetRegisterValue(destination.Code, value);
        }
        public void ExecuteLoadBRToR(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            Register destination = inst.args[1] as Register;

            ulong address = GetRegisterValue(source.Code);

            byte value = Ram.ReadByte((long)address);

            ExecuteMovQIToR(new Instruction(OPCode.LoadBIToR, [new ImmDWord(value), destination]));

            SetRegisterValue(destination.Code, value);
        }
        #endregion

        #region SaveInst
        public void ExecuteSaveQIToR(Instruction inst)
        {
            ImmQWord source = inst.args[0] as ImmQWord;
            Register destination = inst.args[1] as Register;

            byte[] bytes = BitConverter.GetBytes(source.Value);

            Ram.Write((long)GetRegisterValue(destination.Code), source.Value);
        }
        public void ExecuteSaveQRToR(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            Register destination = inst.args[1] as Register;

            byte[] bytes = BitConverter.GetBytes(GetRegisterValue(source.Code));

            Ram.Write((long)GetRegisterValue(destination.Code), GetRegisterValue(source.Code));
        }
        public void ExecuteSaveDIToR(Instruction inst)
        {
            ImmDWord source = inst.args[0] as ImmDWord;
            Register destination = inst.args[1] as Register;

            byte[] bytes = BitConverter.GetBytes(source.Value);

            Ram.Write((long)GetRegisterValue(destination.Code), source.Value);
        }
        public void ExecuteSaveDRToR(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            Register destination = inst.args[1] as Register;

            byte[] bytes = BitConverter.GetBytes(GetRegisterValue(source.Code));

            Ram.Write((long)GetRegisterValue(destination.Code), GetRegisterValue(source.Code));
        }
        public void ExecuteSaveWIToR(Instruction inst)
        {
            ImmWord source = inst.args[0] as ImmWord;
            Register destination = inst.args[1] as Register;

            byte[] bytes = BitConverter.GetBytes(source.Value);

            Ram.Write((long)GetRegisterValue(destination.Code), source.Value);
        }
        public void ExecuteSaveWRToR(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            Register destination = inst.args[1] as Register;

            byte[] bytes = BitConverter.GetBytes(GetRegisterValue(source.Code));

            Ram.Write((long)GetRegisterValue(destination.Code), GetRegisterValue(source.Code));
        }
        public void ExecuteSaveBIToR(Instruction inst)
        {
            ImmByte source = inst.args[0] as ImmByte;
            Register destination = inst.args[1] as Register;

            Ram.Write((long)GetRegisterValue(destination.Code), source.Value);
        }
        public void ExecuteSaveBRToR(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            Register destination = inst.args[1] as Register;

            Ram.Write((long)GetRegisterValue(destination.Code), GetRegisterValue(source.Code));
        }
        public void ExecuteSaveQIToI(Instruction inst)
        {
            ImmQWord source = inst.args[0] as ImmQWord;
            ImmQWord destination = inst.args[1] as ImmQWord;

            byte[] bytes = BitConverter.GetBytes(source.Value);

            Ram.Write((long)destination.Value, source.Value);
        }
        public void ExecuteSaveQRToI(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            ImmQWord destination = inst.args[1] as ImmQWord;

            byte[] bytes = BitConverter.GetBytes(GetRegisterValue(source.Code));

            Ram.Write((long)destination.Value, GetRegisterValue(source.Code));
        }
        public void ExecuteSaveDIToI(Instruction inst)
        {
            ImmDWord source = inst.args[0] as ImmDWord;
            ImmQWord destination = inst.args[1] as ImmQWord;

            byte[] bytes = BitConverter.GetBytes(source.Value);

            Ram.Write((long)destination.Value, source.Value);
        }
        public void ExecuteSaveDRToI(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            ImmQWord destination = inst.args[1] as ImmQWord;

            byte[] bytes = BitConverter.GetBytes(GetRegisterValue(source.Code));

            Ram.Write((long)destination.Value, GetRegisterValue(source.Code));
        }
        public void ExecuteSaveWIToI(Instruction inst)
        {
            ImmWord source = inst.args[0] as ImmWord;
            ImmQWord destination = inst.args[1] as ImmQWord;

            byte[] bytes = BitConverter.GetBytes(source.Value);

            Ram.Write((long)destination.Value, source.Value);
        }
        public void ExecuteSaveWRToI(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            ImmQWord destination = inst.args[1] as ImmQWord;

            byte[] bytes = BitConverter.GetBytes(GetRegisterValue(source.Code));

            Ram.Write((long)destination.Value, GetRegisterValue(source.Code));
        }
        public void ExecuteSaveBIToI(Instruction inst)
        {
            ImmByte source = inst.args[0] as ImmByte;
            ImmQWord destination = inst.args[1] as ImmQWord;

            Ram.Write((long)destination.Value, source.Value);
        }
        public void ExecuteSaveBRToI(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            ImmQWord destination = inst.args[1] as ImmQWord;

            Ram.Write((long)destination.Value, GetRegisterValue(source.Code));
        }
        #endregion

        #region MovInst
        public void ExecuteMovQIToR(Instruction inst)
        {
            ImmQWord source = inst.args[0] as ImmQWord;
            Register destination = inst.args[1] as Register;

            SetRegisterValue(destination.Code, source.Value);
        }
        public void ExecuteMovQRToR(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            Register destination = inst.args[1] as Register;

            SetRegisterValue(destination.Code, GetRegisterValue(source.Code));
        }
        public void ExecuteMovDIToR(Instruction inst)
        {
            ImmDWord source = inst.args[0] as ImmDWord;
            Register destination = inst.args[1] as Register;

            ulong value = GetRegisterValue(destination.Code);
            uint newvalue = source.Value;

            // Step 1: Clear the first byte
            value &= 0xFFFFFFFF00000000;

            // Step 2: Set the new byte
            value |= (ulong)newvalue;

            SetRegisterValue(destination.Code, value);
        }
        public void ExecuteMovDRToR(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            Register destination = inst.args[1] as Register;

            ulong value = GetRegisterValue(destination.Code);
            uint newvalue = (uint)(GetRegisterValue(source.Code) & 0xFFFFFFFF);

            // Step 1: Clear the first byte
            value &= 0xFFFFFFFF00000000;

            // Step 2: Set the new byte
            value |= (ulong)newvalue;

            SetRegisterValue(destination.Code, value);
        }
        public void ExecuteMovWIToR(Instruction inst)
        {
            ImmWord source = inst.args[0] as ImmWord;
            Register destination = inst.args[1] as Register;

            ulong value = GetRegisterValue(destination.Code);
            ushort newvalue = source.Value;

            // Step 1: Clear the first byte
            value &= 0xFFFFFFFFFFFF0000;

            // Step 2: Set the new byte
            value |= (ulong)newvalue;

            SetRegisterValue(destination.Code, value);
        }
        public void ExecuteMovWRToR(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            Register destination = inst.args[1] as Register;

            ulong value = GetRegisterValue(destination.Code);
            ushort newvalue = (ushort)(GetRegisterValue(source.Code) & 0xFFFF);

            // Step 1: Clear the first byte
            value &= 0xFFFFFFFFFFFF0000;

            // Step 2: Set the new byte
            value |= (ulong)newvalue;

            SetRegisterValue(destination.Code, value);
        }
        public void ExecuteMovBIToR(Instruction inst)
        {
            ImmByte source = inst.args[0] as ImmByte;
            Register destination = inst.args[1] as Register;

            ulong value = GetRegisterValue(destination.Code);
            byte newvalue = source.Value;

            // Step 1: Clear the first byte
            value &= 0xFFFFFFFFFFFFFF00;

            // Step 2: Set the new byte
            value |= (ulong)newvalue;

            SetRegisterValue(destination.Code, value);
        }
        public void ExecuteMovBRToR(Instruction inst)
        {
            Register source = inst.args[0] as Register;
            Register destination = inst.args[1] as Register;

            ulong value = GetRegisterValue(destination.Code);
            byte newvalue = (byte)(GetRegisterValue(source.Code) & 0xFF);

            // Step 1: Clear the first byte
            value &= 0xFFFFFFFFFFFFFF00;

            // Step 2: Set the new byte
            value |= (ulong)newvalue;

            SetRegisterValue(destination.Code, value);
        }
        #endregion

        #region Parsing
        public Instruction ParseInstruction()
        {
            OPCode opcode = (OPCode)Ram.ReadByte((long)Cpu.ProgramCounter);  // Cast to OPCode

            Instruction inst = new(opcode, []);

            Cpu.ProgramCounter++;

            switch (inst.operation)
            {
                case OPCode.MovBRToR:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.MovBIToR:
                    inst.args.Add(ParseImmediateB());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.MovWRToR:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.MovWIToR:
                    inst.args.Add(ParseImmediateW());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.MovDRToR:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.MovDIToR:
                    inst.args.Add(ParseImmediateD());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.MovRToR:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.MovIToR:
                    inst.args.Add(ParseImmediateQ());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.SaveIToR:
                    inst.args.Add(ParseImmediateQ());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.SaveRToR:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.SaveDIToR:
                    inst.args.Add(ParseImmediateD());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.SaveDRToR:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.SaveWIToR:
                    inst.args.Add(ParseImmediateW());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.SaveWRToR:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.SaveBIToR:
                    inst.args.Add(ParseImmediateB());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.SaveBRToR:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.SaveIToI:
                    inst.args.Add(ParseImmediateQ());
                    inst.args.Add(ParseImmediateQ());
                    break;
                case OPCode.SaveRToI:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseImmediateQ());
                    break;
                case OPCode.SaveDIToI:
                    inst.args.Add(ParseImmediateD());
                    inst.args.Add(ParseImmediateQ());
                    break;
                case OPCode.SaveDRToI:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseImmediateQ());
                    break;
                case OPCode.SaveWIToI:
                    inst.args.Add(ParseImmediateW());
                    inst.args.Add(ParseImmediateQ());
                    break;
                case OPCode.SaveWRToI:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseImmediateQ());
                    break;
                case OPCode.SaveBIToI:
                    inst.args.Add(ParseImmediateB());
                    inst.args.Add(ParseImmediateQ());
                    break;
                case OPCode.SaveBRToI:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseImmediateQ());
                    break;
                case OPCode.LoadIToR:
                    inst.args.Add(ParseImmediateQ());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.LoadRToR:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.LoadDIToR:
                    inst.args.Add(ParseImmediateD());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.LoadDRToR:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.LoadWIToR:
                    inst.args.Add(ParseImmediateW());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.LoadWRToR:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.LoadBIToR:
                    inst.args.Add(ParseImmediateB());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.LoadBRToR:
                    inst.args.Add(ParseRegister());
                    inst.args.Add(ParseRegister());
                    break;
                case OPCode.Inc:
                    break;
                case OPCode.Dec:
                    break;
            }

            return inst;
        }

        public ImmByte ParseImmediateB()
        {
            ImmByte imm = new(Ram.ReadByte((long)Cpu.ProgramCounter));

            Cpu.ProgramCounter++;

            return imm;
        }
        public ImmWord ParseImmediateW()
        {
            ImmWord imm = new(Ram.ReadUInt16((long)Cpu.ProgramCounter));

            Cpu.ProgramCounter += 2;

            return imm;
        }
        public ImmDWord ParseImmediateD()
        {
            ImmDWord imm = new(Ram.ReadUInt32((long)Cpu.ProgramCounter));

            Cpu.ProgramCounter += 4;

            return imm;
        }
        public ImmQWord ParseImmediateQ()
        {
            ImmQWord imm = new(Ram.ReadUInt64((long)Cpu.ProgramCounter));

            Cpu.ProgramCounter += 8;

            return imm;
        }

        public Register ParseRegister()
        {
            RegisterCode regcode = (RegisterCode)Ram.ReadByte((long)Cpu.ProgramCounter);

            Register reg = new()
            {
                Code = regcode
            };

            Cpu.ProgramCounter++;

            return reg;
        }
        #endregion

        private void SetRegisterValue(RegisterCode code, ulong value)
        {
            switch (code)
            {
                case RegisterCode.R0:
                    Cpu.R0 = value;
                    break;
                case RegisterCode.R1:
                    Cpu.R1 = value;
                    break;
                case RegisterCode.R2:
                    Cpu.R2 = value;
                    break;
                case RegisterCode.R3:
                    Cpu.R3 = value;
                    break;
                case RegisterCode.R4:
                    Cpu.R4 = value;
                    break;
                case RegisterCode.R5:
                    Cpu.R5 = value;
                    break;
                case RegisterCode.R6:
                    Cpu.R6 = value;
                    break;
                case RegisterCode.R7:
                    Cpu.R7 = value;
                    break;
                case RegisterCode.R8:
                    Cpu.R8 = value;
                    break;
                case RegisterCode.R9:
                    Cpu.R9 = value;
                    break;
                case RegisterCode.RCOUNT:
                    Cpu.RCOUNT = value;
                    break;
                case RegisterCode.Flags:
                    Cpu.Flags = value;
                    break;
                case RegisterCode.StackPointer:
                    Cpu.StackPointer = value;
                    break;
                case RegisterCode.StackStart:
                    Cpu.StackStart = value;
                    break;
                case RegisterCode.StackEnd:
                    Cpu.StackEnd = value;
                    break;
                case RegisterCode.ProgramStartLocation:
                    Cpu.ProgramStartLocation = value;
                    break;
                case RegisterCode.ProgramCounter:
                    Cpu.ProgramCounter = value;
                    break;
            }
        }

        private ulong GetRegisterValue(RegisterCode code)
        {
            return code switch
            {
                RegisterCode.R0 => Cpu.R0,
                RegisterCode.R1 => Cpu.R1,
                RegisterCode.R2 => Cpu.R2,
                RegisterCode.R3 => Cpu.R3,
                RegisterCode.R4 => Cpu.R4,
                RegisterCode.R5 => Cpu.R5,
                RegisterCode.R6 => Cpu.R6,
                RegisterCode.R7 => Cpu.R7,
                RegisterCode.R8 => Cpu.R8,
                RegisterCode.R9 => Cpu.R9,
                RegisterCode.RCOUNT => Cpu.RCOUNT,
                RegisterCode.Flags => Cpu.Flags,
                RegisterCode.StackPointer => Cpu.StackPointer,
                RegisterCode.StackStart => Cpu.StackStart,
                RegisterCode.StackEnd => Cpu.StackEnd,
                RegisterCode.ProgramStartLocation => Cpu.ProgramStartLocation,
                RegisterCode.ProgramCounter => Cpu.ProgramCounter,
                _ => 0
            };
        }

        public bool LoadBootRom()
        {
            if (Bootrom.Length > Ram.Capacity)
            {
                Console.WriteLine("Cannot Save bootrom because ROM is larger than RAM Size.");
                return false;
            }

            long pos = Ram.Capacity - Bootrom.Length;
            Ram.WriteArray<byte>((int)pos, Bootrom, 0, Bootrom.Length);

            Cpu.ProgramStartLocation = (UInt64)pos;
            Cpu.ProgramCounter = Cpu.ProgramStartLocation;

            return true;
        }
    }

    public class Test
    {
        public static void Main()
        {
            System sys = new(File.ReadAllBytes("C:\\Users\\Developer\\source\\repos\\ApplePieCodes\\cpuemu\\cpuemu\\TestRom.bin"), 80); // Move 21 To RAM 0
            sys.Run();

            Console.WriteLine(sys);
        }
    }
}