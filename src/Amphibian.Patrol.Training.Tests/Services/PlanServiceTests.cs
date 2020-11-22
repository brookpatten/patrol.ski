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
    public class PlanServiceTests
    {
        private PlanService _planService;
        private Mock<IPlanRepository> _planRepositoryMock;
        private IMapper _mapper;
        private Mock<ILogger<PlanService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _planRepositoryMock = new Mock<IPlanRepository>();
            _mapper = DtoMappings.GetMapperConfiguration().CreateMapper();
            _loggerMock = new Mock<ILogger<PlanService>>();
            _planService = new PlanService(_planRepositoryMock.Object, _loggerMock.Object, _mapper);
        }

        [Test]
        public async Task CanRetrieveStructuredPlan()
        {
            var planId = 1;
            _planRepositoryMock.Setup(x => x.GetPlan(planId))
                .Returns(Task.FromResult(new Plan() { Id = planId, Name = "Test" }))
                .Verifiable();
            _planRepositoryMock.Setup(x => x.GetSectionsForPlan(planId))
                .Returns(Task.FromResult((new List<Section>() { new Section() {Id=1,PatrolId=1,Name="Test Section" } }).AsEnumerable()))
                .Verifiable();
            _planRepositoryMock.Setup(x => x.GetSectionLevelsForPlan(planId))
                .Returns(Task.FromResult((new List<SectionLevelDto>() { new SectionLevelDto() { Id = 1, SectionId=1,Order=1 } }).AsEnumerable()))
                .Verifiable();
            _planRepositoryMock.Setup(x => x.GetSectionSkillsForPlan(planId))
                .Returns(Task.FromResult((new List<SectionSkillDto>() { new SectionSkillDto() { Id = 1, SectionId = 1, Order = 1 } }).AsEnumerable()))
                .Verifiable();


            var plan = await _planService.GetPlan(planId);

            Assert.AreEqual(1, plan.Sections.Count());
            Assert.AreEqual(1, plan.Sections.First().Levels.Count());
            Assert.AreEqual(1, plan.Sections.First().Skills.Count());
            _planRepositoryMock.Verify();
        }
    }
}
