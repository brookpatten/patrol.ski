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
using Amphibian.Patrol.Training.Api.Infrastructure;
using Amphibian.Patrol.Training.Api.Dtos;

namespace Amphibian.Patrol.Training.Api.Controllers
{
    [ApiController]
    public class PlanController : ControllerBase
    {
        private readonly ILogger<PlanController> _logger;
        private readonly IPatrolRepository _patrolRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IPlanService _planService;

        public PlanController(ILogger<PlanController> logger, IPatrolRepository patrolRepository, IPlanRepository planRepository, IPlanService planService)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _planRepository = planRepository;
            _planService = planService;
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
            var plan = await _planService.GetPlan(planId, User.GetUserId());
            var userPatrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());
            if(userPatrols.Any(x=>x.Id==plan.PatrolId))
            {
                return Ok(plan);
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
            var userPatrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());
            if (userPatrols.Any(x => x.Id == patrolId))
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

            var userPatrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());
            if (userPatrols.Any(x => x.Id == existingPlan.PatrolId))
            {
                if (await _planService.IsPlanFormatValid(dto))
                {
                    await _planService.UpdatePlan(dto);
                    var updatedPlan = await _planService.GetPlan(dto.Id, User.GetUserId());
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
    }
}