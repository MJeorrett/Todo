using Todo.Domain.Extensions;
using Xunit;

namespace Todo.Domain.UnitTests.Extensions;

public class EnumExtensionsTests
{
    public enum TestCase
    {
        A,
        a,
        Single,
        single,
        DoubleWord,
        doubleWord,
    }

    [Theory]
    [InlineData(TestCase.a)]
    [InlineData(TestCase.A)]
    public void ShouldReturnSingleLetterUpperCase(TestCase testCase)
    {
        var actual = testCase.GetUserFriendlyName();

        Assert.Equal("A", actual);
    }


    [Theory]
    [InlineData(TestCase.single)]
    [InlineData(TestCase.Single)]
    public void ShouldReturnSingleWordUpperCase(TestCase testCase)
    {
        var actual = testCase.GetUserFriendlyName();

        Assert.Equal("Single", actual);
    }

    [Theory]
    [InlineData(TestCase.doubleWord)]
    [InlineData(TestCase.DoubleWord)]
    public void ShouldReturnDoubleWordSentanceCase(TestCase testCase)
    {
        var actual = testCase.GetUserFriendlyName();

        Assert.Equal("Double word", actual);
    }
}
