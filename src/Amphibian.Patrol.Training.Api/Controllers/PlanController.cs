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
    }
}