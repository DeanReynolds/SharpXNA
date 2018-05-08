using System;
using System.Diagnostics;

namespace LiteNetLib.Utils
{
    public static class NetBitWriter
    {
        public static byte ReadByte(byte[] fromBuffer, int numberOfBits, int readBitOffset)
        {
            int bytePtr = readBitOffset >> 3;
            int startReadAtIndex = readBitOffset - (bytePtr * 8);
            if (startReadAtIndex == 0 && numberOfBits == 8)
                return fromBuffer[bytePtr];
            byte returnValue = (byte)(fromBuffer[bytePtr] >> startReadAtIndex);
            int numberOfBitsInSecondByte = numberOfBits - (8 - startReadAtIndex);
            if (numberOfBitsInSecondByte < 1)
                return (byte)(returnValue & (255 >> (8 - numberOfBits)));
            byte second = fromBuffer[bytePtr + 1];
            second &= (byte)(255 >> (8 - numberOfBitsInSecondByte));
            return (byte)(returnValue | (byte)(second << (numberOfBits - numberOfBitsInSecondByte)));
        }
        
        public static void ReadBytes(byte[] fromBuffer, int numberOfBytes, int readBitOffset, byte[] destination, int destinationByteOffset)
        {
            int readPtr = readBitOffset >> 3;
            int startReadAtIndex = readBitOffset - (readPtr * 8);
            if (startReadAtIndex == 0)
            {
                Buffer.BlockCopy(fromBuffer, readPtr, destination, destinationByteOffset, numberOfBytes);
                return;
            }
            int secondPartLen = 8 - startReadAtIndex;
            int secondMask = 255 >> secondPartLen;
            for (int i = 0; i < numberOfBytes; i++)
            {
                int b = fromBuffer[readPtr] >> startReadAtIndex;
                readPtr++;
                int second = fromBuffer[readPtr] & secondMask;
                destination[destinationByteOffset++] = (byte)(b | (second << secondPartLen));
            }
            return;
        }
        
        public static void WriteByte(byte source, int numberOfBits, byte[] destination, int destBitOffset)
        {
            if (numberOfBits == 0)
                return;
            source = (byte)(source & (0xFF >> (8 - numberOfBits)));
            int p = destBitOffset >> 3;
            int bitsUsed = destBitOffset & 0x7;
            int bitsFree = 8 - bitsUsed;
            int bitsLeft = bitsFree - numberOfBits;
            if (bitsLeft >= 0)
            {
                int mask = (0xFF >> bitsFree) | (0xFF << (8 - bitsLeft));
                destination[p] = (byte)((destination[p] & mask) | (source << bitsUsed));
                return;
            }
            destination[p] = (byte)((destination[p] & (0xFF >> bitsFree)) | (source << bitsUsed));
            p += 1;
            destination[p] = (byte)((destination[p] & (0xFF << (numberOfBits - bitsFree))) | (source >> bitsFree));
        }
        
