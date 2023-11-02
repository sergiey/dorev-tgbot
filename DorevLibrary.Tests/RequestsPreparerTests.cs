using DorevLibrary;

namespace DorevTests;

public class RequestsPreparerTests
{
    [Theory]
    [InlineData ("дѣло", "дело")]
    [InlineData ("домъ", "дом")]
    [InlineData ("сосѣдскій", "соседский")]
    [InlineData ("дерево", "дерево")]
    public void ReplaceUnusedCharacters(string origin, string expectedResult)
    {
        // Arrange

        // Act
        var actualResult = RequestPreparer.GetNormalizedWord(origin);

        // Assert
        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData ("действующий", "действу")]
    [InlineData ("деревянный", "деревя")]
    public void SheinkWord(string origin, string expectedResult)
    {
        // Arrange

        // Act
        var actualResult = RequestPreparer.ShrinkWord(origin);

        // Assert
        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData ("слово", "\\Aслово")]
    [InlineData ("твёрдо", "\\Aтвёрдо")]
    public void AnchorToBeginningOfWord (string origin, string expectedResult)
    {
        // Arrange

        // Act
        var actualResult = RequestPreparer.GetBeginStringMatchRegexp(origin);

        // Assert
        Assert.Equal(expectedResult, actualResult);
    }
}
