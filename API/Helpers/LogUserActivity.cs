using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API;

// Classe che implementa il filtro di azione asincrono per registrare l'attività dell'utente
public class LogUserActivity : IAsyncActionFilter
{
    // Metodo che viene eseguito durante l'esecuzione dell'azione
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Esegue l'azione successiva nel filtro
        var resultContext = await next();

        // Verifica se l'utente è autenticato
        if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

        // Ottiene l'ID dell'utente autenticato
        var userId = resultContext.HttpContext.User.GetUserId();

        // Ottiene il repository dell'utente dai servizi richiesti
        var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        
        // Ottiene l'utente dal repository utilizzando l'ID
        var user = await repo.GetUserByIdAsync(userId);
        if (user == null) return;
        
        // Aggiorna l'ultima attività dell'utente
        user.LastActive = DateTime.UtcNow;
        
        // Salva le modifiche nel repository
        await repo.SaveAllAsync();
    }
}