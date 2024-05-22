using System.ComponentModel.Design;
using DotnetApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Dapper;
using DotnetApi.Data;
using DotnetApi.Models;
using DotnetApi.Dtos;
using System.Reflection.Emit;
using System.Data;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;


namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly ReusableSql _reusableSql;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _reusableSql = new ReusableSql(config);
    }

    [HttpGet("TestConnect")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    [HttpGet("GetUsers/{userId}/{active}")]
    // public IActionResult test()
    public IEnumerable<UserComplete> GetUsers(int userId, bool active)
    {
        string sql = "EXEC TutorialAppSchema.spUsers_Get";

        string parameters = "";
        DynamicParameters sqlParameters = new DynamicParameters();
        
        if(userId != 0)
        {
            parameters += ", @UserId = @UserIdParam";
            sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
        }
        if(active)
        {
            parameters+= ", @Active = @ActiveParam";
            sqlParameters.Add("@ActiveParam", active, DbType.Boolean);
        }

        if(parameters.Length > 0)
        {
            sql += parameters.Substring(1);
        }

            IEnumerable<UserComplete> users = _dapper.LoadDataWithParameters<UserComplete>(sql, sqlParameters);
            return users;
    }


    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {
        if(_reusableSql.UpsertUser(user))
        {
            return Ok();
        }
       
       throw new Exception("Failed to Update User");
    }

  
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Delete 
            @UserId = @UserIdParam";

        DynamicParameters sqlParameters = new DynamicParameters();

        sqlParameters.Add("@UserIdParam", userId, DbType.Int32);

        if(_dapper.ExecuteSQL(sql, sqlParameters))
        {
            return Ok();
        }
       
       throw new Exception("Failed to Delete User");

    }


   
}

