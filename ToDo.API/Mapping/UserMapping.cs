using ToDo.API.Dtos.RequestTask;
using ToDo.API.Dtos.Response;
using ToDo.API.Dtos.UserDtos;
using ToDo.Data.Entities;

namespace ToDo.API.Mapping
{
    public static class UserMapping
    {
        public static User ToEntity(this CreateUserRequestDto dto)
        {
            return new User
            {
                Username = dto.Name,
                Password = dto.Password,
                Email = dto.Email
            };
        }

        public static CreateUserResponseDto ToResponseDto(this User entity)
        {
            return new CreateUserResponseDto(entity.Id, entity.Username);
        }

        // Added missing extension method
        public static CreateUserResponseDto ToCreateUserResponseDto(this User entity)
        {
            return new CreateUserResponseDto(entity.Id, entity.Username);
        }
    }
}
