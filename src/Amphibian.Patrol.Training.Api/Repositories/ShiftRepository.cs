using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

using Dapper;
using Dommel;
using AutoMapper;

using Amphibian.Patrol.Training.Api.Dtos;
using Amphibian.Patrol.Training.Api.Models;

namespace Amphibian.Patrol.Training.Api.Repositories
{
    public class ShiftRepository: IShiftRepository
    {
        private readonly IDbConnection _connection;
        
        public ShiftRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<ShiftTrainerDto>> GetAvailableTrainerShiftsForTrainee(int patrolId, int traineeUserId, DateTime after)
        {
            return await _connection.QueryAsync<ShiftTrainerDto,UserIdentifier,ShiftTrainerDto>(
                @"select distinct
                    st.id
                    ,st.TrainingShiftId
                    ,ts.StartsAt
                    ,ts.EndsAt
                    , (select count(te.id) from trainees te where te.shifttrainerid=st.id) traineecount                    
                    ,u.id
                    ,u.Email
                    ,u.firstname
                    ,u.LastName
                    from shifttrainers st
                    inner join trainingshifts ts on 
	                    ts.Id = st.TrainingShiftId
	                    and ts.PatrolId = @patrolId
	                    and ts.StartsAt > @after
                    inner join users u on 
	                    u.id=st.TrainerUserId
                    --join assignments to exclude trainers who cannot help
                    inner join assignments asmnts on 
	                    asmnts.userid=@traineeUserId
	                    and asmnts.CompletedAt is null
                    inner join plans p on 
	                    p.id=asmnts.PlanId
                        and p.patrolId = @patrolId
                    inner join plansections ps on
	                    ps.planid=p.id
                    inner join sections s on 
	                    s.id=ps.SectionId
                    inner join sectiongroups sg on 
	                    sg.SectionId=s.id
                    inner join groups g on 
	                    g.id=sg.groupid
                    inner join groupusers gu on 
	                    gu.GroupId=g.id
	                    and gu.UserId = st.TrainerUserId
                    --join signatures to exclude things already signed
                    left join sectionskills ss on
	                    ss.SectionId = s.id
                    left join sectionlevels sl on
	                    sl.SectionId = s.Id
                    left join signatures sig on
	                    sig.AssignmentId = asmnts.id
	                    and sig.sectionskillid=ss.id
	                    and sig.sectionlevelid=sl.id
                    --join trainees to exclude shifts/trainers already signed up for
                    left join trainees t on 
	                    t.ShiftTrainerId = st.id
	                    and t.TraineeUserId = @traineeUserId
                    where 
	                    t.id is null 
	                    and sig.id is null
                    order by
	                    ts.startsat, u.lastname, u.firstname",
                (st,ui)=> 
                {
                    st.TrainerUser = ui;
                    return st;
                }
                , new { patrolId, traineeUserId, after });
        }
        public async Task<IEnumerable<ShiftTrainerDto>> GetCommittedTrainerShiftsForTrainee(int patrolId, int traineeUserId, DateTime after)
        {
            return await _connection.QueryAsync<ShiftTrainerDto,UserIdentifier,ShiftTrainerDto>(
                @"select
                st.id
                , st.TrainingShiftId
                , ts.StartsAt
                , ts.EndsAt
                , t.id traineeid
                , (select count(te.id) from trainees te where te.shifttrainerid=st.id) traineecount
                , u.id
                , u.Email
                , u.firstname
                , u.LastName
                from 
                    shifttrainers st
                inner join trainees t on
                    t.ShiftTrainerId = st.id
                    and t.TraineeUserId = @traineeUserId
                inner join trainingshifts ts on
                    ts.Id = st.TrainingShiftId
                    and ts.PatrolId = @patrolId
                    and ts.StartsAt > @after
                inner join users u on
                    u.id = st.TrainerUserId
                order by
	                    ts.startsat",
                (st, ui) =>
                {
                    st.TrainerUser = ui;
                    return st;
                }, new { patrolId, traineeUserId, after });
        }
        public async Task<IEnumerable<ShiftTrainerDto>> GetTrainerShifts(int patrolId, int trainerUserid, DateTime after)
        {
            return await _connection.QueryAsync<ShiftTrainerDto, UserIdentifier, ShiftTrainerDto>(
                @"select
                st.id
                , st.TrainingShiftId
                , ts.StartsAt
                , ts.EndsAt
                , (select count(te.id) from trainees te where te.shifttrainerid=st.id) traineecount
                , u.id
                , u.Email
                , u.firstname
                , u.LastName
                from 
                    shifttrainers st
                inner join trainingshifts ts on
                    ts.Id = st.TrainingShiftId
                    and ts.PatrolId = @patrolId
                    and ts.StartsAt > @after
                inner join users u on
                    u.id = st.TrainerUserId
                where
                    st.traineruserid = @trainerUserId
                order by
	                    ts.startsat",
                (st, ui) =>
                {
                    st.TrainerUser = ui;
                    return st;
                }, new { patrolId, trainerUserid, after });
        }
        public async Task<Trainee> InsertTrainee(Trainee trainee)
        {
            trainee.Id = (int)await _connection.InsertAsync(trainee)
                .ConfigureAwait(false);
            return trainee;
        }

        public async Task<Trainee> GetTrainee(int id)
        {
            return await _connection.GetAsync<Trainee>(id);
        }

        public async Task DeleteTrainee(Trainee trainee)
        {
            await _connection.DeleteAsync<Trainee>(trainee);
        }

        public async Task<ShiftTrainer> GetShiftTrainer(int id)
        {
            return await _connection.GetAsync<ShiftTrainer>(id);
        }

        public async Task<TrainingShift> GetTrainingShift(int id)
        {
            return await _connection.GetAsync<TrainingShift>(id);
        }
    }
}
