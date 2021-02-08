using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Models
{
    public class ApiLog
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Route { get; set; }
        public string QueryString { get; set; }
        public string Verb { get; set; }
        public DateTime StartedAt { get; set; }
        public int? DurationMs { get; set; }
        public int? ResponseCode { get; set; }
    }
    public class ApiLogDto:ApiLog
    {
        public UserIdentifier User { get; set; }
    }
    public class RouteMetrics
    {
        public string Route { get; set; }
        public int MinMs { get; set; }
        public int AvgMs { get; set; }
        public int MaxMs { get; set; }
        public int SumMs { get; set; }
    }
}
