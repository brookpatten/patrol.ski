using Amphibian.Patrol.Api.Dtos;
using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface IScheduleService
    {
        //admin functions
        Task<ScheduledShift> ScheduleShift(ScheduledShiftUpdateDto dto);
        Task CancelShift(int scheduledShiftId);
        Task ApproveShiftSwap(int scheduledShiftAssignmentId, int userId);
        Task RejectShiftSwap(int scheduledShiftAssignmentId, int userId);
        Task CancelScheduledShiftAssignment(int scheduledShiftAssignmentId);
        Task<ScheduledShiftAssignment> AddScheduledShiftAssignment(int scheduledShiftId, int? userId);

        //swap functions
        Task ReleaseShift(int scheduledShiftAssignmentId);
        Task ClaimShift(int scheduledShiftAssignmentId, int userId);
        Task CancelShiftRelease(int scheduledShiftAssignmentId);

        //utilities
        Task<IEnumerable<ScheduledShiftAssignmentDto>> ReplicatePeriod(int patrolId ,bool clearTargetPeriodFirst ,bool testOnly ,DateTime replicatedPeriodStart, DateTime replicatedPeriodEnd, DateTime targetPeriodStart, DateTime targetPeriodEnd, bool replicateWorkItems = true);
    }
}
