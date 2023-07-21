namespace Web.DatingApp.API.Web.DatingApp.Middlewares
{
    public static class MiddlewareInjector
    {
        public static void AddMiddleWares(WebApplicationBuilder builder, WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "datingapp");
                c.RoutePrefix = string.Empty;
            });

            app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
