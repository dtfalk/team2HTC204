using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class LoggingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Log the request URL
        Console.WriteLine($"Request URL: {request.RequestUri}");

        // Log the request method
        Console.WriteLine($"Request Method: {request.Method}");

        // Log the request headers
        foreach (var header in request.Headers)
        {
            Console.WriteLine($"Request Header: {header.Key} = {string.Join(", ", header.Value)}");
        }

        if (request.Content != null)
        {
            var requestContent = await request.Content.ReadAsStringAsync();
            Console.WriteLine($"Request Content: {requestContent}");
        }

        var response = await base.SendAsync(request, cancellationToken);

        // Log the response status code
        Console.WriteLine($"Response Status Code: {response.StatusCode}");

        // Log the response headers
        foreach (var header in response.Headers)
        {
            Console.WriteLine($"Response Header: {header.Key} = {string.Join(", ", header.Value)}");
        }

        if (response.Content != null)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {responseContent}");
        }

        return response;
    }
}