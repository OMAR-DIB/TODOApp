using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ToDo.API.ClaimProvider;
using ToDo.API.Dtos.ToDosDtos;
using ToDo.API.Services.ToDoServices;
using ToDo.Data.Entities.@enum;

namespace ToDo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDosController : ControllerBase
    {
        private readonly IToDoService _todoService;
        private readonly IValidator<CreateToDosRequestDto> _createValidator;
        private readonly IValidator<UpdateToDosRequestDto> _updateValidator;
        private readonly IClaimsProvider _claimsProvider;
        private readonly ILogger<ToDosController> _logger;

        public ToDosController(
            IToDoService todoService,
            IValidator<CreateToDosRequestDto> createValidator,
            IValidator<UpdateToDosRequestDto> updateValidator,
            IClaimsProvider claimsProvider,
            ILogger<ToDosController> logger)
        {
            _todoService = todoService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _claimsProvider = claimsProvider;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDosResponseDto>>> GetAllToDos()
        {
            try
            {
                var userId = (int)_claimsProvider.GetUserID();
                var todos = await _todoService.GetUserToDosAsync(userId);
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllToDos");
                return StatusCode(500, "An error occurred while retrieving todos");
            }
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<ToDosResponseDto>>> GetToDosByStatus(TaskStatuss status)
        {
            try
            {
                var userId = (int)_claimsProvider.GetUserID();
                var todos = await _todoService.GetToDosByStatusAsync(status, userId);
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetToDosByStatus for status: {Status}", status);
                return StatusCode(500, "An error occurred while retrieving todos by status");
            }
        }

        [HttpGet("date/{date}")]
        public async Task<ActionResult<IEnumerable<ToDosResponseDto>>> GetToDosForDate(DateOnly date)
        {
            try
            {
                var userId = (int)_claimsProvider.GetUserID();
                var todos = await _todoService.GetToDosForDateAsync(date, userId);
                return Ok(todos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetToDosForDate for date: {Date}", date);
                return StatusCode(500, "An error occurred while retrieving todos for date");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToDosResponseDto>> GetToDo(int id)
        {
            try
            {
                var userId = (int)_claimsProvider.GetUserID();
                var todo = await _todoService.GetToDoByIdAsync(id, userId);
                if (todo == null)
                {
                    return NotFound($"ToDo with ID {id} not found");
                }
                return Ok(todo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetToDo for ID: {TodoId}", id);
                return StatusCode(500, "An error occurred while retrieving the todo");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ToDosResponseDto>> CreateToDo(CreateToDosRequestDto dto)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var userId = (int)_claimsProvider.GetUserID();
                var todo = await _todoService.CreateToDoAsync(dto, userId);
                return CreatedAtAction(nameof(GetToDo), new { id = todo.Id }, todo);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateToDo");
                return StatusCode(500, "An error occurred while creating the todo");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ToDosResponseDto>> UpdateToDo(int id, UpdateToDosRequestDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest("ID mismatch between route and body");
                }

                var validationResult = await _updateValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var userId = (int)_claimsProvider.GetUserID();
                var todo = await _todoService.UpdateToDoAsync(dto, userId);
                return Ok(todo);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateToDo for ID: {TodoId}", id);
                return StatusCode(500, "An error occurred while updating the todo");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteToDo(int id)
        {
            try
            {
                var userId = (int)_claimsProvider.GetUserID();
                var success = await _todoService.DeleteToDoAsync(id, userId);
                if (!success)
                {
                    return NotFound($"ToDo with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteToDo for ID: {TodoId}", id);
                return StatusCode(500, "An error occurred while deleting the todo");
            }
        }
    }
}
