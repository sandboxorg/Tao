﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Tao.Core
{
    /// <summary>
    /// Represents a class that reads the PR File header from a Portable Executable file.
    /// </summary>
    public class COFFHeader
    {
        /// <summary>
        /// Parses the PR file header from the given input <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The input stream that contains the PR file header.</param>
        public void Read(Stream stream)
        {
            var binaryReader = new BinaryReader(stream);
            ReadPESignature(binaryReader);

            // Read the target machine type
            MachineType = (MachineType)binaryReader.ReadInt16();

            // Read the section count
            NumberOfSections = binaryReader.ReadInt16();

            // Read the time stamp
            TimeDateStamp = binaryReader.ReadInt32();
        }

        /// <summary>
        /// Reads the PE signature from the binary stream.
        /// </summary>
        /// <param name="binaryReader">The reader that contains the stream with the PE signature.</param>
        private void ReadPESignature(BinaryReader binaryReader)
        {
            var bytes = binaryReader.ReadBytes(4);

            var text = Encoding.ASCII.GetString(bytes);
            HasPortableExecutableSignature = text == "PE\0\0";
        }

        /// <summary>
        /// Gets the value indicating whether or not the previously read stream contains a PE signature.
        /// </summary>
        /// <value>A boolean flag that determines whether or not the previously read stream contains a PE signature.</value>
        public bool HasPortableExecutableSignature
        {
            get; private set;
        }

        /// <summary>
        /// Gets the value indicating the size of the section table.
        /// </summary>
        /// <value>The size of the section table.</value>
        public int NumberOfSections
        {
            get; private set;
        }

        /// <summary>
        /// Gets the value indicating the target <see cref="MachineType"/> for the given PE header.
        /// </summary>
        /// <value>The machine type of the target image.</value>
        public MachineType MachineType
        {
            get; private set;
        }

        /// <summary>
        /// Gets the value indicating the number of seconds since 00:00 January 1, 1970 when the file was created.
        /// </summary>
        /// <value>The number of seconds that have elapsed since 00:00 January 1, 1970.</value>
        public int TimeDateStamp
        {
            get; private set;
        }
    }
}
