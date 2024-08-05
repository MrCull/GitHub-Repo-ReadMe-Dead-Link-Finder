using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LinksChecker;

public class ReadMeMarkdownGetter(HttpClient httpClient) : IMarkdownGetter
{
    public async Task<string> GetReadMeMarkDownAsync(string rawProjectBaseUrlWithBranch)
    {
        string readMeMarkDown = "";
        try
        {
            string url = $"{rawProjectBaseUrlWithBranch}/README.md";
            readMeMarkDown = await httpClient.GetStringAsync(url);
        }
        catch (HttpRequestException hre)
        {
            Console.WriteLine($"HttpRequestException Error for {rawProjectBaseUrlWithBranch}");
            Console.WriteLine(hre);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error for {rawProjectBaseUrlWithBranch}");
            Console.WriteLine(ex);
            throw;
        }

        return readMeMarkDown;
    }
}
