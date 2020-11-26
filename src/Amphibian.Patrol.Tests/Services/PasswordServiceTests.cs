using NUnit.Framework;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Services;

namespace Amphibian.Patrol.Tests.Services
{
    [TestFixture(Category = "Services")]
    public class PasswordServiceTests
    {
        private int _defaultIterations = 5;
        private int _defaultHashLength = 32;
        private PasswordService _service;
        private User _user;
        private string _password = "password123456";

        [SetUp]
        public void Setup()
        {
            _service = new PasswordService(_defaultIterations, _defaultHashLength);
            _user = new User();
        }

        [Test]
        public void SetPasswordShouldSetSaltHashAndIterations()
        {
            _service.SetPassword(_user, _password);

            Assert.NotNull(_user.PasswordSalt);
            Assert.NotNull(_user.PasswordHash);
            Assert.NotNull(_user.PasswordHashIterations);
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
            var differentXervice = new PasswordService(_defaultIterations+1, _defaultHashLength);
            Assert.True(differentXervice.CheckPassword(_user, _password));
        }

        [Test]
        public void TestPasswordShouldMatchSetPasswordEvenIfDefaultHashLengthIsDifferent()
        {
            _service.SetPassword(_user, _password);
            var differentXervice = new PasswordService(_defaultIterations, _defaultHashLength + 32);
            Assert.True(differentXervice.CheckPassword(_user, _password));
        }
    }
}