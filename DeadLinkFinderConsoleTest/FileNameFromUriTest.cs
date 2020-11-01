using DeadLinkFinderConsole;
using NUnit.Framework;
using System;

namespace DeadLinkFinderConsoleTest
{
    public class FileNameFromUriTest
    {
        [TestCase("https://www.google.com/", "www.google.com")]
        [TestCase("www.google.com/", "www.google.com")]
        [TestCase("www.google.com", "www.google.com")]
        [TestCase("www.google.com/app.exe", "www.google.com_app.exe")]
        [TestCase("https://www.google.com/webhp?hl=en&sa=X&ved=0ahUKEwjT4ZnS3dPsAhV6D2MBHeqoAA8QPAgI", "www.google.com_webhp_hl_en_sa_X_ved_0ahUKEwjT4ZnS3dPsAhV6D2MBHeqoAA8QPAgI")]
        public void ConvertToWindowsFileName_UilString_FileNameIsCorrect(Uri uri, string expectedFileName)
        {
            // arrange
            var sut = new FileNameFromUri();

            // act
            string actualFilename = sut.ConvertToWindowsFileName(uri);

            // assert
            Assert.AreEqual(expectedFileName, actualFilename);
        }
    }
}
