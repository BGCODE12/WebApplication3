using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApplication3.Context;
using WebApplication3.Repositories.AttendanceLogRawRepository;
using WebApplication3.Repositories.AttendanceSessionRepository;
using WebApplication3.Repositories.DailyAttendanceSummaryRepository;
using WebApplication3.Repositories.DeviceRepository;
using WebApplication3.Repositories.EmployeeRepositiry;
using WebApplication3.Repositories.LeaveRequestRepository;
using WebApplication3.Repositories.LeaveTypeRepository;
using WebApplication3.Repositories.UserRepository;
using WebApplication3.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ===== DB =====
builder.Services.AddSingleton<Db>();

// ===== Controllers =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== DI =====
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IShiftDayRepository, ShiftDayRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IAttendanceSessionRepository, AttendanceSessionRepository>();
builder.Services.AddScoped<IAttendanceLogRawRepository, AttendanceLogRawRepository>();
builder.Services.AddScoped<IDailyAttendanceSummaryRepository, DailyAttendanceSummaryRepository>();

// ===== JWT AUTH =====
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // مهم لـ Postman و localhost
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // حتى لا يتأخر التوكن
    };
});

// ===== Authorization =====
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdminOnly", p => p.RequireRole("SuperAdmin"));
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("UnitAdminOnly", p => p.RequireRole("UnitAdmin"));
    options.AddPolicy("EmployeeOnly", p => p.RequireRole("Employee"));

    options.AddPolicy("AdminOrSuperAdmin", p => p.RequireRole("SuperAdmin", "Admin"));
    options.AddPolicy("UnitAdminOrSuperAdmin", p => p.RequireRole("SuperAdmin", "UnitAdmin"));
});


var app = builder.Build();

// ===== Swagger =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ===== Middleware =====
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
