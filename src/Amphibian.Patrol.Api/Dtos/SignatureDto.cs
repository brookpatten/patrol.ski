using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Dtos
{
    public class SignatureDto
    {
		public int Id { get; set; }
		public int SectionSkillId { get; set; }
		public int SectionLevelId { get; set; }
		public DateTime SignedAt { get; set; }
		public UserIdentifier SignedBy { get; set; }
	}
}
