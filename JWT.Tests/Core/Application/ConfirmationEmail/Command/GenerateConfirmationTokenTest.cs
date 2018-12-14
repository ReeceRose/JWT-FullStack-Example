﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JWT.Application.ConfirmationEmail.Command;
using JWT.Tests.Helpers;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace JWT.Tests.Core.Application.ConfirmationEmail.Command
{
    public class GenerateConfirmationTokenTest
    {
        public List<IdentityUser> Users { get; set; }

        public Mock<MockUserManager> UserManager { get; }
        public GenerateConfirmationTokenCommandHandler Handler { get; }

        public GenerateConfirmationTokenTest()
        {
            // Arrange
            Users = new List<IdentityUser>()
            {
                new IdentityUser()
                {
                    Email = "test@test.ca",
                    UserName = "test@test.ca",
                    EmailConfirmed = false
                }
            };
            UserManager = new Mock<MockUserManager>();
            Handler = new GenerateConfirmationTokenCommandHandler(UserManager.Object);
        }

        [Fact]
        public void GenerateConfirmationToken_ShouldReturnToken()
        {
            // Arrange
            UserManager.Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<IdentityUser>())).Returns(Task.FromResult("1234567890"));
            // Act
            var token = Handler.Handle(new GenerateConfirmationTokenCommand(Users.First()), default(CancellationToken)).Result;
            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }
    }
}