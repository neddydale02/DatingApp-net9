using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize] // Richiede l'autenticazione per accedere a qualsiasi metodo del controller
// Controller per la gestione degli utenti
public class UsersController(IUserRepository userRepository, IMapper mapper) : BaseApiController
{
    // Metodo per ottenere tutti gli utenti
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await userRepository.GetMembersAsync(); // Ottiene tutti gli utenti in una lista iterabile

        return Ok(users);
    }

    // Metodo per ottenere un utente specifico per ID
    [HttpGet("{username}")]  // /api/users/{username}
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await userRepository.GetMemberAsync(username); // Ottiene l'utente per username

        if (user == null) return NotFound(); // Restituisce 404 se l'utente non viene trovato

        return user; // Mappa l'utente nel DTO
    }
    // Metodo per aggiornare un utente
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Ottiene il nome utente dal token

        if(username == null) return BadRequest("No username found in token"); // Verifica se il nome utente è presente nel token

        var user = await userRepository.GetUserByUsernameAsync(username); // Ottiene l'utente dal repository utilizzando il nome utente

        if (user == null) return BadRequest("Could not find user"); // Verifica se l'utente è stato trovato

        mapper.Map(memberUpdateDto, user); // Mappa i dati aggiornati dell'utente
        
        if (await userRepository.SaveAllAsync()) return NoContent(); // Salva le modifiche nel repository

        return BadRequest("Failed to update user"); // Restituisce un errore se l'aggiornamento fallisce
    }
}
