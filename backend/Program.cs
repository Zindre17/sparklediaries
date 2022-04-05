using backend;
using backend.Endpoints;
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
builder.Services.AddTransient<GameStore>();

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

app.MapGet("hunts", (bool? active, HuntStore store)
    => active is null
        ? store.GetAll()
        : active.Value
            ? store.GetActive()
            : store.GetCompleted());

app.MapGet("hunts/types", (HuntStore store) => store.GetHuntTypes());

app.MapGet("hunts/{id}", async (int id, HuntStore store) =>
    {
        var result = await store.GetHunt(id);
        return result is null ? Results.NotFound() : Results.Ok(result);
    });

app.MapPost("hunts", (NewHunt hunt, HuntStore huntStore, GameStore gameStore)
    => Endpoints.NewHunt(hunt, huntStore, gameStore));

app.MapPost("hunts/{id}/+", async (int id, int count, HuntStore store)
    => await store.AddEncounters(id, count) ? Results.Ok() : Results.NotFound());

app.MapPost("hunts/{id}/completed", async (int id, HuntStore store)
    => await store.CompleteHunt(id) ? Results.Ok() : Results.NotFound());

app.MapGet("games", (GameStore store) => store.All());

app.Run();
