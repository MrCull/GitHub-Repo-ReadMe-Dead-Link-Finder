using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebsiteLinksChecker;

namespace WebsiteLinksCheckerTest
{

    public class LinkCheckerTest
    {

        [Test]
        public async Task CheckLinks_TwoLinksInPageForUri_OneOkTheOtherBadAsync()
        {
            // arrange
            Uri mainPageUri = new("https://github.com/MrCull/GitHub-Repo-ReadMe-Dead-Link-Finder");

            string okLink = "https://ok.com/";
            string notFoundLink = "https://notfound.com/";

            Mock<HttpMessageHandler> mockHttpMessageHandler = new();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(hrh => hrh.RequestUri.OriginalString == okLink), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("some text.."),
                    RequestMessage = new HttpRequestMessage(HttpMethod.Get, okLink)
                });


            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(hrh => hrh.RequestUri.OriginalString == notFoundLink), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    RequestMessage = new HttpRequestMessage(HttpMethod.Get, notFoundLink)
                });

            string branch = "master";
            string fakeRealLifeMarkdoenContent = "some markdown";

            HttpClient httpClient = new(mockHttpMessageHandler.Object);

            Mock<ILinkGetter> linkGetterMock = new();
            linkGetterMock.Setup(lg => lg.ExtractUrlsAsync(mainPageUri.AbsoluteUri, branch, fakeRealLifeMarkdoenContent))
                .ReturnsAsync([okLink, notFoundLink]);

            LinkChecker linkChecker = new(httpClient, linkGetterMock.Object);

            // act
            Dictionary<string, HttpResponseMessage> results = await linkChecker.CheckLinks(mainPageUri.AbsoluteUri, branch, fakeRealLifeMarkdoenContent);

            // assert
            Assert.That(results, Is.Not.Null.Or.Empty);
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[okLink].StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(results[notFoundLink].StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}