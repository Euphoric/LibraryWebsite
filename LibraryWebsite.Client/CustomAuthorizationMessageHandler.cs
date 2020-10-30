using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace LibraryWebsite.Client
{
    /// <summary>
    /// Custom HttpMessageHandler based on <see cref="AuthorizationMessageHandler"/>, that works even without token.
    /// </summary>
    public class CustomAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider _provider;
        private AccessToken? _lastToken;
        private AuthenticationHeaderValue? _cachedHeader;
        private Uri[]? _authorizedUris;
        private AccessTokenRequestOptions? _tokenOptions;

        /// <summary>
        /// Initializes a new instance of <see cref="CustomAuthorizationMessageHandler"/>.
        /// </summary>
        /// <param name="provider">The <see cref="IAccessTokenProvider"/> to use for provisioning tokens.</param>
        public CustomAuthorizationMessageHandler(IAccessTokenProvider provider)
        {
            _provider = provider;
        }

        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.Now;
            if (_authorizedUris == null)
            {
                throw new InvalidOperationException($"The '{nameof(CustomAuthorizationMessageHandler)}' is not configured. " +
                                                    $"Call '{nameof(CustomAuthorizationMessageHandler.ConfigureHandler)}' and provide a list of endpoint urls to attach the token to.");
            }

            if (_authorizedUris.Any(uri => uri.IsBaseOf(request.RequestUri)))
            {
                if (_lastToken == null || now >= _lastToken.Expires.AddMinutes(-5))
                {
                    var tokenResult = _tokenOptions != null ?
                        await _provider.RequestAccessToken(_tokenOptions) :
                        await _provider.RequestAccessToken();

                    if (tokenResult.TryGetToken(out var token))
                    {
                        _lastToken = token;
                        _cachedHeader = new AuthenticationHeaderValue("Bearer", _lastToken.Value);
                    }
                    else
                    {
                        _lastToken = null;
                        _cachedHeader = null;
                    }
                }

                if (_cachedHeader != null)
                {
                    // We don't try to handle 401s and retry the request with a new token automatically since that would mean we need to copy the request
                    // headers and buffer the body and we expect that the user instead handles the 401s. (Also, we can't really handle all 401s as we might
                    // not be able to provision a token without user interaction).
                    request.Headers.Authorization = _cachedHeader;
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }

        public CustomAuthorizationMessageHandler ConfigureHandler(
            IEnumerable<string> authorizedUrls,
            IEnumerable<string> scopes = null,
            string returnUrl = null)
        {
            if (_authorizedUris != null)
            {
                throw new InvalidOperationException("Handler already configured.");
            }

            if (authorizedUrls == null)
            {
                throw new ArgumentNullException(nameof(authorizedUrls));
            }

            var uris = authorizedUrls.Select(uri => new Uri(uri, UriKind.Absolute)).ToArray();
            if (uris.Length == 0)
            {
                throw new ArgumentException("At least one URL must be configured.", nameof(authorizedUrls));
            }

            _authorizedUris = uris;
            var scopesList = scopes?.ToArray();
            if (scopesList != null || returnUrl != null)
            {
                _tokenOptions = new AccessTokenRequestOptions
                {
                    Scopes = scopesList,
                    ReturnUrl = returnUrl
                };
            }

            return this;
        }
    }
}