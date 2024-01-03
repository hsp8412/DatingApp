using System.Security.Claims;
using API.DTOs;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        return Ok(await _userRepository.GetMembersAsync());
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto?>> GetUser(string username)
    {
        return await _userRepository.GetMemberAsync(username);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (username == null)
        {
            return BadRequest("Invalid user");
        }
        var user = await _userRepository.GetUserByUsernameAsync(username);

        if (user == null)
        {
            return NotFound();
        }

        _mapper.Map(memberUpdateDto, user);

        if (await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user");
    }
}
