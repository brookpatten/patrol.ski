using Amphibian.Patrol.Training.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public interface IAssignmentRepository
    {
        Task<Assignment> GetAssignment(int assignmentId);
        Task<IEnumerable<Assignment>> GetAssignmentsForUser(int userId);
        Task<IEnumerable<Assignment>> GetAssignmentsForPlan(int planId);
        Task<IEnumerable<Signature>> GetSignaturesForAssignment(int assignmentId);
        Task InsertSignature(Signature signature);
    }
}