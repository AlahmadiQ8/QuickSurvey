using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuickSurvey.Core.SessionAggregate;

namespace QuickSurvey.Web.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ILogger<BasicAuthenticationHandler> _logger;
        private readonly ISessionRepository _repository;

        private const string DefaultAuthenticationFailureMessage = "Invalid Credentials";

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ISessionRepository repository, IHttpContextAccessor httpContextAccessor, ILogger<BasicAuthenticationHandler> logger1) : base(options, logger, encoder, clock)
        {
            _repository = repository;
            _logger = logger1;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Response.Headers.Add("WWW-Authenticate", Scheme.Name);

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

            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.Name, username),
                new("SessionId", sessionId.ToString())
            }, Scheme.Name);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);
            _logger.LogInformation($"Successfully authenticated user {username} for sessionId {sessionId}");
            return AuthenticateResult.Success(ticket);
        }

        private (int sessionId, string username) ExtractIdentity()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                throw new AuthenticationException("Authorization header missing.");
            }

            var authHeader = Request.Headers["Authorization"].ToString();
            var authHeaderRegex = new Regex(@"Basic (.*)");

            if (!authHeaderRegex.IsMatch(authHeader))
            {
                throw new AuthenticationException("Authorization code not formatted properly.");
            }

            var authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderRegex.Replace(authHeader, "$1")));
            var authSplit = authBase64.Split(Convert.ToChar(":"), 2);

            if (!int.TryParse(authSplit[0], out var sessionId))
                throw new AuthenticationException("Authorization code not formatted properly.");

            var username = authSplit[1];

            return (sessionId, username);
        }
    }
}