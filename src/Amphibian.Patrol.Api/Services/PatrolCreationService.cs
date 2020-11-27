using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Serilog.Sinks.SystemConsole.Themes;
using GenFu;

namespace Amphibian.Patrol.Api.Services
{
    /// <summary>
    /// create default and demo data
    /// </summary>
    public class PatrolCreationService : IPatrolCreationService
    {
        private ILogger<PatrolCreationService> _logger;
        private IPatrolRepository _patrolRepository;
        private IPlanRepository _planRepository;
        private IGroupRepository _groupRepository;
        private IUserRepository _userRepository;
        private IAssignmentRepository _assignmentRepository;
        private IAnnouncementService _announcementService;
        private IPlanService _planService;
        private IEventRepository _eventRepository;

        public enum BuiltInPlan { AlpineSki, AlpineSnowboard };

        public PatrolCreationService(ILogger<PatrolCreationService> logger, IPatrolRepository patrolRepository, IPlanRepository planRepository, 
            IGroupRepository groupRepository, IUserRepository userRepository, IAssignmentRepository assignmentRepository, IPlanService planService, 
            IAnnouncementService announcementService, IEventRepository eventRepository)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _planRepository = planRepository;
            _groupRepository = groupRepository;
            _userRepository = userRepository;
            _assignmentRepository = assignmentRepository;
            _planService = planService;
            _announcementService = announcementService;
            _eventRepository = eventRepository;
        }

        public async Task CreateBuiltInPlan(BuiltInPlan plan, int patrolId)
        {
            if(plan==BuiltInPlan.AlpineSki)
            {
                await CreateDefaultPlan(patrolId, "PSIA", "Ski", new List<string>()
                {
                    "Controlled Parallel Stop (2 Directions)",
                    "Descending Leaves (2 Directions)",
                    "Traverse with Forward Traversing Sideslip (2 Directions)",
                    "Gliding Wedge",
                    "Wedge Sideslip Transition (2 Directions)",
                    "Pivot Sideslip Transitions (2 Directions)",
                    "Confined Area (Parallel Turns)",
                    "Free Skiing"
                });
            }
            else if(plan==BuiltInPlan.AlpineSnowboard)
            {
                await CreateDefaultPlan(patrolId, "AASI", "Snowboard", new List<string>()
                {
                    "Controlled Emergency Stop (Toe & Heel Side)",
                    "Descending Leaves (Toe & Heel Side)",
                    "Traverse with Forward Traversing Sideslip (Toe & Heel Side)",
                    "Linked Skidded Turns (Forward)",
                    "Linked Skidded Turns (Switch)",
                    "Pivot Sideslip Transitions (Toe-to-Heel, Heel-to-Toe)",
                    "Confined Area Dynamic Turns",
                    "Free Riding"
                });
            }
        }
        
        public async Task CreateDefaultInitialSetup(int patrolId)
        {
            await CreateBuiltInPlan(BuiltInPlan.AlpineSki, patrolId);
            await CreateBuiltInPlan(BuiltInPlan.AlpineSnowboard, patrolId);
        }

