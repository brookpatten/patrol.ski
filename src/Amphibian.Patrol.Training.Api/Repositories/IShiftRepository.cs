using Amphibian.Patrol.Training.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amphibian.Patrol.Training.Api.Dtos;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public interface IShiftRepository
    {
        Task<IEnumerable<ShiftTrainerDto>> GetAvailableTrainerShiftsForTrainee(int patrolId,int traineeUserId, DateTime after);
        Task<IEnumerable<ShiftTrainerDto>> GetCommittedTrainerShiftsForTrainee(int patrolId, int traineeUserId, DateTime after);
        Task<IEnumerable<ShiftTrainerDto>> GetTrainerShifts(int patrolId, int trainerUserId, DateTime after);
        Task<ShiftTrainer> GetShiftTrainer(int id);
        Task<TrainingShift> GetTrainingShift(int id);
        Task<Trainee> InsertTrainee(Trainee trainee);
        Task DeleteTrainee(Trainee trainee);
        Task<Trainee> GetTrainee(int id);
    }
}
