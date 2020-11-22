using NUnit.Framework;
using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Services;
using Amphibian.Patrol.Training.Api.Repositories;
using Amphibian.Patrol.Training.Api.Dtos;
using AutoMapper;
using Moq;
using Amphibian.Patrol.Training.Api.Mappings;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Amphibian.Patrol.Training.Tests.Services
{
    public class UserServiceTests
    {
        private UserService _userService;
        private Mock<IGroupRepository> _groupRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<IPatrolRepository> _patrolRepositoryMock;
        private Mock<ILogger<UserService>> _loggerMock;
        private IMapper _mapper;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<UserService>>();
            _groupRepositoryMock = new Mock<IGroupRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _patrolRepositoryMock = new Mock<IPatrolRepository>();
            _mapper = DtoMappings.GetMapperConfiguration().CreateMapper();
            _userService = new UserService(_loggerMock.Object, _userRepositoryMock.Object, _emailServiceMock.Object, _groupRepositoryMock.Object, _patrolRepositoryMock.Object, _mapper);
        }

        [Test]
        public async Task AddUserToPatrolCreatesIfNoneExists()
        {
            string email = "test";
            _userRepositoryMock.Setup(x => x.GetUser(email))
                .Returns(Task.FromResult<User>(null))
                .Verifiable();
            _userRepositoryMock.Setup(x => x.InsertUser(It.Is<User>(y => y.Email == email)))
                .Verifiable();
            _emailServiceMock.Setup(x => x.SendNewUserWelcomeEmail(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();
            _patrolRepositoryMock.Setup(x => x.InsertPatrolUser(It.IsAny<PatrolUser>()))
                .Verifiable();

            await _userService.AddUserToPatrol(1, null, "first", "last", email);

            _userRepositoryMock.Verify();
            _emailServiceMock.Verify();
            _patrolRepositoryMock.Verify();
        }

        [Test]
        public async Task AddUserToPatrolCreatesIfExists()
        {
            string email = "test";
            _userRepositoryMock.Setup(x => x.GetUser(email))
                .Returns(Task.FromResult<User>(new User() { Id = 1 }))
                .Verifiable();
            _emailServiceMock.Setup(x => x.SendExistingUserWelcomeEmail(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .Verifiable();
            _patrolRepositoryMock.Setup(x => x.InsertPatrolUser(It.Is<PatrolUser>(x=>x.UserId==1)))
                .Verifiable();

            await _userService.AddUserToPatrol(1, null, "first", "last", email);

            _userRepositoryMock.Verify();
            _emailServiceMock.Verify();
            _patrolRepositoryMock.Verify();
        }

        [Test]
        public async Task AddUserToGroup()
        {
            string email = "test";
            _groupRepositoryMock.Setup(x => x.GetGroup(1))
                .Returns(Task.FromResult<Group>(new Group() { Id = 1,PatrolId=1 }))
                .Verifiable();
            _groupRepositoryMock.Setup(x => x.GetGroupsForUser(1,1))
                .Returns(Task.FromResult(new List<Group>().AsEnumerable()))
                .Verifiable();
            
            _groupRepositoryMock.Setup(x => x.InsertGroupUser(It.Is<GroupUser>(x => x.UserId == 1)))
                .Verifiable();

            await _userService.AddUserToGroup(1,1);

            _groupRepositoryMock.Verify();
        }

        [Test]
        public async Task RemoveUserFromGroup()
        {
            string email = "test";
            _groupRepositoryMock.Setup(x => x.GetGroupUser(1,1))
                .Returns(Task.FromResult<GroupUser>(new GroupUser() { Id = 1, UserId = 1, GroupId=1 }))
                .Verifiable();
            _groupRepositoryMock.Setup(x => x.DeleteGroupUser(It.Is<GroupUser>(x=>x.Id==1)))
                .Returns(Task.FromResult(new List<Group>().AsEnumerable()))
                .Verifiable();


            await _userService.RemoveUserFromGroup(1, 1);

            _groupRepositoryMock.Verify();
        }
    }
}
