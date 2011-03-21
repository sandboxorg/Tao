﻿using System.IO;
using Tao.Interfaces;

namespace Tao.Readers
{
    /// <summary>
    /// Represents a type that reads the file alignment from a coff header stream.
    /// </summary>
    public class ReadFileAlignment : IFunction<Stream, int>
    {
        private readonly IFunction<Stream> _optionalHeaderSeeker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadFileAlignment"/> class.
        /// </summary>
        public ReadFileAlignment(IFunction<Stream> optionalHeaderSeeker)
        {
            _optionalHeaderSeeker = optionalHeaderSeeker;
        }

        /// <summary>
        /// Reads the file alignment from a coff header stream.
        /// </summary>
        /// <param name="input">the input stream.</param>
        /// <returns>The file alignment.</returns>
        public int Execute(Stream input)
        {
            _optionalHeaderSeeker.Execute(input);

            input.Seek(0x24, SeekOrigin.Current);
            var reader = new BinaryReader(input);

            int result = reader.ReadInt32();

            return result;
        }
    }
}