using System;

namespace API.Helpers;

public class LikesParams : PaginationParams
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; } = 5;

    public int UserId { get; set; }
    public required string Predicate { get; set; } = "liked";
}
