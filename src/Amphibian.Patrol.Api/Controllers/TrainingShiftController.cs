using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Infrastructure;
using Amphibian.Patrol.Api.Services;

namespace Amphibian.Patrol.Api.Controllers
{
    [ApiController]
    public class TrainingShiftController : ControllerBase
    {
        private readonly ILogger<TrainingShiftController> _logger;
        private readonly IShiftRepository _shiftRepository;
        private readonly IPatrolRepository _patrolRepository;
        private readonly ISystemClock _clock;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        
        public TrainingShiftController(ILogger<TrainingShiftController> logger, IShiftRepository shiftRepository,IPatrolRepository patrolRepository, ISystemClock clock, IEmailService emailService, IUserRepository userRepository)
        {
            _logger = logger;
            _shiftRepository = shiftRepository;
            _patrolRepository = patrolRepository;
            _clock = clock;
            _emailService = emailService;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("trainingshifts/training/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetTraining(int patrolId)
        {
            var upcomingShifts = await _shiftRepository.GetScheduledShiftAssignments(patrolId, this.User.GetUserId(), _clock.UtcNow.UtcDateTime);

            upcomingShifts = upcomingShifts.Where(x => x.TraineeCount > 0);

            return Ok(upcomingShifts);
        }

        [HttpGet]
        [Route("trainingshifts/available/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetAvailable(int patrolId)
        {
            var availableShifts = await _shiftRepository.GetAvailableTrainerShiftsForTrainee(patrolId, this.User.GetUserId(), _clock.UtcNow.UtcDateTime);
            return Ok(availableShifts);
        }

        [HttpGet]
        [Route("trainingshifts/committed/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetCommitted(int patrolId)
        {
            var shifts = await _shiftRepository.GetCommittedTrainerShiftsForTrainee(patrolId, this.User.GetUserId(), _clock.UtcNow.UtcDateTime);
            return Ok(shifts);
        }

        [HttpPost]
        [Route("trainingshifts/commit")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> Commit(int scheduledShiftAssignmentId)
        {
            //todo: move to service
            var shiftTrainer = await _shiftRepository.GetScheduledShiftAssignment(scheduledShiftAssignmentId);
            if(!shiftTrainer.AssignedUserId.HasValue)
            {
                throw new InvalidOperationException("The specified shift assignment does not have a person associated with it");
            }
            var trainingShift = await _shiftRepository.GetScheduledShift(shiftTrainer.ScheduledShiftId);
            var patrols = await _patrolRepository.GetPatrolsForUser(this.User.GetUserId());

            if(patrols.Any(x=>x.Id==trainingShift.PatrolId))
            {
                var trainee = new Trainee()
                {
                    ScheduledShiftAssignmentId = scheduledShiftAssignmentId,
                    TraineeUserId = this.User.GetUserId()
                };
                await _shiftRepository.InsertTrainee(trainee);

                //send notification to trainer
                var trainer = await _userRepository.GetUser(shiftTrainer.AssignedUserId.Value);
                var patrol = await _patrolRepository.GetPatrol(trainingShift.PatrolId);
                var traineeUser = await _userRepository.GetUser(trainee.TraineeUserId);
                await _emailService.SendTraineeSignup(trainer, traineeUser, patrol, trainingShift);

                return Ok();
            }
            else
            {
                //user does not have access to this trainer
                return Forbid();
            }
        }

        [HttpPost]
        [Route("trainingshifts/cancel")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> Cancel(int traineeId)
        {
            var trainee = await _shiftRepository.GetTrainee(traineeId);

            if (trainee.TraineeUserId == this.User.GetUserId())
            {
                await _shiftRepository.DeleteTrainee(trainee);

                //send notification to trainer
                var assignment = await _shiftRepository.GetScheduledShiftAssignment(trainee.ScheduledShiftAssignmentId);
                var shift = await _shiftRepository.GetScheduledShift(assignment.ScheduledShiftId);

                //this shouldn't really happen since we don't let people sign up to train on empty assignments, but just in case
                if (assignment.AssignedUserId.HasValue)
                {
                    var trainer = await _userRepository.GetUser(assignment.AssignedUserId.Value);
                    var patrol = await _patrolRepository.GetPatrol(shift.PatrolId);
                    var traineeUser = await _userRepository.GetUser(trainee.TraineeUserId);
                    await _emailService.SendTraineeCancel(trainer, traineeUser, patrol, shift);
                }

                return Ok();
            }
            else
            {
                return Forbid();
            }
        }
    }
}
