using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Dtos;
using Amphibian.Patrol.Training.Api.Repositories;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Amphibian.Patrol.Training.Api.Services
{
    public class AssignmentService : IAssignmentService
    {
        private IAssignmentRepository _assignmentRepository;
        private ILogger<AssignmentService> _logger;
        private IMapper _mapper;
        public AssignmentService(IAssignmentRepository assignmentRepository, ILogger<AssignmentService> logger, IMapper mapper)
        {
            _assignmentRepository = assignmentRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<AssignmentDto> GetAssignment(int id)
        {
            var assignment = await _assignmentRepository.GetAssignment(id);
            var signatures = await _assignmentRepository.GetSignaturesForAssignment(id);

            var assignmentDto = _mapper.Map<Assignment, AssignmentDto>(assignment);
            assignmentDto.Signatures = _mapper.Map<IEnumerable<Signature>, IEnumerable<SignatureDto>>(signatures);

            return assignmentDto;
        }
    }
}
