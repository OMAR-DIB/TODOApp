namespace ToDo.API.Dtos.Response
{
    public class ResponseTaskDto
    {


        public int Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; } = string.Empty;
        public DateOnly ToDoAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
