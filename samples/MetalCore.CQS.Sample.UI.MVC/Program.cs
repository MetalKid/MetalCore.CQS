using MetalCore.CQS.Sample.UI.MVC;
using SimpleInjector;

var container = IoCSetup.SetupIoC();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
var services = builder.Services;


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
services.AddSimpleInjector(container, options =>
{
    options.AddAspNetCore()
        .AddControllerActivation()
        .AddViewComponentActivation()
        .AddPageModelActivation();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Services.UseSimpleInjector(container);

app.UseAuthorization();

app.MapControllers();

container.Verify();

app.Run();
