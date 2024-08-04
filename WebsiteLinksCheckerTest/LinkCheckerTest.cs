using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebsiteLinksChecker;

namespace WebsiteLinksCheckerTest
{

    public class LinkCheckerTest
    {

        [Test]
        [TestCase(null)]
        [TestCase("readme")]
        public void CheckLinksWithResultsAsync_TwoLinksInPageForUri_OneOkTheOtherBad(string elementId)
        {
            // arrange
            Uri mainPageUri = new("https://dummy.com/");

            string okLink = "https://ok.com/";
            string notFoundLink = "https://notfound.com/";

            Mock<HttpMessageHandler> mockHttpMessageHandler = new();

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(hrh => hrh.RequestUri.OriginalString == mainPageUri.ToString()), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = GetHtmlWithTwoLinksInside(elementId, okLink, notFoundLink)
                });


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

            HttpClient httpClient = new(mockHttpMessageHandler.Object);

            LinkChecker linkChecker = new(httpClient, new LinkGetter(httpClient, elementId));

            // act
            Dictionary<string, HttpResponseMessage> results = linkChecker.CheckLinks(mainPageUri);

            // assert
            Assert.That(results, Is.Not.Null.Or.Empty);
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[okLink].StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(results[notFoundLink].StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }


        private HttpContent GetHtmlWithTwoLinksInside(string elementId, string okLink, string deadLink)
        {
            StringBuilder html = new();
            html.Append("<html>");
            html.Append($"<div id='{elementId}'><div><div>");
            html.Append($"<a href='{okLink}'>ok</a>");
            html.Append($"<a href='{deadLink}'>dead</a>");
            html.Append("</div></div></div>");

            html.Append("</html>");

            return new StringContent(html.ToString());
        }
    }
}