        public async Task CreateDefaultPlan(int patrolId,string trainingOrgName,string sport, List<string> skills)
        {
            var planSkills = new List<Skill>();
            foreach(var skillName in skills)
            {
                var skill = new Skill() { Name = skillName, PatrolId = patrolId };
                await _planRepository.InsertSkill(skill);
                planSkills.Add(skill);
            }

            var tobogganSkillNames = new List<string>() { "Toboggan Approach - Alone", "Loaded Toboggan - Front Operator", "Loaded Toboggan - Tail Rope Operator" };
            var tobogganSkills = new List<Skill>();
            foreach (var skillName in tobogganSkillNames)
            {
                var skill = new Skill() { Name = skillName, PatrolId = patrolId };
                await _planRepository.InsertSkill(skill);
                tobogganSkills.Add(skill);
            }

            var planLevelNames = new List<string>() { "First", "Second", "Third", "Final" };
            var planLevels = new List<Level>();
            foreach (var levelName in planLevelNames)
            {
                var level = new Level() { Name = levelName, PatrolId = patrolId };
                await _planRepository.InsertLevel(level);
                planLevels.Add(level);
            }

            //var finalLevelNames = new List<string>() { "Final" };
            //var finalLevels = new List<Level>();
            //foreach (var levelName in finalLevelNames)
            //{
            //    var level = new Level() { Name = levelName, PatrolId = patrolId };
            //    await _planRepository.InsertLevel(level);
            //    finalLevels.Add(level);
            //}

            var existinGroups = await _groupRepository.GetGroupsForPatrol(patrolId);

            //groups
            var groupSki1 = new Group() { Name = $"{trainingOrgName} Level 1", PatrolId = patrolId };
            await _groupRepository.InsertGroup(groupSki1);
            var groupSki23 = new Group() { Name = $"{trainingOrgName} Level 2+", PatrolId = patrolId };
            await _groupRepository.InsertGroup(groupSki23);
            Group groupSkiFinal = existinGroups.SingleOrDefault(x => x.Name == $"{sport} Final");
            if(groupSkiFinal==null)
            {
                groupSkiFinal = new Group() { Name = $"{sport} Final", PatrolId = patrolId };
                await _groupRepository.InsertGroup(groupSkiFinal);
            }
            Group groupToboggan = existinGroups.SingleOrDefault(x => x.Name == $"Toboggan");
            if (groupToboggan == null)
            {
                groupToboggan = new Group() { Name = $"Toboggan", PatrolId = patrolId };
                await _groupRepository.InsertGroup(groupToboggan);
            }

            //plan
            var plan = new Plan() { Name = $"{sport} Alpine", PatrolId = patrolId };
            await _planRepository.InsertPlan(plan);

            //sections
            var sectionSkiLevel1 = new Section() { Name = $"{sport} Skills Level 1", PatrolId = patrolId, Color = sport== "Ski" ? "#6fa8dc" : "#93c47d" };
            await _planRepository.InsertSection(sectionSkiLevel1);
            var sectionSkiLevel23 = new Section() { Name = $"{sport} Skills Level 2/3", PatrolId = patrolId, Color = sport == "Ski" ? "#93c47d" : "#6fa8dc" };
            await _planRepository.InsertSection(sectionSkiLevel23);
            var sectionSkiToboggan = new Section() { Name = $"Toboggan", PatrolId = patrolId, Color = "#fce5cd" };
            await _planRepository.InsertSection(sectionSkiToboggan);
            var sectionSkiFinal = new Section() { Name = $"Final", PatrolId = patrolId, Color = "#d5a6bd" };
            await _planRepository.InsertSection(sectionSkiFinal);

            //add sections to plan
            await _planRepository.InsertPlanSection(new PlanSection() { PlanId = plan.Id, SectionId=sectionSkiLevel1.Id});
            await _planRepository.InsertPlanSection(new PlanSection() { PlanId = plan.Id, SectionId = sectionSkiLevel23.Id });
            await _planRepository.InsertPlanSection(new PlanSection() { PlanId = plan.Id, SectionId = sectionSkiToboggan.Id });
            await _planRepository.InsertPlanSection(new PlanSection() { PlanId = plan.Id, SectionId = sectionSkiFinal.Id });

            //map groups to sections
            await _planRepository.InsertSectionGroup(new SectionGroup() { GroupId = groupSki1.Id, SectionId = sectionSkiLevel1.Id });
            await _planRepository.InsertSectionGroup(new SectionGroup() { GroupId = groupSki23.Id, SectionId = sectionSkiLevel23.Id });
            await _planRepository.InsertSectionGroup(new SectionGroup() { GroupId = groupToboggan.Id, SectionId = sectionSkiToboggan.Id });
            await _planRepository.InsertSectionGroup(new SectionGroup() { GroupId = groupSkiFinal.Id, SectionId = sectionSkiFinal.Id });

            //level 1 section
            for (var i=0;i<planSkills.Count;i++)
            {
                var sectionSkill = new SectionSkill() { RowIndex = i, SectionId = sectionSkiLevel1.Id, SkillId = planSkills[i].Id };
                await _planRepository.InsertSectionSkill(sectionSkill);
            }
            var section1Level = new SectionLevel() { ColumnIndex = 0, SectionId = sectionSkiLevel1.Id, LevelId = planLevels[0].Id };
            await _planRepository.InsertSectionLevel(section1Level);

            //level 2/3 section
            for (var i = 0; i < planSkills.Count; i++)
            {
                var sectionSkill = new SectionSkill() { RowIndex = i, SectionId = sectionSkiLevel23.Id, SkillId = planSkills[i].Id };
                await _planRepository.InsertSectionSkill(sectionSkill);
            }
            var section231Level = new SectionLevel() { ColumnIndex = 1, SectionId = sectionSkiLevel23.Id, LevelId = planLevels[1].Id };
            await _planRepository.InsertSectionLevel(section231Level);
            var section232Level = new SectionLevel() { ColumnIndex = 2, SectionId = sectionSkiLevel23.Id, LevelId = planLevels[2].Id };
            await _planRepository.InsertSectionLevel(section232Level);

            //toboggan
            for (var i = 0; i < tobogganSkills.Count; i++)
            {
                var sectionSkill = new SectionSkill() { RowIndex = planSkills.Count + i, SectionId = sectionSkiToboggan.Id, SkillId = tobogganSkills[i].Id };
                await _planRepository.InsertSectionSkill(sectionSkill);
            }
            var sectionToboggan1 = new SectionLevel() { ColumnIndex = 0, SectionId = sectionSkiToboggan.Id, LevelId = planLevels[0].Id };
            await _planRepository.InsertSectionLevel(sectionToboggan1);
            var sectionToboggan2 = new SectionLevel() { ColumnIndex = 1, SectionId = sectionSkiToboggan.Id, LevelId = planLevels[1].Id };
            await _planRepository.InsertSectionLevel(sectionToboggan2);
            var sectionToboggan3 = new SectionLevel() { ColumnIndex = 2, SectionId = sectionSkiToboggan.Id, LevelId = planLevels[2].Id };
            await _planRepository.InsertSectionLevel(sectionToboggan3);
            
            //final
            for (var i = 0; i < planSkills.Count; i++)
            {
                var sectionSkill = new SectionSkill() { RowIndex = i, SectionId = sectionSkiFinal.Id, SkillId = planSkills[i].Id };
                await _planRepository.InsertSectionSkill(sectionSkill);
            }
            for (var i = 0; i < tobogganSkills.Count; i++)
            {
                var sectionSkill = new SectionSkill() { RowIndex = planSkills.Count + i, SectionId = sectionSkiFinal.Id, SkillId = tobogganSkills[i].Id };
                await _planRepository.InsertSectionSkill(sectionSkill);
            }
            var sectionFinal = new SectionLevel() { ColumnIndex = 3, SectionId = sectionSkiFinal.Id, LevelId = planLevels[3].Id };
            await _planRepository.InsertSectionLevel(sectionFinal);
        }

