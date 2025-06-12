using NUnit.Framework;
using Util;

public class RangeTests
{
    [Test]
    public void Inside_ReturnsTrue_WhenValueWithinRange()
    {
        var range = new Range<int>(0, 10);
        Assert.IsTrue(range.Inside(5));
    }

    [Test]
    public void Inside_ReturnsFalse_WhenValueOutOfRange()
    {
        var range = new Range<int>(0, 10);
        Assert.IsFalse(range.Inside(11));
    }

    [Test]
    public void SqrInside_ReturnsTrue_WhenSquareWithinRange()
    {
        var range = new Range<int>(0, 3);
        Assert.IsTrue(range.SqrInside(4));
    }

    [Test]
    public void SqrInside_ReturnsFalse_WhenSquareOutOfRange()
    {
        var range = new Range<int>(0, 3);
        Assert.IsFalse(range.SqrInside(10));
    }
}
