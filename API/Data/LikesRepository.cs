using System;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

// Repository per gestire i "mi piace"
public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    // Aggiunge un "mi piace"
    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }

    // Rimuove un "mi piace"
    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }

    // Ottiene gli ID degli utenti che l'utente corrente ha messo "mi piace"
    public async Task<IEnumerable<int>> GetCurrenntUserLikeIds(int currentUserId)
    {
        return await context.Likes
            .Where(l => l.SourceUserId == currentUserId)
            .Select(l => l.TargetUserId)
            .ToListAsync();
    }

    // Ottiene un "mi piace" specifico tra due utenti
    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    // Ottiene gli utenti che l'utente ha messo "mi piace" o che hanno messo "mi piace" all'utente
    public async Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDto> query;

        switch (likesParams.Predicate)
        {
            case "liked":
                // Utenti a cui l'utente ha messo "mi piace"
                query = likes
                    .Where(l => l.SourceUserId == likesParams.UserId)
                    .Select(x => x.TargetUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                    break;
            case "likedBy":
                // Utenti che hanno messo "mi piace" all'utente
                query = likes
                    .Where(l => l.TargetUserId == likesParams.UserId)
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                    break;
            default:
                // Utenti che hanno messo "mi piace" reciprocamente
                var likeIds = await GetCurrenntUserLikeIds(likesParams.UserId);

                query = likes
                    .Where(x => x.TargetUserId == likesParams.UserId && likeIds.Contains(x.SourceUserId))
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                    break;
        }
        return await PagedList<MemberDto>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
    }
}
