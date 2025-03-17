
namespace BlazorAppIdentityDotNet.HttpHandlers
{
    public class CustomHeaderHandler : DelegatingHandler
    {
        public CustomHeaderHandler() : base(new HttpClientHandler())
        {
        }
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Custom-Header", "My custom header value");
            return base.SendAsync(request, cancellationToken);
        }
    }
}
