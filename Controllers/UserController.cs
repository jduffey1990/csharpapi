using System.ComponentModel.Design;
using DotnetApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Dapper;
using DotnetApi.Data;
using DotnetApi.Models;
using DotnetApi.Dtos;
using System.Reflection.Emit;


namespace DotnetAPI.Controllers; 

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
 //This controller is present now to show the starting point, as this is a learning file.  We created all of the endpoints with single functionality.  Once we added stored procedures to the DB, we were able to simplify in the UserCompleteController.
{
    DataContextDapper _dapper;
    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnect")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    [HttpGet("GetUsers")]
    // public IActionResult test()
    public IEnumerable<User> GetUsers()
    {
        string sql = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
            FROM TutorialAppSchema.Users";
            IEnumerable<User> users = _dapper.LoadData<User>(sql);
            return users;
    }


    [HttpGet("GetSingleUser/{userId}")]
    // public IActionResult test()
    public User GetSingleUser(int userId)
    {
        string sql = @"
            SELECT [UserId],
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active] 
            FROM TutorialAppSchema.Users
            WHERE UserId =" + userId.ToString();
            User user = _dapper.LoadDataSingle<User>(sql);
            return user;
       
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
            UPDATE TutorialAppSchema.Users
            SET [FirstName] = @FirstName, 
                [LastName] = @LastName,
                [Email] = @Email,
                [Gender] = @Gender,
                [Active] = @Active
            WHERE UserId = @UserId";


        var parameters = new DynamicParameters();
        parameters.Add("@FirstName", user.FirstName);
        parameters.Add("@LastName", user.LastName);
        parameters.Add("@Email", user.Email);
        parameters.Add("@Gender", user.Gender);
        parameters.Add("@Active", user.Active);
        parameters.Add("@UserId", user.UserId);
                

        if(_dapper.ExecuteSQL(sql, parameters))
        {
            return Ok();
        }
       
       throw new Exception("Failed to Update User");
    }

    [HttpPost("AddUser")]
    // public IActionResult test()
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.Users (
                [FirstName],
                [LastName],
                [Email],
                [Gender],
                [Active]
            ) VALUES (
                @FirstName, 
                @LastName, 
                @Email, 
                @Gender, 
                @Active
            );";

        var parameters = new DynamicParameters();
        parameters.Add("@FirstName", user.FirstName);
        parameters.Add("@LastName", user.LastName);
        parameters.Add("@Email", user.Email);
        parameters.Add("@Gender", user.Gender);
        parameters.Add("@Active", user.Active);

        Console.WriteLine(sql);    

        if(_dapper.ExecuteSQL(sql, parameters))
        {
            return Ok();
        }
       
       throw new Exception("Failed to Add User");
       
    }
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"
        DELETE FROM TutorialAppSchema.Users
            WHERE UserId = " + userId.ToString();


        Console.WriteLine(sql);    

        if(_dapper.ExecuteSQL(sql))
        {
            return Ok();
        }
       
       throw new Exception("Failed to Delete User");

    }


    [HttpGet("UserSalary/{userId}")]
    // public IActionResult test()
    public IEnumerable<UserSalary> GetUserSalary(int userId)
    {
        return _dapper.LoadData<UserSalary>(
            @"SELECT UserSalary.UserId
                    , UserSalary.Salary
            FROM TutorialAppSchema.UserSalary
                WHERE UserId = " + userId.ToString()
        );
       
    }


    [HttpPost("UserSalary")]
    // public IActionResult test()
    public IActionResult PostUserSalary(UserSalary userSalaryForInsert)
    {
           string sql =  @"
            INSERT INTO TutorialAppSchema.UserSalary (
                    UserId,
                    Salary
                ) Values (" + userSalaryForInsert.UserId.ToString()
                + ", " + userSalaryForInsert.Salary.ToString()
                + ")";

            if(_dapper.ExecuteSQL(sql)){
                return Ok(userSalaryForInsert);
            }
            
        throw new Exception("Couldn't Add New Salary");
       
    }
    [HttpPut("UserSalary")]
    // public IActionResult test()
    public IActionResult PutUserSalary(UserSalary userSalaryForUpdate)
    {
           string sql =  @"
            UPDATE TutorialAppSchema.UserSalary SET Salary ="
            +userSalaryForUpdate.Salary.ToString()
            + " WHERE UserId=" + userSalaryForUpdate.UserId.ToString();

            if(_dapper.ExecuteSQL(sql)){
                return Ok(userSalaryForUpdate);
            }
            
        throw new Exception("Updating UserSalary Failed on Save");
       
    }

    [HttpDelete("UserSalary/{userId}")]
    // public IActionResult test()
    public IActionResult DeleteUserSalary(int userId)
    {
           string sql =  "DELETE FROM TutorialAppSchema.UserSalary WHERE UserId= " + userId.ToString();

            if(_dapper.ExecuteSQLWithRowCount(sql) > 0){
                return Ok();
            }
            
        throw new Exception("Deleting UserSalary Failed on Save");
       
    }

     [HttpGet("UserJobInfo/{userId}")]
    // public IActionResult test()
    public IEnumerable<UserJobInfo> GetUserJobInfo(int userId)
    {
        return _dapper.LoadData<UserJobInfo>(
            @"SELECT UserJobInfo.UserId
                    , UserJobInfo.JobTitle
                    , UserJobInfo.Department
            FROM TutorialAppScheman.UserJobInfo
                WHERE UserId = " + userId.ToString()
        );
       
    }


    [HttpPost("UserJobInfo")]
    // public IActionResult test()
    public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
    {
           string sql =  @"
            INSERT INTO TutorialAppSchema.UserJobInfo (
                    UserId,
                    JobTitle,
                    Department
                ) Values (" + userJobInfoForInsert.UserId.ToString()
                + ", '" + userJobInfoForInsert.JobTitle
                + "', '" + userJobInfoForInsert.Department
                + "')";

            if(_dapper.ExecuteSQL(sql)){
                return Ok(userJobInfoForInsert);
            }
            
        throw new Exception("Couldn't Add New Job Info");
       
    }
    [HttpPut("UserJobInfo")]
    // public IActionResult test()
    public IActionResult PutUserJobInfo(UserJobInfo userJobInfoForUpdate)
    {
           string sql =  @"
            UPDATE TutorialAppSchema.UserJobInfo SET JobTitle ='"
            +userJobInfoForUpdate.JobTitle
            + "', Department='"
            +userJobInfoForUpdate.Department
            + "' WHERE UserId=" + userJobInfoForUpdate.UserId.ToString();

            if(_dapper.ExecuteSQL(sql)){
                return Ok(userJobInfoForUpdate);
            }
            
        throw new Exception("Updating UserJobInfo Failed on Save");
       
    }

    [HttpDelete("UserJobInfo/{userId}")]
    // public IActionResult test()
    public IActionResult DeleteUserJobInfo(int userId)
    {
           string sql =  "DELETE FROM TutorialAppSchema.UserJobInfo WHERE UserId= " + userId.ToString();

            if(_dapper.ExecuteSQL(sql)){
                return Ok();
            }
            
        throw new Exception("Deleting UserJobInfo Failed on Save");
       
    }
}

