using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MtTicket.API.Data;
using MtTicket.API.Mappings;
using MtTicket.API.Repositories.Booking;
using MtTicket.API.Repositories.Event;
using MtTicket.API.Repositories.UnitOfWork;
using MtTicket.API.Repositories.User;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add FluentValidation 
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        policy.WithOrigins(allowedOrigins ?? new[] { "http://localhost:5173" })
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Cấu hình Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng Ký Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Cấu hình Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MtTicket API",
        Version = "v1",
        Description = "API cho hệ thống đặt vé",
        Contact = new OpenApiContact
        {
            Name = "Support",
            Email = "support@mtticket.com"
        }
    });

    // Cấu hình JWT trong Swagger (sẽ thêm sau khi làm authentication)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MtTicket API v1");
        c.RoutePrefix = string.Empty; // Swagger UI ở root
    });
}

// Middleware pipeline
app.UseHttpsRedirection();

// CORS phải đặt trước UseAuthorization
app.UseCors("AllowFrontend");

// Exception handling middleware (sẽ tạo sau)
// app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication(); // Sẽ cấu hình sau
app.UseAuthorization();

app.MapControllers();

// Tự động migrate database khi chạy (chỉ dùng trong development)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        dbContext.Database.EnsureCreated();
        await DbSeeder.SeedAsync(dbContext);
        Log.Information("Database connected successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error connecting to database");
    }
}

app.Run();