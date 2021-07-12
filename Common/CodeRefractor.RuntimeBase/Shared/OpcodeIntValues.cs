namespace CodeRefractor.Shared
{
    public static class OpcodeIntValues
    {
        public const int Switch = 0x45;
        public const int Nop = 0x0;
        public const int NewObj = 0x73;
        public const int CallInterface = 0x29;
        public const int Call = 0x28;
        public const int CallVirt = 0x6F;
        public const int Br = 0x38;
        public const int BrS = 0x2B;
        public const int Blt = 0x3F;
        public const int BltS = 0x32;
        public const int BltUn = 0x44;
        public const int BltUnS = 0x37;
        public const int Bge = 0x3C;
        public const int BgeS = 0x2F;
        public const int BgeUn = 0x41;
        public const int BgeUnS = 0x34;
        public const int Bgt = 0x3D;
        public const int BgtS = 0x30;
        public const int BgtUn = 0x42;
        public const int BgtUnS = 0x35;
        public const int Ble = 0x3E;
        public const int BleS = 0x31;
        public const int BleUn = 0x43;
        public const int BleUnS = 0x36;
        public const short BneUn = 0x40;
        public const short BneUnS = 0x33;
        public const int Beq = 0x3B;
        public const int BeqS = 0x2E;
        public const int BrTrue = 0x3A;
        public const int BrTrueS = 0x2D;
        public const int BrZero = 0x39;
        public const int BrZeroS = 0x2C;
        public const int Leave = 0xDD;
        public const int LeaveS = 0xDE;
    }
}