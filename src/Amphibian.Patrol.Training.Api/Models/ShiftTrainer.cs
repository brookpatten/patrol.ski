using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Models
{
    public class ShiftTrainer
    {
        public int Id { get; set; }
        public int TrainingShiftId { get; set; }
        public int TrainerUserId { get; set; }
    }
}
