using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize] // Richiede l'autenticazione per accedere a qualsiasi metodo del controller
// Controller per la gestione degli utenti
public class UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService) : BaseApiController
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
        var user =await userRepository.GetUserByUsernameAsync(User.GetUsername()); // Ottiene l'utente per username
        
        if (user == null) return BadRequest("Could not find user"); // Verifica se l'utente è stato trovato

        mapper.Map(memberUpdateDto, user); // Mappa i dati aggiornati dell'utente
        
        if (await userRepository.SaveAllAsync()) return NoContent(); // Salva le modifiche nel repository

        return BadRequest("Failed to update user"); // Restituisce un errore se l'aggiornamento fallisce
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername()); // Ottiene l'utente per username

        if (user == null) return BadRequest("Could not update user"); // Verifica se l'utente è stato trovato

        var result = await photoService.AddPhotoAsync(file); // Aggiunge la foto all'utente

        if (result.Error != null) return BadRequest(result.Error.Message); // Restituisce un errore se la foto non viene aggiunta

        // Crea un nuovo oggetto Photo
        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri, // Imposta l'URL della foto
            PublicId = result.PublicId // Imposta l'ID pubblico della foto
        };

        if (user.Photos.Count == 0) photo.IsMain = true; // Imposta la foto principale se l'utente non ha foto

        user.Photos.Add(photo); // Aggiunge la foto all'utente

        if (await userRepository.SaveAllAsync()) 
        return CreatedAtAction(nameof(GetUser), 
            new {username = user.UserName}, mapper.Map<PhotoDto>(photo)); // Mappa la foto nel DTO se l'aggiornamento ha successo

        else return BadRequest("Problem adding photo"); // Restituisce un errore se l'aggiornamento fallisce
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername()); // Ottiene l'utente per username

        if (user == null) return BadRequest("Could not find user"); // Verifica se l'utente è stato trovato

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId); // Ottiene la foto per ID

        if (photo == null || photo.IsMain) return BadRequest("Cannot use it as main photo"); // Restituisce un errore se la foto non è stata trovata o è già la foto principale

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain); // Ottiene la foto principale

        if (currentMain != null) currentMain.IsMain = false; // Imposta la foto principale a false

        photo.IsMain = true; // Imposta la nuova foto principale a true

        if (await userRepository.SaveAllAsync()) return NoContent(); // Salva le modifiche nel repository

        return BadRequest("Failed to set main photo"); // Restituisce un errore se l'aggiornamento fallisce
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername()); // Ottiene l'utente per username

        if (user == null) return BadRequest("Could not find user"); // Verifica se l'utente è stato trovato

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId); // Ottiene la foto per ID

        if (photo == null) return BadRequest("This photo cannot be deleted"); // Restituisce 404 se la foto non viene trovata

        if (photo.IsMain) return BadRequest("You cannot delete your main photo"); // Restituisce un errore se si tenta di eliminare la foto principale

        if (photo.PublicId != null) // Verifica se la foto ha un ID pubblico
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId); // Elimina la foto dal servizio cloud

            if (result.Error != null) return BadRequest(result.Error.Message); // Restituisce un errore se la foto non viene eliminata
        }

        user.Photos.Remove(photo); // Rimuove la foto dall'utente

        if (await userRepository.SaveAllAsync()) return Ok(); // Salva le modifiche nel repository

        return BadRequest("Failed to delete the photo"); // Restituisce un errore se l'eliminazione fallisce
    }
}
