using Microsoft.AspNetCore.Mvc;
using DotnetApi.Data;



namespace DotnetAPI.Controllers;


[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public TestController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);

    }

    [HttpGet("Connect")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }
    [HttpGet]
    public string Test()
    {
        return "Your application is up and running";
    }


   
}

