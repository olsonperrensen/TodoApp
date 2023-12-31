using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using TodoLibrary.DataAccess;
using TodoLibrary.Models;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace TodoAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodosController : ControllerBase

{
    private readonly ITodoData _data;
    public TodosController(ITodoData data)
    {
        _data = data;
    }

    private int GetUserId(){
        string userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        int userId = int.Parse(userIdText);
        return userId;
    }

    // GET: api/Todos
    [HttpGet]
    public async Task<ActionResult<List<TodoModel>>> Get()
    {
        var output = await _data.GetAllAssigned(GetUserId());
        return Ok(output);
    }

    // GET api/Todos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoModel>> Get(int todoId)
    {
        var output = await _data.GetOneAssigned(GetUserId(), todoId);
        return Ok(output);
    }

    // POST api/Todos
    [HttpPost]
    public async Task<ActionResult<TodoModel>> Post([FromBody] string task)
    {
        var output = await _data.Create(GetUserId(),task);
        return Ok(output);
    }
    
    // PUT api/Todos/5
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] string value)
    {
        throw new NotImplementedException();
    }

    // PUT api/Todos/5/Complete
    [HttpPut("{id}/Complete")]
    public IActionResult Complete(int id)
    {
        throw new NotImplementedException();
    }

    // DELETE api/Todos/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        throw new NotImplementedException();
    }
}
