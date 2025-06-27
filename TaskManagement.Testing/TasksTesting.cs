using Moq;
using Moq.AutoMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Dtos;
using TaskManagement.Entities;
using TaskManagement.Services;
using TaskManagement.Services.Interfaces;
using Xunit.Sdk;

namespace TaskManagement.Testing
{
    public class TasksTesting
    {
        private readonly AutoMocker _mocker;

        public TasksTesting()
        {
            _mocker = new AutoMocker();
        }
        [Fact]
        public async void GetAllTasks_WithValidTasks_ReturnsTasks()
        {
            //Arrange
            var mockRepo = new Mock<ITaskRepository>();
            mockRepo.Setup(r => r.AddAsync(It.IsAny<TaskItem>()))
                .Callback((TaskItem task) =>
                {
                    task.Id = 1;
                    task.Title = "Test Task 1";
                    task.Description = "Test Description 1";
                    task.EXPValue = 10;
                    task.Category = Category.Intelligence;
                })
                .Returns(Task.CompletedTask);

            //Act
            await taskService.AddAsync(new CreateTaskDto
            {
                Title = "Test Task 1",
                Description = "Test Description 1",
                EXPValue = 10,
                Category = Category.Intelligence
            });

            //Assert
            mockRepo.Verify(repo => repo.AddAsync(It.IsAny<TaskItem>()), Times.Once);

        }
    }
}
