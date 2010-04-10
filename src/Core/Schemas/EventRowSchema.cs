﻿using System;
using System.Collections.Generic;
using System.Text;
using Tao.Interfaces;

namespace Tao.Core.Schemas
{
    /// <summary>
    /// Represents the row schema for the Event table.
    /// </summary>
    public class EventRowSchema : BaseRowSchema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventRowSchema"/> class.
        /// </summary>
        public EventRowSchema() : base(0, 1, 0, 1, 0, 0)
        {
        }

        /// <summary>
        /// Gets the value indicating the list of table indexes and the number of indexes that point to each table type.
        /// </summary>
        /// <value>The number of table indexes.</value>
        public override IEnumerable<ITuple<IEnumerable<TableId>, int>> Item7
        {
            get
            {
                yield return CreateTuple(1, TableId.TypeDef, TableId.TypeRef, TableId.TypeSpec);
            }
        }
    }
}