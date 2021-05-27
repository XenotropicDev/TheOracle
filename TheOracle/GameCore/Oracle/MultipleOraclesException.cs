using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TheOracle.GameCore.Oracle
{
    [Serializable]
    public class MultipleOraclesException : Exception
    {
        public IEnumerable<OracleTable> Tables;

        public MultipleOraclesException()
        {
        }

        public MultipleOraclesException(IEnumerable<OracleTable> tables)
        {
            this.Tables = tables;
        }

        public MultipleOraclesException(string message) : base(message)
        {
        }

        public MultipleOraclesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MultipleOraclesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}