using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Ropufu.Homepage.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
SqlConnectionStringBuilder connectionBuilder = new(
    builder.Configuration.GetConnectionString($"{Environment.OSVersion.Platform}"));
connectionBuilder.Password = builder.Configuration["RopufuDbPassword"];
string connectionString = connectionBuilder.ConnectionString;
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
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
