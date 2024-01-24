using System;
#if !NET8_0_OR_GREATER
using System.Runtime.Serialization;
#endif

namespace StrictCSV;

#if !NET8_0_OR_GREATER
[Serializable]
#endif
public sealed class StrictCsvException : Exception
{
    public StrictCsvException()
    {
    }

    public StrictCsvException(string? message) : base(message)
    {
    }

    public StrictCsvException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

#if !NET8_0_OR_GREATER
    private StrictCsvException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
#endif
}
