using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tests.IntegrationTests.config
{
    public static class AntiForgeryHelper
    {
        public static async Task<string> GetAntiForgeryToken(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var match = Regex.Match(content, @"<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" />");
            return match.Success ? match.Groups[1].Value : null;
        }
    }
}

