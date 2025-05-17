using System.Text;
using System.IdentityModel.Tokens.Jwt;
using ClassRoomClone_App.Server.CustomAuthorization;
using ClassRoomClone_App.Server.Repositories.Implements;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Implements;
using ClassRoomClone_App.Server.Services.Interfaces;
using ClassRoomClone_App.Server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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
                //.AllowCredentials() // Uncomment if credentials needed
                );
            });

            // Clear default claim type mapping to keep JWT claim names as-is
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            // JWT Authentication Configuration
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwtSettings = builder.Configuration.GetSection("JwtSettings"); // Correct key name

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddHttpContextAccessor();

            // Authorization policies and handlers
            builder.Services.AddScoped<IAuthorizationHandler, ClassRoleAuthorizationHandler>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("TeacherOrSubTeacher", policy =>
                    policy.Requirements.Add(new ClassRoleRequirement("Teacher", "SubTeacher")));

                options.AddPolicy("TeacherOnly", policy =>
                    policy.Requirements.Add(new ClassRoleRequirement("Teacher")));

                options.AddPolicy("StudentOnly", policy =>
                    policy.Requirements.Add(new ClassRoleRequirement("Student")));
            });

            // Add controllers and swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure EF Core DbContext
            builder.Services.AddDbContext<DbContextClassName>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register singleton services
            builder.Services.AddSingleton<MegaStorageService>();

            // Register user context service
            builder.Services.AddScoped<IUserContextService, UserContextService>();

            // Register repositories
            builder.Services.AddScoped<IClassRepository, ClassRepository>();
            builder.Services.AddScoped<IClassParticipantsRepository, ClassParticipantsRepository>();
            builder.Services.AddScoped<IClassWorkRepository, ClassWorkRepository>();
            builder.Services.AddScoped<IToDoRepository, ToDoRepository>();
            builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
            builder.Services.AddScoped<ITopicRepository, TopicRepository>();
            builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
            builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<IAssignmentSubmissionRepository, AssignmentSubmissionRepository>();
            builder.Services.AddScoped<ISubmissionResponseRepository, SubmissionResponseRepository>();
            builder.Services.AddScoped<IGradeRepository, GradeRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IAssignmentCreateRepository, AssignmentCreateRepository>();

            // Register services
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

            // Serve default files and static files
            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
