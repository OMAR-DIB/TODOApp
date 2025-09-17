using System.ComponentModel.DataAnnotations;

namespace ToDo.API.Dtos.UserDtos
{
    public class CreateUserResponseDto
    {
        public CreateUserResponseDto(int id, string name) // Changed long to int to match User.Id
        {
            Id = id;   // Fixed property name casing
            Name = name;
        }

        public int Id { get; } // Changed from ID to Id and long to int
        public string Name { get; }
    }
}
