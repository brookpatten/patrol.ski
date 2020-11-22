using Amphibian.Patrol.Training.Api.Dtos;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Services
{
    public interface IAssignmentService
    {
        Task<AssignmentDto> GetAssignment(int id);
    }
}