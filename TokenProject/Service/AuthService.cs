using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TokenProject.Helper;
using TokenProject.Model;

namespace TokenProject.Service;

public class AuthService:IAuthServive
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private IConfiguration _config;
    private readonly IOptions<Jwt> _jwt;

    public AuthService(UserManager<ApplicationUser> userManager,IMapper mapper, IConfiguration config,IOptions<Jwt> jwt)
    {
        _userManager = userManager;
        _mapper = mapper;
        _config = config;
        _jwt = jwt;
    }
    public async Task<AuthModel> Register(RegisterModel dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
            return new AuthModel() { Message = "The Email Already Exist" };

        if (await _userManager.FindByNameAsync(dto.UserName) is not null)
            return new AuthModel() { Message = "The User Name Already Exist" };
        var newUser = _mapper.Map<ApplicationUser>(dto);

        var createdUser = await _userManager.CreateAsync(newUser);
        var signedToRole = await _userManager.AddToRoleAsync(newUser, "User");
        var jwtSecurityToken = await CreateJwtToken(newUser);

        return new AuthModel
        {
            Email = newUser.Email,
            ExpireOn = jwtSecurityToken.ValidTo,
            IsAuthenticated = true,
            Roles = new List<string> { "User" },
            Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
            Username = newUser.UserName
        };

    }


    private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();

        foreach (var role in roles)
            roleClaims.Add(new Claim("roles", role));

        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
        .Union(userClaims)
        .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Value.key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Value.issuer,
        audience: _jwt.Value.audience,
        claims: claims,
            expires: DateTime.Now.AddDays(_jwt.Value.duration),
            signingCredentials: signingCredentials);

        return jwtSecurityToken;
    }
}
