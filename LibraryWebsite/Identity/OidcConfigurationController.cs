using System;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;

namespace LibraryWebsite.Identity
{
    public class OidcConfigurationController : Controller
    {
        private readonly IClientRequestParametersProvider _clientRequestParametersProvider;

        public OidcConfigurationController(IClientRequestParametersProvider clientRequestParametersProvider)
        {
            _clientRequestParametersProvider = clientRequestParametersProvider;
        }

        /// <summary>
        /// Provides OICD configuration to clients so they can authenticate against identity server.
        /// </summary>
        [HttpGet("_configuration/{clientId}")]
        public IActionResult GetClientRequestParameters([FromRoute]string clientId)
        {
            var parameters = _clientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
            return Ok(parameters);
        }
    }
}
