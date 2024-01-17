using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
    {
        var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _context.Likes.AsQueryable();

        if (likesParams.Predicate == "liked")
        {
            likes = likes.Where(l => l.SourceUserId == likesParams.UserId);
            users = likes.Select(l => l.TargetUser);
        }

        if (likesParams.Predicate == "likedBy")
        {
            likes = likes.Where(l => l.TargetUserId == likesParams.UserId);
            users = likes.Select(l => l.SourceUser);
        }

        var likedUsers = users.Select(u => new LikeDto
        {
            Id = u.Id,
            Age = u.DateOfBirth.CalculateAge(),
            City = u.City,
            KnownAs = u.knownAs,
            PhotoUrl = u.Photos.FirstOrDefault(p => p.IsMain) != null ? u.Photos.FirstOrDefault(p => p.IsMain).Url : null,
            UserName = u.UserName,
        });

        return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<AppUser?> GetUserWithLikes(int userId)
    {
        return await _context.Users
            .Include(u => u.LikedUsers)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
