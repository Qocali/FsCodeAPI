using Application.Interface.Repository;
using Domain.Entities;
using FsCodeProjectApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Moq;

namespace FsCodeTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async System.Threading.Tasks.Task GetAllReminderAsync()
        {
            var mockRepository = new Mock<IReminderRepo>();
            var expectedReminders = new List<Reminder>
            {
                new Reminder { Id = 1, Content = "Reminder 1" },
                new Reminder { Id = 2, Content = "Reminder 2" },
                // Add more sample reminders as needed
            };
            mockRepository.Setup(r => r.Read()).ReturnsAsync(expectedReminders);
            var controller = new ReminderController(mockRepository.Object,null,null,null);

            // Act
            var result = await controller.GetAllReminders();


            var okResult = result as OkObjectResult;
            var actualReminders = okResult.Value as List<Reminder>;
            Assert.AreEqual(expectedReminders, actualReminders);
        }
    }
}