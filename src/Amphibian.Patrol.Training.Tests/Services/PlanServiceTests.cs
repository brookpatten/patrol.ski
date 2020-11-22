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
        private Mock<IGroupRepository> _groupRepositoryMock;
        private IMapper _mapper;
        private Mock<ILogger<PlanService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _planRepositoryMock = new Mock<IPlanRepository>();
            _mapper = DtoMappings.GetMapperConfiguration().CreateMapper();
            _loggerMock = new Mock<ILogger<PlanService>>();
            _groupRepositoryMock = new Mock<IGroupRepository>();
            _planService = new PlanService(_planRepositoryMock.Object, _loggerMock.Object, _mapper,_groupRepositoryMock.Object);
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
                .Returns(Task.FromResult((new List<SectionLevelDto>() { new SectionLevelDto() { Id = 1, SectionId=1, ColumnIndex = 1 } }).AsEnumerable()))
                .Verifiable();
            _planRepositoryMock.Setup(x => x.GetSectionSkillsForPlan(planId))
                .Returns(Task.FromResult((new List<SectionSkillDto>() { new SectionSkillDto() { Id = 1, SectionId = 1, RowIndex = 1 } }).AsEnumerable()))
                .Verifiable();

            var user = new User()
            {
                Id = 1,
                Email = "Email",
                FirstName = "FirstName",
                LastName = "LastName"
            };

            var plan = await _planService.GetPlan(planId,user.Id);

            Assert.AreEqual(1, plan.Sections.Count());
            Assert.AreEqual(1, plan.Sections.First().Levels.Count());
            Assert.AreEqual(1, plan.Sections.First().Skills.Count());
            _planRepositoryMock.Verify();
        }

        [Test]
        public async Task CanCreateNewPlan()
        {
            string name = "test";
            int patrolId = 1;

            _planRepositoryMock.Setup(x => x.InsertPlan(It.Is<Plan>(y => y.Name == name && y.PatrolId == patrolId)))
                .Verifiable();

            await _planService.CreatePlan(name, patrolId, null);

            _planRepositoryMock.Verify();
        }

        [Test]
        public async Task CanCreateNewPlanFromTemplate()
        {
            string name = "test";
            int patrolId = 1;
            var sections = new List<Section>() 
            { 
                new Section() 
                {
                    Id = 1,
                    Name = "section",
                    PatrolId = patrolId
                } 
            };
            var sectionLevels = new List<SectionLevelDto>() 
            {
                new SectionLevelDto()
                {
                    Id = 1,
                    ColumnIndex = 0,
                    SectionId = 1,
                    Level = new Level()
                    {
                        Id = 1,
                        Name = "Level",
                        PatrolId = patrolId
                    }
                }
            };
            var sectionSkills = new List<SectionSkillDto>() 
            {
                new SectionSkillDto()
                {
                    Id = 1,
                    RowIndex = 0,
                    SectionId = 1,
                    Skill = new Skill()
                    {
                        Id = 1,
                        Name = "Skill",
                        PatrolId = patrolId
                    }
                }
            };
            var sectioNGroups = new List<SectionGroupDto>()
            { };

            var existingLevels = sectionLevels.Select(x => x.Level).ToList();
            var existingSkills = sectionSkills.Select(x => x.Skill).ToList();

            _planRepositoryMock.Setup(x => x.InsertPlan(It.Is<Plan>(y => y.Name == name && y.PatrolId == patrolId)))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.InsertSection(It.Is<Section>(y => y.Name == sections.First().Name)))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.InsertPlanSection(It.IsAny<PlanSection>()))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.InsertSectionLevel(It.Is<SectionLevel>(y => y.LevelId == sectionLevels.First().Level.Id)))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.InsertSectionSkill(It.Is<SectionSkill>(y => y.SkillId == sectionSkills.First().Skill.Id)))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.GetSectionsForPlan(2)).Returns(Task.FromResult(sections.AsEnumerable()))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.GetSectionLevelsForPlan(2)).Returns(Task.FromResult(sectionLevels.AsEnumerable()))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.GetSectionSkillsForPlan(2)).Returns(Task.FromResult(sectionSkills.AsEnumerable()))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.GetLevels(patrolId)).Returns(Task.FromResult(existingLevels.AsEnumerable()))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.GetSkills(patrolId)).Returns(Task.FromResult(existingSkills.AsEnumerable()))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.GetSectionGroupsForPlan(2)).Returns(Task.FromResult(sectioNGroups.AsEnumerable()))
                .Verifiable();

            await _planService.CreatePlan(name, patrolId, 2);

            _planRepositoryMock.Verify();
            _planRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task CanCreateNewPlanAndCopyLevelsIfFromDifferentPatrol()
        {
            string name = "test";
            int patrolId = 1;
            var sections = new List<Section>()
            {
                new Section()
                {
                    Id = 1,
                    Name = "section",
                    PatrolId = patrolId
                }
            };
            var sectionLevels = new List<SectionLevelDto>()
            {
                new SectionLevelDto()
                {
                    Id = 1,
                    ColumnIndex = 0,
                    SectionId = 1,
                    Level = new Level()
                    {
                        Id = 1,
                        Name = "Level",
                        PatrolId = patrolId
                    }
                }
            };
            var sectionSkills = new List<SectionSkillDto>()
            {
                new SectionSkillDto()
                {
                    Id = 1,
                    RowIndex = 0,
                    SectionId = 1,
                    Skill = new Skill()
                    {
                        Id = 1,
                        Name = "Skill",
                        PatrolId = patrolId
                    }
                }
            };

            var existingLevels = sectionLevels.Select(x => x.Level).ToList();
            var existingSkills = sectionSkills.Select(x => x.Skill).ToList();

            _planRepositoryMock.Setup(x => x.InsertPlan(It.Is<Plan>(y => y.Name == name && y.PatrolId == patrolId+1)))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.InsertSection(It.Is<Section>(y => y.Name == sections.First().Name)))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.InsertPlanSection(It.IsAny<PlanSection>()))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.InsertSectionLevel(It.IsAny<SectionLevel>()))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.InsertSectionSkill(It.IsAny<SectionSkill>()))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.GetSectionsForPlan(2)).Returns(Task.FromResult(sections.AsEnumerable()))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.GetSectionLevelsForPlan(2)).Returns(Task.FromResult(sectionLevels.AsEnumerable()))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.GetSectionSkillsForPlan(2)).Returns(Task.FromResult(sectionSkills.AsEnumerable()))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.GetLevels(patrolId+1)).Returns(Task.FromResult(new List<Level>().AsEnumerable()))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.GetSkills(patrolId+1)).Returns(Task.FromResult(new List<Skill>().AsEnumerable()))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.InsertLevel(It.Is<Level>(y => y.Name == sectionLevels.First().Level.Name)))
                .Verifiable();

            _planRepositoryMock.Setup(x => x.InsertSkill(It.Is<Skill>(y => y.Name == sectionSkills.First().Skill.Name)))
                .Verifiable();

            await _planService.CreatePlan(name, patrolId+1, 2);

            _planRepositoryMock.Verify();
            
        }

        [Test]
        public async Task PlanIsValidIfNoSections()
        {
            var plan = new PlanDto() { Name = "test" };
            var result = await _planService.IsPlanFormatValid(plan);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task PlanIsInValidIfItHasEmptySections()
        {
            var plan = new PlanDto()
            {
                Name = "test",
                Sections = new List<SectionDto>() {
                    new SectionDto(){ }
                }
            };
            var result = await _planService.IsPlanFormatValid(plan);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task PlanIsValidIfItHasContiguousSection()
        {
            _planRepositoryMock.Setup(x => x.GetLevels(1))
                .Returns(Task.FromResult(new List<Level>() { new Level() { Id = 1 } }.AsEnumerable()))
                .Verifiable();
            _planRepositoryMock.Setup(x => x.GetSkills(1))
                .Returns(Task.FromResult(new List<Skill>() { new Skill() { Id = 1 } }.AsEnumerable()))
                .Verifiable();

            var plan = new PlanDto()
            {
                Name = "test",
                PatrolId = 1,
                Sections = new List<SectionDto>() {
                    new SectionDto(){ 
                        Levels = new List<SectionLevelDto>()
                        {
                            new SectionLevelDto()
                            {
                                ColumnIndex=0,
                                Level = new Level()
                                {
                                    Id = 1,
                                }
                            },
                            new SectionLevelDto()
                            {
                                ColumnIndex=1,
                                Level = new Level()
                                {
                                    Id = 1,
                                }
                            },
                            new SectionLevelDto()
                            {
                                ColumnIndex=2,
                                Level = new Level()
                                {
                                    Id = 1,
                                }
                            }
                        },
                        Skills = new List<SectionSkillDto>()
                        {
                            new SectionSkillDto()
                            {
                                RowIndex=0,
                                Skill = new Skill()
                                {
                                    Id=1
                                }
                            },
                            new SectionSkillDto()
                            {
                                RowIndex=1,
                                Skill = new Skill()
                                {
                                    Id=1
                                }
                            },
                            new SectionSkillDto()
                            {
                                RowIndex=2,
                                Skill = new Skill()
                                {
                                    Id=1
                                }
                            }
                        }
                    }
                }
            };
            var result = await _planService.IsPlanFormatValid(plan);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task PlanIsInvalidIfItUsesSkillsFromWrongPatrol()
        {
            _planRepositoryMock.Setup(x => x.GetLevels(1))
                .Returns(Task.FromResult(new List<Level>() { }.AsEnumerable()))
                .Verifiable();
            _planRepositoryMock.Setup(x => x.GetSkills(1))
                .Returns(Task.FromResult(new List<Skill>() { new Skill() { Id = 1 } }.AsEnumerable()))
                .Verifiable();

            var plan = new PlanDto()
            {
                Name = "test",
                PatrolId = 1,
                Sections = new List<SectionDto>() {
                    new SectionDto(){
                        Levels = new List<SectionLevelDto>()
                        {
                            new SectionLevelDto()
                            {
                                ColumnIndex=0,
                                Level = new Level()
                                {
                                    Id = 1,
                                }
                            },
                        },
                        Skills = new List<SectionSkillDto>()
                        {
                            new SectionSkillDto()
                            {
                                RowIndex=0,
                                Skill = new Skill()
                                {
                                    Id=1
                                }
                            },
                        }
                    }
                }
            };
            var result = await _planService.IsPlanFormatValid(plan);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task PlanIsInvalidIfItUsesLevelsFromWrongPatrol()
        {
            _planRepositoryMock.Setup(x => x.GetLevels(1))
                .Returns(Task.FromResult(new List<Level>() { new Level() { Id = 1 } }.AsEnumerable()))
                .Verifiable();
            _planRepositoryMock.Setup(x => x.GetSkills(1))
                .Returns(Task.FromResult(new List<Skill>() {  }.AsEnumerable()))
                .Verifiable();

            var plan = new PlanDto()
            {
                Name = "test",
                PatrolId = 1,
                Sections = new List<SectionDto>() {
                    new SectionDto(){
                        Levels = new List<SectionLevelDto>()
                        {
                            new SectionLevelDto()
                            {
                                ColumnIndex=0,
                                Level = new Level()
                                {
                                    Id = 1,
                                }
                            },
                        },
                        Skills = new List<SectionSkillDto>()
                        {
                            new SectionSkillDto()
                            {
                                RowIndex=0,
                                Skill = new Skill()
                                {
                                    Id=1
                                }
                            },
                        }
                    }
                }
            };
            var result = await _planService.IsPlanFormatValid(plan);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task PlanIsInvalidIfLevelsAreNotContiguous()
        {
            _planRepositoryMock.Setup(x => x.GetLevels(1))
                .Returns(Task.FromResult(new List<Level>() { new Level() { Id = 1 } }.AsEnumerable()))
                .Verifiable();
            _planRepositoryMock.Setup(x => x.GetSkills(1))
                .Returns(Task.FromResult(new List<Skill>() { new Skill() { Id = 1 } }.AsEnumerable()))
                .Verifiable();

            var plan = new PlanDto()
            {
                Name = "test",
                Sections = new List<SectionDto>() {
                    new SectionDto(){
                        Levels = new List<SectionLevelDto>()
                        {
                            new SectionLevelDto()
                            {
                                ColumnIndex=0,
                                Level = new Level()
                                {
                                    Id = 1,
                                }
                            },
                            new SectionLevelDto()
                            {
                                ColumnIndex=2,
                                Level = new Level()
                                {
                                    Id = 1,
                                }
                            }
                        },
                        Skills = new List<SectionSkillDto>()
                        {
                            new SectionSkillDto()
                            {
                                RowIndex=0,
                                Skill = new Skill()
                                {
                                    Id=1
                                }
                            },
                            new SectionSkillDto()
                            {
                                RowIndex=1,
                                Skill = new Skill()
                                {
                                    Id=1
                                }
                            },
                            new SectionSkillDto()
                            {
                                RowIndex=2,
                                Skill = new Skill()
                                {
                                    Id=1
                                }
                            }
                        }
                    }
                }
            };
            var result = await _planService.IsPlanFormatValid(plan);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task PlanIsInValidIfSkillsAreNotContiguous()
        {
            _planRepositoryMock.Setup(x => x.GetLevels(1))
                .Returns(Task.FromResult(new List<Level>() { new Level() { Id = 1 } }.AsEnumerable()))
                .Verifiable();
            _planRepositoryMock.Setup(x => x.GetSkills(1))
                .Returns(Task.FromResult(new List<Skill>() { new Skill() { Id = 1 } }.AsEnumerable()))
                .Verifiable();

            var plan = new PlanDto()
            {
                Name = "test",
                Sections = new List<SectionDto>() {
                    new SectionDto(){
                        Levels = new List<SectionLevelDto>()
                        {
                            new SectionLevelDto()
                            {
                                ColumnIndex=0,
                                Level = new Level()
                                {
                                    Id = 1,
                                }
                            },
                            new SectionLevelDto()
                            {
                                ColumnIndex=1,
                                Level = new Level()
                                {
                                    Id = 1,
                                }
                            },
                            new SectionLevelDto()
                            {
                                ColumnIndex=2,
                                Level = new Level()
                                {
                                    Id = 1,
                                }
                            }
                        },
                        Skills = new List<SectionSkillDto>()
                        {
                            new SectionSkillDto()
                            {
                                RowIndex=0,
                                Skill = new Skill()
                                {
                                    Id=1
                                }
                            },
                            new SectionSkillDto()
                            {
                                RowIndex=2,
                                Skill = new Skill()
                                {
                                    Id=1
                                }
                            }
                        }
                    }
                }
            };
            var result = await _planService.IsPlanFormatValid(plan);
            Assert.IsFalse(result);
        }
    }
}
