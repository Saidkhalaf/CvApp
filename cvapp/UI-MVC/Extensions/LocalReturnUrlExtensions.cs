using Microsoft.AspNetCore.WebUtilities;

namespace cvapp.ui.web.mvc.Extensions;

public static class LocalReturnUrlExtensions
{
    public static string WithQueryValues(this string localReturnUrl, params (string Key, string Value)[] values)
    {
        if (string.IsNullOrWhiteSpace(localReturnUrl))
        {
            return string.Empty;
        }

        // Extract hash fragment
        var hashIndex = localReturnUrl.IndexOf('#');
        var hash = hashIndex >= 0 ? localReturnUrl[hashIndex..] : string.Empty;
        var urlWithoutHash = hashIndex >= 0 ? localReturnUrl[..hashIndex] : localReturnUrl;

        // Parse path and query string
        var queryIndex = urlWithoutHash.IndexOf('?');
        var path = queryIndex >= 0 ? urlWithoutHash[..queryIndex] : urlWithoutHash;
        var queryString = queryIndex >= 0 ? urlWithoutHash[queryIndex..] : string.Empty;

        // Build query parameters
        var queryParams = QueryHelpers.ParseQuery(queryString)
            .ToDictionary(pair => pair.Key, pair => pair.Value.ToString(), StringComparer.OrdinalIgnoreCase);

        foreach (var (key, value) in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                queryParams[key] = value;
            }
        }

        // Reconstruct URL
        var targetUrl = queryParams.Count > 0 ? QueryHelpers.AddQueryString(path, queryParams) : path;
        return $"{targetUrl}{hash}";
    }
}