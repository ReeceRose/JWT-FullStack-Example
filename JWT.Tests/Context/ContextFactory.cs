﻿using System;
using JWT.Domain.Entities;
using JWT.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JWT.Tests.Context
{
    public class ContextFactory
    {
        public static ApplicationDbContext Create()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            context.Users.Add(new ApplicationUser() { Email = "test@test.ca", Id = "123", UserName = "test-user" });
            context.SaveChanges();
            return context;
        }

        public static void Destroy(ApplicationDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}