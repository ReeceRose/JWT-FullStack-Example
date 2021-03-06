﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JWT.Application.User.Query.GenerateLoginToken
{
    public class GenerateLoginTokenQueryHandler :IRequestHandler<GenerateLoginTokenQuery, string>
    {
        private readonly IConfiguration _configuration;

        public GenerateLoginTokenQueryHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<string> Handle(GenerateLoginTokenQuery request, CancellationToken cancellationToken)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
            
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: request.Claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signingCredentials
            );
            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}