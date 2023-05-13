using AutoMapper;
using Domain.Dtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.mapping
{
   public class Reminderprofile:Profile
    {
        public Reminderprofile()
        {
            CreateMap<CreateReminderDto,Reminder>();
        }
    }
}
