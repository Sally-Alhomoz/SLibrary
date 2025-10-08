using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using SLibraryAPI;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Repositories;
using SLibrary.DataAccess;
using SLibrary.DataAccess.Models;
using SLibrary.Business.Interfaces;
using SLibrary.Business.Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IBookManager, BookManager>();
builder.Services.AddScoped<IReservationManager, ReservationManager>();
builder.Services.AddScoped<IUserManager , UserManager>();

string mode = builder.Configuration["Storage:Mode"] ?? "Database";

if (mode == "File")
{
    string bookFile = builder.Configuration["Storage:BookFilePath"] ?? "books.json";
    string resFile = builder.Configuration["Storage:ReservationFilePath"] ?? "reservations.json";

    builder.Services.AddSingleton<IBookRepository>(new FileBookRepository(bookFile));
    builder.Services.AddSingleton<IReservationRepository>(new FileReservationRepository(resFile));
}
else
{
    builder.Services.AddDbContext<SLibararyDBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString")));

    builder.Services.AddScoped<IBookRepository, DBBookRepository>();
    builder.Services.AddScoped<IReservationRepository, DBReservationRepository>();
    builder.Services.AddScoped<IUserRepository, DBUserRepository>();
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("##sally##Slibrary##sally##Slibrary"))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
