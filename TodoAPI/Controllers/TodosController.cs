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
    private readonly ILogger<TodosController> _logger;

    public TodosController(ITodoData data, ILogger<TodosController> logger)
    {
        _data = data;
        _logger = logger;
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
        try
        {
            _logger.LogInformation("The GET call to api/Todos was called.");
            var output = await _data.GetAllAssigned(GetUserId());
            return Ok(output);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "The GET call to api/Todos failed.");
            return BadRequest("Something went wrong!");
        }

    }

    // GET api/Todos/5
    [HttpGet("{todoId}")]
    public async Task<ActionResult<TodoModel>> Get(int todoId)
    {
        try
        {
            _logger.LogInformation("The GET call to api/Todos/{TodoId} was called.",todoId);
            var output = await _data.GetOneAssigned(GetUserId(), todoId);
            return Ok(output);
        }
        catch (Exception e)
        {
            _logger.LogError(e, 
                "The GET call to {ApiPath} failed. The id was {TodoId}",
                $"api/Todos/Id",todoId);
            return BadRequest("Something went wrong!");
        }
    }

    // POST api/Todos
    [HttpPost]
    public async Task<ActionResult<TodoModel>> Post([FromBody] string task)
    {
        try
        {
            _logger.LogInformation("The POST call to api/Todos was called. Task value was {Task}",task);
            var output = await _data.Create(GetUserId(), task);
            return Ok(output);
        }
        catch (Exception e)
        {
            _logger.LogError(e,
                "The POST call to api/Todos failed. Task value was {Task}",
                task);
            return BadRequest("Something went wrong!");
        }
    }

    // PUT api/Todos/5
    [HttpPut("{todoId}")]
    public async Task<IActionResult> Put(int todoId, [FromBody] string task)
    {
        try
        {
            _logger.LogInformation("The PUT call to api/Todos/{TodoId} was called. Update Task value was {Task}",todoId,task);
            await _data.UpdateTask(GetUserId(), todoId, task);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"The PUT call to api/Todos/{TodoId} failed. Update Task value was {Task}",todoId,task);
            return BadRequest("Something went wrong!");
        }
    }

    // PUT api/Todos/5/Complete
    [HttpPut("{todoId}/Complete")]
    public async Task<IActionResult> Complete(int todoId)
    {
        try
        {
            _logger.LogInformation("The PUT call to api/Todos/{TodoId}/Complete was called.",todoId);
            await _data.CompleteTodo(todoId, GetUserId());
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest("Something went wrong!");
        }
    }

    // DELETE api/Todos/5
    [HttpDelete("{todoId}")]
    public async Task<IActionResult> Delete(int todoId)
    {
        try
        {
            _logger.LogInformation("The DELETE call to api/Todos/{TodoId} was called.",todoId);
            await _data.Delete(todoId, GetUserId());
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e,"The DELETE call to api/Todos/{TodoId} failed.",todoId);
            return BadRequest("Something went wrong");
        }
    }
}
