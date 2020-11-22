using Amphibian.Patrol.Training.Api.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Services
{
    public interface IAssignmentService
    {
        Task<AssignmentDto> GetAssignment(int id);
        Task CreateSignatures(int assignmentId, int byUserId, IList<NewSignatureDto> newSignatures);
    }
}