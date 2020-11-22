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
        Task<IEnumerable<Signature>> GetSignaturesForAssignment(int assignmentId);
        Task<IEnumerable<SignatureDto>> GetSignaturesWithUsersForAssignment(int assignmentId);
        Task InsertSignature(Signature signature);
    }
}