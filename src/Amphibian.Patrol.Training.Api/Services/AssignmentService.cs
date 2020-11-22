﻿using System;
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
            var signatures = await _assignmentRepository.GetSignaturesWithUsersForAssignment(id);

            var assignmentDto = _mapper.Map<Assignment, AssignmentDto>(assignment);
            assignmentDto.Signatures = signatures;

            return assignmentDto;
        }

        public async Task CreateSignatures(int assignmentId, int byUserId, IList<NewSignatureDto> newSignatures)
        {
            var existingSignatures = await _assignmentRepository.GetSignaturesForAssignment(assignmentId);
            var now = DateTime.UtcNow;

            //remove any "new" signatures that were previously signed by someone else
            var uniqueSignatures = newSignatures.Where(x => !existingSignatures.Any(y => y.SectionLevelId == x.SectionLevelId && y.SectionSkillId == x.SectionSkillId));

            foreach(var sig in uniqueSignatures)
            {
                var signature = new Signature()
                {
                    AssignmentId = assignmentId,
                    SectionLevelId = sig.SectionLevelId,
                    SectionSkillId = sig.SectionSkillId,
                    SignedByUserId = byUserId,
                    SignedAt = now
                };
                await _assignmentRepository.InsertSignature(signature);
            }
        }
    }
}
