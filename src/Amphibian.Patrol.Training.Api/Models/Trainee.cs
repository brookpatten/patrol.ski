using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Models
{
    public class Trainee
    {
        public int Id {get;set;}
        public int ShiftTrainerId { get; set; }
        public int TraineeUserId { get; set; }
    }
}
