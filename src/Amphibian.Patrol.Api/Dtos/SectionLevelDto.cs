﻿using Amphibian.Patrol.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Api.Dtos
{
    public class SectionLevelDto
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public int ColumnIndex { get; set; }
        public Level Level { get; set; }
        public int LevelId { get; set; }
    }
}
