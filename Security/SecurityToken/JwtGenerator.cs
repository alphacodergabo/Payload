﻿using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Security.SecurityToken.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Security.SecurityToken
{
    public class JwtGenerator : IJwtGenerador
    {
        private readonly IConfiguration _configuration;
        public JwtGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CrearToken(UserJWT usuario)
        {
            string encryptionKid = _configuration["secretKey"];
            var claims = new List<Claim>(){
                // new Claim("Nombre", "prueba"),
                // new Claim("Mail", "Claim normal"),
                // new Claim("Nombre_Completo", "usuario"),
                new Claim("dataUser", JsonConvert.SerializeObject(usuario))
            };


            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionKid));
            var signingCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
            var encryptionCredentials = new EncryptingCredentials(secret, JwtConstants.DirectKeyUseAlg, SecurityAlgorithms.Aes256CbcHmacSha512);
            var tokenOptions = new JwtSecurityTokenHandler().CreateJwtSecurityToken(new SecurityTokenDescriptor()
            {
                Audience = "audience",
                Issuer = "issuer",
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                EncryptingCredentials = encryptionCredentials,
                SigningCredentials = signingCredentials
            });

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}