        public async Task CreateDemoInitialSetup(int patrolId,int adminUserId)
        {
            var random = new Random();

            await CreateDefaultInitialSetup(patrolId);
            
            //trainer setup
            var skiTrainer = new User() { FirstName = "Jim", LastName = "Teachesskiing", Email = $"Jim.{Guid.NewGuid()}@patrol.training" };
            await _userRepository.InsertUser(skiTrainer);
            var skiTrainerPatrolUser = new PatrolUser()
            {
                PatrolId = patrolId,
                UserId = skiTrainer.Id
            };
            await _patrolRepository.InsertPatrolUser(skiTrainerPatrolUser);

            var snowboardTrainer = new User() { FirstName = "Stephen", LastName = "Teachessnowboarding", Email = $"Stephen.{Guid.NewGuid()}@patrol.training" };
            await _userRepository.InsertUser(snowboardTrainer);
            var snowboardTrainerPatrolUser = new PatrolUser()
            {
                PatrolId = patrolId,
                UserId = snowboardTrainer.Id
            };
            await _patrolRepository.InsertPatrolUser(snowboardTrainerPatrolUser);

            var tobogganTrainer = new User() { FirstName = "Vern", LastName = "Teachestoboggan", Email = $"Vern.{Guid.NewGuid()}@patrol.training" };
            await _userRepository.InsertUser(tobogganTrainer);
            var tobogganTrainerPatrolUser = new PatrolUser()
            {
                PatrolId = patrolId,
                UserId = tobogganTrainer.Id
            };
            await _patrolRepository.InsertPatrolUser(tobogganTrainerPatrolUser);

            //put the trainers in the right groups
            var groups = await _groupRepository.GetGroupsForPatrol(patrolId);
            foreach(var group in groups)
            {
                GroupUser gu = new GroupUser() { GroupId = group.Id };
                if(group.Name.Contains("PSIA"))
                {
                    gu.UserId = skiTrainer.Id;
                }
                else if(group.Name.Contains("AASI"))
                {
                    gu.UserId = snowboardTrainer.Id;
                }
                else if(group.Name.Contains("Toboggan"))
                {
                    gu.UserId = tobogganTrainer.Id;
                }
                else if (group.Name.Contains("Final"))
                {
                    gu.UserId = tobogganTrainer.Id;
                }
                await _groupRepository.InsertGroupUser(gu);
            }

            //assignment/trainee setup
            var now = DateTime.UtcNow;
            var lastMonth = now - new TimeSpan(30, 0, 0, 0);
            var in2Years = lastMonth + new TimeSpan(365 * 2, 0, 0, 0);

            var plans = await _planRepository.GetPlansForPatrol(patrolId);

            var planDtos = new List<PlanDto>();
            foreach(var plan in plans)
            {
                var dto = await _planService.GetPlan(plan.Id);
                planDtos.Add(dto);
            }

            int randomTraineeCount = 15;

            GenFu.GenFu.Configure<User>();
            var trainees = A.ListOf<User>(randomTraineeCount);

            for(int i=0;i< randomTraineeCount; i++)
            {
                bool ski = (new Random().Next(2) == 1);
                string sport = ski ? "Ski" : "Snowboard";

                var plan = planDtos.Single(x => x.Name == $"{sport} Alpine");

                var skiTrainee = trainees[i];
                skiTrainee.Id = 0;
                skiTrainee.Email = skiTrainee.FirstName + "." + skiTrainee.LastName + ".demo." + Guid.NewGuid().ToString() + "@patrol.training";
                skiTrainee.PasswordHashIterations = null;
                await _userRepository.InsertUser(skiTrainee);

                var patrolUser = new PatrolUser()
                {
                    PatrolId = patrolId,
                    UserId = skiTrainee.Id
                };
                await _patrolRepository.InsertPatrolUser(patrolUser);

                var skiAssignment = new Assignment() { AssignedAt = lastMonth+new TimeSpan(random.Next(7),0,0,0,0), DueAt = in2Years, PlanId = plan.Id, UserId = skiTrainee.Id };
                await _assignmentRepository.InsertAssignment(skiAssignment);

                await RandomlySignAssignment(skiAssignment,plan, ski ? skiTrainer.Id : snowboardTrainer.Id,tobogganTrainer.Id,tobogganTrainer.Id, skiAssignment.AssignedAt, now, 20 +random.Next(0, 10));
            }


            //TODO: make up some shift data too

            //announcements
            var announcement = new Announcement()
            {
                CreatedByUserId = adminUserId,
                AnnouncementMarkdown = @"Welcome to <strong>Patrol</strong>.ski.  This is where your patrol announcements are shown.<br/>
<em>You can use many kinds of formatting</em><br/>
<ul>
<li>Lists</li>
<li>Headings</li>
<li>Quotes</li>
<li><a href='https://patrol.ski'>links</a></li>
</ul>
<hr/>
<em>You can also choose to have these announcements emailed to your patrol</em>
",

        
                CreatedAt = DateTime.UtcNow,
                PatrolId = patrolId,
                Subject="Announcement",
                PostAt = DateTime.Now
            };
            await _announcementService.PostAnnouncement(announcement);

            //events
            var patrolEvent = new Event()
            {
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = adminUserId,
                Name = "Season Begins",
                Location = "Mt. Dumptruck",
                StartsAt = DateTime.UtcNow + new TimeSpan(3, 0, 0, 0),
                EndsAt = DateTime.UtcNow + new TimeSpan(3, 8, 0, 0),
                PatrolId = patrolId
            };
            await _eventRepository.InsertEvent(patrolEvent);
        }

