using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace TodoApi.Controllers
{    
    [Authorize]
    [Route("rems/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpGet("Test")]
        public IActionResult Test()
        {
            var userName = User.Identity.Name;

            return Ok($"Super secret content, I hope you've got clearance for this {userName}...");
        }

        // GET rems/account
        [AllowAnonymous]
        [HttpGet]
        public ActionResult<string> Get()
        {
          string authHeader = Request.Headers["Authorization"];

          if (authHeader != null && authHeader.StartsWith("Basic")) {
            string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
            Encoding encoding = Encoding.GetEncoding("utf-8");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int seperatorIndex = usernamePassword.IndexOf(':');

            var username = usernamePassword.Substring(0, seperatorIndex);
            var password = usernamePassword.Substring(seperatorIndex + 1);

            var claims = new[]
            {
                new Claim("name", username),
                new Claim("emailaddress", "testpersonTRALALA@test.nl"),
                new Claim("role", "user"),
                new Claim("fullname", "Peter van Dijk"),
                new Claim("customers", "4,89,100" ),
            };

            // var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("dd%88*377f6d&f£$$£$FdddFF33fssDG^!3dd%88*377f6d&f£$$£$FdddFF33fssDG^!3dd%88*377f6d&f£$$£$FdddFF33fssDG^!3dd%88*377f6d&f£$$£$FdddFF33fssDG^!3dd%88*377f6d&f£$$£$FdddFF33fssDG^!3"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5002",
                audience: "rems",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
          } else {
              //Handle what happens if that isn't the case
              throw new Exception("The authorization header is either empty or isn't Basic.");
          }

        }
    }
}
