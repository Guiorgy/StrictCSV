using System.Text;

namespace StrictCSV;

public enum ScsvEncoding
{
    UTF8 = 0,
    UTF8WithBom = 1,
    UTF16BigEndian = 2,
    UTF16LittleEndian = 3,
    UTF32BigEndian = 4,
    UTF32LittleEndian = 5
}

internal static class Encodings
{
    public static readonly UTF8Encoding UTF8 = new(false);
    public static readonly UTF8Encoding UTF8WithBom = new(true);
    public static readonly UnicodeEncoding UTF16BigEndian = new(true, true);
    public static readonly UnicodeEncoding UTF16LittleEndian = new(false, true);
    public static readonly UTF32Encoding UTF32BigEndian = new(true, true);
    public static readonly UTF32Encoding UTF32LittleEndian = new(false, true);

    public static Encoding GetEncoding(this ScsvEncoding scsvEncoding)
    {
        return scsvEncoding switch
        {
            ScsvEncoding.UTF8 => UTF8,
            ScsvEncoding.UTF8WithBom => UTF8WithBom,
            ScsvEncoding.UTF16BigEndian => UTF16BigEndian,
            ScsvEncoding.UTF16LittleEndian => UTF16LittleEndian,
            ScsvEncoding.UTF32BigEndian => UTF32BigEndian,
            ScsvEncoding.UTF32LittleEndian => UTF32LittleEndian,
            _ => throw new
#if NET7_0_OR_GREATER
            System.Diagnostics.UnreachableException
#else
            System.NotImplementedException
#endif
            ($"The value {scsvEncoding} of type {nameof(ScsvEncoding)} not handled.")
        };
    }
}
