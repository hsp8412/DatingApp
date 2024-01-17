using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.interfaces;

public interface ILikesRepository
{
    Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId);

    Task<AppUser?> GetUserWithLikes(int UserId);

    Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
}
