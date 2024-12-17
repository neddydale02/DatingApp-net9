using API.Data;
using API.Interfaces;
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
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Ritorna la collezione di servizi configurata
        return services;
    }
}
