using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Models
{
    public class Signature
    {
		public int Id { get; set; }
		public int AssignmentId { get; set; }
		public int SectionSkillId { get; set; }
		public int SectionLevelId { get; set; }
		public int SignedByUserId { get; set; }
		public DateTime SignedAt { get; set; }
	}
}
