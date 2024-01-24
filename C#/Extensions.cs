using System;
using System.Collections.Generic;

namespace StrictCSV;
internal static class Extensions
{
    /// <summary>
    /// Converts an integer into its ordinal string represantion.
    /// </summary>
    /// <param name="value">The integer to convert.</param>
    /// <returns>The ordinal string representation of the specified integer.</returns>
    /// <exception cref="ArgumentException">If <paramref name="value"/> is negative.</exception>
    public static string ToOrdinal(this int value)
    {
        if (value < 0) throw new ArgumentException("Value must not be negative.", nameof(value));

        return (value % 100) switch
        {
            11 or 12 or 13 => value + "th",
            _ => (value % 10) switch {
                1 => value + "st",
                2 => value + "nd",
                3 => value + "rd",
                _ => value + "th"
            }
        };
    }

    /// <summary>
    /// Enumerate and transform a two-dimensional array.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
    /// <param name="source">A two-dimensional array of values to enumerate and invoke a transform function on.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of enumerating and invoking the transform function on each element of source.</returns>
    /// <exception cref="NotImplementedException">If <paramref name="source"/> is not a two-dimensional array.</exception>
    public static IEnumerable<TResult> Select<TSource, TResult>(this TSource[,] source, Func<TSource, TResult> selector)
    {
        if (source.Rank != 2) throw new NotImplementedException("Only two-dimensional arrays are supported.");

        static IEnumerable<TResult> Enumerator(TSource[,] source, Func<TSource, TResult> selector)
        {
            var length0 = source.GetLength(0);
            var length1 = source.GetLength(1);

            for (int i = 0; i < length0; i++)
            {
                for (int j = 0; j < length1; j++)
                {
                    yield return selector(source[i, j]);
                }
            }
        }

        return Enumerator(source, selector);
    }
}
