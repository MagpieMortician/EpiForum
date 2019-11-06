using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EpiAPI.Services;
using EpiAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MessageBoard.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private readonly EpiAPIContext _db;

        public UserController(IUserService userService, EpiAPIContext db)
        {
            _userService = userService;
            _db = db;
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll()
        {
            var users = _db.Users.Include(u => u.Questions).ThenInclude(u => u.Answers).AsQueryable();
            return Ok(users);
        }

      [HttpPost]
        public void Post([FromBody] User newUser)
        {
            _db.Users.Add(newUser);
            _db.SaveChanges();
        }
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]User userParam)
        {
            Console.WriteLine(userParam.Username);
            Console.WriteLine(userParam.Password);
            var user = _userService.Authenticate(userParam.Username, userParam.Password);
            System.Console.WriteLine();

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }
        [Authorize]
        [HttpGet("userpage")]  
        public ActionResult<User> GetUserPage()
        {   
            var identity = (ClaimsIdentity)User.Identity;
            var foundId = identity.FindFirst(ClaimTypes.Name).Value;
            Console.WriteLine("this is from the controller" + identity);
            Console.WriteLine("this is the second log from the controller" + foundId);
            User foundUser = _db.Users.Include(u => u.Questions).ThenInclude(u => u.Answers).FirstOrDefault(u => u.UserID == Convert.ToInt32(foundId));
            return foundUser;
        }
    }
}