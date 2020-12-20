using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Amphibian.Patrol.Api.Repositories;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Infrastructure;
using Amphibian.Patrol.Api.Dtos;

namespace Amphibian.Patrol.Api.Controllers
{
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly ILogger<PlanController> _logger;
        private readonly IPatrolRepository _patrolRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IPlanService _planService;
        private readonly IPatrolService _patrolService;

        public PlanController(ILogger<PlanController> logger, IPatrolRepository patrolRepository, IPlanRepository planRepository, IPlanService planService, IPatrolService patrolService)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _planRepository = planRepository;
            _planService = planService;
            _patrolService = patrolService;

        }

        [HttpGet]
        [Route("plans")]
        [Authorize]
        public async Task<IActionResult> Plans(int patrolId)
        {
            if(User.PatrolIds().Any(x=>x==patrolId))
            {
                var plans = await _planRepository.GetPlansForPatrol(patrolId);
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
            var plan = await _planService.GetPlan(planId, User.UserId());
            if(User.PatrolIds().Any(x=>x==plan.PatrolId))
            {
                return Ok(plan);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("plan/levels/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetLevels(int patrolId)
        {
            if (User.PatrolIds().Any(x => x == patrolId))
            {
                var levels = await _planRepository.GetLevels(patrolId);
                return Ok(levels);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("plan/skills/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetSkills(int patrolId)
        {
            if (User.PatrolIds().Any(x=>x==patrolId))
            {
                var skills = await _planRepository.GetSkills(patrolId);
                return Ok(skills);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("plan/create")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> Create(int patrolId, string name, int? copyFromPlanId)
        {
            if (User.RoleInPatrol(patrolId).CanMaintainPlans())
            {
                var plan = await _planService.CreatePlan(name, patrolId, copyFromPlanId);
                return Ok(plan);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("plan/update")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> Update(PlanDto dto)
        {
            var existingPlan = await _planRepository.GetPlan(dto.Id);

            if (User.RoleInPatrol(existingPlan.PatrolId).CanMaintainPlans())
            {
                if (await _planService.IsPlanFormatValid(dto))
                {
                    await _planService.UpdatePlan(dto);
                    var updatedPlan = await _planService.GetPlan(dto.Id, User.UserId());
                    return Ok(updatedPlan);
                }
                else
                {
                    return Problem("The plan is invalid");
                }
            }
            else
            {
                return Forbid();
            }
        }


        [HttpPost]
        [Route("plan/levels/create")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateLevel(Level level)
        {
            if (User.RoleInPatrol(level.PatrolId).CanMaintainPlans())
            {
                await _planRepository.InsertLevel(level);
                return Ok(level);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("plan/skills/create")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateSkill(Skill skill)
        {
            if (User.RoleInPatrol(skill.PatrolId).CanMaintainPlans())
            {
                await _planRepository.InsertSkill(skill);
                return Ok(skill);
            }
            else
            {
                return Forbid();
            }
        }
    }
}