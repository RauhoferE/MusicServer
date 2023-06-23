using System.Security.Claims;

namespace MusicServer.Entities.DTOs
{
    public class LoginUserClaimsResult
    {
        public ICollection<Claim> AuthenticationClaims { get; set; }

        public ICollection<Claim> FrontendClaims { get; set; }
    }
}
