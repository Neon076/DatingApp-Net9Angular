using API.Data;
using API.Helpers;
using API.interfaces;
using API.services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        });
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();

        services.AddCors();

        services.AddScoped<ITokenInterface, TokenService>();
        services.AddScoped<IUserRespository , UserRepository>();
        services.AddScoped<IPhotoService , PhotoService>();
        services.AddScoped<ILikesRepository , LikesRepository>();
        services.AddScoped<IMessageRepository , MessageRepository>();
        services.AddScoped<IPhotoRepository , PhotoRepository>();
        services.AddScoped<IUnitOfWork , UnitOfWork>();
        services.AddScoped<LogUserActitvity>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySetttings>(config.GetSection("CloudinarySettings"));
        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();
        services.AddSignalR(options => { options.EnableDetailedErrors = true; });

        return services;
    }
}
