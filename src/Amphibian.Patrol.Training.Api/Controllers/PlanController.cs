using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Amphibian.Patrol.Training.Api.Repositories;
using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Amphibian.Patrol.Training.Api.Extensions;

namespace Amphibian.Patrol.Training.Api.Controllers
{
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly ILogger<PlanController> _logger;
        private readonly IPatrolRepository _patrolRepository;
        private readonly IPlanRepository _planRepository;

        public PlanController(ILogger<PlanController> logger, IPatrolRepository patrolRepository, IPlanRepository planRepository)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _planRepository = planRepository;
        }

        [HttpGet]
        [Route("plans")]
        [Authorize]
        public async Task<IActionResult> Plans(int patrolId)
        {
            var patrol = (await _patrolRepository.GetPatrolsForUser(User.GetUserId())).SingleOrDefault(x=>x.Id==patrolId);
            if(patrol!=null)
            {
                var plans = await _planRepository.GetPlansForPatrol(patrol.Id);
                return Ok(plans);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("plan/{planId}")]
        [Authorize]
        public async Task<IActionResult> PlanDetails(int planId)
        {
            var plan = await _planRepository.GetPlan(planId);
            var userPatrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());
            if(userPatrols.Any(x=>x.Id==plan.PatrolId))
            {
                //TODO: this can be optimized into less queries later
                //plansections
                var planSections = await _planRepository.GetSectionsForPlan(planId);

                var sectionLevels = new List<SectionLevel>();
                var sectionSkills = new List<SectionSkill>();

                foreach(var section in planSections)
                {
                    sectionLevels.AddRange(await _planRepository.GetSectionLevels(section.Id));
                    sectionSkills.AddRange(await _planRepository.GetSectionSkills(section.Id));
                }

                var patrolLevels = await _planRepository.GetLevels(plan.PatrolId);
                var patrolSkills = await _planRepository.GetSkills(plan.PatrolId);

                return Ok(new
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    Sections = planSections.Select(x => new
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Skills = sectionSkills.Select(y => new
                        {
                            Order = y.Order,
                            Id = y.SkillId,
                            Name = patrolSkills.Single(x=>x.Id==y.SkillId).Name,
                            Description = patrolSkills.Single(x => x.Id == y.SkillId).Description
                        }).ToList(),
                        Levels = sectionLevels.Select(y => new
                        {
                            Order = y.Order,
                            Id = y.LevelId,
                            Name = patrolLevels.Single(x => x.Id == y.LevelId).Name,
                            Description = patrolLevels.Single(x => x.Id == y.LevelId).Description
                        }).ToList(),
                    }).ToList()
                });
            }
            else
            {
                return Forbid();
            }
        }
    }
}