using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace StrictCSV;

/// <summary>
/// A simple sample implementation of the Strict CSV format specifications version 0.1.0
/// </summary>
public static class StrictCsvSimple
{
    private const string S2368_Justification = "This is only a sample implementation of the Strict CSV format specifications, so multidimensional arrays are fine.";

    /// <summary>
    /// Deserializes a <strong>Strict CSV</strong> formatted file.
    /// </summary>
    /// <param name="scsvFilePath">Path to the <strong>Strict CSV</strong> formatted file.</param>
    /// <returns>The deserialized header and values arrays.</returns>
    /// <exception cref="StrictCsvException">If parsing failed due to a formatting error.</exception>
    public static (string?[] Header, string?[,] Values) DeserializeFile(string scsvFilePath)
    {
        using StreamReader reader = new(scsvFilePath, Encodings.UTF8);

        string scsv = reader.ReadToEnd();

        if (!reader.CurrentEncoding.Equals(Encodings.UTF8) && !reader.CurrentEncoding.Equals(Encodings.UTF8WithBom)
            && !reader.CurrentEncoding.Equals(Encodings.UTF16BigEndian) && !reader.CurrentEncoding.Equals(Encodings.UTF16LittleEndian)
            && !reader.CurrentEncoding.Equals(Encodings.UTF32BigEndian) && !reader.CurrentEncoding.Equals(Encodings.UTF32LittleEndian))
        {
            throw new StrictCsvException("Only UTF-8, UTF-16 (both big and little endian) and UTF-32 (both big and little endian) are supported. UTF-8 is not required to have a BOM.");
        }

        return Deserialize(scsv);
    }

    private static string?[] DeserializeRow(string row)
    {
        static int CountDoubleQuotesFromEnd(string source, bool skipFirst)
        {
            int length = skipFirst ? source.Length : source.Length + 1;

            for (int i = 1; i < length; i++)
                if (source[^i] != '"') return i - 1;

            return length - 1;
        }

        static string? Unescape(string? source)
            => source?.Replace("\"\"", "\"") // two consecutive double quotes represent a single escaped double quote
                .Replace("\r", Environment.NewLine); // lines in multiline values are separated by CR

        var splits = row.Split(','); // values are separated by a comma

        List<string?> values = new(splits.Length);

        List<string?> merged = new(splits.Length);
        foreach (var potentialValue in splits)
        {
            if (merged.Count == 0)
            {
                if (potentialValue.Length == 0) // empty values represent null
                {
                    values.Add(null);
                    continue;
                }

                if (potentialValue.Length == 1) throw new StrictCsvException("Only compact format is supported.");
                if (potentialValue[0] != '"') throw new StrictCsvException("Non-null values must be surrounded by double quotes.");

                if (CountDoubleQuotesFromEnd(potentialValue, true) % 2 == 1) // two consecutive double quotes represent a single escaped double quote, so if there's an odd number of quotes at the end, we have an unescaped quote, i.e. closing quote
                {
                    values.Add(potentialValue[1..^1]);
                    continue;
                }

                merged.Add(potentialValue); // this comma is part of the value
                continue;
            }

            merged.Add(potentialValue);

            if (CountDoubleQuotesFromEnd(potentialValue, false) % 2 == 1) // two consecutive double quotes represent a single escaped double quote, so if there's an odd number of quotes at the end, we have an unescaped quote, i.e. closing quote
            {
                values.Add(string.Concat(merged)[1..^1]); // values are surrounded by double quotes
                merged.Clear();
            }
        }

        if (values.Count == 0) throw new StrictCsvException("Only compact format is supported.");

        return values.Select(Unescape).ToArray();
    }

