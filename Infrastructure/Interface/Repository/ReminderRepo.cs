using Application.Interface.Repository;
using Domain.Entities;
using infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Interface.Repository
{
    public class ReminderRepo : IReminderRepo
    {
        private readonly AppDbContext _db;
         public ReminderRepo(AppDbContext db)
        {
            _db = db;
        }

        public async Task Create(Reminder reminder)
        {
            _db.Reminders.Add(reminder);
            _db.SaveChanges();
        }
        public async Task<Reminder> GetById(int id)
        {
            return await _db.Reminders.FirstOrDefaultAsync(x=>x.Id==id);
        }

        public async Task Delete(int id)
        {
            var reminder = _db.Reminders.Find(id);
            if (reminder != null)
            {
                _db.Reminders.Remove(reminder);
                _db.SaveChanges();
            }
        }

        public async Task<List<Reminder>> Read()
        {
            var reminders =await _db.Reminders.ToListAsync();
            return reminders;
        }

        public async Task Update(Reminder reminder)
        {
            _db.Reminders.Update(reminder);
            _db.SaveChanges();
        }
    }
}
