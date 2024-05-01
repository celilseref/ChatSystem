using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServerApp.DTO;
using ServerApp.models;
using ServerApp.Models;

namespace ServerApp.Data
{
    public class UserRepository : IUserRepository
    {

       private readonly ChatContext _context;

   public UserRepository(ChatContext context)
    {
        _context = context;
    }

    public User GetUserByUserName(string userName)
    {
        return _context.Users.FirstOrDefault(u => u.UserName == userName);
    }

        public IEnumerable<User> GetAllUsers()
{
    return _context.Users.Select(u => new User
    {
        FullName = u.FullName,
        UserName = u.UserName,
        IsActive=u.IsActive
    }).ToList();
}

 public async Task<UserForProfileDto> GetUserByUserNameAsync(string userName)
    {
        var user = await _context.Users
            .Where(u => u.UserName == userName)
            .Select(u => new UserForProfileDto
            {
                FullName = u.FullName,
                UserImg = u.userImg,
                Bio = u.Bio,
                UserName = u.UserName,
                Email = u.Email,
                IsActive=u.IsActive
            })
            .FirstOrDefaultAsync();

        return user;
    }

public async Task<UserFoRChatInfoDTO> GetUserByUserForChat(string userName)
{
    var user = await _context.Users
        .Where(u => u.UserName == userName)
        .Select(u => new UserFoRChatInfoDTO
        {
            FullName = u.FullName,
            UserImg = u.userImg,
            UserName = u.UserName,
            IsActive=u.IsActive
        })
        .FirstOrDefaultAsync();

    return user;
}


    }

    
}