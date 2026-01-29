using Archive.Models.Configuration;
using Archive.Services.Mongo;
using Archive.Models.Interface;
using Archive.Services.Controller;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection(MongoSettings.SectionName));
builder.Services.AddOpenApi();
builder.Services.AddSingleton<FlightTelemetryMongoProxy>();
builder.Services.AddSingleton<IContrillerUtilscs, ContrillerUtils>();
builder.Services.AddSingleton<ICacheMongo, CacheMongo>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:4200")
                     .AllowAnyHeader()
                     .AllowAnyMethod();
    });
});
builder.Services.AddMemoryCache();

WebApplication app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAngularApp");

app.MapControllers();

app.Run();
