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

            var context = new DefaultHttpContext();
            
            await _authenticationHandler.InitializeAsync(new AuthenticationScheme("", "", typeof(AuthenticationHandler)), context);

        }

        [Test]
        public async Task AuthenticateWithNoAuthorizeShouldFail()
        {
            var result = await _authenticationHandler.AuthenticateAsync();

            Assert.AreEqual(AuthenticateResult.Fail("").Succeeded, result.Succeeded);
        }
    }
}
