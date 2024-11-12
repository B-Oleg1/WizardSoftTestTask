using Microsoft.EntityFrameworkCore;
using WizardSoftTestTaskAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Подключаем XML-комментарии к Swagger
    var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
    foreach (var xmlFile in xmlFiles)
    {
        options.IncludeXmlComments(xmlFile);
    }
});

builder.Services.AddDbContext<CatalogsDbContext>(options => options.UseInMemoryDatabase("Catalogs"));

var app = builder.Build();

// true - временно, чтобы можно было смотреть интерфейс API после билда
if (true || app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
