using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController(DataContext dataContext) : BaseApiController
{
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth()
    {
        return "Secret Text";
    }
    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {

        var thing = dataContext.Users.Find(-1);

        if (thing == null) return NotFound();

        return thing;
    }
    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {

        return BadRequest("This was not a Good Request");
    }
    [HttpGet("server-error")]
    public ActionResult<AppUser> GetServerError()
    {

        var thing = dataContext.Users.Find(-1) ?? throw new Exception("A bad thing has happened");

        return thing;
    }
}
