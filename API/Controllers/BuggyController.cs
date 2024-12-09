using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController (DataContext dataContext): BaseApiController
{
    [HttpGet("atuh")]
    public ActionResult<string> GetAuth(){
        return "Secret Text";
    }
    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound(){

        var thing = dataContext.Users.Find(-1);

        if(thing == null) return NotFound();

        return thing;
    }
    [HttpGet("server-error")]
    public ActionResult<string> GetAuth(){
        return "Secret Text";
    }
    [HttpGet("atuh")]
    public ActionResult<string> GetAuth(){
        return "Secret Text";
    }
    [HttpGet("atuh")]
    public ActionResult<string> GetAuth(){
        return "Secret Text";
    }
}
