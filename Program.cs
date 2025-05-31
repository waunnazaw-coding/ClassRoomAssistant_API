using System.Configuration;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ClassRoomClone_App.Server.CustomAuthorization;
using ClassRoomClone_App.Server.Helpers;
using ClassRoomClone_App.Server.Repositories.Implements;
using ClassRoomClone_App.Server.Repositories.Interfaces;
using ClassRoomClone_App.Server.Services.Implements;
using ClassRoomClone_App.Server.Services.Interfaces;
using ClassRoomClone_App.Server.Models;
using ClassRoomClone_App.Server.Notifications;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

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
                        .WithOrigins("http://localhost:5174" , "http://localhost:5175" , "http://localhost:5173") 
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials() // Uncomment if credentials needed
                );
            });
            
            
            // Configure Serilog from appsettings.json and console sink
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration) // Read settings from appsettings.json
                .WriteTo.Console() // Ensure console logging is enabled
                .CreateLogger();

            // Replace default logging with Serilog
            builder.Host.UseSerilog();

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
                    
                    // Allow JWT token to be passed in query string for SignalR
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            Console.WriteLine($"AccessToken: {accessToken}");

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/notificationHub"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddHttpContextAccessor();
            
            // Register custom IUserIdProvider
            builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();
            
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));


            builder.Services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
                Account account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
                return new CloudinaryDotNet.Cloudinary(account);
            });

            builder.Services.AddScoped<CloudinaryService>();

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
            
            //Register Notification
            builder.Services.AddSignalR();

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

            
            builder.Services.AddSwaggerGen(options =>
            {
                //options.SwaggerDoc("v1", new OpenApiInfo { Title = "LMS API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });
           

            var app = builder.Build();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


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

            // 5. Endpoints (controllers and SignalR hubs)
            app.UseEndpoints(endpoints =>
            {
                
                endpoints.MapHub<NotificationHub>("/notificationHub")
                .RequireAuthorization()
                .RequireCors("AllowFrontend");;
                
                endpoints.MapControllers();
            });

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
    
    // Custom IUserIdProvider implementation
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // Use ClaimTypes.NameIdentifier or adjust to your claim
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
