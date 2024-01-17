using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class LikesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly ILikesRepository _likesRepository;

    public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
    {
        _userRepository = userRepository;
        _likesRepository = likesRepository;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var userId = User.GetUserId();
        if (userId == null) return BadRequest("Invalid token");
        var sourceUserId = userId ?? 0;
        var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);
        if (sourceUser == null) return NotFound("Source user not found");

        var targetUser = await _userRepository.GetUserByUsernameAsync(username);
        if (targetUser == null) return NotFound("Liked user not found");

        if (targetUser.Id == sourceUserId) return BadRequest("You cannot like yourself");

        var existingLike = await _likesRepository.GetUserLike(sourceUserId, targetUser.Id);
        if (existingLike != null) return BadRequest("Like already exists");

        var like = new UserLike
        {
            SourceUserId = sourceUserId,
            TargetUserId = targetUser.Id
        };

        sourceUser.LikedUsers.Add(like);
        if (await _userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Failed to add like");
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        var userId = User.GetUserId();
        if (userId == null) return BadRequest("Invalid token");
        likesParams.UserId = userId;
        var users = await _likesRepository.GetUserLikes(likesParams);
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
        return Ok(users);
    }
}
