using FluentValidation;
using UserService.Application.DTOs;

namespace UserService.Application.Validations
{
    public class UserProfileRequestDtoContract : AbstractValidator<UserProfileRequestDto>
    {
        public UserProfileRequestDtoContract()
        {
            RuleFor(x => x.AvatarUrl)
                .NotEmpty().WithMessage("Avatar URL cannot be empty.")
                .Must(BeAValidUrl).WithMessage("Avatar URL is not valid.");
        }

        private bool BeAValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult))
            {
                return false;
            }

            if (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(uriResult.Host) || !uriResult.Host.Contains("."))
            {
                return false;
            }

            return true;
        }
    }
}