using API.Controllers;
using API.DTOs;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API;

[Authorize]
public class MessagesController(IUnitOfWork unitOfWork, 
    IMapper mapper) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername();

        // Controlla se l'utente sta cercando di inviare un messaggio a se stesso
        if (username == createMessageDto.RecipientUsername.ToLower())
            return BadRequest("You cannot message yourself");
        
        var sender = await unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

        // Controlla se il destinatario o il mittente non esistono
        if (recipient == null || sender == null || sender.UserName == null || recipient.UserName == null) return BadRequest("Cannot send message at this time");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        unitOfWork.MessageRepository.AddMessage(message);

        // Salva il messaggio nel repository
        if (await unitOfWork.Complete()) return Ok(mapper.Map<MessageDto>(message));

        return BadRequest("Failed to save message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser(
        [FromQuery]MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername();

        var messages = await unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

        // Aggiunge l'header di paginazione alla risposta
        Response.AddPaginationHeader(messages);

        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername();

        // Ottiene il thread di messaggi tra l'utente corrente e l'username specificato
        return Ok(await unitOfWork.MessageRepository.GetMessageThread(currentUsername, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

        var message = await unitOfWork.MessageRepository.GetMessage(id);

        // Controlla se il messaggio esiste
        if (message == null) return BadRequest("Cannot delete this message");

        // Controlla se l'utente ha i permessi per cancellare il messaggio
        if (message.SenderUsername != username && message.RecipientUsername != username) 
            return Forbid();

        // Marca il messaggio come cancellato dal mittente o dal destinatario
        if (message.SenderUsername == username) message.SenderDeleted = true;
        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        // Cancella il messaggio se è stato cancellato da entrambi
        if (message is {SenderDeleted: true, RecipientDeleted: true}) {
            unitOfWork.MessageRepository.DeleteMessage(message);
        }

        // Salva le modifiche nel repository
        if (await unitOfWork.Complete()) return Ok();

        return BadRequest("Problem deleting the message");
    }
}