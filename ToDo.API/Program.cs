
using Microsoft.EntityFrameworkCore;
using ToDo.API.Services;
using ToDo.Data;
using ToDo.Data.Configurations;
using ToDo.Data.DI;
using ToDo.Data.Repositories.Task;

namespace ToDo.API
{
    public class Program
    {
        public static void Main(string[] args) 
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddDbContext<AppDbContext>(options =>
            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // db config //
            var mssqlSection = builder.Configuration.GetSection("DbConfiguration");
            builder.Services.AddMyApplicationDbContext(mssqlSection);
            ///////////////////////

            builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            builder.Services.AddScoped<IToDosServices, ToDosServices>();
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
