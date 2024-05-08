using System.ComponentModel.Design;
using DotnetApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using DotnetApi.Models;
using DotnetApi.Dtos;
using System.Reflection.Emit;
using DotnetAPI.Data;
using AutoMapper;
using System.Linq;


namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    IUserRepository _userRepository;
    IMapper _mapper;
    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        _userRepository = userRepository;

        _mapper = new Mapper(new MapperConfiguration(cfg =>{
            cfg.CreateMap<UserToAddDto, User>();
            cfg.CreateMap<UserSalary, UserSalary>();
            cfg.CreateMap<UserJobInfo, UserJobInfo>();
        }));

    }

    [HttpGet("GetUsers")]
    // public IActionResult test()
    public IEnumerable<User> GetUsersEF()
    {
            IEnumerable<User> users = _userRepository.GetUsersEF();
            return users;
    }


    [HttpGet("GetSingleUser/{userId}")]
    // public IActionResult test()
    public User GetSingleUserEF(int userId)
    {
            return _userRepository.GetSingleUserEF(userId);
    }

    [HttpPut("EditUser")]
    public IActionResult EditUserEF(User user)
    {
        User? userDb = _userRepository.GetSingleUserEF(user.UserId);

            if(userDb != null)
            {
                userDb.Active = user.Active;
                userDb.FirstName = user.FirstName;
                userDb.LastName = user.LastName;
                userDb.Email = user.Email;
                userDb.Gender = user.Gender;
                if(_userRepository.SaveChanges())
                {
                    return Ok();
                }

                throw new Exception("Failed to Update User");
            }

       throw new Exception("Failed to Get User");
    }

    [HttpPost("AddUser")]
    // public IActionResult test()
    public IActionResult AddUserEF(UserToAddDto user)
    {
        User userDb = _mapper.Map<User>(user);

        _userRepository.AddEntity<User>(userDb);
        if(_userRepository.SaveChanges())
        {
            return Ok();
        }
            

       throw new Exception("Failed to Add User");
       
    }
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUserEF(int userId)
    {
        User? userDb = _userRepository.GetSingleUserEF(userId);
            if(userDb != null)
            {
                _userRepository.RemoveEntity<User>(userDb);
                if(_userRepository.SaveChanges())
                {
                    return Ok();
                }

                throw new Exception("Failed to Delete User");
            }

       throw new Exception("Failed to Get User");
    }

    [HttpGet("UserSalary/{userId}")]
    // public IActionResult test()
    public UserSalary GetUserSalaryEF(int userId)
    {
           return _userRepository.GetSingleUserSalaryEF(userId);
       
    }

    [HttpPost("UserSalary")]
    // public IActionResult test()
    public IActionResult PostUserSalaryEF(UserSalary userForInsert)
    {

        _userRepository.AddEntity<UserSalary>(userForInsert);
        if(_userRepository.SaveChanges())
        {
            return Ok();
        }
       throw new Exception("Adding UserSalary Failed on Save");
       
    }
    
    [HttpPut("UserSalary")]
    public IActionResult PutUserSalaryEF(UserSalary userForUpdate)
    {
        UserSalary? userToUpdate = _userRepository.GetSingleUserSalaryEF(userForUpdate.UserId);

            if(userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if(_userRepository.SaveChanges())
                {
                    return Ok();
                }

                throw new Exception("Failed to Update User");
            }

       throw new Exception("Failed to Get User");
    }

    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUserSalaryEF(int userId)
    {
        UserSalary? userToDelete = _userRepository.GetSingleUserSalaryEF(userId);

            if(userToDelete != null)
            {
                _userRepository.RemoveEntity<UserSalary>(userToDelete);
                if(_userRepository.SaveChanges())
                {
                    return Ok();
                }

                throw new Exception("Deleting UserSalary failed on Save");
            }

       throw new Exception("Failed to find UserSalary to Delete");

    }

    [HttpGet("UserJobInfo/{userId}")]
    // public IActionResult test()
    public UserJobInfo GetUserJobInfoEF(int userId)
    {
           return _userRepository.GetSingleUserJobInfoEF(userId);
       
    }

    [HttpPost("UserJobInfo")]
    // public IActionResult test()
    public IActionResult PostUserJobInfoEF(UserJobInfo userForInsert)
    {

        _userRepository.AddEntity<UserJobInfo>(userForInsert);
        if(_userRepository.SaveChanges())
        {
            return Ok();
        }
       throw new Exception("Adding UserJobInfo Failed on Save");
       
    }
    
    [HttpPut("UserJobInfo")]
    public IActionResult PutUserJobInfoEF(UserJobInfo userForUpdate)
    {
        UserJobInfo? userToUpdate = _userRepository.GetSingleUserJobInfoEF(userForUpdate.UserId);

            if(userToUpdate != null)
            {
                _mapper.Map(userForUpdate, userToUpdate);
                if(_userRepository.SaveChanges())
                {
                    return Ok();
                }

                throw new Exception("Failed to Update User");
            }

       throw new Exception("Failed to Get User");
    }

    [HttpDelete("UserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfoEF(int userId)
    {
        UserJobInfo? userToDelete = _userRepository.GetSingleUserJobInfoEF(userId);

            if(userToDelete != null)
            {
                _userRepository.RemoveEntity<UserJobInfo>(userToDelete);
                if(_userRepository.SaveChanges())
                {
                    return Ok();
                }

                throw new Exception("Deleting UserJobInfo failed on Save");
            }

       throw new Exception("Failed to find UserJobInfo to Delete");

    }
}
