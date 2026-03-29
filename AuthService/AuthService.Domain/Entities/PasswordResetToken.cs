using AuthService.Shared;
using DotnetBaseKit.Components.Domain.Sql.Entities.Base;

namespace AuthService.Domain.Entities
{
    public class ResetPasswordToken : BaseEntity
    {
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public bool Used { get; set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; }

        protected ResetPasswordToken() { }

        public ResetPasswordToken(Guid userId, string token)
        {
            UserId = userId;
            TokenHash = TokenHasher.HashToken(token);
            ExpirationDate = DateTime.UtcNow.AddHours(1);
            Used = false;
        }

        public void MarkAsUsed()
        {
            Used = true;
        }

        public bool IsValid(string token)
        {
            return !Used
                && ExpirationDate > DateTime.UtcNow
                && TokenHash == TokenHasher.HashToken(token);
        }


        public override void Validate()
        {
        }
    }
}