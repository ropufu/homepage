using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Ropufu.Homepage.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables(prefix: "ROPUFU");

// Add services to the container.
string connectionName = $"{Environment.OSVersion.Platform}";
string connectionString =
    builder.Configuration.GetConnectionString(connectionName) ??
    throw new InvalidOperationException($"Connection string '{connectionName}' not found.");
SqlConnectionStringBuilder connectionBuilder = new(connectionString)
{
    Password = builder.Configuration["ROPUFU_DB_PASSWORD"]
};

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionBuilder.ConnectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser<int>>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();

WebApplication app = builder.Build();

app.UseExceptionHandler("/Error");
app.UseStaticFiles();
app.UseStatusCodePagesWithReExecute("/Sorry");

app.UseRouting();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
