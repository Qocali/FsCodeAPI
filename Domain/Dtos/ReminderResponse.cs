﻿using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos
{
    public class ReminderResponse
    {
        public List<Reminder> Reminders { get; set; }
        public int CurrentPage { get; set; }
        public int Pages{ get; set; }
    }
}
