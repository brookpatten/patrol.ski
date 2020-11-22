﻿using NUnit.Framework;
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
using System;

namespace Amphibian.Patrol.Training.Tests.Services
{
    [TestFixture(Category = "Services")]
    public class AssignmentServiceTests
    {
        private AssignmentService _assignmentService;
        private Mock<IAssignmentRepository> _assignmentRepositoryMock;
        private IMapper _mapper;
        private Mock<ILogger<AssignmentService>> _loggerMock;
        private Mock<IPlanService> _planServiceMock;

        [SetUp]
        public void Setup()
        {
            _assignmentRepositoryMock = new Mock<IAssignmentRepository>();
            _mapper = DtoMappings.GetMapperConfiguration().CreateMapper();
            _loggerMock = new Mock<ILogger<AssignmentService>>();
            _planServiceMock = new Mock<IPlanService>();
            
            _assignmentService = new AssignmentService(_assignmentRepositoryMock.Object,_planServiceMock.Object, _loggerMock.Object, _mapper);
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
            var planId = 1;


            _assignmentRepositoryMock.Setup(x => x.GetSignaturesForAssignment(assignmentId))
                .Returns(Task.FromResult((new List<Signature>()).AsEnumerable()))
                .Verifiable();

            _assignmentRepositoryMock.Setup(x => x.GetAssignment(assignmentId))
                .Returns(Task.FromResult(new Assignment()
                {
                    Id = assignmentId,
                    PlanId = planId,
                    CompletedAt = DateTime.UtcNow
                })).Verifiable();

            int sectionSkillId = 5;
            int sectionLevelId = 6;

            _assignmentRepositoryMock.Setup(x => x.InsertSignature(It.Is<Signature>(y => y.SectionSkillId == sectionSkillId && y.SectionLevelId == sectionLevelId)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _assignmentService.CreateSignatures(assignmentId, 1, new List<NewSignatureDto>() { new NewSignatureDto() { SectionLevelId = sectionLevelId, SectionSkillId = sectionSkillId } });

            _assignmentRepositoryMock.Verify();
        }

        [Test]
        public async Task AssignmentCompletesWhenAllSignaturesExist()
        {
            var assignmentId = 1;
            var planId = 1;

            _assignmentRepositoryMock.Setup(x => x.GetSignaturesForAssignment(assignmentId))
                .Returns(Task.FromResult((new List<Signature>()).AsEnumerable()))
                .Verifiable();

            _assignmentRepositoryMock.Setup(x => x.GetAssignment(assignmentId))
                .Returns(Task.FromResult(new Assignment()
                {
                    Id = assignmentId,
                    PlanId = planId
                })).Verifiable();

            int sectionSkillId = 5;
            int sectionLevelId = 6;

            _assignmentRepositoryMock.Setup(x => x.InsertSignature(It.Is<Signature>(y => y.SectionSkillId == sectionSkillId && y.SectionLevelId == sectionLevelId)))
                .Returns(Task.CompletedTask)
                .Verifiable();

            _planServiceMock.Setup(x => x.GetPlan(planId,null))
                .Returns(Task.FromResult(new PlanDto()
                {
                    Id = planId,
                    Name="test plan",
                    PatrolId = 1,
                    Sections = new List<SectionDto>()
                    {
                        new SectionDto()
                        {
                            Levels = new List<SectionLevelDto>()
                            {
                                new SectionLevelDto()
                                {
                                    Id = sectionLevelId
                                }
                            },
                            Skills = new List<SectionSkillDto>()
                            {
                                new SectionSkillDto()
                                {
                                    Id = sectionSkillId
                                }
                            }
                        }
                    }
                })).Verifiable();
            _assignmentRepositoryMock.Setup(x => x.UpdateAssignment(It.Is<Assignment>(x => x.CompletedAt.HasValue))).Verifiable();

            await _assignmentService.CreateSignatures(assignmentId, 1, new List<NewSignatureDto>() { new NewSignatureDto() { SectionLevelId = sectionLevelId, SectionSkillId = sectionSkillId } });

            
            _assignmentRepositoryMock.Verify();
        }
    }
}