    /// <summary>
    /// Deserializes a <strong>Strict CSV</strong> formatted string.
    /// </summary>
    /// <param name="scsv"><strong>Strict CSV</strong> formatted string.</param>
    /// <returns>The deserialized header and values arrays.</returns>
    /// <exception cref="StrictCsvException">If parsing failed due to a formatting error.</exception>
    public static (string?[] Header, string?[,] Values) Deserialize(string scsv)
    {
        static (T First, IEnumerable<T> TheRest) PopFirst<T>(IEnumerable<T> source) => (source.First(), source.Skip(1));

        var rowStrings = scsv.Split('\n'); // rows are separated by LF
        var (header, rows) = PopFirst(rowStrings.Select(DeserializeRow)); // the first row is the header
        var values = new string?[rowStrings.Length - 1, header.Length];

        int i = 0;
        foreach (var row in rows)
        {
            if (row.Length != header.Length) throw new StrictCsvException($"{(i + 1).ToOrdinal()} row length ({row.Length}) didn't match the headers length ({header.Length})."); // all rows have the same number of values as the header

            for (int j = 0; j < header.Length; j++)
                values[i, j] = row[j];

            ++i;
        }

        return (header, values);
    }

    /// <summary>
    /// Serializes header and value arrays into a <strong>Strict CSV</strong> formatted byte array in the specified encoding.
    /// </summary>
    /// <param name="header">The headers.</param>
    /// <param name="values">The values.</param>
    /// <param name="encoding">The desired encoding.</param>
    /// <returns>A <strong>Strict CSV</strong> formatted byte array in the specified encoding.</returns>
    /// <exception cref="StrictCsvException">If <paramref name="header"/> is empty, <paramref name="values"/> is not a two-dimensional array,
    /// or the width of <paramref name="values"/> doesn't match <paramref name="header"/>.</exception>
    [SuppressMessage("Blocker Code Smell", "S2368:Public methods should not have multidimensional array parameters", Justification = S2368_Justification)]
    public static byte[] Serialize(string?[] header, string?[,] values, ScsvEncoding encoding = default)
    {
        if (header.Length == 0) throw new StrictCsvException("The header must be defined.");
        if (values.Rank != 2) throw new StrictCsvException("Values must be a two-dimensional array.");
        if (values.GetLength(1) != header.Length) throw new StrictCsvException("All rows must have the same number of values as the header.");

        var systemEncoding = encoding.GetEncoding();

        static string? Escape(string? source) =>
            source?.Replace("\"", "\"\"") // two consecutive double quotes represent a single escaped double quote
                .Replace(Environment.NewLine, "\r") // lines in multiline values are separated by CR
                .Replace('\n', '\r'); // LF is not supported inside values

        var escapedHeader = header.Select(Escape);
        var escapedValues = values.Select(Escape);

        StringBuilder builder = new();

        int column = 0;
        foreach (var value in escapedHeader.Concat(escapedValues))
        {
            if (value != null) // empty values represent null
                builder.Append('"').Append(value).Append('"'); // Non-null values are surrounded by double quotes
            builder.Append(','); // values are separated by a comma

            if (++column ==  header.Length)
            {
                column = 0;
                builder.Length--; // remove the last appended comma
                builder.Append('\n'); // rows are separated by LF
            }
        }
        builder.Length--; // remove the last appended LF

        return [..systemEncoding.GetPreamble(), ..systemEncoding.GetBytes(builder.ToString())];
    }

    /// <summary>
    /// Serializes header and value arrays int the <strong>Strict CSV</strong> format and writes them into the specified file with the specified encoding.
    /// If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="scsvFilePath">Path to the <strong>Strict CSV</strong> formatted file.</param>
    /// <param name="header">The headers.</param>
    /// <param name="values">The values.</param>
    /// <param name="encoding">The desired encoding.</param>
    /// <exception cref="StrictCsvException">If <paramref name="header"/> is empty, <paramref name="values"/> is not a two-dimensional array,
    /// or the width of <paramref name="values"/> doesn't match <paramref name="header"/>.</exception>
    [SuppressMessage("Blocker Code Smell", "S2368:Public methods should not have multidimensional array parameters", Justification = S2368_Justification)]
    public static void SerializeFile(string scsvFilePath, string?[] header, string?[,] values, ScsvEncoding encoding = default)
    {
        File.WriteAllBytes(scsvFilePath, Serialize(header, values, encoding));
    }
}
