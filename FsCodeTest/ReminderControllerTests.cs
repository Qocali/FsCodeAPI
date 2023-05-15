using Application.Interface.Repository;
using Application.Interface.Services;
using Domain.Dtos;
using Domain.Entities;
using FsCodeProjectApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Moq;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Threading.Tasks;
using AutoMapper;
using Application.mapping;

namespace FsCodeTest
{
    public class ReminderControllerTests
    {
        private ReminderController _controller;
        private Mock<IReminderRepo> _repositoryMock;
        private Mock<IEmailService> _emailServiceMock;
        private Mock<ITelegramService> _telegramServiceMock;
        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IReminderRepo>();
            _emailServiceMock = new Mock<IEmailService>();
            _telegramServiceMock = new Mock<ITelegramService>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<Reminderprofile>());
            var mapper = config.CreateMapper();
            mapper.Map<Reminder>(It.IsAny<CreateReminderDto>());
            _controller = new ReminderController(
                _repositoryMock.Object,
                mapper,
                _emailServiceMock.Object,
                _telegramServiceMock.Object);
        }
        [Test]
        public async System.Threading.Tasks.Task CreateReminder()
        {
            

            _repositoryMock.Setup(r => r.Create(It.IsAny<Reminder>()));
            var data = new CreateReminderDto { To = "baba@gmail.com",Content="testing",SendAt=DateTime.Now.AddDays(2),Method="email" };
            // Act
            var result = await _controller.CreateReminder(data);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
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