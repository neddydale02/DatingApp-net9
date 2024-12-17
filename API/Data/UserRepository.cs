using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

// Implementazione del repository degli utenti
public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
{

    // Metodo per ottenere un membro (utente) specifico per nome utente
    public async Task<MemberDto?> GetMemberAsync(string username)
    {
        return await context.Users
            .Where(x => x.UserName == username) // Filtra gli utenti per username
            .ProjectTo<MemberDto>(mapper.ConfigurationProvider) // Proietta l'utente nel DTO
            .SingleOrDefaultAsync(); // Restituisce un singolo utente o null se non trovato
    }

    // Metodo per ottenere tutti i membri (utenti)
    public async Task<IEnumerable<MemberDto>> GetMembersAsync()
    {
        return await context.Users
            .ProjectTo<MemberDto>(mapper.ConfigurationProvider) // Proietta gli utenti nel DTO
            .ToListAsync(); // Restituisce la lista degli utenti
    }

    // Metodo per ottenere un utente per ID
    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id); // Trova l'utente per ID
    }

    // Metodo per ottenere un utente per nome utente
    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        return await context.Users
            .Include(x => x.Photos) // Include le foto dell'utente
            .SingleOrDefaultAsync(x => x.UserName == username); // Trova l'utente per nome utente
    }

    // Metodo per ottenere tutti gli utenti
    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await context.Users
            .Include(x => x.Photos) // Include le foto degli utenti
            .ToListAsync(); // Restituisce la lista degli utenti
    }

    // Metodo per salvare tutte le modifiche
    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0; // Restituisce true se ci sono modifiche salvate
    }

    // Metodo per aggiornare un utente
    public void Update(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified; // Segna l'entità come modificata
    }
}