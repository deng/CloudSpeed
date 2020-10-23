using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CloudSpeed.Entities;
using CloudSpeed.Identity;
using CloudSpeed.Settings;
using CloudSpeed.Web.Models;
using CloudSpeed.Web.Requests;
using CloudSpeed.Web.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace CloudSpeed.Managers
{
    public class MemberManager
    {
        private readonly JwtSetting _jwtSetting;
        private readonly UserManager<Member> _memberManager;

        public MemberManager(JwtSetting jwtSetting, UserManager<Member> memberManager)
        {
            _jwtSetting = jwtSetting;
            _memberManager = memberManager;
        }

        public async Task<ApiResponse<SignedInMember>> CreateMember(MemberCreateRequest request)
        {
            if (string.IsNullOrEmpty(request.Address))
                return ApiResponse.BadRequestResult<SignedInMember>("invalid address");

            if (string.IsNullOrEmpty(request.Password))
                return ApiResponse.BadRequestResult<SignedInMember>("invalid password");

            var member = new Member()
            {
                UserName = request.Address,
            };

            var createdResult = await _memberManager.CreateAsync(member, request.Password);

            if (!createdResult.Succeeded)
            {
                return ApiResponse.BadRequestResult<SignedInMember>(createdResult.Errors.FirstOrDefault().Code ?? "create failure");
            }

            var userClaim = new MemberClaim()
            {
                UserId = member.Id,
                Address = member.UserName,
            };
            var token = CreateMemberToken(userClaim);
            return ApiResponse.Ok(new SignedInMember()
            {
                Token = token,
                Address = member.UserName,
            });
        }

        public async Task<ApiResponse<bool>> CheckMember(MemberCheckRequest request)
        {
            if (string.IsNullOrEmpty(request.Address))
                return ApiResponse.BadRequestResult<bool>("invalid address");

            var member = await _memberManager.FindByNameAsync(request.Address);
            return ApiResponse.Ok<bool>(member != null);
        }

        public async Task<ApiResponse<SignedInMember>> Login(MemberLoginRequest request)
        {
            var member = await _memberManager.FindByNameAsync(request.Address);
            if (member != null && !string.IsNullOrEmpty(request.Password))
            {
                var validateResponse = await ValidateMember(member, request.Password);
                if (!(validateResponse.Success))
                {
                    return ApiResponse.BadRequestResult<SignedInMember>(validateResponse.Error);
                }
                var userClaim = new MemberClaim()
                {
                    UserId = member.Id,
                    Address = member.UserName,
                };
                var token = CreateMemberToken(userClaim);
                return ApiResponse.Ok(new SignedInMember()
                {
                    Token = token,
                    Address = member.UserName,
                });
            }
            return ApiResponse.BadRequestResult<SignedInMember>("login failure");
        }

        public async Task<string> GetUserId(string userName)
        {
            var member = await _memberManager.FindByNameAsync(userName);
            return member?.Id ?? string.Empty;
        }

        private async Task<ApiResponse<string>> ValidateMember(Member member, string password)
        {
            if (await _memberManager.IsLockedOutAsync(member))
            {
                return ApiResponse.BadRequestResult("lockout");
            }
            var checkPassword = await _memberManager.CheckPasswordAsync(member, password);
            if (!checkPassword)
            {
                await _memberManager.AccessFailedAsync(member);
                return ApiResponse.BadRequestResult("login failure");
            }
            else
            {
                await _memberManager.ResetAccessFailedCountAsync(member);
                return ApiResponse.Ok();
            }
        }

        private string CreateMemberToken(MemberClaim member)
        {
            var claims = new[]
            {
                new Claim("account", member.Address),
                new Claim("id", member.UserId),
            };

            var jwtTokenKey = _jwtSetting.JwtTokenKey;
            var jwtTokenIssuer = _jwtSetting.JwtTokenIssuer;
            var jwtTokenAudience = _jwtSetting.JwtTokenAudience;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(jwtTokenIssuer, jwtTokenAudience, claims, expires: DateTime.Now.AddDays(30), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}