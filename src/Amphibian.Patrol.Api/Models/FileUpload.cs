using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public class FileUpload
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? PatrolId { get; set; }
        public string Name { get; set; }
        public long FileSize { get; set; }
        public string GetUniqueName()
        {
            return $"{(PatrolId.HasValue ? PatrolId.Value + "-" : "")}{UserId.Value}-{Id}-{Name}";
        }
    }
}
