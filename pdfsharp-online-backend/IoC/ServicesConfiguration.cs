namespace pdfsharp_online_backend.IoC
{
    public static class ServicesConfiguration
    {

        public static void AddPdfSharpOnlineServices(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
        {
            //Added this dependency in oredr to access the current request's HttpContext and get user infos
            services.AddHttpContextAccessor();

            services.AddHttpClient();

            services.AddScoped<IImageItemService, ImageItemService>();

        }

        public static void AddDbContextService(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        }
    }
}
