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
    [TestFixture(Category = "Services")]
    public class AssignmentServiceTests
    {
        private AssignmentService _assignmentService;
        private Mock<IAssignmentRepository> _assignmentRepositoryMock;
        private IMapper _mapper;
        private Mock<ILogger<AssignmentService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _assignmentRepositoryMock = new Mock<IAssignmentRepository>();
            _mapper = DtoMappings.GetMapperConfiguration().CreateMapper();
            _loggerMock = new Mock<ILogger<AssignmentService>>();
            _assignmentService = new AssignmentService(_assignmentRepositoryMock.Object, _loggerMock.Object, _mapper);
        }

        [Test]
        public async Task CanRetrieveStructuredAssignment()
        {
            var assignmentId = 1;
            _assignmentRepositoryMock.Setup(x => x.GetAssignment(assignmentId))
                .Returns(Task.FromResult(new Assignment() { Id = assignmentId,PlanId=1,UserId=1,AssignedAt=new System.DateTime(2020,1,1,0,0,0) }))
                .Verifiable();


            _assignmentRepositoryMock.Setup(x => x.GetAssignment(assignmentId))
                .Returns(Task.FromResult(new Assignment() { Id=1,PlanId=1,UserId=1}))
                .Verifiable();
            _assignmentRepositoryMock.Setup(x => x.GetSignaturesWithUsersForAssignment(assignmentId))
                .Returns(Task.FromResult((new List<SignatureDto>() { new SignatureDto() { Id = 1,SectionLevelId=1,SectionSkillId=1 } }).AsEnumerable()))
                .Verifiable();

            var assignment = await _assignmentService.GetAssignment(assignmentId);

            Assert.AreEqual(1, assignment.Signatures.Count());
            _assignmentRepositoryMock.Verify();
        }

        [Test]
        public async Task CanCreateSignatures()
        {
            var assignmentId = 1;
            _assignmentRepositoryMock.Setup(x => x.GetSignaturesForAssignment(assignmentId))
                .Returns(Task.FromResult((new List<Signature>()).AsEnumerable()))
                .Verifiable();

            int sectionSkillId = 5;
            int sectionLevelId = 6;

            _assignmentRepositoryMock.Setup(x => x.InsertSignature(It.Is<Signature>(y => y.SectionSkillId == sectionSkillId && y.SectionLevelId == sectionLevelId)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _assignmentService.CreateSignatures(assignmentId, 1, new List<NewSignatureDto>() { new NewSignatureDto() { SectionLevelId = sectionLevelId, SectionSkillId = sectionSkillId } });

            _assignmentRepositoryMock.Verify();
        }
    }
}
