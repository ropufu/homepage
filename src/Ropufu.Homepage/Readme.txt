
~~ References ~~
https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/linux-nginx
https://stackoverflow.com/a/11938495
https://stackoverflow.com/a/31209809

~~ Conventions ~~
https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md
Deviations:
--- Prefer using "this" when accessing members, e.g.,
    "this._count = 0" instead of "_count = 0";
    "this.Reset()" instead of "Reset()".
--- Prefer using "TypeName" when accessing static members, e.g.,
    "Counter.s_count = 0" instead of "s_count = 0";
    "Counter.Reset()" instead of "Reset()".


    
~~ Protect sensitive data  ~~
Follow these instructions to set up ConnectionStrings safely:
https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets

==========================================================================
./appsettings.json
==========================================================================
--- Add "ConnectionStrings > Win32NT" and "ConnectionStrings > Unix"
--- Make sure not to specify "Password".
==========================================================================
==========================================================================


==========================================================================
secrets.json
==========================================================================
--- Add "RopufuDbPassword"
==========================================================================
==========================================================================
Note: on production server, add
    Environment=ROPUFU_DB_PASSWORD=<whatever_your_password_is>
to the kestrel service file.


==========================================================================
./Properties/launchSettings.json
==========================================================================
--- Remove the https option from "profiles > applicationUrl"
==========================================================================
==========================================================================


==========================================================================
./Program.cs
==========================================================================
--- Change the line
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    to
        SqlConnectionStringBuilder connectionBuilder = new(
            builder.Configuration.GetConnectionString($"{Environment.OSVersion.Platform}"));
        builder.Configuration.AddEnvironmentVariables(prefix: "ROPUFU");
        connectionBuilder.Password = builder.Configuration["ROPUFU_DB_PASSWORD"];
        string connectionString = connectionBuilder.ConnectionString;
--- Remove the "app.UseHttpsRedirection();" line.
--- Remove the "app.UseHsts();" line.
--- Replace the entire block following "// Configure the HTTP request pipeline."
    with "app.UseExceptionHandler("/Error");".
--- Add "app.UseStatusCodePagesWithReExecute("/Sorry");"
    after the "app.UseStaticFiles();" line.
--- Add "app.MapControllers();"
    after the "app.MapRazorPages();" line.
--- Add "using Microsoft.AspNetCore.HttpOverrides;" and then
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
    before the "app.UseAuthentication();" line.
--- Change the line
        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    to
        builder.Services.AddDefaultIdentity<IdentityUser<int>>(options => options.SignIn.RequireConfirmedAccount = true)
    shortly before the "builder.Services.AddRazorPages();" line.
==========================================================================
==========================================================================


~~ Project Properties~~

Build Tab:
--- Change Warning level to 5
--- Check "Treat warnings as errors"


~~ Copy/paste ~~
All ./Pages/ except for ./Pages/Shared/

~~ Copy/paste ~~
./Controllers/
./Data/
./Ropufu/
./TagHelpers/
./libman.json
./Readme.txt
./wiki.json
./Pages/ except for _*
./wwwroot/favicon.ico
./wwwroot/images/
./wwwroot/js/
./wwwroot/css/ropufu.css


~~ Edit manually ~~
==========================================================================
./Pages/Shared/_Layout.cshtml
==========================================================================
==========================================================================


==========================================================================
./Pages/Shared/_LoginPartial.cshtml
==========================================================================
--- Change "IdentityUser" to "IdentityUser<int>"
==========================================================================
==========================================================================


==========================================================================
./Pages/_ViewImports.cshtml
==========================================================================
--- Add the following line at the end:
        @addTagHelper *, Ropufu.Homepage
==========================================================================
==========================================================================


~~ If creating database from scratch ~~
--- Go to View > Other Windows > Package Manager Console (PMC).
--- Type "Add-Migration InitialCreate" in PMC.
--- Type "Update-Database" in PMC.


~~~~
