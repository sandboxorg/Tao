﻿using System;
using System.Collections.Generic;
using System.Text;
using Tao.Interfaces;
using Tao.Model;

namespace Tao.Signatures
{
    /// <summary>
    /// Represents a type that reads type signatures.
    /// </summary>
    public class TypeSignatureReader : IFunction<IEnumerable<byte>, TypeSignature>
    {
        private readonly IFunction<byte, ITuple<TableId, uint>> _typeDefOrRefEncodedReader;       
        private readonly IFunction<ITuple<Queue<byte>, ICollection<CustomMod>>, int> _readCustomMods;

        private readonly Dictionary<ElementType, Func<IList<byte>, TypeSignature>> _entries =
            new Dictionary<ElementType, Func<IList<byte>, TypeSignature>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeSignatureReader"/> class.
        /// </summary>
        /// <param name="typeDefOrRefEncodedReader">The reader that will read the embbedded type def or type ref token.</param>
        /// <param name="readCustomMods">The reader that will read the CustomMod signatures</param>
        public TypeSignatureReader(IFunction<byte, ITuple<TableId, uint>> typeDefOrRefEncodedReader, IFunction<ITuple<Queue<byte>, ICollection<CustomMod>>, int> readCustomMods)
        {
            if (typeDefOrRefEncodedReader == null)
                throw new ArgumentNullException("typeDefOrRefEncodedReader");

            _typeDefOrRefEncodedReader = typeDefOrRefEncodedReader;
            _readCustomMods = readCustomMods;

            CreateEntries();
        }

        private void CreateEntries()
        {
            Func<IList<byte>, TypeSignature> defaultCreator = bytes => new TypeSignature();
            _entries[ElementType.Boolean] = defaultCreator;
            _entries[ElementType.Char] = defaultCreator;
            _entries[ElementType.I1] = defaultCreator;
            _entries[ElementType.I2] = defaultCreator;
            _entries[ElementType.I4] = defaultCreator;
            _entries[ElementType.I8] = defaultCreator;
            _entries[ElementType.U1] = defaultCreator;
            _entries[ElementType.U4] = defaultCreator;
            _entries[ElementType.U8] = defaultCreator;
            _entries[ElementType.R4] = defaultCreator;
            _entries[ElementType.R8] = defaultCreator;
            _entries[ElementType.Object] = defaultCreator;
            _entries[ElementType.String] = defaultCreator;

            _entries[ElementType.Class] = CreateTypeDefOrRefEncodedSignature;
            _entries[ElementType.ValueType] = CreateTypeDefOrRefEncodedSignature;
            _entries[ElementType.Ptr] = CreatePointerSignature;
        }

        /// <summary>
        /// Reads the type signature from the given <paramref name="input"/> bytes.
        /// </summary>
        /// <param name="input">The bytes that represent the type signature.</param>
        /// <returns>A <see cref="TypeSignature"/> instance.</returns>
        public TypeSignature Execute(IEnumerable<byte> input)
        {
            var bytes = new List<byte>(input);
            var elementType = (ElementType)bytes[0];

            if (!_entries.ContainsKey(elementType))
                throw new NotSupportedException(string.Format("Element type not supported: {0}", elementType));

            var createSignature = _entries[elementType];
            var signature = createSignature(bytes);
            signature.ElementType = elementType;

            return signature;
        }

        private TypeSignature CreatePointerSignature(IList<byte> bytes)
        {
            if (bytes.Count == 0)
                throw new ArgumentException("Unexpected end of byte stream", "bytes");

            var mods = new List<CustomMod>();
            var byteQueue = new Queue<byte>(bytes);
            var elementType = (ElementType)byteQueue.Dequeue();
            var bytesRead = _readCustomMods.Execute(byteQueue, mods);

            PointerSignature result = null;

            var targetIndex = bytesRead > 0 ? bytesRead : 0;
            targetIndex++;

            var nextElementType = (ElementType)bytes[targetIndex];
            if (nextElementType != ElementType.Void)
            {
                result = CreateTypePointerSignature(bytes, targetIndex);
            }
            else
            {
                result = new VoidPointerSignature();
            }

            foreach (var mod in mods)
            {
                result.CustomMods.Add(mod);
            }

            return result;
        }

        private PointerSignature CreateTypePointerSignature(IList<byte> bytes, int targetIndex)
        {
            PointerSignature result;
            var typePointerSignature = new TypePointerSignature();

            var attachedSignatureSize = bytes.Count - targetIndex;
            var remainingBytes = new byte[attachedSignatureSize];
            var currentIndex = 0;
            for (var i = targetIndex; i <= attachedSignatureSize; i++)
            {
                remainingBytes[currentIndex++] = bytes[i];
            }

            var attachedSignature = Execute(remainingBytes);
            typePointerSignature.TypeSignature = attachedSignature;
            result = typePointerSignature;
            return result;
        }       

        private TypeSignature CreateTypeDefOrRefEncodedSignature(IList<byte> bytes)
        {
            var decodedToken = _typeDefOrRefEncodedReader.Execute(bytes[1]);
            var encodedSignature = new TypeDefOrRefEncodedSignature
                                       {
                                           TableId = decodedToken.Item1,
                                           TableIndex = decodedToken.Item2
                                       };

            return encodedSignature;
        }
    }
}