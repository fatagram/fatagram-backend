using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Fatagram.Application.Dtos.Message;
using Fatagram.Domain.Enums;
using FluentValidation;

namespace Fatagram.Application.Validators.Message
{
    public class MessageDtoValidator : AbstractValidator<SendMessageDto>
    {
        public MessageDtoValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .When(x => x.Type == MessageType.Text)
                .WithErrorCode("CONTENT_REQUIRED_FOR_TEXT")
                .WithMessage("Content is required for text messages.");

            RuleFor(x => x.Media)
                .Empty()
                .When(x => x.Type == MessageType.Text)
                .WithErrorCode("MEDIA_NOT_ALLOWED_FOR_TEXT")
                .WithMessage("Media is not allowed for text messages.");

            RuleFor(x => x.Media)
                .NotEmpty()
                .When(x => x.Type == MessageType.Media)
                .WithErrorCode("MEDIA_REQUIRED_FOR_MEDIA")
                .WithMessage("Media is required for media messages.");

            RuleFor(x => x.Media)
                .Must(media =>
                {
                    if (media == null)
                        return true;

                    int imgCount = media.Count(m => m.Type == MediaType.Image);
                    int otherCount = media.Count(m => m.Type != MediaType.Image);

                    return !(imgCount > 0 && otherCount > 0);
                })
                .When(x => x.Type == MessageType.Media)
                .WithErrorCode("MEDIA_TYPE_MISMATCH")
                .WithMessage("Can not mix iamge and non-image media in the same message.");
        }
    }
}
