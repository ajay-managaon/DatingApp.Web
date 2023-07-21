using Web.DatingApp.API.Web.DatingApp.Database;
using Web.DatingApp.API.Web.DatingApp.IocContainer;
using Web.DatingApp.API.Web.DatingApp.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.InjectDependencies(builder);

var host = builder.Build();
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DatingAppDbContext>();
        await SeedData.SeedUsers(context);
    }
    catch(Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError("An error occured suring migration");
    }
}

MiddlewareInjector.AddMiddleWares(builder, host);


