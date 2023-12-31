using TodoLibrary.Models;

namespace TodoLibrary.DataAccess
{
    public interface ITodoData
    {
        Task CompleteTodo(int todoId, int assignedTo);
        Task<TodoModel?> Create(int assignedTo, string task);
        Task Delete(int todoId, int assignedTo);
        Task<List<TodoModel>> GetAllAssigned(int assignedTo);
        Task<TodoModel?> GetOneAssigned(int assignedTo, int todoId);
        Task UpdateTask(int todoId, int assignedTo, string task);
    }
}