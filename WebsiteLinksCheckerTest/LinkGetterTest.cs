using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebsiteLinksChecker;

namespace WebsiteLinksCheckerTest;

[TestFixture]
public class LinkGetterTest
{
    private const string projectBaseUrl = "https://github.com/user/project/";
    private const string branch = "main";
    private readonly ILinkGetter linkGetter = new LinkGetter();

    [Test]
    public async Task ExtractUrlsAsyncTest_OneLink_LinkReturned()
    {
        // Arrange        
        const string markdownContent = "This is a test content with a link to [Google](https://www.google.com)";

        // Act
        List<string> result = await linkGetter.ExtractUrlsAsync(projectBaseUrl, branch, markdownContent);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First(), Is.EqualTo("https://www.google.com"));
    }

    [Test]
    public async Task ExtractUrlsAsyncTest_TwoLinks_LinkReturned()
    {
        // Arrange
        string markdownContent = "This is a test content with links to [Google](https://www.google.com) and [GitHub](https://github.com)";

        // Act
        List<string> result = await linkGetter.ExtractUrlsAsync(projectBaseUrl, branch, markdownContent);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result, Contains.Item("https://www.google.com"));
        Assert.That(result, Contains.Item("https://github.com"));
    }

    [Test]
    public async Task ExtractUrlsAsyncTest_NoLink_EmptyListReturned()
    {
        // Arrange
        string markdownContent = "This is a test content without any links";

        // Act
        List<string> result = await linkGetter.ExtractUrlsAsync(projectBaseUrl, branch, markdownContent);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task ExtractUrlsAsyncTest_RelativeLink_CorrectLinkReturned()
    {
        // Arrange
        string markdownContent = "This is a test content with a relative link to [APITesterApp](/APITesterApp)";

        // Act
        List<string> result = await linkGetter.ExtractUrlsAsync(projectBaseUrl, branch, markdownContent);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First(), Is.EqualTo("https://github.com/user/project/blob/main/APITesterApp"));
    }

    [Test]
    public async Task ExtractUrlsAsyncTest_MultipleRelativeLinks_CorrectLinksReturned()
    {
        // Arrange
        string markdownContent = "This is a test content with relative links to [APITesterApp](/APITesterApp) and [TestApp](/TestApp)";

        // Act
        List<string> result = await linkGetter.ExtractUrlsAsync(projectBaseUrl, branch, markdownContent);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result, Contains.Item("https://github.com/user/project/blob/main/APITesterApp"));
        Assert.That(result, Contains.Item("https://github.com/user/project/blob/main/TestApp"));
    }

    [Test]
    public async Task ExtractUrlsAsyncTest_RelativeLinkWithoutForwardSlash_CorrectLinkReturned()
    {
        // Arrange
        string markdownContent = "This is a test content with a relative link to [APITesterApp](APITesterApp)";

        // Act
        List<string> result = await linkGetter.ExtractUrlsAsync(projectBaseUrl, branch, markdownContent);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result.First(), Is.EqualTo("https://github.com/user/project/blob/main/APITesterApp"));
    }

    [Test]
    public async Task ExtractUrlsAsyncTest_MultipleRelativeAndAbsoluteLinks_CorrectLinksReturned()
    {
        // Arrange
        string markdownContent = "This is a test content with relative and absolute links to [APITesterApp](/APITesterApp) and [GitHub](https://github.com)";

        // Act
        List<string> result = await linkGetter.ExtractUrlsAsync(projectBaseUrl, branch, markdownContent);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result, Contains.Item("https://github.com/user/project/blob/main/APITesterApp"));
        Assert.That(result, Contains.Item("https://github.com"));
    }
}
