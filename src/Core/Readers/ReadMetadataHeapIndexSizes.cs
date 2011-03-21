﻿using System;
using System.IO;
using Tao.Interfaces;

namespace Tao.Readers
{
    /// <summary>
    /// Represents a class that reads the heap sizes for the #Strings, #Guid, and #Blob heaps.
    /// </summary>
    public class ReadMetadataHeapIndexSizes : IFunction<Stream, ITuple<int, int, int>>
    {
        private readonly IFunction<Stream, byte?> _readHeapSizesField;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadMetadataHeapIndexSizes"/> class.
        /// </summary>
        public ReadMetadataHeapIndexSizes(IFunction<Stream, byte?> readHeapSizesField)
        {
            _readHeapSizesField = readHeapSizesField;
        }

        /// <summary>
        /// Reads the heap sizes for the #Strings, #Guid, and #Blob heaps.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <returns>A tuple containing the index sizes for the #Strings, #Guid, and #Blob heaps.</returns>
        public ITuple<int, int, int> Execute(Stream input)
        {
            var bitVector = _readHeapSizesField.Execute(input);

            var stringIndexSize = GetIndexSize(0x1, bitVector);
            var guidIndexSize = GetIndexSize(0x2, bitVector);
            var blobIndexSize = GetIndexSize(0x4, bitVector);

            return Tuple.New(stringIndexSize, guidIndexSize, blobIndexSize);
        }

        /// <summary>
        /// Returns the index size from the bit vector.
        /// </summary>
        /// <param name="targetBit">The target bit.</param>
        /// <param name="bitVector">The HeapSizes field.</param>
        /// <returns>The heap index size.</returns>
        private int GetIndexSize(byte targetBit, byte? bitVector)
        {
            var useDwordIndices = (bitVector & targetBit) != 0;
            return useDwordIndices ? 4 : 2;
        }
    }
}
