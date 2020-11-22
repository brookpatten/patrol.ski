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

        [Route("plans")]
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
    }
}