        public static void WriteBytes(byte[] source, int sourceByteOffset, int numberOfBytes, byte[] destination, int destBitOffset)
        {
            int dstBytePtr = destBitOffset >> 3;
            int firstPartLen = (destBitOffset % 8);
            if (firstPartLen == 0)
            {
                Buffer.BlockCopy(source, sourceByteOffset, destination, dstBytePtr, numberOfBytes);
                return;
            }
            int lastPartLen = 8 - firstPartLen;
            for (int i = 0; i < numberOfBytes; i++)
            {
                byte src = source[sourceByteOffset + i];
                destination[dstBytePtr] &= (byte)(255 >> lastPartLen);
                destination[dstBytePtr] |= (byte)(src << firstPartLen);
                dstBytePtr++;
                destination[dstBytePtr] &= (byte)(255 << firstPartLen);
                destination[dstBytePtr] |= (byte)(src >> lastPartLen);
            }
            return;
        }
        
#if UNSAFE
		public static unsafe ushort ReadUInt16(byte[] fromBuffer, int numberOfBits, int readBitOffset)
		{
			Debug.Assert(((numberOfBits > 0) && (numberOfBits <= 16)), "ReadUInt16() can only read between 1 and 16 bits");

			if (numberOfBits == 16 && ((readBitOffset % 8) == 0))
			{
				fixed (byte* ptr = &(fromBuffer[readBitOffset / 8]))
				{
					return *(((ushort*)ptr));
				}
			}
#else
        public static ushort ReadUInt16(byte[] fromBuffer, int numberOfBits, int readBitOffset)
        {
            Debug.Assert(((numberOfBits > 0) && (numberOfBits <= 16)), "ReadUInt16() can only read between 1 and 16 bits");
#endif
            ushort returnValue;
            if (numberOfBits <= 8)
            {
                returnValue = ReadByte(fromBuffer, numberOfBits, readBitOffset);
                return returnValue;
            }
            returnValue = ReadByte(fromBuffer, 8, readBitOffset);
            numberOfBits -= 8;
            readBitOffset += 8;
            if (numberOfBits <= 8)
                returnValue |= (ushort)(ReadByte(fromBuffer, numberOfBits, readBitOffset) << 8);
#if BIGENDIAN
			uint retVal = returnValue;
			retVal = ((retVal & 0x0000ff00) >> 8) | ((retVal & 0x000000ff) << 8);
			return (ushort)retVal;
#else
            return returnValue;
#endif
        }
        
#if UNSAFE
		public static unsafe uint ReadUInt32(byte[] fromBuffer, int numberOfBits, int readBitOffset)
		{
			NetException.Assert(((numberOfBits > 0) && (numberOfBits <= 32)), "ReadUInt32() can only read between 1 and 32 bits");

			if (numberOfBits == 32 && ((readBitOffset % 8) == 0))
			{
				fixed (byte* ptr = &(fromBuffer[readBitOffset / 8]))
				{
					return *(((uint*)ptr));
				}
			}
#else

        public static uint ReadUInt32(byte[] fromBuffer, int numberOfBits, int readBitOffset)
        {
#endif
            uint returnValue;
            if (numberOfBits <= 8)
            {
                returnValue = ReadByte(fromBuffer, numberOfBits, readBitOffset);
                return returnValue;
            }
            returnValue = ReadByte(fromBuffer, 8, readBitOffset);
            numberOfBits -= 8;
            readBitOffset += 8;
            if (numberOfBits <= 8)
            {
                returnValue |= (uint)(ReadByte(fromBuffer, numberOfBits, readBitOffset) << 8);
                return returnValue;
            }
            returnValue |= (uint)(ReadByte(fromBuffer, 8, readBitOffset) << 8);
            numberOfBits -= 8;
            readBitOffset += 8;
            if (numberOfBits <= 8)
            {
                uint r = ReadByte(fromBuffer, numberOfBits, readBitOffset);
                r <<= 16;
                returnValue |= r;
                return returnValue;
            }
            returnValue |= (uint)(ReadByte(fromBuffer, 8, readBitOffset) << 16);
            numberOfBits -= 8;
            readBitOffset += 8;
            returnValue |= (uint)(ReadByte(fromBuffer, numberOfBits, readBitOffset) << 24);
#if BIGENDIAN
			return
				((returnValue & 0xff000000) >> 24) |
				((returnValue & 0x00ff0000) >> 8) |
				((returnValue & 0x0000ff00) << 8) |
				((returnValue & 0x000000ff) << 24);
#else
            return returnValue;
#endif
        }
        
        public static void WriteUInt16(ushort source, int numberOfBits, byte[] destination, int destinationBitOffset)
        {
            if (numberOfBits == 0)
                return;
#if BIGENDIAN
			// reorder bytes
			uint intSource = source;
			intSource = ((intSource & 0x0000ff00) >> 8) | ((intSource & 0x000000ff) << 8);
			source = (ushort)intSource;
#endif
            if (numberOfBits <= 8)
            {
                NetBitWriter.WriteByte((byte)source, numberOfBits, destination, destinationBitOffset);
                return;
            }
            NetBitWriter.WriteByte((byte)source, 8, destination, destinationBitOffset);
            numberOfBits -= 8;
            if (numberOfBits > 0)
                NetBitWriter.WriteByte((byte)(source >> 8), numberOfBits, destination, destinationBitOffset + 8);
        }
        
        public static int WriteUInt32(uint source, int numberOfBits, byte[] destination, int destinationBitOffset)
        {
#if BIGENDIAN
			// reorder bytes
			source = ((source & 0xff000000) >> 24) |
				((source & 0x00ff0000) >> 8) |
				((source & 0x0000ff00) << 8) |
				((source & 0x000000ff) << 24);
#endif

            int returnValue = destinationBitOffset + numberOfBits;
            if (numberOfBits <= 8)
            {
                NetBitWriter.WriteByte((byte)source, numberOfBits, destination, destinationBitOffset);
                return returnValue;
            }
            NetBitWriter.WriteByte((byte)source, 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;

            if (numberOfBits <= 8)
            {
                NetBitWriter.WriteByte((byte)(source >> 8), numberOfBits, destination, destinationBitOffset);
                return returnValue;
            }
            NetBitWriter.WriteByte((byte)(source >> 8), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;

            if (numberOfBits <= 8)
            {
                NetBitWriter.WriteByte((byte)(source >> 16), numberOfBits, destination, destinationBitOffset);
                return returnValue;
            }
            NetBitWriter.WriteByte((byte)(source >> 16), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;

            NetBitWriter.WriteByte((byte)(source >> 24), numberOfBits, destination, destinationBitOffset);
            return returnValue;
        }

        /// <summary>
        /// Writes the specified number of bits into a byte array
        /// </summary>

        public static int WriteUInt64(ulong source, int numberOfBits, byte[] destination, int destinationBitOffset)
        {
#if BIGENDIAN
			source = ((source & 0xff00000000000000L) >> 56) |
				((source & 0x00ff000000000000L) >> 40) |
				((source & 0x0000ff0000000000L) >> 24) |
				((source & 0x000000ff00000000L) >> 8) |
				((source & 0x00000000ff000000L) << 8) |
				((source & 0x0000000000ff0000L) << 24) |
				((source & 0x000000000000ff00L) << 40) |
				((source & 0x00000000000000ffL) << 56);
#endif

            int returnValue = destinationBitOffset + numberOfBits;
            if (numberOfBits <= 8)
            {
                NetBitWriter.WriteByte((byte)source, numberOfBits, destination, destinationBitOffset);
                return returnValue;
            }
            NetBitWriter.WriteByte((byte)source, 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;

            if (numberOfBits <= 8)
            {
                NetBitWriter.WriteByte((byte)(source >> 8), numberOfBits, destination, destinationBitOffset);
                return returnValue;
            }
            NetBitWriter.WriteByte((byte)(source >> 8), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;

            if (numberOfBits <= 8)
            {
                NetBitWriter.WriteByte((byte)(source >> 16), numberOfBits, destination, destinationBitOffset);
                return returnValue;
            }
            NetBitWriter.WriteByte((byte)(source >> 16), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;

            if (numberOfBits <= 8)
            {
                NetBitWriter.WriteByte((byte)(source >> 24), numberOfBits, destination, destinationBitOffset);
                return returnValue;
            }
            NetBitWriter.WriteByte((byte)(source >> 24), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;

            if (numberOfBits <= 8)
            {
                NetBitWriter.WriteByte((byte)(source >> 32), numberOfBits, destination, destinationBitOffset);
                return returnValue;
            }
            NetBitWriter.WriteByte((byte)(source >> 32), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;

            if (numberOfBits <= 8)
            {
                NetBitWriter.WriteByte((byte)(source >> 40), numberOfBits, destination, destinationBitOffset);
                return returnValue;
            }
            NetBitWriter.WriteByte((byte)(source >> 40), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;

            if (numberOfBits <= 8)
            {
                NetBitWriter.WriteByte((byte)(source >> 48), numberOfBits, destination, destinationBitOffset);
                return returnValue;
            }
            NetBitWriter.WriteByte((byte)(source >> 48), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;

            if (numberOfBits <= 8)
            {
                NetBitWriter.WriteByte((byte)(source >> 56), numberOfBits, destination, destinationBitOffset);
                return returnValue;
            }
            NetBitWriter.WriteByte((byte)(source >> 56), 8, destination, destinationBitOffset);
            destinationBitOffset += 8;
            numberOfBits -= 8;

            return returnValue;
        }

        //
        // Variable size
        //

        /// <summary>
        /// Write Base128 encoded variable sized unsigned integer
        /// </summary>
        /// <returns>number of bytes written</returns>

        public static int WriteVariableUInt32(byte[] intoBuffer, int offset, uint value)
        {
            int retval = 0;
            uint num1 = (uint)value;
            while (num1 >= 0x80)
            {
                intoBuffer[offset + retval] = (byte)(num1 | 0x80);
                num1 = num1 >> 7;
                retval++;
            }
            intoBuffer[offset + retval] = (byte)num1;
            return retval + 1;
        }

        /// <summary>
        /// Reads a UInt32 written using WriteUnsignedVarInt(); will increment offset!
        /// </summary>

        public static uint ReadVariableUInt32(byte[] buffer, ref int offset)
        {
            int num1 = 0;
            int num2 = 0;
            while (true)
            {
                byte num3 = buffer[offset++];
                num1 |= (num3 & 0x7f) << (num2 & 0x1f);
                num2 += 7;
                if ((num3 & 0x80) == 0)
                    return (uint)num1;
            }
        }
    }
}