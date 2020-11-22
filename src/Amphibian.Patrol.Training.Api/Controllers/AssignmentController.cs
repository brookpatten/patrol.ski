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
using Amphibian.Patrol.Training.Api.Dtos;

namespace Amphibian.Patrol.Training.Api.Controllers
{
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly ILogger<AssignmentController> _logger;
        private readonly IPatrolRepository _patrolRepository;
        private readonly IPlanRepository _planRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private IPlanService _planService;
        private IAssignmentService _assignmentService;

        public AssignmentController(ILogger<AssignmentController> logger, IPatrolRepository patrolRepository, IPlanRepository planRepository, 
            IAssignmentRepository assignmentRepository, IPlanService planService, IAssignmentService assignmentService)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _planRepository = planRepository;
            _assignmentRepository = assignmentRepository;
            _planService = planService;
            _assignmentService = assignmentService;
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
            var assignment = await _assignmentService.GetAssignment(assignmentId);
            var plan = await _planService.GetPlan(assignment.PlanId,User.GetUserId());
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());
            if (patrols.Any(x => x.Id == plan.PatrolId))
            {
                return Ok(
                    new
                    {
                        Plan = plan,
                        Assignment = assignment
                    });
            }
            else
            {
                return Forbid();
            }
        }

        public class CreateSignaturesDto
        {
            public int AssignmentId { get; set; }
            public List<NewSignatureDto> Signatures { get; set; }
        }

        [HttpPost]
        [Route("assignment/sign")]
        [Authorize]
        public async Task<IActionResult> CreateSignatures(CreateSignaturesDto dto)
        {
            var assignment = await _assignmentService.GetAssignment(dto.AssignmentId);
            var plan = await _planService.GetPlan(assignment.PlanId, User.GetUserId());
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());
            //TODO: also make sure the user has the right to create these signatures
            if (patrols.Any(x => x.Id == plan.PatrolId))
            {
                await _assignmentService.CreateSignatures(assignment.Id, User.GetUserId(), dto.Signatures);
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }
    }
}