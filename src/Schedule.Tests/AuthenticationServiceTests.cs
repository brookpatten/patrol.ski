using NUnit.Framework;
using Schedule.Api.Models;
using Schedule.Api.Services;

namespace Schedule.Tests
{
    public class Tests
    {
        private int _defaultIterations = 5;
        private AuthenticationService _service;
        private User _user;
        private string _password = "password123456";

        [SetUp]
        public void Setup()
        {
            _service = new AuthenticationService(_defaultIterations);
            _user = new User();
        }

        [Test]
        public void SetPasswordShouldSetSaltHashAndIterations()
        {
            _service.SetPassword(_user, _password);

            Assert.NotNull(_user.PasswordSalt);
            Assert.NotNull(_user.PasswordHash);
            Assert.NotNull(_user.PasswordIterations);
        }

        [Test]
        public void TestPasswordShouldMatchSetPassword()
        {
            _service.SetPassword(_user, _password);
            Assert.True(_service.CheckPassword(_user, _password));
        }

        [Test]
        public void TestPasswordShouldMatchSetPasswordEvenIfDefaultIterationIsDifferent()
        {
            _service.SetPassword(_user, _password);
            var differentXervice = new AuthenticationService(_defaultIterations+1);
            Assert.True(differentXervice.CheckPassword(_user, _password));
        }
    }
}