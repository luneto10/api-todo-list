using TodoListAPI.Models;
using TodoListAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDbSettings"));

// Register MongoDBServices as a singleton and inject the MongoDBSettings configuration
builder.Services.AddSingleton<MongoDBServices>();

builder.Services.AddControllers();

// Add CORS policy allowing specific origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",                          // Development environment (Vite default URL)
            "https://minimalist-todo-list-weld.vercel.app"    // Production environment on Vercel
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});



// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apply CORS policy before authorization
app.UseCors("AllowFrontend");
app.UseAuthorization();

app.MapControllers();

app.Run();