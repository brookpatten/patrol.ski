using Amphibian.Patrol.Api.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Services
{
    public interface IAssignmentService
    {
        Task<AssignmentDto> GetAssignment(int id);
        Task CreateSignatures(int assignmentId, int byUserId, IList<NewSignatureDto> newSignatures);
        Task<bool> AllowCreateSignatures(int assignmentId, int byUserId, IList<NewSignatureDto> newSignatures);
        Task CreateAssignments(int planId, IList<int> toUserIds, DateTime? dueAt);
    }
}