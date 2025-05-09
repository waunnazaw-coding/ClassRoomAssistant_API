using System.Text;
using ClassRoomClone_App.Server.CustomAuthorization;
using Microsoft.EntityFrameworkCore;
using ClassRoomClone_App.Server.Repositories.Implements;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Implements;
using ClassRoomClone_App.Server.Services.Interfaces;
using ClassRoomClone_App.Server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace ClassRoomClone_App.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // CORS Configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    policy => policy
                        .WithOrigins("http://localhost:5174") // Frontend origin
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                );
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("TeacherOrSubTeacher", policy =>
                    policy.Requirements.Add(new ClassRoleRequirement("Teacher", "SubTeacher")));

                options.AddPolicy("TeacherOnly", policy =>
                    policy.Requirements.Add(new ClassRoleRequirement("Teacher")));

                options.AddPolicy("StudentOnly", policy =>
                    policy.Requirements.Add(new ClassRoleRequirement("Student")));
            });

            builder.Services.AddScoped<IAuthorizationHandler, ClassRoleHandler>();

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
            builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<IAssignmentSubmissionRepository, AssignmentSubmissionRepository>();
            builder.Services.AddScoped<ISubmissionResponseRepository, SubmissionResponseRepository>();
            builder.Services.AddScoped<IGradeRepository, GradeRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Register Services
            builder.Services.AddScoped<IClassService, ClassService>();
            builder.Services.AddScoped<ITopicService, TopicService>();
            builder.Services.AddScoped<IClassParticipantsService, ClassParticipantsService>();
            builder.Services.AddScoped<IAssignmentService, AssignmentService>();
            builder.Services.AddScoped<IMaterialService, MaterialService>();
            builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
            builder.Services.AddScoped<IClassWorkService, ClassWorkService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IAssignmentSubmissionService, AssignmentSubmissionService>();
            builder.Services.AddScoped<IGradeService, GradeService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // --- FIX: Correct Middleware Order for CORS ---
            app.UseRouting(); // Needed for endpoint routing

            app.UseCors("AllowFrontend"); // CORS must be here, after UseRouting and before Auth

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
