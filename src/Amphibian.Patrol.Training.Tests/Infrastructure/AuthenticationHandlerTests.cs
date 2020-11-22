using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Moq;
using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Services;
using Amphibian.Patrol.Training.Api.Infrastructure;
using IAuthenticationService = Amphibian.Patrol.Training.Api.Services.IAuthenticationService;
using Microsoft.Extensions.WebEncoders.Testing;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;

namespace Amphibian.Patrol.Training.Tests.Infrastructure
{
    [TestFixture(Category = "Infrastructure")]
    public class AuthenticationHandlerTests
    {
        private Mock<IAuthenticationService> _authenticationServiceMock;
        private Mock<UrlEncoder> _urlEncoderMock;
        private AuthenticationHandler _authenticationHandler;
        private Mock<ISystemClock> _systemClockMock;
        private DefaultHttpContext _context;

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
            _authenticationHandler = new AuthenticationHandler(om.Object, loggerFactory.Object, _urlEncoderMock.Object, _systemClockMock.Object, _authenticationServiceMock.Object);

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

            _authenticationServiceMock.Setup(x => x.CreateNewTokenForUser(userObj)).
                Returns(Task.FromResult(new Token() { UserId = userObj.Id,TokenGuid = Guid.NewGuid()}))
                .Verifiable();

            var result = await _authenticationHandler.AuthenticateAsync();

            _authenticationServiceMock.Verify();
            Assert.AreEqual(true,result.Succeeded);
            Assert.NotNull(result.Ticket);
        }

        [Test]
        public async Task AuthenticateWithInvalidTokenAuthorizationHeaderShouldFail()
        {
            string user = "user";
            Guid token = Guid.NewGuid();
            var credentialString = "Token " + token;

            _context.Request.Headers.Add("Authorization", new Microsoft.Extensions.Primitives.StringValues(credentialString));

            _authenticationServiceMock.Setup(x => x.AuthenticateUserWithToken(token))
                .Returns(Task.FromResult((User)null))
                .Verifiable();

            var result = await _authenticationHandler.AuthenticateAsync();

            _authenticationServiceMock.Verify();
            Assert.AreEqual(AuthenticateResult.Fail("").Succeeded, result.Succeeded);
        }

        [Test]
        public async Task AuthenticateWithValidTokenAuthorizationHeaderShouldSucceed()
        {
            string user = "user";
            Guid token = Guid.NewGuid();
            var credentialString = "Token " + token;

            _context.Request.Headers.Add("Authorization", new Microsoft.Extensions.Primitives.StringValues(credentialString));

            _authenticationServiceMock.Setup(x => x.AuthenticateUserWithToken(token))
                .Returns(Task.FromResult(new User() { 
                    Email = user,
                    Id= 1
                }))
                .Verifiable();

            var result = await _authenticationHandler.AuthenticateAsync();

            _authenticationServiceMock.Verify();
            Assert.AreEqual(true, result.Succeeded);
            Assert.NotNull(result.Ticket);
        }
    }
}
