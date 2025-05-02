using Microsoft.EntityFrameworkCore;
using ClassRoomClone_App.Server.Repositories.Implements;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Implements;
using ClassRoomClone_App.Server.Services.Interfaces;
using ClassRoomClone_App.Server.Models; // Ensure you have the correct namespace for DbContext

namespace ClassRoomClone_App.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<DbContextClassName>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register Repositories 
            builder.Services.AddScoped<IClassRepository, ClassRepository>();
            builder.Services.AddScoped<IClassParticipantsRepository, ClassParticipantsRepository>();
            builder.Services.AddScoped<IClassWorkRepository, ClassWorkRepository>();
            builder.Services.AddScoped<IToDoRepository, ToDoRepository>();
            builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            builder.Services.AddScoped<ITopicRepository, TopicRepository>();
            builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
            builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            builder.Services.AddScoped<IClassWorkRepository, ClassWorkRepository>();
            
            //Register Services
            builder.Services.AddScoped<IClassService, ClassService>();
            builder.Services.AddScoped<ITopicService, TopicService>();
            builder.Services.AddScoped<IClassParticipantsService, ClassParticipantsService>();
            builder.Services.AddScoped<IAssignmentService, AssignmentService>();
            builder.Services.AddScoped<IMaterialService, MaterialService>();
            builder.Services.AddScoped<IClassWorkService, ClassWorkService>();
           
            
            
            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}