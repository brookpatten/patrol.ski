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
using Amphibian.Patrol.Training.Api.Extensions;
using Amphibian.Patrol.Training.Api.Dtos;
using Amphibian.Patrol.Training.Api.Infrastructure;

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
        private IUserRepository _userRepository;
        private IPatrolService _patrolService;

        public AssignmentController(ILogger<AssignmentController> logger, IPatrolRepository patrolRepository, IPlanRepository planRepository, 
            IAssignmentRepository assignmentRepository, IPlanService planService, IAssignmentService assignmentService, IUserRepository userRepository, IPatrolService patrolService)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _planRepository = planRepository;
            _assignmentRepository = assignmentRepository;
            _planService = planService;
            _assignmentService = assignmentService;
            _userRepository = userRepository;
            _patrolService = patrolService;
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
            var user = (UserIdentifier)await _userRepository.GetUser(assignment.UserId);
            if (patrols.Any(x => x.Id == plan.PatrolId))
            {
                return Ok(
                    new
                    {
                        User = user,
                        Plan = plan,
                        Assignment = assignment
                    });
            }
            else
            {
                return Forbid();
            }
        }

        public class AssignmentSearch
        {
            public int PatrolId { get; set; }
            public int? UserId { get; set; }
            public int? PlanId { get; set; }
            public bool? Completed { get; set; }
        }
        [HttpPost]
        [Route("assignment/search")]
        [Authorize]
        public async Task<IActionResult> SearchAssignments(AssignmentSearch search)
        {
            if ((await _patrolService.GetUserRoleInPatrol(User.GetUserId(), search.PatrolId)).CanMaintainAssignments())
            {
                var assignments = await _assignmentRepository.GetAssignments(search.PatrolId, search.PlanId, search.UserId, search.Completed);
                return Ok(assignments);
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

        /// <summary>
        /// create new signatures and return a complete list of signatures for the assignment
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("assignment/sign")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateSignatures(CreateSignaturesDto dto)
        {
            if(await _assignmentService.AllowCreateSignatures(dto.AssignmentId,User.GetUserId(),dto.Signatures))
            {
                await _assignmentService.CreateSignatures(dto.AssignmentId, User.GetUserId(), dto.Signatures);
                var allSignatures = await _assignmentRepository.GetSignaturesWithUsersForAssignment(dto.AssignmentId);
                return Ok(allSignatures);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("assignments/incomplete-for-trainer/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetIncompleteAssignmentForTrainer(int patrolId)
        {
            var patrols = await _patrolRepository.GetPatrolsForUser(User.GetUserId());
            //TODO: also make sure the user has the right to create these signatures
            if (patrols.Any(x => x.Id == patrolId))
            {
                var assignments = await _assignmentRepository.GetIncompleteAssignments(patrolId, User.GetUserId());
                return Ok(assignments);
            }
            else
            {
                return Forbid();
            }
        }
    }
}