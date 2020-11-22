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
    public class AssignmentController : ControllerBase
    {
        private readonly ILogger<AssignmentController> _logger;
        private readonly IPatrolRepository _patrolRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IAssignmentRepository _assignmentRepository;

        public AssignmentController(ILogger<AssignmentController> logger, IPatrolRepository patrolRepository, IPlanRepository planRepository, IAssignmentRepository assignmentRepository)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _planRepository = planRepository;
            _assignmentRepository = assignmentRepository;
        }

        [HttpGet]
        [Route("assignments/by-plan/{planId}")]
        [Authorize]
        public async Task<IActionResult> GetAssignmentsByPlan(int planId)
        {
            var plan = await _planRepository.GetPlan(planId);
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());
            if (patrols.Any(x => x.Id == plan.PatrolId))
            {
                var assignments = await _assignmentRepository.GetAssignmentsForPlan(plan.Id);
                return Ok(assignments);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("assignments")]
        [Authorize]
        public async Task<IActionResult> GetAssignmentsForUser()
        {
            var assignments = await _assignmentRepository.GetAssignmentsForUser(User.GetUserId());
            return Ok(assignments);
        }

        [HttpGet]
        [Route("assignment/{assignmentId}")]
        [Authorize]
        public async Task<IActionResult> GetAssignment(int assignmentId)
        {
            var assignment = await _assignmentRepository.GetAssignment(assignmentId);
            var plan = await _planRepository.GetPlan(assignment.PlanId);
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());
            if (patrols.Any(x => x.Id == plan.PatrolId))
            {
                return Ok(assignment);
            }
            else
            {
                return Forbid();
            }
        }
    }
}