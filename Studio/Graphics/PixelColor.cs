namespace Studio.Graphics
{
    using System;
    using System.Runtime.InteropServices;

    // Definition of colors
    // from http://stackoverflow.com/questions/1176910/finding-specific-pixel-colors-of-a-bitmapimage
    // Using this construction we can acces pixel.Blue etc.

    // Should we also introduce HSV in bytes in this construction + conversion??
    // Should we introduce some low level operations using bitmasks and shift >> operators

    [StructLayout(LayoutKind.Explicit)]
    public struct PixelColor
    {
        // 32 bit BGRA 
        [FieldOffset(0)]
        public UInt32 ColorBGRA;
        // 8 bit components
        [FieldOffset(0)]
        public byte Blue;
        [FieldOffset(1)]
        public byte Green;
        [FieldOffset(2)]
        public byte Red;
        [FieldOffset(3)]
        public byte Alpha;
    }
}