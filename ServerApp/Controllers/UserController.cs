using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServerApp.Data;
using ServerApp.DTO;
using ServerApp.models;
using ServerApp.Models;

//using ServerApp.Models;

namespace ServerApp.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<Role> _roleManager;

        private readonly IUserRepository _userRepository; 
        
        private readonly IMapper _mapper;
        public UserController(UserManager<User> userManager,SignInManager<User> signInManager,IConfiguration configuration,RoleManager<Role> roleManager,IUserRepository userRepository,IMapper mapper)
        {
            _userManager=userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _userRepository = userRepository;
             _mapper = mapper;
           
        }


    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = _userRepository.GetAllUsers();
        return Ok(users);
    }

    [HttpGet("{userName}")]
    public async Task<IActionResult> Get(string userName)
    {
        var user = await _userRepository.GetUserByUserNameAsync(userName);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("chat/{userName}")]
public async Task<IActionResult> GetUserForChat(string userName)
{
    var user = await _userRepository.GetUserByUserForChat(userName);

    if (user == null)
    {
        return NotFound();
    }

    return Ok(user);
}


[HttpPut("{id}")]
[Consumes("multipart/form-data")]
public async Task<IActionResult> UpdateUser(int id, [FromForm] UserForUpdateDTO userForUpdateDTO, [FromForm] IFormFile profilImage)
{
    try
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user == null)
        {
            return NotFound();
        }

        if (profilImage != null && profilImage.Length > 0)
{
    var extension = Path.GetExtension(profilImage.FileName);
    var randomName = $"{Guid.NewGuid()}{extension}";
    var videoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", randomName);

    using (var Stream = new FileStream(videoPath, FileMode.Create))
    {
        await profilImage.CopyToAsync(Stream);
    }

    userForUpdateDTO.userImg = randomName;
  
}


        _mapper.Map(userForUpdateDTO, user);

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return NoContent();
        }

        // If update fails, retrieve detailed error messages
        var errors = result.Errors.Select(e => e.Description).ToList();

        // Construct error message
        var errorMessage = $"Update failed for user with ID {id}. Errors: {string.Join(",", errors)}";

        return BadRequest(errorMessage);
    }
    catch (FileNotFoundException ex)
    {
        // Handle the exception when the file is not found
        return BadRequest($"File not found. Exception message: {ex.Message}");
    }
    catch (DirectoryNotFoundException ex)
    {
        // Handle the exception when the directory is not found
        return BadRequest($"Directory not found. Exception message: {ex.Message}");
    }
    catch (PathTooLongException ex)
    {
        // Handle the exception when the path is too long
        return BadRequest($"Path too long. Exception message: {ex.Message}");
    }
    catch (IOException ex)
    {
        // Handle the exception when there is an I/O error
        return StatusCode(StatusCodes.Status500InternalServerError, $"I/O error. Exception message: {ex.Message}");
    }
    catch (UnauthorizedAccessException ex)
    {
        // Handle the exception when access is denied
        return BadRequest($"Access denied. Exception message: {ex.Message}");
    }
    catch (Exception ex)
    {
        // Handle other types of exceptions that were not anticipated
        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
    }
}



[HttpPost("register")]
public async Task<IActionResult> Register(UserForRegisterDTO model)
{
    var user = new User
    {
        UserName = model.UserName,
        Email = model.Email,
        FullName = model.FullName
    };

    var result = await _userManager.CreateAsync(user, model.Password);

    if (!result.Succeeded)
    {
        return BadRequest(new
        {
            message = "An error occurred while creating the user"
        });
    }

    // Default role is "User"
    await _userManager.AddToRoleAsync(user, "User");

    return Ok(new
    {
        token = GenerateJwtToken(user, new List<string> { "User" })
    });
}

        
       [HttpPost("login")] 
      public async Task<IActionResult> Login(UserForLoginDTO model){
          
          var user= await _userManager.FindByNameAsync(model.UserName);
         

          if(user==null){
            return BadRequest(new{
                message="username is incorrect"
            });
          }

         var result= await _signInManager.CheckPasswordSignInAsync(user,model.Password,false);

         List<string> roles = (List<string>)await _userManager.GetRolesAsync(user);
         

         if(result.Succeeded){
            return Ok(new{
              token= GenerateJwtToken(user,roles)
             
            });
         }

         return Unauthorized();

      }

        private string GenerateJwtToken(User user, List<string> roles)
        {
            var tokenHandler= new JwtSecurityTokenHandler();
            var key= Encoding.ASCII.GetBytes( _configuration.GetSection("AppSettings:Secret").Value);
            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject= new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.UserName) ,
                    new Claim(ClaimTypes.Role,String.Join(",",roles))

                }),

                
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)

            };
            
         
             

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }












        
    }

}