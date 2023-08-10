using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication25.Models;
using WebApplication25.Models.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
    // in status of faile this will  handel error to true  SecondConection
    options.InvokeHandlersAfterFailure = true;
});
builder.Services.AddDbContext<DB>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaulteConection"),
    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
});

 
builder.Services.AddDbContext<Model>(options => options
                .UseSqlServer(builder.Configuration.GetConnectionString("SecondConection") ,o=>o.EnableRetryOnFailure() ));

 
//builder.Services.AddDbContext<Model>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("SecondConection"),
//    o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));
//});

builder.Services.AddRazorPages();
// by defaulte this service is set to connect to AD 
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
 
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
