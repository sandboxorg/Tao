﻿using System;
using System.Collections.Generic;
using System.Text;
using Tao.Interfaces;

namespace Tao.Core
{
    /// <summary>
    /// Represents a class that determines the size of a single metadata table row from the given <see cref="TableId"/>
    /// and tuple containing the size of each metadata stream heap index.
    /// </summary>
    public class CalculateMetadataTableRowSize : IFunction<ITuple<TableId, ITuple<int, int, int>>, int>
    {
        private readonly IFunction<TableId, ITuple<int, int, int, int, int>> _getMetadataTableSizeCounts;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculateMetadataTableRowSize"/> class.
        /// </summary>
        public CalculateMetadataTableRowSize(IFunction<TableId, ITuple<int, int, int, int, int>> getMetadataTableSizeCounts)
        {
            _getMetadataTableSizeCounts = getMetadataTableSizeCounts;
        }

        /// <summary>
        /// Represents a class that determines the size of a single metadata table row from the given <see cref="TableId"/>
        /// and tuple containing the size of each metadata stream heap index.
        /// </summary>
        /// <param name="input">The tuple containing the <see cref="TableId"/> and the size of each metadata stream heap index.</param>
        /// <returns></returns>
        public int Execute(ITuple<TableId, ITuple<int, int, int>> input)
        {
            var tableId = input.Item1;
            var heapSizes = input.Item2;

            var columnSizeCounts = _getMetadataTableSizeCounts.Execute(tableId);

            var stringIndexSize = heapSizes.Item1;
            var blobIndexSize = heapSizes.Item2;
            var guidIndexSize = heapSizes.Item3;

            var wordColumns = columnSizeCounts.Item1;
            var dwordColumns = columnSizeCounts.Item2;
            var stringColumns = columnSizeCounts.Item3;
            var blobColumns = columnSizeCounts.Item4;
            var guidColumns = columnSizeCounts.Item5;

            var result = wordColumns*2 + dwordColumns*4 + stringColumns*stringIndexSize + blobColumns*blobIndexSize +
                         guidColumns*guidIndexSize;

            return result;
        }
    }
}