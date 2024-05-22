using System.Data;
using Dapper;
using DotnetApi.Data;
using DotnetApi.Dtos;
using DotnetApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers
{
    [Authorize] //token management, telling the controller that this needs authorization, see login for workaround
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Posts/{postId}/{userId}/{searchValue}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchValue = "none")
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get";
            string stringParameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if(userId != 0)
            {
                stringParameters += ", @UserId = @UserIdParam";
                sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
            }
            if(postId != 0)
            {
                stringParameters += ", @PostId = @PostIdParam";
                sqlParameters.Add("@PostIdParam", postId, DbType.Int32);
            }
            if(searchValue.ToLower() != "none")
            {
                stringParameters += ", @SearchValue = @SearchValueParam";
                sqlParameters.Add("@SearchValueParam", searchValue, DbType.String);
            }    

            if(stringParameters.Length > 0)
            {
                sql += stringParameters.Substring(1);
            }

            IEnumerable<Post> posts = _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
            return posts;

        }

       

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId = @UserIdParam";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, DbType.Int32);

            return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);


        }

        [HttpPut("UpsertPost")]
        public IActionResult UpsertPost(Post postToUpsert)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                                @UserId = @UserIdParam
                                , @PostTitle = @PostTitleParam
                                , @PostContent = @PostContentParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, DbType.Int32);
            sqlParameters.Add("@PostTitleParam", postToUpsert.PostTitle, DbType.String);
            sqlParameters.Add("@PostContentParam", postToUpsert.PostContent, DbType.String);

            if(postToUpsert.PostId > 0)
            {
                sql += ", @PostId = @PostIdParam";
                sqlParameters.Add("@PostIdParam", postToUpsert.PostId, DbType.Int32);
            }
        
            if(_dapper.ExecuteSQL(sql, sqlParameters))
            {
                return Ok();
            }
            throw new Exception("Failed to Upsert a Post");
        }

        
        [HttpDelete("Post/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = @"EXEC TutorialAppSchema.spPost_Delete 
                           @UserId = @UserIdParam
                           , @PostId = @PostIdParam";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, DbType.Int32);
            sqlParameters.Add("@PostIdParam", postId, DbType.Int32);
    

            if(_dapper.ExecuteSQL(sql))
            {
                return Ok();
            }
        
            throw new Exception("Failed to Delete User");
        }

        
    }
}