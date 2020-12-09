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
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication;

namespace Amphibian.Patrol.Api.Controllers
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
        private ISystemClock _clock;

        public AssignmentController(ILogger<AssignmentController> logger, IPatrolRepository patrolRepository, IPlanRepository planRepository, 
            IAssignmentRepository assignmentRepository, IPlanService planService, IAssignmentService assignmentService, IUserRepository userRepository, 
            IPatrolService patrolService, ISystemClock systemClock)
        {
            _logger = logger;
            _patrolRepository = patrolRepository;
            _planRepository = planRepository;
            _assignmentRepository = assignmentRepository;
            _planService = planService;
            _assignmentService = assignmentService;
            _userRepository = userRepository;
            _patrolService = patrolService;
            _clock = systemClock;
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
        [Route("assignments/search")]
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

        public class CreateAssignmentDto
        {
            public int PlanId { get; set; }
            public IList<int> ToUserIds{get;set;} 
            public DateTime? DueAt { get; set; }
        }
        [HttpPost]
        [Route("assignments/create")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> CreateAssignments(CreateAssignmentDto dto)
        {
            var plan = await _planRepository.GetPlan(dto.PlanId);
            var validPatrolUsers = await _patrolRepository.GetUsersForPatrol(plan.PatrolId);

            //TODO: also make sure the user has the right to create these signatures
            if ((await _patrolService.GetUserRoleInPatrol(User.GetUserId(), plan.PatrolId)).CanMaintainAssignments())
            {
                if(dto.ToUserIds.All(x=>validPatrolUsers.Any(y=>x==y.Id)))
                {
                    await _assignmentService.CreateAssignments(dto.PlanId, dto.ToUserIds, dto.DueAt);
                    return Ok();
                }
                else
                {
                    //tried to assign to a user for which they don't have access
                    return Forbid();
                }
            }
            else
            {
                //user does not have access to plan/patrol
                return Forbid();
            }
        }

        [HttpPut]
        [Route("assignments/update")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> UpdateAssignment(Assignment dto)
        {
            var existing = await _assignmentRepository.GetAssignment(dto.Id);
            var plan = await _planRepository.GetPlan(existing.PlanId);
            
            //TODO: also make sure the user has the right to create these signatures
            if ((await _patrolService.GetUserRoleInPatrol(User.GetUserId(), plan.PatrolId)).CanMaintainAssignments())
            {
                existing.DueAt = dto.DueAt;
                //existing.AssignedAt = dto.AssignedAt;
                if (!existing.CompletedAt.HasValue && dto.CompletedAt.HasValue)
                {
                    existing.CompletedAt = dto.CompletedAt;
                }
                await _assignmentRepository.UpdateAssignment(existing);
                return Ok();
            }
            else
            {
                //user does not have access to plan/patrol
                return Forbid();
            }
        }

        [HttpGet]
        [Route("assignments/counts-by-day")]
        [Authorize]
        public async Task<IActionResult> CountsByDay(int patrolId,DateTime? start, DateTime? end)
        {
            if ((await _patrolService.GetUserRoleInPatrol(User.GetUserId(), patrolId)).CanMaintainAssignments())
            {
                if (!start.HasValue)
                {
                    start = _clock.UtcNow.Subtract(new TimeSpan(30, 0, 0, 0)).UtcDateTime;
                }
                if (!end.HasValue)
                {
                    end = _clock.UtcNow.UtcDateTime;
                }
                var result = await _assignmentRepository.GetAssignmentCountsByDay(patrolId, start.Value, end.Value);
                return Ok(result);
            }
            else
            {
                return Forbid();
            }
        }

        [HttpGet]
        [Route("assignments/progress-by-day")]
        [Authorize]
        public async Task<IActionResult> GetAssignmentProgressByDay(int patrolId, DateTime? start, DateTime? end, int? planId, int? userId)
        {
            if ((await _patrolService.GetUserRoleInPatrol(User.GetUserId(), patrolId)).CanMaintainAssignments())
            {
                if (!start.HasValue)
                {
                    start = _clock.UtcNow.Subtract(new TimeSpan(30, 0, 0, 0)).UtcDateTime;
                }
                if (!end.HasValue)
                {
                    end = _clock.UtcNow.UtcDateTime;
                }
                var result = await _assignmentRepository.GetAssignmentProgressByDay(patrolId, start.Value, end.Value, planId, userId);

                

                return Ok(result
                    .GroupBy(x=> new { x.AssignmentId, x.AssignedAt, x.CompletedAt, x.PlanId, x.PlanName, x.UserEmail, x.UserFirstName, x.UserLastName})
                    .Select(x=>new { x.Key.AssignmentId, x.Key.AssignedAt, x.Key.CompletedAt, x.Key.PlanId, x.Key.PlanName, x.Key.UserEmail, x.Key.UserFirstName, x.Key.UserLastName, Days = x.Select(y=>new { y.Day, y.completedsignatures, y.requiredsignatures}).ToList() }));
            }
            else
            {
                return Forbid();
            }
        }
    }
}