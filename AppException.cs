using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HWRG
{
    [Serializable]
    public class AppException : Exception
    {
        public int ErrorCode { get; private set; }

        public AppException()
        {
            ErrorCode = ErrorCodes.DefaultErrorCode;
        }

        public AppException(string message)
            : base(message)
        {
            ErrorCode = ErrorCodes.DefaultErrorCode;
        }

        public AppException(string message, Exception inner)
            : base(message, inner)
        {
            ErrorCode = ErrorCodes.DefaultErrorCode;
        }

        public AppException(int errorCode)
        {
            ErrorCode = errorCode;
        }

        public AppException(string message, int errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        public AppException(string message, Exception inner, int errorCode)
            : base(message, inner)
        {
            ErrorCode = errorCode;
        }

        protected AppException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            this.ErrorCode = info.GetInt32("ErrorCode");
        }
    }
}
