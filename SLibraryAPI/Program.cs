using Microsoft.EntityFrameworkCore;
using SLibrary.Business;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using SLibraryAPI;
using SLibrary.DataAccess.Interfacses;
using SLibrary.DataAccess.Repositories;
using SLibrary.DataAccess;
using SLibrary.DataAccess.Models;
using SLibrary.Business.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IBookManager, BookManager>();
builder.Services.AddScoped<IReservationManager, ReservationManager>();

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
}


builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<SLibararyDBContext>();

builder.Services.AddSwaggerGen(c =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenJwtAuthen();

builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
