using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

using Dapper;
using Dommel;

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

        public Task<IEnumerable<AssignmentHeaderDto>> GetAssignmentsForUser(int userId)
        {
            return _connection.QueryAsync<AssignmentHeaderDto>(
                @"select 
                    a.Id
                    ,a.planid
                    ,a.userid
                    ,u.firstname userfirstname
                    ,u.lastname userlastname
                    ,a.assignedat
                    ,a.dueat
                    ,p2.name planname
                    ,(
                        select count(p.id)
	                    from plans p 
	                    inner join plansections ps on ps.planid=p.id
	                    inner join sectionskills ss on ss.sectionid=ps.sectionid
	                    inner join sectionlevels sl on sl.sectionid=ps.sectionid
	                    where p.id=a.planid
                    ) signaturesrequired
                    ,(
                        select count(id) from signatures s where s.assignmentid=a.id
                    ) signatures
                    from assignments a
                    inner join plans p2 on p2.id=a.planid
                    inner join users u on u.id=a.userid
                    where a.userid=@userId", new { userId });
        }

        public Task<IEnumerable<AssignmentHeaderDto>> GetAssignmentsForPlan(int planId)
        {
            return _connection.QueryAsync<AssignmentHeaderDto>(
                @"select 
                    a.Id
                    ,a.planid
                    ,a.userid
                    ,u.firstname userfirstname
                    ,u.lastname userlastname
                    ,a.assignedat
                    ,p.name planname
                    ,a.dueat 
                    ,(
                        select count(p.id)
	                    from plans p 
	                    inner join plansections ps on ps.planid=p.id
	                    inner join sectionskills ss on ss.sectionid=ps.sectionid
	                    inner join sectionlevels sl on sl.sectionid=ps.sectionid
	                    where p.id=a.planid
                    ) signaturesrequired
                    ,(
                        select count(id) from signatures s where s.assignmentid=a.id
                    ) signatures
                    from assignments a
                    inner join users u on u.id=a.userid
                    inner join plans p on p.patrolid=@patrolId
                    where a.planid=@planId", new { planId });
        }

        public Task<IEnumerable<AssignmentHeaderDto>> GetIncompleteAssignments(int patrolId)
        {
            return _connection.QueryAsync<AssignmentHeaderDto>(
                @"select 
                    a.Id
                    ,a.planid
                    ,a.userid
                    ,u.firstname userfirstname
                    ,u.lastname userlastname
                    ,a.assignedat
                    ,p.name planname
                    ,a.dueat 
                    ,(
                        select count(p.id)
	                    from plans p 
	                    inner join plansections ps on ps.planid=p.id
	                    inner join sectionskills ss on ss.sectionid=ps.sectionid
	                    inner join sectionlevels sl on sl.sectionid=ps.sectionid
	                    where p.id=a.planid
                    ) signaturesrequired
                    ,(
                        select count(id) from signatures s where s.assignmentid=a.id
                    ) signatures
                    from assignments a
                    inner join users u on u.id=a.userid
                    inner join plans p on p.patrolid=@patrolId
                    where a.completedat is null
                    order by u.lastname,p.name
                    ", new { patrolId });
        }

        public Task<Assignment> GetAssignment(int assignmentId)
        {
            return _connection.GetAsync<Assignment>(assignmentId);
        }

        public Task<IEnumerable<Signature>> GetSignaturesForAssignment(int assignmentId)
        {
            return _connection.SelectAsync<Signature>(x => x.AssignmentId == assignmentId);
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
            signature.Id=(int)await _connection.InsertAsync(signature);
        }

        public async Task UpdateAssignment(Assignment assignment)
        {
            await _connection.UpdateAsync(assignment);
        }
    }
}
