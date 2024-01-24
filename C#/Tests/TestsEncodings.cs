using StrictCSV;

namespace Tests;

[TestClass]
public sealed class TestsEncodings
{
    private static void AssertDeserialized(string?[] header, string?[,] values)
    {
        Assert.AreEqual(2, header.Length);
        Assert.AreEqual("Value Type", header[0]);
        Assert.AreEqual("Example", header[1]);

        Assert.AreEqual(5 * 2, values.Length);
        Assert.AreEqual(5, values.GetLength(0));
        Assert.AreEqual(2, values.GetLength(1));
        Assert.AreEqual("Normal", values[0, 0]);
        Assert.AreEqual("something or another", values[0, 1]);
        Assert.AreEqual("Contains Double Quotes", values[1, 0]);
        Assert.AreEqual("\"The way to get started is to quit talking and begin doing\" -Walt Disney", values[1, 1]);
        Assert.AreEqual("Multiline", values[2, 0]);
        Assert.AreEqual(string.Format("first line{0}second line{0}third line", Environment.NewLine), values[2, 1]);
        Assert.AreEqual("Empty String", values[3, 0]);
        Assert.AreEqual("", values[3, 1]);
        Assert.AreEqual("Null", values[4, 0]);
        Assert.AreEqual(null, values[4, 1]);
    }

    private static void TestsFile(string path, ScsvEncoding encoding)
    {
        var (header, values) = StrictCsvSimple.DeserializeFile(path);
        AssertDeserialized(header, values);

        var expected = File.ReadAllBytes(path);
        var actual = StrictCsvSimple.Serialize(header, values, encoding);
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Utf8()
    {
        TestsFile(@"Samples\Encodings\utf8.scsv", ScsvEncoding.UTF8);
    }

    [TestMethod]
    public void Utf8WithBom()
    {
        TestsFile(@"Samples\Encodings\utf8-withbom.scsv", ScsvEncoding.UTF8WithBom);
    }

    [TestMethod]
    public void Utf16BigEndian()
    {
        TestsFile(@"Samples\Encodings\utf16-big.scsv", ScsvEncoding.UTF16BigEndian);
    }

    [TestMethod]
    public void Utf16LittleEndian()
    {
        TestsFile(@"Samples\Encodings\utf16-little.scsv", ScsvEncoding.UTF16LittleEndian);
    }

    [TestMethod]
    public void Utf32BigEndian()
    {
        TestsFile(@"Samples\Encodings\utf32-big.scsv", ScsvEncoding.UTF32BigEndian);
    }

    [TestMethod]
    public void Utf32LittleEndian()
    {
        TestsFile(@"Samples\Encodings\utf32-little.scsv", ScsvEncoding.UTF32LittleEndian);
    }
}