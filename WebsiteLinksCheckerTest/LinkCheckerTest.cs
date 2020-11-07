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
            var mainPageUri = new Uri("https://dummy.com/");

            string okLink = "https://ok.com/";
            string notFoundLink = "https://notfound.com/";

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

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

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);

            var linkChecker = new LinkChecker(httpClient, elementId, new LinkGetter());

            // act
            Dictionary<string, HttpResponseMessage> results = linkChecker.CheckLinksAsync(mainPageUri).Result;

            // assert
            CollectionAssert.IsNotEmpty(results);
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[okLink].StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(results[notFoundLink].StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }


        private HttpContent GetHtmlWithTwoLinksInside(string elementId, string okLink, string deadLink)
        {
            var html = new StringBuilder();
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