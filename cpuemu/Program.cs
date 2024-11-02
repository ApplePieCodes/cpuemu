using System.Collections;

namespace cpuemu
{
    public class System
    {
        public UInt64 stackpointer = 0;
        public UInt64 stackstart = 0;
        public UInt64 stackend = 0;
        public UInt64 r0 = 0;
        public UInt64 r1 = 0;
        public UInt64 r2 = 0;
        public UInt64 r3 = 0;
        public UInt64 r4 = 0;
        public UInt64 r5 = 0;
        public UInt64 r6 = 0;
        public UInt64 r7 = 0;
        public UInt64 r8 = 0;
        public UInt64 r9 = 0;
        public byte[] bootrom;
        public byte[] ram;

        public System(byte[] rom, int ramsize)
        {
            bootrom = rom;
            ram = new byte[ramsize];
        }

        public bool LoadBootRom(bool higherhalf = true)
        {
            if (higherhalf)
            {

            }
        }
    }
}