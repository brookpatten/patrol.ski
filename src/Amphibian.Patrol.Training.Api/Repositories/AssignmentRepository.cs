using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;

using Amphibian.Patrol.Training.Api.Models;
using Amphibian.Patrol.Training.Api.Dtos;
using AutoMapper;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;

        public AssignmentRepository(IDbConnection connection, IMapper mapper)
        {
            _connection = connection;
            _mapper = mapper;
        }

        public Task<IEnumerable<Assignment>> GetAssignmentsForUser(int userId)
        {
            return _connection.QueryAsync<Assignment>(@"select Id,planid,userid,assignedat,dueat from assignments where userid=@userId", new { userId });
        }

        public Task<IEnumerable<Assignment>> GetAssignmentsForPlan(int planId)
        {
            return _connection.QueryAsync<Assignment>(@"select Id,planid,userid,assignedat,dueat from assignments where planId=@planId", new { planId });
        }

        public Task<Assignment> GetAssignment(int assignmentId)
        {
            return _connection.GetAsync<Assignment>(assignmentId);
        }

        public Task<IEnumerable<Signature>> GetSignaturesForAssignment(int assignmentId)
        {
            return _connection.QueryAsync<Signature>(@"select id,assignmentid,sectionskillid,sectionlevelid,signedbyuserid,signedat from signatures where assignmentid=@assignmentId", new { assignmentId });
        }

        public Task<IEnumerable<SignatureDto>> GetSignaturesWithUsersForAssignment(int assignmentId)
        {
            return _connection.QueryAsync<Signature,UserIdentifier,SignatureDto>(
                @"select 
                    s.id
                    ,s.assignmentid
                    ,s.sectionskillid
                    ,s.sectionlevelid
                    ,s.signedat
                    ,s.signedbyuserid
                    ,u.id
                    ,u.email
                    ,u.firstname
                    ,u.lastname
                from 
                    signatures s
                inner join users u on u.id=s.signedbyuserid
                where 
                    assignmentid=@assignmentId",(s,u)=>
                {
                    var dto = _mapper.Map<Signature, SignatureDto>(s);
                    dto.SignedBy = u;
                    return dto;
                },splitOn: "id",param:new { assignmentId });
        }

        public async Task InsertSignature(Signature signature)
        {
            await _connection.InsertAsync(signature);
        }
    }
}
