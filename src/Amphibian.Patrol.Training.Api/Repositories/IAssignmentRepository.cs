using System;

using Amphibian.Patrol.Training.Api.Dtos;
using Amphibian.Patrol.Training.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public interface IAssignmentRepository
    {
        Task<Assignment> GetAssignment(int assignmentId);
        Task<IEnumerable<AssignmentHeaderDto>> GetAssignmentsForUser(int userId);
        Task<IEnumerable<AssignmentHeaderDto>> GetAssignmentsForPlan(int planId);
        Task<IEnumerable<AssignmentHeaderDto>> GetAssignments(int patrolId, int? planId, int? assignedToUserId, bool? complete);
        Task<IEnumerable<Signature>> GetSignaturesForAssignment(int assignmentId);
        Task<IEnumerable<SignatureDto>> GetSignaturesWithUsersForAssignment(int assignmentId);
        Task InsertSignature(Signature signature);
        Task UpdateAssignment(Assignment assignment);
        Task<IEnumerable<AssignmentHeaderDto>> GetIncompleteAssignments(int patrolId);
        Task<IEnumerable<AssignmentHeaderDto>> GetIncompleteAssignments(int patrolId, int userId);
        Task InsertAssignment(Assignment assignment);
        Task<IEnumerable<AssignmentCountByPlanByDayDto>> GetAssignmentCountsByDay(int patrolId, DateTime start, DateTime end);
        Task<IEnumerable<AssignmentProgressByDayDto>> GetAssignmentProgressByDay(int patrolId, DateTime start, DateTime end, int? planId, int? userId);
    }
}