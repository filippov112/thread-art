
using Application;
using Infrastructure;
using Infrastructure.Data;
using Scalar.AspNetCore;
using Web;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
else
{
    await app.InitialiseDatabaseAsync();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapSwagger("/openapi/{documentName}.json");
app.MapScalarApiReference();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