        private async Task RandomlySignAssignment(Assignment assignment,PlanDto plan, int trainerId, int tobogganTrainerId, int finalTrainerId,DateTime from,DateTime to,int? count)
        {
            var random = new Random();
            var signatures = new List<Signature>();

            if(!count.HasValue)
            {
                int max = 0;
                foreach(var section in plan.Sections)
                {
                    var levelCount = section.Levels.Max(x => x.ColumnIndex) - section.Levels.Min(x => x.ColumnIndex);
                    var skillCount = section.Skills.Max(x => x.RowIndex) - section.Skills.Min(x => x.RowIndex);
                    max = max + (levelCount * skillCount);
                }
                count = random.Next(max);
            }

            //find an interval with which to space out signatures
            var interval = new TimeSpan((to - from).Ticks / count.Value);
            DateTime current = from;

            //get sections
            var firstSection = plan.Sections.Single(x => x.Name.EndsWith("Skills Level 1"));
            var firstSectionSignatures = new List<Signature>();
            var secondSection = plan.Sections.Single(x => x.Name.EndsWith("Skills Level 2/3"));
            var secondSectionSignatures = new List<Signature>();
            var tobogganSection = plan.Sections.Single(x => x.Name.EndsWith("Toboggan"));
            var tobogganSectionSignatures = new List<Signature>();

            for (int i=0;i<count.Value;i++)
            {
                //find a random place to put the sig
                if(firstSectionSignatures.Count <= firstSection.Skills.Max(x => x.RowIndex) - firstSection.Skills.Min(x => x.RowIndex))
                {
                    var available = firstSection.Skills.Where(x => !firstSectionSignatures.Any(y => y.SectionSkillId == x.Id));
                    if (available.Count() > 0)
                    {
                        var selected = available.ToList()[random.Next(available.Count())];
                        var sig = new Signature() { AssignmentId = assignment.Id, SectionSkillId = selected.Id, SectionLevelId = firstSection.Levels.First().Id, SignedAt = current, SignedByUserId = trainerId };
                        firstSectionSignatures.Add(sig);
                    }
                    else
                    {
                        throw new InvalidOperationException(@"¯\_( ͡❛ ͜ʖ ͡❛)_/¯");
                    }
                }
                else
                {
                    var randomSection = random.Next(2);
                    if(secondSectionSignatures.Count < secondSection.Skills.Count() || randomSection==0)
                    {
                        //second section
                        var available = secondSection.Skills.Where(x => secondSectionSignatures.Count(y => y.SectionSkillId == x.Id) <2);
                        if (available.Count() > 0)
                        {
                            var selected = available.ToList()[random.Next(available.Count())];
                            var sig = new Signature()
                            {
                                AssignmentId = assignment.Id,
                                SectionSkillId = selected.Id,
                                SectionLevelId = secondSection.Levels.ToList()[secondSectionSignatures.Count(y => y.SectionSkillId == selected.Id)].Id,
                                SignedAt = current,
                                SignedByUserId = trainerId
                            };
                            secondSectionSignatures.Add(sig);
                        }
                        else
                        {
                            throw new InvalidOperationException(@"¯\_( ͡❛ ͜ʖ ͡❛)_/¯");
                        }
                    }
                    else
                    {
                        //toboggan
                        var available = tobogganSection.Skills.Where(x => tobogganSectionSignatures.Count(y => y.SectionSkillId == x.Id) < 3);
                        if (available.Count() > 0)
                        {
                            var selected = available.ToList()[random.Next(available.Count())];
                            var sig = new Signature()
                            {
                                AssignmentId = assignment.Id,
                                SectionSkillId = selected.Id,
                                SectionLevelId = tobogganSection.Levels.ToList()[tobogganSectionSignatures.Count(y => y.SectionSkillId == selected.Id)].Id,
                                SignedAt = current,
                                SignedByUserId = tobogganTrainerId
                            };
                            tobogganSectionSignatures.Add(sig);
                        }
                        else
                        {
                            throw new InvalidOperationException(@"¯\_( ͡❛ ͜ʖ ͡❛)_/¯");
                        }
                    }
                }

                signatures.Clear();
                signatures.AddRange(firstSectionSignatures);
                signatures.AddRange(secondSectionSignatures);
                signatures.AddRange(tobogganSectionSignatures);

                //some amount of time up to 3 days
                var randomInterval = new TimeSpan((long)(random.NextDouble() * (new TimeSpan(3,0,0,0)).Ticks));

                if (signatures.Count > firstSection.Skills.Count() * 2 && random.Next(5)==0)
                {
                    //randomly complete the assignment
                    foreach(var section in plan.Sections)
                    {
                        foreach(var skill in section.Skills)
                        {
                            foreach(var level in section.Levels)
                            {
                                var sig = signatures.SingleOrDefault(x => x.SectionLevelId == level.Id && x.SectionSkillId == skill.Id);
                                if(sig==null)
                                {
                                    sig = new Signature()
                                    {
                                        AssignmentId = assignment.Id,
                                        SectionLevelId = level.Id,
                                        SectionSkillId = skill.Id,
                                        SignedAt = current,
                                        SignedByUserId = finalTrainerId
                                    };
                                    signatures.Add(sig);
                                }
                            }
                        }
                    }

                    assignment.CompletedAt = current;
                    await _assignmentRepository.UpdateAssignment(assignment);
                    break;
                }
                else if (current+ randomInterval < to)
                {
                    current = current + randomInterval;
                }
                else
                {
                    break;
                }
            }

            

            foreach(var sig in signatures)
            {
                await _assignmentRepository.InsertSignature(sig);
            }
        }

        public async Task<Models.Patrol> CreateNewPatrol(int userId, string name)
        {
            var patrol = new Models.Patrol()
            {
                Name = name,
                EnableAnnouncements = true,
                EnableTraining = true,
                EnableEvents = true
            };
            await _patrolRepository.InsertPatrol(patrol);
            var patrolUser = new PatrolUser()
            {
                PatrolId = patrol.Id,
                Role = Role.Administrator,
                UserId = userId,
                
            };
            await _patrolRepository.InsertPatrolUser(patrolUser);
            return patrol;
        }
        public async Task<Tuple<User, Models.Patrol>> CreateDemoUserAndPatrol()
        {
            var users = A.ListOf<User>(1);
            var user = users.First();
            user.Email = user.FirstName + "." + user.LastName + ".demo." + Guid.NewGuid() + "@patrol.training";
            user.PasswordHashIterations = null;

            await _userRepository.InsertUser(user);

            

            var patrol = await CreateNewPatrol(user.Id, "Mt. Rumpke Ski Patrol");

            await CreateDemoInitialSetup(patrol.Id,user.Id);
            return new Tuple<User, Models.Patrol>(user, patrol);
        }
    }
}
