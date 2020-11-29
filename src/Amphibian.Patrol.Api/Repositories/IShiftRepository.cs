using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amphibian.Patrol.Api.Dtos;

namespace Amphibian.Patrol.Api.Repositories
{
    public interface IShiftRepository
    {
        //training queries
        Task<IEnumerable<ScheduledShiftAssignmentDto>> GetAvailableTrainerShiftsForTrainee(int patrolId,int traineeUserId, DateTime after);
        Task<IEnumerable<ScheduledShiftAssignmentDto>> GetCommittedTrainerShiftsForTrainee(int patrolId, int traineeUserId, DateTime after);
        
        //trainee
        Task<Trainee> InsertTrainee(Trainee trainee);
        Task DeleteTrainee(Trainee trainee);
        Task<Trainee> GetTrainee(int id);
        
        //scheduled shifts
        Task<ScheduledShift> GetScheduledShift(int id);
        Task<IEnumerable<ScheduledShift>> GetScheduledShifts(int patrolId, DateTime startsAt, DateTime endsAt);
        Task UpdateScheduledShift(ScheduledShift shift);
        Task InsertScheduledShift(ScheduledShift shift);
        Task DeleteScheduledShift(ScheduledShift shift);

        //shifts
        Task<IEnumerable<Shift>> GetShifts(int patrolId,int? startHour = null, int? startMinute = null, int? endHour = null, int? endMinute=null);
        Task<Shift> GetShift(int id);
        Task InsertShift(Shift shift);
        Task UpdateShift(Shift shift);
        Task DeleteShift(Shift shift);

        //scheduled shift assignments
        Task<IEnumerable<ScheduledShiftAssignmentDto>> GetScheduledShiftAssignments(int patrolId, int? userId=null, DateTime? from = null, DateTime? to = null, ShiftStatus? status=null);
        Task<ScheduledShiftAssignment> GetScheduledShiftAssignment(int id);
        Task<IEnumerable<ScheduledShiftAssignment>> GetScheduledShiftAssignmentsForScheduledShift(int scheduledShiftId);
        Task InsertScheduledShiftAssignment(ScheduledShiftAssignment shift);
        Task UpdateScheduledShiftAssignment(ScheduledShiftAssignment shift);
        Task DeleteScheduledShiftAssignment(ScheduledShiftAssignment shift);
        
    }
}
