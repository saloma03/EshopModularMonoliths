var builder = WebApplication.CreateBuilder(args);

//add services to the container
//builder.Services.AddControllers();

//register custom services

//builder.Services.AddTransient<IMyService, MyService>();
//builder.Services.AddScoped<IMyRepository, MyRepository>();
//builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services
    .AddCatalogModule(builder.Configuration)
    .AddBasketModule(builder.Configuration)
    .AddOrderingModule(builder.Configuration);

var app = builder.Build();

// configure the http request pipline

//app.UseRouting(); 

//app.UseAuthorization();

//app.UseEndpoints(endpoints =>
//{
//endpoints.MapControllers(); });

//app.UseStaticFiles();
//app.UseAuthentication();

//configure the http request pipline
app
    .UseCatalogModule()
    .UseBasketModule()
    .UseOrderingModule();

app.Run();
