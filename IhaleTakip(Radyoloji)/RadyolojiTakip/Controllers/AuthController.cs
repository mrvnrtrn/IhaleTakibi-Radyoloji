using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApiCore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Cors;
using RadyolojiDataService;

namespace WebApiCore.Controllers
{
    [Produces("application/json")]
    [Route("auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        private string Baglanti()
        {
            return _configuration["ConnectionStrings:DefaultConnection"];
        }

        [HttpGet("get")]
        // public IEnumerable<string> Get()
        public ActionResult Get()
        {
            return Ok(new string[] { "Servis çalışıyor !!" });
        }

        [HttpPost("register")]
        public async Task<ActionResult> InsertUser([FromBody] RegisterViewModel model)
        {
            var user = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                return Ok(new { Username = user.UserName });
            }
            else
                return BadRequest(new { Username = user.UserName });

        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model)
        {
            string kullanicitipi = "Customer";

            model = getUserKontrol(model);
            if (model != null)
            {
                var userData = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Username,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                if (model.Email == "engin@engin.com") { kullanicitipi = "Admin"; }

                var silmeok = await _userManager.DeleteAsync(userData);
                var result = await _userManager.CreateAsync(userData, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(userData, kullanicitipi);
                }

                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null /*&& await _userManager.CheckPasswordAsync(user, model.Password)*/  )
                {
                    var claim = new[] { new Claim(JwtRegisteredClaimNames.Sub, user.UserName) };
                    var signinKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));

                    int expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);

                    var token = new JwtSecurityToken(
                      issuer: _configuration["Jwt:Site"],
                      audience: _configuration["Jwt:Site"],
                      expires: DateTime.UtcNow.AddDays(expiryInMinutes),  //  Günlük
                      signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                    );

                    return Ok(
                      new
                      {
                          token = new JwtSecurityTokenHandler().WriteToken(token),
                          expiration = token.ValidTo,
                          username = user.UserName,
                          usertype = kullanicitipi,
                          model = CurrentUserInfo(model.Username,model.Password)
                      });;
                }
            }

            return Unauthorized();
        }

        private LoginViewModel getUserKontrol(LoginViewModel model)
        {
            var cnn = new SqlConnection(Baglanti());

            var usermodel = cnn.Query<UserModel>(" SELECT ref_no, ad_soyad, e_mail,sifre, kadro, departman, yetki, depo_kodu " +
                  " FROM tbl_personel  WHERE ad_soyad='" + model.Username.ToString().Trim() + "' AND sifre='" + model.Password.ToString().Trim() + "' ").FirstOrDefault();
            if (usermodel != null)
            {
                model.Email = usermodel.e_mail.Trim();
                return model;
            }
            else return null;
        }

        public IActionResult CurrentUserInfo(string Username, string Password)
        {
            try
            {
                var cnn = new SqlConnection(Baglanti());
                string SQL = @"SELECT * FROM vw_personel WHERE ad_soyad='" + Username.ToString().Trim() + "' AND sifre='" + Password.ToString().Trim() + "' ";
                var db_rows = cnn.Query<vw_personel>(SQL);
                return Ok(db_rows);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}