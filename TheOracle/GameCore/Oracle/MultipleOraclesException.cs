using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TheOracle.GameCore.Oracle
{
    [Serializable]
    public class MultipleOraclesException : Exception
    {
        public MultipleOraclesException()
        {
        }

        public MultipleOraclesException(object tables)
        {
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