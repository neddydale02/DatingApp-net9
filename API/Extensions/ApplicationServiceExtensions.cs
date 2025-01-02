using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    // Metodo di estensione per aggiungere servizi all'applicazione
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration config)
    {
        // Aggiunge i controller al servizio
        services.AddControllers();

        // Configura il contesto del database utilizzando SQLite
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });

        // Aggiunge il supporto per CORS (Cross-Origin Resource Sharing)
        services.AddCors();

        // Aggiunge il servizio per la gestione dei token
        services.AddScoped<ITokenService, TokenService>();

        // Aggiunge il servizio per la gestione degli utenti
        services.AddScoped<IUserRepository, UserRepository>();

        // Aggiunge il servizio per la gestione delle foto
        services.AddScoped<IPhotoService, PhotoService>();

        // Aggiunge il servizio per la gestione delle foto (vecchio, prima di Cloudinary)
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Configura le impostazioni di Cloudinary
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

        // Ritorna la collezione di servizi configurata
        return services;
    }
}
