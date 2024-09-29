using Beary.Articles.FileSystem.Extensions;

namespace Beary.Articles.FileSystem.Test;

public class StringExtensions_Clean_Should
{
    [Theory]
    [InlineData("", "")]
    [InlineData(null, null)]
    [InlineData("None of this data should change", "None of this data should change")]
    [InlineData("please {ContactPageLink:contact me}.", "please contact me.")]
    [InlineData("or {PageLink:Speaking-Engagements|any of my talks}, at your User Group", "or any of my talks, at your User Group")]
    [InlineData("was {FileLink:MSDNMag-WebAppFollies-200607.pdf|Web App Follies} from the", "was Web App Follies from the")]
    [InlineData("My talks: {PageLink:Speaking-Engagements|talks list}, Magazine Article: {FileLink:MSDNMag-WebAppFollies-200607.pdf|Web App Follies}, Other Stuff: dude", "My talks: talks list, Magazine Article: Web App Follies, Other Stuff: dude")]
    public void ReturnTheCleanedResult(string testData, string expected)
    {
        var actual = testData.Clean();
        Assert.Equal(expected, actual);
    }
}