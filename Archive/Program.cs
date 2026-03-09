using Archive.Models.Configuration;
using Archive.Models.Interface;
using Archive.Models.Interface.Export;
using Archive.Models.Mongo;
using Archive.Services.Controller;
using Archive.Services.Export;
using Archive.Services.Mongo;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection(MongoSettings.SectionName));
builder.Services.AddOpenApi();
builder.Services.AddSingleton<FlightTelemetryMongoProxy>();
builder.Services.AddSingleton<IContrillerUtilscs, ContrillerUtils>();
builder.Services.AddSingleton<IFlightTelemetryMongoProxy, FlightTelemetryMongoProxy>();
builder.Services.AddSingleton<IExportService, ExportService>();
builder.Services.AddSingleton<IFactoryFormat, FrameExporterFactory>();
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
