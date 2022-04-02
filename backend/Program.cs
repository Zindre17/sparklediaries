using backend;
using backend.Requests;
using backend.Services;
using backend.Stores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
    options.AddPolicy("all", builder =>
        builder.AllowAnyOrigin().AllowAnyHeader()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(nameof(DatabaseOptions)));

builder.Services.AddTransient<DatabaseService>();
builder.Services.AddTransient<HuntStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("all");

app.UseAuthorization();

app.MapGet("/hunt", (HuntStore store) => store.GetAll());

app.MapGet("/hunt/active", (HuntStore store) => store.GetActive());

app.MapGet("/hunt/{id}", async (int id, HuntStore store) =>
    {
        var result = await store.GetHunt(id);
        return result is null ? Results.NotFound() : Results.Ok(result);
    });

app.MapPost("hunt/new", async (NewHunt hunt, HuntStore store)
    => hunt.Game is null || hunt.Type is null
        ? Results.BadRequest("Hunt must have a game and type")
        : Results.Ok(await store.CreateHunt(hunt.Game, hunt.Type, hunt.Target)));

app.MapPost("hunt/{id}/+", async (int id, int count, HuntStore store)
    => await store.AddEncounters(id, count) ? Results.Ok() : Results.NotFound());

app.MapPost("/hunt/{id}/completed", async (int id, HuntStore store)
    => await store.CompleteHunt(id) ? Results.Ok() : Results.NotFound());

app.Run();
