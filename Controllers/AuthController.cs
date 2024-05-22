using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Dapper;
using DotnetApi.Data;
using DotnetApi.Dtos;
using DotnetApi.Models;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    [Authorize] //token management, telling the controller that this needs authorization, see login for workaround
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly ReusableSql _reusableSql;

        private readonly AuthHelper _authHelper;
        private readonly IMapper _mapper;

        public AuthController(IConfiguration config)
        {
            
            _dapper = new DataContextDapper(config);
            _reusableSql = new ReusableSql(config);
            _authHelper = new AuthHelper(config);
            _mapper = new Mapper(new MapperConfiguration((cfg) => 
            {
                cfg.CreateMap<UserForRegistrationDto, UserComplete>();
            }));
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if(userForRegistration.Password == userForRegistration.PasswordConfirm)
            {   
                DynamicParameters sqlParameters = new DynamicParameters();
                
                string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = @EmailParam";
                sqlParameters.Add("@EmailParam", userForRegistration.Email, DbType.String);

                IEnumerable<string> existingUsers = _dapper.LoadDataWithParameters<string>(sqlCheckUserExists, sqlParameters);
                if(existingUsers.Count() == 0)
                {
                    UserForLoginDto userForSetPassword = new UserForLoginDto(){
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password
                    };

                    if(_authHelper.SetPassword(userForSetPassword))//see authHelper for Hash and Salt creations
                    { 
                       UserComplete userComplete = _mapper.Map<UserComplete>(userForRegistration);
                       userComplete.Active = true;
        
                        if(_reusableSql.UpsertUser(userComplete))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to Add User");

                    }
                    throw new Exception("Failed to Register User");
                }
                throw new Exception("User with email already exists!");

            }
            throw new Exception("Passwords do not Match");
            
        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
        {
             if(_authHelper.SetPassword(userForSetPassword))
             {
                return Ok();
             }
             throw new Exception("Failed to update Password");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {   
            string sqlForHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get 
                @Email = @EmailParam";

            DynamicParameters sqlParameters = new DynamicParameters();

            // SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
            // emailParameter.Value = userForLogin.Email;
            // sqlParameters.Add(emailParameter);

            sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);

            UserForLoginConfirmationDto userForConfirmation = _dapper
                .LoadDataSingleWithParameters<UserForLoginConfirmationDto>(sqlForHashAndSalt, sqlParameters);    

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

            // if(passwordHash == userForConfirmation.PasswordHash) won't work
            for(int index = 0; index < passwordHash.Length; index++)
            {
                if(passwordHash[index] != userForConfirmation.PasswordHash[index])
                {
                    return StatusCode(401, "Incorrect Password");
                }
            }

            string userIdSql = @"
                Select [UserId] FROM TutorialAppSchema.Users WHERE EMAIL = '" + 
                userForLogin.Email + "'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string>{
                {"token", _authHelper.CreateToken(userId)}
            });
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";

            string userIdSql = "SELECT UserID FROM TutorialAppSchema.Users WHERE UserId = "
            + userId;

            int userIdFromDB = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string>{
                {"token", _authHelper.CreateToken(userIdFromDB)}
            });
        }

        
    }
}