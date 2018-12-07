﻿using System;
using System.CodeDom.Compiler;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using JWT.Application.Token.Query.GetToken;
using JWT.Application.Users.Queries.GetUserByEmail;
using JWT.Domain.Exceptions;
using JWT.Infrastructure.Notifications;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JWT.Application.Users.Queries.LoginUser
{
    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, string>
    {
        private readonly IMediator _mediator;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;


        public LoginUserQueryHandler(IMediator mediator, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _mediator = mediator;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        
        public async Task<string> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            var user = _mediator.Send(new GetUserByEmailQuery(request.Email), cancellationToken).Result;
            
            if (user == null)
            {
                throw new InvalidCredentialException();
            }
            
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
            
            if (!result.Succeeded)
            {
                throw new InvalidCredentialException();
            }
            if (result.IsLockedOut)
            {
                throw new AccountLockedException();
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                //TODO: Generate a custom exception
                throw new InvalidCastException();
            }

            return await _mediator.Send(new GetTokenQuery(_userManager.GetClaimsAsync(user).Result), cancellationToken: cancellationToken);
        }
    }
}