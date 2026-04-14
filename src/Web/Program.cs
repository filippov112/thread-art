using Application.Interfaces;
using Application.UseCases;
using Infrastructure;
using Web.Interfaces;
using Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<ImageProcessor>();
builder.Services.AddTransient<IPainter, Painter>();
builder.Services.AddTransient<IPathManager, PathManager>();
builder.Services.AddTransient<IStreamController, StreamController>();
builder.Services.AddTransient<IProgressLogger, ProgressLoggerAdapter>();
builder.Services.AddHostedService<FileCleanupService>();

builder.Services.AddSignalR();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapHub<ProgressHub>("/progressHub");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Image}/{action=Index}/{id?}");

app.Run();
