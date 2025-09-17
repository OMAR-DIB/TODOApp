using ToDo.API.Dtos.SubTaskDtos;
using ToDo.API.Mapping;
using ToDo.Data.Entities;
using ToDo.Data.Repositories;

namespace ToDo.API.Services.SubTaskServices
{
    public class SubTaskService : ISubTaskService
    {
        private readonly IGenericRepository<SubTask> _subTaskRepository;
        private readonly IGenericRepository<ToDos> _todoRepository;
        private readonly ILogger<SubTaskService> _logger;

        public SubTaskService(
            IGenericRepository<SubTask> subTaskRepository,
            IGenericRepository<ToDos> todoRepository,
            ILogger<SubTaskService> logger)
        {
            _subTaskRepository = subTaskRepository;
            _todoRepository = todoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<SubTaskResponseDto>> GetSubTasksByToDoAsync(int todoId, int userId)
        {
            try
            {
                // First verify that the todo belongs to the user
                var todoExists = await _todoRepository.AnyAsync(t => t.Id == todoId && t.UserId == userId && !t.IsDeleted);
                if (!todoExists)
                {
                    throw new InvalidOperationException($"ToDo with ID {todoId} not found or you don't have permission to access it");
                }

                var subTasks = await _subTaskRepository.GetManyByFilterAsync(
                    st => st.ToDoId == todoId,
                    "ToDo"
                );

                return subTasks.Select(st => st.ToResponseDto()).OrderByDescending(st => st.CreatedAt);
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                _logger.LogError(ex, "Error occurred while retrieving subtasks for todo: {TodoId} and user: {UserId}", todoId, userId);
                throw new InvalidOperationException($"Failed to retrieve subtasks for todo: {todoId}", ex);
            }
        }

        public async Task<SubTaskResponseDto?> GetSubTaskByIdAsync(int id, int userId)
        {
            try
            {
                var subTask = await _subTaskRepository.GetOneByFilter(
                    st => st.Id == id && st.ToDo!.UserId == userId,
                    "ToDo"
                );

                return subTask?.ToResponseDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving subtask with ID: {SubTaskId} for user: {UserId}", id, userId);
                throw new InvalidOperationException($"Failed to retrieve subtask with ID: {id}", ex);
            }
        }

        public async Task<SubTaskResponseDto> CreateSubTaskAsync(CreateSubTaskRequestDto dto, int userId)
        {
            try
            {
                // Verify that the todo belongs to the user
                var todoExists = await _todoRepository.AnyAsync(t => t.Id == dto.ToDoId && t.UserId == userId && !t.IsDeleted);
                if (!todoExists)
                {
                    throw new InvalidOperationException($"ToDo with ID {dto.ToDoId} not found or you don't have permission to add subtasks to it");
                }

                var subTask = dto.ToEntity();
                var createdSubTask = await _subTaskRepository.AddAsync(subTask);
                await _subTaskRepository.SaveChangesAsync();

                // Fetch with navigation properties
                var result = await _subTaskRepository.GetByIDAsync(createdSubTask.Id, "ToDo");

                _logger.LogInformation("SubTask created successfully with ID: {SubTaskId} for user: {UserId}", createdSubTask.Id, userId);
                return result!.ToResponseDto();
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                _logger.LogError(ex, "Error occurred while creating subtask for user: {UserId}", userId);
                throw new InvalidOperationException("Failed to create subtask", ex);
            }
        }

        public async Task<SubTaskResponseDto> UpdateSubTaskAsync(UpdateSubTaskRequestDto dto, int userId)
        {
            try
            {
                var existingSubTask = await _subTaskRepository.GetOneByFilter(
                    st => st.Id == dto.Id && st.ToDo!.UserId == userId,
                    "ToDo"
                );

                if (existingSubTask == null)
                {
                    throw new InvalidOperationException($"SubTask with ID {dto.Id} not found or you don't have permission to update it");
                }

                dto.UpdateEntity(existingSubTask);
                await _subTaskRepository.UpdateAsync(existingSubTask);
                await _subTaskRepository.SaveChangesAsync();

                _logger.LogInformation("SubTask updated successfully with ID: {SubTaskId} for user: {UserId}", dto.Id, userId);
                return existingSubTask.ToResponseDto();
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                _logger.LogError(ex, "Error occurred while updating subtask with ID: {SubTaskId} for user: {UserId}", dto.Id, userId);
                throw new InvalidOperationException($"Failed to update subtask with ID: {dto.Id}", ex);
            }
        }

        public async Task<bool> DeleteSubTaskAsync(int id, int userId)
        {
            try
            {
                var subTask = await _subTaskRepository.GetOneByFilter(
                    st => st.Id == id && st.ToDo!.UserId == userId,
                    "ToDo"
                );

                if (subTask == null)
                {
                    return false;
                }

                await _subTaskRepository.DeleteAsync(subTask, softDelete: true);
                await _subTaskRepository.SaveChangesAsync();

                _logger.LogInformation("SubTask deleted successfully with ID: {SubTaskId} for user: {UserId}", id, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting subtask with ID: {SubTaskId} for user: {UserId}", id, userId);
                throw new InvalidOperationException($"Failed to delete subtask with ID: {id}", ex);
            }
        }

        public async Task<bool> SubTaskExistsAsync(int id, int userId)
        {
            try
            {
                return await _subTaskRepository.AnyAsync(st => st.Id == id && st.ToDo!.UserId == userId && !st.IsDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if subtask exists with ID: {SubTaskId} for user: {UserId}", id, userId);
                throw new InvalidOperationException($"Failed to check if subtask exists with ID: {id}", ex);
            }
        }
    }
}
