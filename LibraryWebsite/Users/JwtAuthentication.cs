using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LibraryWebsite.Users
{
    public class JwtAuthentication
    {
        private readonly IConfiguration _configuration;

        public JwtAuthentication(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class SecurityConfig
        {
            public string Secret { get; set; }
        }

        private SymmetricSecurityKey SecurityKey()
        {
            // configure strongly typed settings objects
            var configSection = _configuration.GetSection("Security");
            var config = configSection.Get<SecurityConfig>();

            if (config == null || string.IsNullOrEmpty(config.Secret))
                throw new Exception("Security secret was not set.");

            var key = Encoding.ASCII.GetBytes(config.Secret);

            return new SymmetricSecurityKey(key);
        }

        public string GenerateSecurityToken(string userName, string[] roles)
        {
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = SecurityKey();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, userName),
                    }
                    .Concat(roles.Select(role => new Claim(ClaimTypes.Role, role)))),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(securityKey,
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public void SetupSecurity(IServiceCollection services)
        {
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = SecurityKey(),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
        }
    }
}