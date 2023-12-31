using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using TodoLibrary.DataAccess;
using TodoLibrary.Models;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Threading.Tasks;

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
    [HttpGet("{todoId}")]
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
    [HttpPut("{todoId}")]
    public async Task<IActionResult> Put(int todoId, [FromBody] string task)
    {
        await _data.UpdateTask(GetUserId(),todoId,task);

        return Ok();
    }

    // PUT api/Todos/5/Complete
    [HttpPut("{todoId}/Complete")]
    public async Task<IActionResult> Complete(int todoId)
    {
        await _data.CompleteTodo(todoId,GetUserId());

        return Ok();
    }

    // DELETE api/Todos/5
    [HttpDelete("{todoId}")]
    public async Task<IActionResult> Delete(int todoId)
    {
        await _data.Delete(todoId, GetUserId());

        return Ok();
    }
}
