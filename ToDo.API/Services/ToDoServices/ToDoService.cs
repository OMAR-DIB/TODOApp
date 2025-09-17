using ToDo.API.Dtos.ToDosDtos;
using ToDo.API.Mapping;
using ToDo.Data.Entities;
using ToDo.Data.Entities.@enum;
using ToDo.Data.Repositories;

namespace ToDo.API.Services.ToDoServices
{
    public class ToDoService : IToDoService
    {
        private readonly IGenericRepository<ToDos> _todoRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly ILogger<ToDoService> _logger;

        public ToDoService(
            IGenericRepository<ToDos> todoRepository,
            IGenericRepository<User> userRepository,
            ILogger<ToDoService> logger)
        {
            _todoRepository = todoRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ToDosResponseDto>> GetAllToDosAsync(int? userId = null)
        {
            try
            {
                IEnumerable<ToDos> todos;

                if (userId.HasValue)
                {
                    todos = await _todoRepository.GetManyByFilterAsync(
                        t => t.UserId == userId.Value,
                        "User,SubTasks"
                    );
                }
                else
                {
                    todos = await _todoRepository.GetAllAsync("User,SubTasks");
                }

                return todos.Select(t => t.ToResponseDto()).OrderByDescending(t => t.CreatedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving todos for user: {UserId}", userId);
                throw new InvalidOperationException("Failed to retrieve todos", ex);
            }
        }

        public async Task<IEnumerable<ToDosResponseDto>> GetToDosByStatusAsync(TaskStatuss status, int? userId = null)
        {
            try
            {
                IEnumerable<ToDos> todos;

                if (userId.HasValue)
                {
                    todos = await _todoRepository.GetManyByFilterAsync(
                        t => t.Status == status && t.UserId == userId.Value,
                        "User,SubTasks"
                    );
                }
                else
                {
                    todos = await _todoRepository.GetManyByFilterAsync(
                        t => t.Status == status,
                        "User,SubTasks"
                    );
                }

                return todos.Select(t => t.ToResponseDto()).OrderByDescending(t => t.CreatedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving todos by status: {Status} for user: {UserId}", status, userId);
                throw new InvalidOperationException($"Failed to retrieve todos by status: {status}", ex);
            }
        }

        public async Task<IEnumerable<ToDosResponseDto>> GetUserToDosAsync(int userId)
        {
            return await GetAllToDosAsync(userId);
        }

        public async Task<ToDosResponseDto?> GetToDoByIdAsync(int id, int? userId = null)
        {
            try
            {
                ToDos? todo;

                if (userId.HasValue)
                {
                    todo = await _todoRepository.GetOneByFilter(
                        t => t.Id == id && t.UserId == userId.Value,
                        "User,SubTasks"
                    );
                }
                else
                {
                    todo = await _todoRepository.GetByIDAsync(id, "User,SubTasks");
                }

                return todo?.ToResponseDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving todo with ID: {TodoId} for user: {UserId}", id, userId);
                throw new InvalidOperationException($"Failed to retrieve todo with ID: {id}", ex);
            }
        }

        public async Task<ToDosResponseDto> CreateToDoAsync(CreateToDosRequestDto dto, int userId)
        {
            try
            {
                // Verify user exists
                var userExists = await _userRepository.AnyAsync(u => u.Id == userId && !u.IsDeleted);
                if (!userExists)
                {
                    throw new InvalidOperationException($"User with ID {userId} not found");
                }

                var todo = dto.ToEntity(userId);
                var createdTodo = await _todoRepository.AddAsync(todo);
                await _todoRepository.SaveChangesAsync();

                // Fetch the created todo with navigation properties
                var result = await _todoRepository.GetByIDAsync(createdTodo.Id, "User,SubTasks");

                _logger.LogInformation("Todo created successfully with ID: {TodoId} for user: {UserId}", createdTodo.Id, userId);
                return result!.ToResponseDto();
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                _logger.LogError(ex, "Error occurred while creating todo for user: {UserId}", userId);
                throw new InvalidOperationException("Failed to create todo", ex);
            }
        }

        public async Task<ToDosResponseDto> UpdateToDoAsync(UpdateToDosRequestDto dto, int userId)
        {
            try
            {
                var existingTodo = await _todoRepository.GetOneByFilter(
                    t => t.Id == dto.Id && t.UserId == userId,
                    "User,SubTasks"
                );

                if (existingTodo == null)
                {
                    throw new InvalidOperationException($"Todo with ID {dto.Id} not found or you don't have permission to update it");
                }

                dto.UpdateEntity(existingTodo);
                await _todoRepository.UpdateAsync(existingTodo);
                await _todoRepository.SaveChangesAsync();

                _logger.LogInformation("Todo updated successfully with ID: {TodoId} for user: {UserId}", dto.Id, userId);
                return existingTodo.ToResponseDto();
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                _logger.LogError(ex, "Error occurred while updating todo with ID: {TodoId} for user: {UserId}", dto.Id, userId);
                throw new InvalidOperationException($"Failed to update todo with ID: {dto.Id}", ex);
            }
        }

        public async Task<bool> DeleteToDoAsync(int id, int userId)
        {
            try
            {
                var todo = await _todoRepository.GetOneByFilter(
                    t => t.Id == id && t.UserId == userId,
                    "SubTasks,Notifications"
                );

                if (todo == null)
                {
                    return false;
                }

                await _todoRepository.DeleteAsync(todo, softDelete: true);
                await _todoRepository.SaveChangesAsync();

                _logger.LogInformation("Todo deleted successfully with ID: {TodoId} for user: {UserId}", id, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting todo with ID: {TodoId} for user: {UserId}", id, userId);
                throw new InvalidOperationException($"Failed to delete todo with ID: {id}", ex);
            }
        }

        public async Task<bool> ToDoExistsAsync(int id, int? userId = null)
        {
            try
            {
                if (userId.HasValue)
                {
                    return await _todoRepository.AnyAsync(t => t.Id == id && t.UserId == userId.Value && !t.IsDeleted);
                }

                return await _todoRepository.AnyAsync(t => t.Id == id && !t.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if todo exists with ID: {TodoId} for user: {UserId}", id, userId);
                throw new InvalidOperationException($"Failed to check if todo exists with ID: {id}", ex);
            }
        }

        public async Task<IEnumerable<ToDosResponseDto>> GetToDosForDateAsync(DateOnly date, int? userId = null)
        {
            try
            {
                IEnumerable<ToDos> todos;

                if (userId.HasValue)
                {
                    todos = await _todoRepository.GetManyByFilterAsync(
                        t => t.ToDoAt == date && t.UserId == userId.Value,
                        "User,SubTasks"
                    );
                }
                else
                {
                    todos = await _todoRepository.GetManyByFilterAsync(
                        t => t.ToDoAt == date,
                        "User,SubTasks"
                    );
                }

                return todos.Select(t => t.ToResponseDto()).OrderByDescending(t => t.CreatedAt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving todos for date: {Date} and user: {UserId}", date, userId);
                throw new InvalidOperationException($"Failed to retrieve todos for date: {date}", ex);
            }
        }
    }
}
