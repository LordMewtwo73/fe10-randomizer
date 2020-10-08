using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE10Randomizer_v0._1
{
    static public unsafe class LZ77
    {
        #region Don't edit this!!!
        private const int SlidingWindowSize = 4096;
        private const int ReadAheadBufferSize = 18;
        private const int BlockSize = 8;
        enum ScanDepth : byte
        {
            Byte = 1,
            HalfWord = 2,
            Word = 4
        };
        private const ScanDepth scanDepth = ScanDepth.Word;
        private const int sizeMultible = 0x20;
        private const int maxSize = 0x8000;
        #endregion




        /// <summary>
        /// Scans the data for potential LZ77 compressions
        /// </summary>
        /// <param name="data">Data to scan</param>
        /// <param name="offset">Starting offset of are to scan</param>
        /// <param name="amount">Size of the area to scan</param>
        /// <returns></returns>
        static public int[] Scan(byte[] data, int offset, int amount, int sizeMultible, int minSize, int maxSize)
        {
            fixed (byte* ptr = &data[offset])
            {
                return Scan(ptr, amount, sizeMultible, minSize, maxSize);
            }
        }

        /// <summary>
        /// Scans an area in memory for potential LZ77 compressions
        /// </summary>
        /// <param name="pointer">Pointer to start of area to scan</param>
        /// <param name="amount">Size of the area to scan in bytes</param>
        /// <returns>An array of offsets relative to the beginning of scan area</returns>
        static public int[] Scan(byte* pointer, int amount, int sizeMultible, int minSize, int maxSize)
        {
            List<int> results = new List<int>();

            for (int i = 0; i < amount; i += (int)scanDepth)
            {
                if (pointer[i] == 0x10)
                {
                    uint header = *((uint*)(pointer + i));
                    header >>= 8;
                    if ((header % sizeMultible == 0)
                        && (header <= maxSize)
                        && (header > 0)
                        && (CanBeUnCompressed(pointer + i, minSize, maxSize)))
                    {
                        results.Add(i);
                    }
                }
            }
            return results.ToArray();
        }



        /// <summary>
        /// Checks if data can be uncompressed
        /// </summary>
        /// <param name="source">Pointer to beginning of data</param>
        /// <returns>True if data can be uncompressed, else false</returns>
        static public bool CanBeUnCompressed(byte* source, int minSize, int maxSize)
        {
            int lenght = GetCompressedDataLenght(source);
            return (lenght != -1) && (lenght <= maxSize);
        }

        /// <summary>
        /// Checks if data can be uncompressed
        /// </summary>
        /// <param name="data">Data with data to test</param>
        /// <param name="offset">Offset of the data to test in data</param>
        /// <param name="maxLenght">Maximun length the comressed data can have</param>
        /// <returns>True if data can be uncompressed, else false</returns>
        static public bool CanBeUnCompressed(byte[] data, int offset, int minSize, int maxSize)
        {
            bool result;
            if (maxSize > data.Length - offset)
                maxSize = data.Length - offset;

            fixed (byte* ptr = &data[offset])
            {
                result = CanBeUnCompressed(ptr, minSize, maxSize);
            }
            return result;
        }




        /// <summary>
        /// Gets the lenght of the compressed data
        /// </summary>
        /// <param name="source">Pointer to the data to check</param>
        /// <returns>Returns lenght or -1 if can't be uncompressed</returns>
        static public int GetCompressedDataLenght(byte* source)
        {
            if (*source != 0x10)
                return -1;

            uint decompressedLenght = (*(uint*)(source)) >> 8;
            int decompressedPosition = 0;
            int length = 4;

            while (decompressedPosition < decompressedLenght)
            {
                byte isCompressed = source[length++];

                for (int i = 0; i < BlockSize && decompressedPosition < decompressedLenght; i++)
                {
                    if ((isCompressed & 0x80) != 0)
                    {
                        byte AmountToCopy = (byte)(((source[length] >> 4) & 0xF) + 3);
                        ushort CopyPosition = (ushort)((((source[length] & 0xF) << 8) + source[length + 1]) + 1);

                        if (CopyPosition > decompressedPosition)
                            return -1;

                        decompressedPosition += AmountToCopy;
                        length += 2;
                    }
                    else
                    {
                        decompressedPosition++;
                        length++;
                    }
                    unchecked
                    {
                        isCompressed <<= 1;
                    }
                }
            }
            if (length % 4 != 0)
                length += 4 - length % 4;

            return length;
        }

        /// <summary>
        /// Gets the lenght of the compressed data
        /// </summary>
        /// <param name="data">Data where compressed data is</param>
        /// <param name="offset">The position of the data</param>
        /// <returns>Returns lenght or -1 if can't be uncompressed</returns>
        static public int GetCompressedDataLenght(byte[] data, int offset)
        {
            fixed (byte* ptr = &data[offset])
            {
                return GetCompressedDataLenght(ptr);
            }
        }


        /// <summary>
        /// Compresses data with LZ77
        /// </summary>
        /// <param name="data">Data to compress</param>
        /// <returns>Compressed data</returns>
        static public byte[] Compress(byte[] data)
        {
            fixed (byte* pointer = &data[0])
            {
                return Compress(pointer, data.Length);
            }
        }

        /// <summary>
        /// Compresses data with LZ77
        /// </summary>
        /// <param name="data">Data to compress</param>
        /// <param name="index">Beginning offset of the data to compress</param>
        /// <param name="length">Length of the data to compress</param>
        /// <returns>Compressed data</returns>
        static public byte[] Compress(byte[] data, int index, int length)
        {
            fixed (byte* pointer = &data[index])
            {
                return Compress(pointer, length);
            }
        }



        /// <summary>
        /// Compresses data with LZ77
        /// </summary>
        /// <param name="source">Pointer to beginning of the data</param>
        /// <param name="lenght">Lenght of the data to compress in bytes</param>
        /// <returns>Compressed data</returns>
        static public byte[] Compress(byte* source, int lenght)
        {
            int position = 0;

            List<byte> CompressedData = new List<byte>();
            CompressedData.Add(0x10);

            {
                byte* pointer = (byte*)&lenght;
                for (int i = 0; i < 3; i++)
                    CompressedData.Add(*(pointer++));
            }

            while (position < lenght)
            {
                byte isCompressed = 0;
                List<byte> tempList = new List<byte>();

                for (int i = 0; i < BlockSize; i++)
                {
                    int[] searchResult = Search(source, position, lenght);

                    if (searchResult[0] > 2)
                    {
                        byte add = (byte)((((searchResult[0] - 3) & 0xF) << 4) + (((searchResult[1] - 1) >> 8) & 0xF));
                        tempList.Add(add);
                        add = (byte)((searchResult[1] - 1) & 0xFF);
                        tempList.Add(add);
                        position += searchResult[0];
                        isCompressed |= (byte)(1 << (BlockSize - (i + 1)));
                    }
                    else if (searchResult[0] >= 0)
                        tempList.Add(source[position++]);
                    else
                        break;
                }
                CompressedData.Add(isCompressed);
                CompressedData.AddRange(tempList);
            }
            while (CompressedData.Count % 4 != 0)
                CompressedData.Add(0);

            return CompressedData.ToArray();
        }

        static private int[] Search(byte* source, int position, int lenght)
        {
            if (position >= lenght)
                return new int[2] { -1, 0 };
            if ((position < 2) || ((lenght - position) < 2))
                return new int[2] { 0, 0 };

            List<int> results = new List<int>();

            for (int i = 1; (i < SlidingWindowSize) && (i < position); i++)
            {
                if (source[position - (i + 1)] == source[position])
                {
                    results.Add(i + 1);
                }
            }
            if (results.Count == 0)
                return new int[2] { 0, 0 };

            int amountOfBytes = 0;

            bool Continue = true;
            while (amountOfBytes < ReadAheadBufferSize && Continue)
            {
                amountOfBytes++;
                for (int i = results.Count - 1; i >= 0; i--)
                {
                    if (source[position + amountOfBytes] != source[position - results[i] + (amountOfBytes % results[i])])
                    {
                        if (results.Count > 1)
                            results.RemoveAt(i);
                        else
                            Continue = false;
                    }
                }
            }
            return new int[2] { amountOfBytes, results[0] }; //lenght of data is first, then position
        }


        /// <summary>
        /// Decompresses LZ77 data
        /// </summary>
        /// <param name="data">Data where compressed data is</param>
        /// <param name="offset">Offset of the compressed data</param>
        /// <returns>Decompressed data or null if decompression fails</returns>
        static public byte[] Decompress(byte[] data, int offset)
        {
            fixed (byte* ptr = &data[offset])
            {
                return Decompress(ptr);
            }
        }

        /// <summary>
        /// Decompresses LZ77 data
        /// </summary>
        /// <param name="source">Pointer to data to decompress</param>
        /// <returns>Decompressed data or null if decompression fails</returns>
        static public byte[] Decompress(byte* source)
        {
            byte[] uncompressedData = new byte[(*(uint*)source) >> 8];
            if (uncompressedData.Length > 0)
            {
                fixed (byte* destination = &uncompressedData[0])
                {
                    if (!Decompress(source, destination))
                        uncompressedData = null;
                }
            }
            return uncompressedData;
        }

        /// <summary>
        /// Decompresses LZ77 data
        /// </summary>
        /// <param name="source">Pointer to compressed data</param>
        /// <param name="target">Pointer to where uncompressed data goes</param>
        /// <returns>True if successful, else false</returns>
        static public bool Decompress(byte* source, byte* target)
        {
            if (*source++ != 0x10)
                return false;

            int positionUncomp = 0;
            int lenght = *(source++) + (*(source++) << 8) + (*(source++) << 16);

            while (positionUncomp < lenght)
            {
                byte isCompressed = *(source++);
                for (int i = 0; i < BlockSize; i++)
                {
                    if ((isCompressed & 0x80) != 0)
                    {
                        int amountToCopy = 3 + (*(source) >> 4);
                        int copyPosition = 1;
                        copyPosition += (*(source++) & 0xF) << 8;
                        copyPosition += *(source++);

                        if (copyPosition > lenght)
                            return false;

                        for (int u = 0; u < amountToCopy; u++)
                        {
                            target[positionUncomp] = target[positionUncomp - u - copyPosition + (u % copyPosition)];
                            positionUncomp++;
                        }
                    }
                    else
                    {
                        target[positionUncomp++] = *source++;
                    }
                    if (!(positionUncomp < lenght))
                        break;

                    unchecked
                    {
                        isCompressed <<= 1;
                    }
                }
            }
            return true;
        }


    }
}
