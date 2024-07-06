using Microsoft.Extensions.FileProviders;
using ZemljopisAPI.DI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
}).AddMvc();


builder.Services.AddDependencies();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
RegisterPaths();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();


void RegisterPaths()
{
    UseFileServer("", "Html", "index.html");

}

void UseFileServer(string requestPath, string physicalPath, string filename)
{
    app.UseFileServer(new FileServerOptions()
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(app.Environment.ContentRootPath, physicalPath)),
        RequestPath = requestPath,
        DefaultFilesOptions = { DefaultFileNames = new List<string> { filename } }
    });
}