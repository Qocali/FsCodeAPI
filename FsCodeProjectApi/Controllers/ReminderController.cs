using Application.Interface.Repository;
using Application.Interface.Services;
using Application.Validation;
using AutoMapper;
using Domain.Dtos;
using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;

namespace FsCodeProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : ControllerBase
    {
        private readonly IReminderRepo _repository;
        private readonly IEmailService _emailService;
        private readonly ITelegramService _telegramService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        public ReminderController(IWebHostEnvironment environment,IReminderRepo repository, IMapper mapper, IEmailService emailService, ITelegramService telegramService)
        {
            _repository = repository;
            _emailService = emailService;
            _telegramService = telegramService;
            _mapper= mapper;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReminders()
        {
            var reminders =await _repository.Read();
            return Ok(reminders);
        }
        [HttpGet("{page}")]
        public async Task<IActionResult> Getpagination(int page,int pageresult)
        {
            var reminder = _repository.Read();
           var pagecount = Math.Ceiling((decimal)reminder.Result.Count/pageresult);
            var reminder2=_repository.Read().Result.Skip((page-1)*pageresult).Take(pageresult).ToList();
            var response=new ReminderResponse
            {
                Reminders=reminder2,
                CurrentPage=page,
                Pages= (int)pagecount
            };
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> CreateReminder(CreateReminderDto reminder)
        {
            var ReminderValidator = new ReminderValidator();

            // Call Validate or ValidateAsync and pass the object which needs to be validated
            var result = ReminderValidator.Validate(reminder);

            if (!result.IsValid)
            {
                return BadRequest("have a false data value!!!");
            }
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
            var remin = _mapper.Map<Reminder>(reminder);
            (System.Threading.Tasks.Task.Delay(timeSpan)).ContinueWith(async _ =>
            {
                // Retrieve the reminder from the repository
                var savedReminder = _repository.GetById(remin.Id);

                if (reminder.Method == "email")
                {
                    await _emailService.SendEmailAsync(reminder.To, "Reminder", reminder.Content);
                }
                else if (reminder.Method == "telegram")
                {
                    await _telegramService.SendMessageAsync(reminder.To, reminder.Content);
                }
            });
            await _repository.Create(remin);
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
        

        [HttpPost("file upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Generate a unique file name to prevent conflicts
                var fileName = Path.GetFileName(file.FileName);
                var uploadDirectory = Path.Combine(_environment.ContentRootPath, "uploads");

                // Create the directory if it doesn't exist
                Directory.CreateDirectory(uploadDirectory);

                var filePath = Path.Combine(uploadDirectory, fileName);

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Optionally, process the uploaded file here

                return Ok("File uploaded successfully.");
            }

            return BadRequest("No file was uploaded.");
        }
        private bool IsValidChatId(string chatId)
        {
            // Add your validation logic for chat IDs here
            // e.g., check if the chat ID exists in the Telegram system
            // For simplicity, we assume any non-empty string is a valid chat ID
            return !string.IsNullOrEmpty(chatId);
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportReminderAsync()
        {
            // Retrieve your product data from the database or any other source
            var reminders = await _repository.Read();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Reminders");

                // Add headers
                worksheet.Cells["A1"].Value = "Content";
                worksheet.Cells["B1"].Value = "Method";
                worksheet.Cells["C1"].Value = "To";

                // Add data rows
                var row = 2;
                foreach (var item in reminders)
                {
                    worksheet.Cells[row, 1].Value = item.Content;
                    worksheet.Cells[row, 2].Value = item.Method;
                    worksheet.Cells[row, 3].Value = item.To;
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Generate the Excel file contents
                var fileContents = package.GetAsByteArray();

                // Set the content type and file name for the response
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "Reminder.xlsx";

                // Return the Excel file as the response
                return File(fileContents, contentType, fileName);
            }
        }
    }
}
