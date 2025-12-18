using System.Collections;

namespace AllHands.Shared.Infrastructure.Utilities;

public static class BitArrayExtensions
{
    public static byte[] ToByteArray(this BitArray bitArray)
    {
        var numBytes = (bitArray.Length + 7) / 8; 
        var bytes = new byte[numBytes];
        
        for (var i = 0; i < bitArray.Length; i++)
        {
            if (bitArray[i])
            {
                bytes[i / 8] |= (byte)(1 << (i % 8)); 
            }
        }
        return bytes;
    }
}
