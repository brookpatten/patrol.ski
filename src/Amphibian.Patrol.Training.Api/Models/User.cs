﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amphibian.Patrol.Training.Api.Models
{
    public class UserIdentifiers
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
    public class User: UserIdentifiers
    {
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
        public int? PasswordHashIterations { get; set; }
    }
}
