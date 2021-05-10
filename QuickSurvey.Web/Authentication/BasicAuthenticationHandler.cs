using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuickSurvey.Core.SessionAggregate;

namespace QuickSurvey.Web.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ILogger<BasicAuthenticationHandler> _logger;
        private readonly ISessionRepository _repository;
        private readonly BasicObfuscator _obfuscator;

        private const string DefaultAuthenticationFailureMessage = "Invalid Credentials";

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ISessionRepository repository, IHttpContextAccessor httpContextAccessor, ILogger<BasicAuthenticationHandler> logger1, BasicObfuscator obfuscator) : base(options, logger, encoder, clock)
        {
            _repository = repository;
            _logger = logger1;
            _obfuscator = obfuscator;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // skip authentication if endpoint has [AllowAnonymous] attribute
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return AuthenticateResult.NoResult();

            int sessionId;
            string username;
            try
            {
                (sessionId, username) = ExtractIdentity();
            }
            catch (AuthenticationException e)
            {
                return AuthenticateResult.Fail(e.Message);
            }

            if (Request.RouteValues["sessionId"] != null)
            {
                var currentSessionId = int.Parse(Request.RouteValues["sessionId"] as string ?? string.Empty);
                if (currentSessionId != sessionId)
                {
                    return AuthenticateResult.Fail(DefaultAuthenticationFailureMessage);
                }
            }
            
            var session = await _repository.GetAsync(sessionId);
            if (session == null)
            {
                _logger.LogInformation($"No session with Id {sessionId} found");
                return AuthenticateResult.NoResult();
            }

            if (session.Participants.All(p => p.Username != username))
            {
                _logger.LogInformation($"session {sessionId} does not contain username {username}");
                return AuthenticateResult.Fail(DefaultAuthenticationFailureMessage);
            }

            Response.Headers.Add("WWW-Authenticate", Scheme.Name);
            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.Name, username),
                new("SessionId", sessionId.ToString()),
                new (ClaimTypes.NameIdentifier, username)
            }, Scheme.Name);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);
            _logger.LogInformation($"Successfully authenticated user {username} for sessionId {sessionId}");

            return AuthenticateResult.Success(ticket);
        }

        private (int sessionId, string username) ExtractIdentity()
        {
            
            if (Request.Query["access_token"].ToString() == null)
            {
                throw new AuthenticationException("No sessionId found");
            }

            int sessionId;
            string username;

            try
            {
                (sessionId, username) = _obfuscator.DeObfuscate(Request.Query["access_token"].ToString());
            }
            catch (FormatException e)
            {
                _logger.LogDebug(e.Message);
                throw new AuthenticationException("No sessionId found");
            }
            
            return (sessionId, username);
        }
    }
}