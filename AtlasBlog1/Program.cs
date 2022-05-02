using AtlasBlog1.Data;
using AtlasBlog1.Models;
using AtlasBlog1.Services;
using AtlasBlog1.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var connectionString = ConnectionService.GetConnectionString(builder.Configuration);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//Register the SwaggerGen Service
builder.Services.AddSwaggerGen(s =>
{
    OpenApiInfo openApiInfo = new()
    {
        Title = "Raven Blog API",
        Version = "v1",
        Description = "Candidate API for the Raven Blog",
        Contact = new()
        {
            Name = "Nichoals Payne",
            Url = new("PORFOLIO WEBSITE")
        },
        License = new()
        {
            Name = "Api Licence",
            Url = new("WHo cares")
        }
    };

    s.SwaggerDoc(openApiInfo.Version, openApiInfo);

});

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<DataService>();

builder.Services.AddScoped<IImageService, BasicImageService>();

builder.Services.AddTransient<IEmailSender, BasicEmailService>();

builder.Services.AddTransient<SlugService>();

builder.Services.AddTransient<SearchService>();


var app = builder.Build();

//When calling a service from this middleware we need an intance of IServiceScope
var scope = app.Services.CreateScope();
var dataService = scope.ServiceProvider.GetRequiredService<DataService>();
await dataService.SetupDbAsync();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//Call on our configured Swagger service
app.UseSwagger();
app.UseSwagger(s =>
{
    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Atlas Blog API");

    if (!app.Environment.IsDevelopment())
    {
        s.RoutePrefix = "";
    }
});

app.MapControllerRoute(
    name: "detail",
    pattern: "Posts/{slug}",
    defaults: new { controller = "Posts", actions = "Details" }
);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
