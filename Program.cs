using Headlight.AppCode;
using Headlight.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

string host = builder.Configuration["DB_HOST"]?.ToString() ?? "postgres";
string port = builder.Configuration["DB_PORT"]?.ToString() ?? "";
string database = "headlight";
string username = builder.Configuration["DB_USER"]?.ToString() ?? "headlightuser";
string password = builder.Configuration["DB_PASS"]?.ToString() ?? "headlightpass*";
string connStr = Utils.GetDbConnectionString(host, port, database, username, password);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connStr,
        npgsqlOptions => {
            npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
            npgsqlOptions.CommandTimeout(60);
        }
    )
);

var app = builder.Build();

// Run migrations
using (IServiceScope scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
