using Application.Interface.Repository;
using Application.Interface.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FsCodeProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : ControllerBase
    {
        private readonly IReminderRepo _repository;
        private readonly IEmailService _emailService;
        private readonly ITelegramService _telegramService;
        public ReminderController(IReminderRepo repository, IEmailService emailService, ITelegramService telegramService)
        {
            _repository = repository;
            _emailService = emailService;
            _telegramService = telegramService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReminders()
        {
            var reminders =await _repository.Read();
            return Ok(reminders);
        }
        [HttpPost]
        public async Task<IActionResult> CreateReminder(Reminder reminder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Perform additional validation on reminder properties
            if (!IsValidEmail(reminder.To) && !IsValidChatId(reminder.To))
            {
                ModelState.AddModelError("To", "Invalid 'To' parameter");
                return BadRequest(ModelState);
            }

            if (reminder.SendAt <= DateTime.Now)
            {
                ModelState.AddModelError("SendAt", "SendAt parameter must be greater than the current date and time");
                return BadRequest(ModelState);
            }

            if (reminder.Method != "email" && reminder.Method != "telegram")
            {
                ModelState.AddModelError("Method", "Invalid 'Method' parameter");
                return BadRequest(ModelState);
            }
            var currentTime = DateTime.Now;
            var timeSpan = reminder.SendAt - currentTime;
            (System.Threading.Tasks.Task.Delay(timeSpan)).ContinueWith(async _ =>
            {
                // Retrieve the reminder from the repository
                var savedReminder = _repository.GetById(reminder.Id);

                if (reminder.Method == "email")
                {
                    await _emailService.SendEmailAsync(reminder.To, "Reminder", reminder.Content);
                }
                else if (reminder.Method == "telegram")
                {
                    await _telegramService.SendMessageAsync(reminder.To, reminder.Content);
                }
            });
            await _repository.Create(reminder);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReminder(int id, Reminder reminder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingReminder =await _repository.GetById(id);
            if (existingReminder == null)
            {
                return NotFound();
            }

            existingReminder.To = reminder.To;
            existingReminder.Content = reminder.Content;
                   existingReminder.SendAt = reminder.SendAt;
            existingReminder.Method = reminder.Method;

            _repository.Update(existingReminder);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteReminder(int id)
        {
            var reminder = _repository.GetById(id);
            if (reminder == null)
            {
                return NotFound();
            }

            _repository.Delete(id);
            return NoContent();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidChatId(string chatId)
        {
            // Add your validation logic for chat IDs here
            // e.g., check if the chat ID exists in the Telegram system
            // For simplicity, we assume any non-empty string is a valid chat ID
            return !string.IsNullOrEmpty(chatId);
        }
    }
}
