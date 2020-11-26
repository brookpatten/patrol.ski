using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Dapper;
using Dommel;

using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using Amphibian.Patrol.Api.Extensions;
using Amphibian.Patrol.Api.Repositories;
using Microsoft.AspNetCore.Authentication;
using Amphibian.Patrol.Api.Models;
using Amphibian.Patrol.Api.Infrastructure;

namespace Amphibian.Patrol.Api.Controllers
{
    [ApiController]
    public class TrainingShiftController : ControllerBase
    {
        private readonly ILogger<TrainingShiftController> _logger;
        private readonly IShiftRepository _shiftRepository;
        private readonly IPatrolRepository _patrolRepository;
        private readonly ISystemClock _clock;
        
        public TrainingShiftController(ILogger<TrainingShiftController> logger, IShiftRepository shiftRepository,IPatrolRepository patrolRepository, ISystemClock clock)
        {
            _logger = logger;
            _shiftRepository = shiftRepository;
            _patrolRepository = patrolRepository;
            _clock = clock;
        }

        [HttpGet]
        [Route("trainingshifts/training/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetTraining(int patrolId)
        {
            var upcomingShifts = await _shiftRepository.GetTrainerShifts(patrolId, this.User.GetUserId(), _clock.UtcNow.DateTime);
            return Ok(upcomingShifts);
        }

        [HttpGet]
        [Route("trainingshifts/available/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetAvailable(int patrolId)
        {
            var availableShifts = await _shiftRepository.GetAvailableTrainerShiftsForTrainee(patrolId, this.User.GetUserId(), _clock.UtcNow.DateTime);
            return Ok(availableShifts);
        }

        [HttpGet]
        [Route("trainingshifts/committed/{patrolId}")]
        [Authorize]
        public async Task<IActionResult> GetCommitted(int patrolId)
        {
            var shifts = await _shiftRepository.GetCommittedTrainerShiftsForTrainee(patrolId, this.User.GetUserId(), _clock.UtcNow.DateTime);
            return Ok(shifts);
        }

        [HttpPost]
        [Route("trainingshifts/commit/{shiftTrainerId}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> Commit(int shiftTrainerId)
        {
            var shiftTrainer = await _shiftRepository.GetShiftTrainer(shiftTrainerId);
            var trainingShift = await _shiftRepository.GetTrainingShift(shiftTrainer.TrainingShiftId);
            var patrols = await _patrolRepository.GetPatrolsForUser(this.User.GetUserId());

            if(patrols.Any(x=>x.Id==trainingShift.PatrolId))
            {
                var trainee = new Trainee()
                {
                    ShiftTrainerId = shiftTrainerId,
                    TraineeUserId = this.User.GetUserId()
                };
                await _shiftRepository.InsertTrainee(trainee);
                return Ok();
            }
            else
            {
                //user does not have access to this trainer
                return Forbid();
            }
        }

        [HttpPost]
        [Route("trainingshifts/cancel/{traineeId}")]
        [Authorize]
        [UnitOfWork]
        public async Task<IActionResult> Cancel(int traineeId)
        {
            var trainee = await _shiftRepository.GetTrainee(traineeId);

            if (trainee.TraineeUserId == this.User.GetUserId())
            {
                await _shiftRepository.DeleteTrainee(trainee);
                return Ok();
            }
            else
            {
                return Forbid();
            }
        }
    }
}
