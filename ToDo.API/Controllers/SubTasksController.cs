using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDo.API.ClaimProvider;
using ToDo.API.Dtos.SubTaskDtos;
using ToDo.API.Services.SubTaskServices;

namespace ToDo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubTasksController : ControllerBase
    {
    private readonly ISubTaskService _subTaskService;
    private readonly IValidator<CreateSubTaskRequestDto> _createValidator;
    private readonly IValidator<UpdateSubTaskRequestDto> _updateValidator;
    private readonly IClaimsProvider _claimsProvider;
    private readonly ILogger<SubTasksController> _logger;

    public SubTasksController(
        ISubTaskService subTaskService,
        IValidator<CreateSubTaskRequestDto> createValidator,
        IValidator<UpdateSubTaskRequestDto> updateValidator,
        IClaimsProvider claimsProvider,
        ILogger<SubTasksController> logger)
    {
        _subTaskService = subTaskService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _claimsProvider = claimsProvider;
        _logger = logger;
    }

    [HttpGet("todo/{todoId}")]
    public async Task<ActionResult<IEnumerable<SubTaskResponseDto>>> GetSubTasksByToDo(int todoId)
    {
        try
        {
            var userId = (int)_claimsProvider.GetUserID();
            var subTasks = await _subTaskService.GetSubTasksByToDoAsync(todoId, userId);
            return Ok(subTasks);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetSubTasksByToDo for TodoId: {TodoId}", todoId);
            return StatusCode(500, "An error occurred while retrieving subtasks");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubTaskResponseDto>> GetSubTask(int id)
    {
        try
        {
            var userId = (int)_claimsProvider.GetUserID();
            var subTask = await _subTaskService.GetSubTaskByIdAsync(id, userId);
            if (subTask == null)
            {
                return NotFound($"SubTask with ID {id} not found");
            }
            return Ok(subTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetSubTask for ID: {SubTaskId}", id);
            return StatusCode(500, "An error occurred while retrieving the subtask");
        }
    }

    [HttpPost]
    public async Task<ActionResult<SubTaskResponseDto>> CreateSubTask(CreateSubTaskRequestDto dto)
    {
        try
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var userId = (int)_claimsProvider.GetUserID();
            var subTask = await _subTaskService.CreateSubTaskAsync(dto, userId);
            return CreatedAtAction(nameof(GetSubTask), new { id = subTask.Id }, subTask);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateSubTask");
            return StatusCode(500, "An error occurred while creating the subtask");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SubTaskResponseDto>> UpdateSubTask(int id, UpdateSubTaskRequestDto dto)
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
            var subTask = await _subTaskService.UpdateSubTaskAsync(dto, userId);
            return Ok(subTask);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateSubTask for ID: {SubTaskId}", id);
            return StatusCode(500, "An error occurred while updating the subtask");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSubTask(int id)
    {
        try
        {
            var userId = (int)_claimsProvider.GetUserID();
            var success = await _subTaskService.DeleteSubTaskAsync(id, userId);
            if (!success)
            {
                return NotFound($"SubTask with ID {id} not found");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteSubTask for ID: {SubTaskId}", id);
            return StatusCode(500, "An error occurred while deleting the subtask");
        }
    }
}
}
