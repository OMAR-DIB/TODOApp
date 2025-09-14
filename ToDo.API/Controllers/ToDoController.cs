using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDo.API.Dtos.RequestTask;
using ToDo.API.Dtos.Response;
using ToDo.API.Services;

namespace ToDo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly IToDosServices _service;
        public ToDoController(IToDosServices service) {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseTaskDto>> GetAll()
        {
            return Ok(await _service.
                GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> create(RequestTaskDto req)
        {
            var task = await _service.AddTask(req);
            return Ok(task);
        }
    }
}
