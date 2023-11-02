using DorevLibrary;

namespace DorevTests;

public class VocabularyTests
{
    [Fact]
    public void CreateNewInstanceOfVocabulary()
    {
        // Arrange
        var connectionString = $"Data Source=dbPath";

        // Act
        var vocab = new Vocabulary(connectionString);

        // Assert
        Assert.NotNull(vocab);
    }

    [Fact]
    public void FindWordInVocabulary()
    {
        // Arrange
        var dirLocation = Path.GetDirectoryName(System.Reflection.Assembly.
                GetExecutingAssembly().Location)!;
        var connectionString = 
            "Data Source=" + dirLocation.Split("DorevLibrary.Tests")[0] +
            "DorevLibrary/Resources/dorev.db";
        var vocab = new Vocabulary(connectionString);

        // Act
        var actualString = vocab.Translate("речка");

        // Assert
        Assert.Contains("рѣ́чка", actualString);
    }
}
