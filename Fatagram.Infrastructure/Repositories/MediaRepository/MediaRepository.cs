using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Application.Common.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.MediaRepository
{
    public class MediaRepository(
        AppDbContext dbContext,
        ILogger<BaseRepository<MessageMedia>>? logger = null
    ) : BaseRepository<MessageMedia>(dbContext, logger), IMediaRepository
    {
        public async Task<List<MessageMedia>> GetMediaAroundAsync(
            Guid conversationId,
            Guid mediaId,
            bool before,
            int count
        )
        {
            if (count <= 0)
                return [];

            var anchor = await _dbContext
                .MessageMedias.Where(m =>
                    m.Id == mediaId && m.Message.ConversationId == conversationId
                )
                .Select(m => new { m.MessageSequence, m.IndexInMessage })
                .FirstOrDefaultAsync();

            if (anchor == null)
                return [];

            if (before)
            {
                var result = await _dbContext
                    .MessageMedias.Where(m =>
                        m.Message.ConversationId == conversationId
                        && (
                            m.MessageSequence < anchor.MessageSequence
                            || (
                                m.MessageSequence == anchor.MessageSequence
                                && m.IndexInMessage < anchor.IndexInMessage
                            )
                        )
                    )
                    .OrderByDescending(m => m.MessageSequence)
                    .ThenByDescending(m => m.IndexInMessage)
                    .Take(count)
                    .AsNoTracking()
                    .ToListAsync();

                result.Reverse();
                return result;
            }

            return await _dbContext
                .MessageMedias.Where(m =>
                    m.Message.ConversationId == conversationId
                    && (
                        m.MessageSequence > anchor.MessageSequence
                        || (
                            m.MessageSequence == anchor.MessageSequence
                            && m.IndexInMessage > anchor.IndexInMessage
                        )
                    )
                )
                .OrderBy(m => m.MessageSequence)
                .ThenBy(m => m.IndexInMessage)
                .Take(count)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MessageMediaAroundAnchorProjection> GetMediaAroundAnchorAsync(
            Guid conversationId,
            Guid? anchorMediaId,
            int count
        )
        {
            var anchorInfo = _dbContext
                .MessageMedias.Where(m => m.Id == anchorMediaId)
                .Select(m => new { m.MessageSequence, m.IndexInMessage });

            var leftQuery = _dbContext
                .MessageMedias.Where(m => m.Message.ConversationId == conversationId)
                .Where(m =>
                    m.MessageSequence < anchorInfo.Select(a => a.MessageSequence).FirstOrDefault()
                    || (
                        m.MessageSequence
                            == anchorInfo.Select(a => a.MessageSequence).FirstOrDefault()
                        && m.IndexInMessage
                            < anchorInfo.Select(a => a.IndexInMessage).FirstOrDefault()
                    )
                )
                .OrderByDescending(m => m.MessageSequence)
                .ThenByDescending(m => m.IndexInMessage)
                .Take(count);

            var rightQuery = _dbContext
                .MessageMedias.Where(m => m.Message.ConversationId == conversationId)
                .Where(m =>
                    m.MessageSequence > anchorInfo.Select(a => a.MessageSequence).FirstOrDefault()
                    || (
                        m.MessageSequence
                            == anchorInfo.Select(a => a.MessageSequence).FirstOrDefault()
                        && m.IndexInMessage
                            > anchorInfo.Select(a => a.IndexInMessage).FirstOrDefault()
                    )
                )
                .OrderBy(m => m.MessageSequence)
                .ThenBy(m => m.IndexInMessage)
                .Take(count);

            var allMedia = await leftQuery.Concat(rightQuery).AsNoTracking().ToListAsync();

            var anchor = await anchorInfo.FirstOrDefaultAsync();
            if (anchor == null)
                return new MessageMediaAroundAnchorProjection();

            return new MessageMediaAroundAnchorProjection
            {
                Left =
                [
                    .. allMedia
                        .Where(m =>
                            m.MessageSequence < anchor.MessageSequence
                            || (
                                m.MessageSequence == anchor.MessageSequence
                                && m.IndexInMessage < anchor.IndexInMessage
                            )
                        )
                        .OrderBy(m => m.MessageSequence)
                        .ThenBy(m => m.IndexInMessage),
                ],
                Right =
                [
                    .. allMedia
                        .Where(m =>
                            m.MessageSequence > anchor.MessageSequence
                            || (
                                m.MessageSequence == anchor.MessageSequence
                                && m.IndexInMessage > anchor.IndexInMessage
                            )
                        )
                        .OrderBy(m => m.MessageSequence)
                        .ThenBy(m => m.IndexInMessage),
                ],
            };
        }
    }
}
