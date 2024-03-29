﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class UserUser : BaseEntity
    {
        public long Id { get; set; }

        public User User { get; set; }

        public User FollowedUser { get; set; }

        public bool ReceiveNotifications { get; set; } = false;
    }
}
