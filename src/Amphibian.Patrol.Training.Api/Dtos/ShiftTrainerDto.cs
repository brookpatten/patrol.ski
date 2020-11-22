using Amphibian.Patrol.Training.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Dtos
{
    public class ShiftTrainerDto
    {
        public int Id { get; set; }
        public int TrainingShiftId { get; set; }
        public int? TraineeId { get; set; }
        public DateTime StartsAt { get; set; }
        public DateTime EndsAt { get; set; }
        public UserIdentifier TrainerUser { get; set; }
        public int TraineeCount { get; set; }
    }
}
