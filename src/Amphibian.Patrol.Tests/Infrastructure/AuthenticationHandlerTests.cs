using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Moq;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Services;
using Amphibian.Patrol.Api.Infrastructure;
using IAuthenticationService = Amphibian.Patrol.Api.Services.IAuthenticationService;
using Microsoft.Extensions.WebEncoders.Testing;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;
using Amphibian.Patrol.Api.Repositories;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Amphibian.Patrol.Api.Extensions;

namespace Amphibian.Patrol.Tests.Infrastructure
{
    [TestFixture(Category = "Infrastructure")]
    public class AuthenticationHandlerTests
    {
        private Mock<IAuthenticationService> _authenticationServiceMock;
        private Mock<UrlEncoder> _urlEncoderMock;
        private AuthenticationHandler _authenticationHandler;
        private Mock<ISystemClock> _systemClockMock;
        private DefaultHttpContext _context;
        private Mock<ITokenRepository> _tokenRepositoryMock;

        [SetUp]
        public async Task Setup()
        {
            var logger = new Mock<ILogger<AuthenticationHandler>>();
            var loggerFactory = new Mock<ILoggerFactory>(MockBehavior.Strict);
            loggerFactory.Setup(x => x.CreateLogger(typeof(AuthenticationHandler).FullName)).Returns(logger.Object);


            var options = new AuthenticationSchemeOptions();
            var om = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>(MockBehavior.Strict);
            om.Setup(x => x.Get(It.IsAny<string>())).Returns(options);
            
            _systemClockMock = new Mock<ISystemClock>();
            _urlEncoderMock = new Mock<UrlEncoder>();
            _authenticationServiceMock = new Mock<IAuthenticationService>();
            _tokenRepositoryMock = new Mock<ITokenRepository>();
            _authenticationHandler = new AuthenticationHandler(om.Object, loggerFactory.Object, _urlEncoderMock.Object, _systemClockMock.Object, _authenticationServiceMock.Object, _tokenRepositoryMock.Object);

            _context = new DefaultHttpContext();
            
            await _authenticationHandler.InitializeAsync(new AuthenticationScheme("", "", typeof(AuthenticationHandler)), _context);

        }

        [Test]
        public async Task AuthenticateWithNoAuthorizationHeaderShouldFail()
        {
            _context.Request.Headers.Add("", new Microsoft.Extensions.Primitives.StringValues(""));

            var result = await _authenticationHandler.AuthenticateAsync();

            Assert.AreEqual(AuthenticateResult.Fail("").Succeeded, result.Succeeded);
        }

        [Test]
        public async Task AuthenticateWithInvalidAuthorizationHeaderShouldFail()
        {
            _context.Request.Headers.Add("Authorization", new Microsoft.Extensions.Primitives.StringValues(""));

            var result = await _authenticationHandler.AuthenticateAsync();

            Assert.AreEqual(AuthenticateResult.Fail("").Succeeded, result.Succeeded);
        }

        [Test]
        public async Task AuthenticateWithIncorrectCredentialAuthorizationHeaderShouldFail()
        {
            string user = "user";
            string password = "password";
            var credentialBytes = Encoding.UTF8.GetBytes($"{user}:{password}");
            var credentialString = "Basic " + Convert.ToBase64String(credentialBytes);

            _context.Request.Headers.Add("Authorization", new Microsoft.Extensions.Primitives.StringValues(credentialString));

            _authenticationServiceMock.Setup(x => x.AuthenticateUserWithPassword(user, password))
                .Returns(Task.FromResult((User)null))
                .Verifiable();

            var result = await _authenticationHandler.AuthenticateAsync();

            _authenticationServiceMock.Verify();
            Assert.AreEqual(AuthenticateResult.Fail("").Succeeded, result.Succeeded);
        }

        [Test]
        public async Task AuthenticateWithCorrectCredentialAuthorizationHeaderShouldSucceed()
        {
            string user = "user";
            string password = "password";
            var credentialBytes = Encoding.UTF8.GetBytes($"{user}:{password}");
            var credentialString = "Basic " + Convert.ToBase64String(credentialBytes);

            _context.Request.Headers.Add("Authorization", new Microsoft.Extensions.Primitives.StringValues(credentialString));

            var userObj = new User() { Email = user, Id = 1 };
            _authenticationServiceMock.Setup(x => x.AuthenticateUserWithPassword(user, password))
                .Returns(Task.FromResult(userObj))
                .Verifiable();

            string jwt = "jwt";

            _authenticationServiceMock.Setup(x => x.IssueJwtToUser(userObj.Id,null)).
                Returns(Task.FromResult(jwt))
                .Verifiable();

            var principle = new ClaimsPrincipal();
            _authenticationServiceMock.Setup(x => x.ValidateSignedJwtToken(jwt)).
                Returns(principle)
                .Verifiable();

            var result = await _authenticationHandler.AuthenticateAsync();

            _authenticationServiceMock.Verify();
            Assert.AreEqual(true,result.Succeeded);
            Assert.NotNull(result.Ticket);
        }

        [Test]
        public async Task AuthenticateWithInvalidTokenAuthorizationHeaderShouldFail()
        {
            Guid token = Guid.NewGuid();
            string jwt = "jwt";
            var credentialString = "Token " + jwt;

            _context.Request.Headers.Add("Authorization", new Microsoft.Extensions.Primitives.StringValues(credentialString));

            var result = await _authenticationHandler.AuthenticateAsync();

            _authenticationServiceMock.Verify();
            Assert.AreEqual(AuthenticateResult.Fail("").Succeeded, result.Succeeded);
        }

        //[Test]
        //public async Task AuthenticateWithValidTokenAuthorizationHeaderShouldSucceed()
        //{
        //    string user = "user";
        //    string jwt = "jwt";
        //    var credentialString = "Token " + jwt;

        //    _context.Request.Headers.Add("Authorization", new Microsoft.Extensions.Primitives.StringValues(credentialString));

        //    Guid jti = Guid.NewGuid();
        //    var permClaims = new List<Claim>();
        //    permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, jti.ToString()));
        //    permClaims.Add(new Claim(JwtRegisteredClaimNames.Iss, "test"));
        //    permClaims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToUnixTime().ToString()));
        //    permClaims.Add(new Claim("uid", 1.ToString()));
        //    permClaims.Add(new Claim("patrols", "[]"));
        //    var principle = new ClaimsPrincipal(new ClaimsIdentity(permClaims));

        //    _authenticationServiceMock.Setup(x => x.ValidateSignedJwtToken(jwt))
        //        .Returns(principle)
        //        .Verifiable();

        //    _tokenRepositoryMock.Setup(x => x.GetToken(jti)).Returns(Task.FromResult(new Token() { TokenGuid = jti })).Verifiable();

        //    var result = await _authenticationHandler.AuthenticateAsync();

        //    _authenticationServiceMock.Verify();
        //    Assert.AreEqual(true, result.Succeeded);
        //    Assert.NotNull(result.Ticket);
        //}
    }
}
