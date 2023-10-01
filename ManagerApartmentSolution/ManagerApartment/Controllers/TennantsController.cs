﻿using ManagerApartment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace ManagerApartment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TennantsController : ControllerBase
    {
        private readonly IConfiguration _config;
        public TennantsController(IConfiguration config)
        {
            _config = config;
        }

        private ManagerApartmentContext _context = new ManagerApartmentContext();


        // GET: api/Tennants
        [HttpGet("getListTennants")]
        public async Task<ActionResult<IEnumerable<Tennant>>> GetListTennants()
        {
            if (_context.Tennants == null)
            {
                return NotFound("Tennants is null");
            }
            return await _context.Tennants.ToListAsync();
        }

        private string? GetCurrentEmail()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                Console.Write(userClaims.Count());

                foreach (var claim in userClaims)
                {
                    Console.WriteLine(claim.ToString());
                }

                return userClaims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            }
            return null;
        }

        //GET: api/Tennant/:Id
        [HttpGet("getTennantById")]
        public async Task<ActionResult<Tennant>> GetTennantById(int Id)
        {
            if (_context.Tennants == null)
            {
                return NotFound("Tennants is null");
            }
            var account = await _context.Tennants.FindAsync(Id);
            if (account == null)
            {
                return NotFound("Not found Id");
            }

            return account;
        }

        // PUT: api/Tennant/:Id
        [HttpPut("updateTennantById")]
        public async Task<IActionResult> PutAccount(int id, Tennant tennant)
        {
            if (id != tennant.TennantId)
            {
                return BadRequest("Update Fail! Please Check ID again");
            }

            _context.Entry(tennant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TennantExists(tennant.Email))
                {
                    return NotFound("TennantExists");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tennant/:Id    
        [HttpPost("createTennantInformation")]
        public async Task<ActionResult<Tennant>> CreateTennantInformation(Tennant tennant)
        {

            if (_context.Tennants == null)
            {
                return Problem("Entity set Tennants  is null.");
            }
            _context.Tennants.Add(tennant);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TennantExists(tennant.Email))
                {
                    return Conflict("TennantExists");
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTennant", new { id = tennant.Email }, tennant);
        }

        // DELETE: api/Tennant/:Id
        [HttpDelete("deleteTennantById")]
        public async Task<IActionResult> DeleteTennantById(int id)
        {
            if (_context.Tennants == null)
            {
                return NotFound("Tennants is null");
            }
            var account = await _context.Tennants.FindAsync(id);
            if (account == null)
            {
                return NotFound("Not Found Id");
            }

            _context.Tennants.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TennantExists(string id)
        {
            return (_context.Tennants?.Any(e => e.Email == id)).GetValueOrDefault();
        }

        // xác thực bởi token, và sẽ lấy body token ra làm dữ diệu 
        [Authorize]
        [HttpGet("Launch")]
        public async Task<ActionResult<Tennant>> Launch()
        {
            var extractedEmail = GetCurrentEmail();

            if (extractedEmail == null) return NotFound("Token hết hạn");

            var result = await _context.Tennants.FirstOrDefaultAsync(row => row.Email == extractedEmail);

            return Ok(result);
        }

        public class SignInBody
        {
            public string? Email { get; set; }
            public string? Password { get; set; }
        }

        public class SignInResponse
        {
            public Tennant Tennant { get; set; }
            public string AccessToken { get; set; }
        }

        private string GenerateToken(Tennant tennant)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // claim này dựa trên email trong tham số tennant
            var claims = new List<Claim>
            {
                new Claim("email", tennant.Email)
            };

            // tạo ra token
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                // có thời gian chết, 15 phút
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("loginIn")]
        public async Task<ActionResult<SignInResponse>> SignIn(SignInBody body)
        {
            if (_context.Tennants == null)
            {
                return Problem("Entity set Tennants  is null.");
            }

            //Account là cái bảng Account trong db
            //First Or Default Async là hàm tìm cái đầu tiên trong db, nếu có thì trả về Account, còn nếu không trả về null
            var result = await _context.Tennants.FirstOrDefaultAsync(

                // code trong xanh là cây
                // sẽ kiểm tra mỗi row trong cái bảng Account
                // nếu thuộc tính email của nó == thuộc tính email trong body + thuộc tính password của nó == thuộc tính password trong body
                // thì sẽ trả về cái row đó => LINQ 
                row =>
                row.Email == body.Email && row.Password == body.Password
                //
                );

            if (result != null)
                // trả về mã 200, và với kết quả thành công
                return Ok(new SignInResponse()
                {
                    Tennant = result,

                    // tạo ra accessToken dựa trên tài khoản
                    AccessToken = GenerateToken(result)
                });

            // trả về mã lỗi 404
            return NotFound("The Tennant is not existed");
        }
    }
}

