using Microsoft.EntityFrameworkCore;
using Server.EF_Core;

var builder = WebApplication.CreateBuilder(args);

// ── Dependency Injection ──────────────────────────────────────────────────────
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlite("Data Source=lostfound.db"));   // ← Datenbankpfad anpassen

builder.Services.AddControllers();

// CORS: WPF-App läuft lokal, daher alle Origins erlauben (nur Entwicklung)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── Pipeline ──────────────────────────────────────────────────────────────────
var app = builder.Build();

// Datenbank beim Start automatisch anlegen / migrieren
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<Context>();
    db.Database.EnsureCreated();   // oder: db.Database.Migrate() bei Migrations
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
