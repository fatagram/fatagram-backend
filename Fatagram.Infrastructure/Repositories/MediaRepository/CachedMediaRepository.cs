using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Application.Abstractions.Cache;
using Fatagram.Infrastructure.Data;
using Fatagram.Application.Common.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Fatagram.Application.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Fatagram.Infrastructure.Repositories.MediaRepository
{
    public class CachedMediaRepository(
        IBaseRepository<MessageMedia> inner,
        AppDbContext dbContext,
        ICacheService cacheService
    ) : BaseRepositoryDecorator<MessageMedia>(inner, dbContext), IMediaRepository
    {
        private readonly ICacheService _cacheService = cacheService;

        public Task<List<MessageMedia>> GetConversationMediaAsync(
            Guid conversationId,
            List<MediaType>? types,
            int cursor,
            int limit
        )
        {
            var inner = (IMediaRepository)_inner;
            return inner.GetConversationMediaAsync(conversationId, types, cursor, limit);
        }

        public async Task<MessageMediaAroundAnchorProjection> GetMediaAroundAnchorAsync(
            Guid conversationId,
            Guid? anchorMediaId,
            int count
        )
        {
            if (anchorMediaId == null || !anchorMediaId.HasValue || count <= 0)
                return new MessageMediaAroundAnchorProjection();

            var mediaMapKey = $"conv:{conversationId}:media:map";
            var mediaTimelineKey = $"conv:{conversationId}:media:timeline";

            var anchorValue = await _cacheService.HashGetAsync(
                mediaMapKey,
                anchorMediaId.Value.ToString()
            );

            var inner = (IMediaRepository)_inner;

            if (anchorValue != null)
            {
                var parts = anchorValue.ToString().Split(':');
                if (
                    parts.Length == 2
                    && int.TryParse(parts[0], out var anchorSeq)
                    && int.TryParse(parts[1], out var anchorIndex)
                )
                {
                    var leftInCache = await _cacheService.SortedSetRangeByScoreAsync<MessageMedia>(
                        mediaTimelineKey,
                        order: Order.Descending,
                        start: anchorSeq - count,
                        stop: anchorSeq
                    );

                    var leftInCacheCorrect = leftInCache
                        .Where(m =>
                            m.MessageSequence < anchorSeq
                            || (m.MessageSequence == anchorSeq && m.IndexInMessage < anchorIndex)
                        )
                        .Take(count)
                        .ToList();

                    var rightInCache = await _cacheService.SortedSetRangeByScoreAsync<MessageMedia>(
                        mediaTimelineKey,
                        order: Order.Ascending,
                        start: anchorSeq,
                        stop: anchorSeq + count
                    );

                    var rightInCacheCorrect = rightInCache
                        .Where(m =>
                            m.MessageSequence > anchorSeq
                            || (m.MessageSequence == anchorSeq && m.IndexInMessage > anchorIndex)
                        )
                        .Take(count)
                        .ToList();

                    List<MessageMedia> leftFromDb = [];
                    if (leftInCacheCorrect.Count < count)
                    {
                        var missingCount = count - leftInCacheCorrect.Count;

                        leftFromDb = await _dbContext
                            .MessageMedias.Where(m =>
                                m.Message.ConversationId == conversationId
                                && (
                                    m.MessageSequence < anchorSeq
                                    || (
                                        m.MessageSequence == anchorSeq
                                        && m.IndexInMessage < anchorIndex
                                    )
                                )
                            )
                            .OrderByDescending(m => m.MessageSequence)
                            .ThenByDescending(m => m.IndexInMessage)
                            .Take(missingCount)
                            .ToListAsync();
                    }

                    return new()
                    {
                        Left = [.. leftInCacheCorrect, .. leftFromDb],
                        Right = rightInCacheCorrect,
                    };
                }
            }

            var anchorInDb = await _dbContext
                .MessageMedias.Where(m => m.Id == anchorMediaId)
                .FirstOrDefaultAsync();

            if (anchorInDb == null)
                return new();

            var leftInDb = await _dbContext
                .MessageMedias.Where(m =>
                    m.Message.ConversationId == conversationId
                    && (
                        m.MessageSequence < anchorInDb.MessageSequence
                        || (
                            m.MessageSequence == anchorInDb.MessageSequence
                            && m.IndexInMessage < anchorInDb.IndexInMessage
                        )
                    )
                )
                .OrderByDescending(m => m.MessageSequence)
                .ThenByDescending(m => m.IndexInMessage)
                .Take(count)
                .ToListAsync();

            var rightInDb = await _dbContext
                .MessageMedias.Where(m =>
                    m.Message.ConversationId == conversationId
                    && (
                        m.MessageSequence > anchorInDb.MessageSequence
                        || (
                            m.MessageSequence == anchorInDb.MessageSequence
                            && m.IndexInMessage > anchorInDb.IndexInMessage
                        )
                    )
                )
                .OrderBy(m => m.MessageSequence)
                .ThenBy(m => m.IndexInMessage)
                .Take(count)
                .ToListAsync();

            List<MessageMedia> rightFromCache = [];
            if (rightInDb.Count < count)
            {
                var missingCount = count - rightInDb.Count;
                var rightInCache = (
                    await _cacheService.SortedSetRangeByScoreAsync<MessageMedia>(
                        mediaTimelineKey,
                        order: Order.Ascending,
                        start: anchorInDb.MessageSequence,
                        stop: anchorInDb.MessageSequence + missingCount
                    )
                );

                rightFromCache =
                [
                    .. rightInCache
                        .Where(m =>
                            m.MessageSequence > anchorInDb.MessageSequence
                            || (
                                m.MessageSequence == anchorInDb.MessageSequence
                                && m.IndexInMessage > anchorInDb.IndexInMessage
                            )
                        )
                        .Take(count),
                ];
            }

            return new() { Left = leftInDb, Right = [.. rightInDb, .. rightFromCache] };
        }

        public async Task<List<MessageMedia>> GetMediaAroundAsync(
            Guid conversationId,
            Guid mediaId,
            bool before,
            int count
        )
        {
            if (count <= 0)
                return [];

            var mediaMapKey = $"conv:{conversationId}:media:map";
            var mediaTimelineKey = $"conv:{conversationId}:media:timeline";

            var anchorValue = await _cacheService.HashGetAsync(mediaMapKey, mediaId.ToString());

            if (anchorValue != null)
            {
                var parts = anchorValue.ToString().Split(':');
                if (
                    parts.Length == 2
                    && int.TryParse(parts[0], out var anchorSeq)
                    && int.TryParse(parts[1], out var anchorIndex)
                )
                {
                    if (before)
                    {
                        var inCache = await _cacheService.SortedSetRangeByScoreAsync<MessageMedia>(
                            mediaTimelineKey,
                            order: Order.Descending,
                            start: anchorSeq - count,
                            stop: anchorSeq
                        );

                        var inCacheCorrect = inCache
                            .Where(m =>
                                m.MessageSequence < anchorSeq
                                || (
                                    m.MessageSequence == anchorSeq && m.IndexInMessage < anchorIndex
                                )
                            )
                            .Take(count)
                            .ToList();

                        if (inCacheCorrect.Count >= count)
                        {
                            inCacheCorrect.Reverse(); // Äáº£o ngÆ°á»£c Ä‘á»ƒ Ä‘Ãºng thá»© tá»± thá»i gian (Reverse for chronological order)
                            return inCacheCorrect;
                        }

                        var missingCount = count - inCacheCorrect.Count;
                        var inDb = await _dbContext
                            .MessageMedias.Where(m =>
                                m.Message.ConversationId == conversationId
                                && (
                                    m.MessageSequence < anchorSeq
                                    || (
                                        m.MessageSequence == anchorSeq
                                        && m.IndexInMessage < anchorIndex
                                    ) // Sá»­a lá»—i dáº¥u > thÃ nh < (Fixed operator bug)
                                )
                            )
                            .OrderByDescending(m => m.MessageSequence)
                            .ThenByDescending(m => m.IndexInMessage)
                            .Take(missingCount)
                            .ToListAsync();

                        // Tá»‘i Æ°u hÃ³a hiá»‡u nÄƒng báº±ng HashSet (Optimize performance using HashSet for O(1) lookup)
                        var cachedIds = inCacheCorrect.Select(c => c.Id).ToHashSet();

                        var result = new List<MessageMedia>(inCacheCorrect);
                        result.AddRange(inDb.Where(dbMedia => !cachedIds.Contains(dbMedia.Id)));

                        result.Reverse(); // Äáº£o ngÆ°á»£c máº£ng cuá»‘i cÃ¹ng (Reverse final array)
                        return result;
                    }

                    // TrÆ°á»ng há»£p after (After case)
                    var inCacheAfter = await _cacheService.SortedSetRangeByScoreAsync<MessageMedia>(
                        mediaTimelineKey,
                        order: Order.Ascending,
                        start: anchorSeq,
                        stop: anchorSeq + count
                    );

                    var inCacheCorrectAfter = inCacheAfter
                        .Where(m =>
                            m.MessageSequence > anchorSeq
                            || (m.MessageSequence == anchorSeq && m.IndexInMessage > anchorIndex)
                        )
                        .Take(count)
                        .ToList();

                    if (inCacheCorrectAfter.Count >= count)
                        return inCacheCorrectAfter;

                    var missingCountAfter = count - inCacheCorrectAfter.Count;
                    var inDbAfter = await _dbContext
                        .MessageMedias.Where(m =>
                            m.Message.ConversationId == conversationId
                            && (
                                m.MessageSequence > anchorSeq
                                || (
                                    m.MessageSequence == anchorSeq && m.IndexInMessage > anchorIndex
                                )
                            )
                        )
                        .OrderBy(m => m.MessageSequence)
                        .ThenBy(m => m.IndexInMessage)
                        .Take(missingCountAfter)
                        .ToListAsync();

                    var cachedIdsAfter = inCacheCorrectAfter.Select(c => c.Id).ToHashSet();
                    var resultAfter = new List<MessageMedia>(inCacheCorrectAfter);
                    resultAfter.AddRange(
                        inDbAfter.Where(dbMedia => !cachedIdsAfter.Contains(dbMedia.Id))
                    );

                    return resultAfter;
                }
            }

            var anchorInDb = await _dbContext
                .MessageMedias.Where(m =>
                    m.Id == mediaId && m.Message.ConversationId == conversationId
                )
                .FirstOrDefaultAsync();

            if (anchorInDb == null)
                return [];

            if (before)
            {
                var inDbFirst = await _dbContext
                    .MessageMedias.Where(m =>
                        m.Message.ConversationId == conversationId
                        && (
                            m.MessageSequence < anchorInDb.MessageSequence
                            || (
                                m.MessageSequence == anchorInDb.MessageSequence
                                && m.IndexInMessage < anchorInDb.IndexInMessage
                            )
                        )
                    )
                    .OrderByDescending(m => m.MessageSequence)
                    .ThenByDescending(m => m.IndexInMessage)
                    .Take(count)
                    .ToListAsync();

                if (inDbFirst.Count >= count)
                {
                    inDbFirst.Reverse();
                    return inDbFirst;
                }

                var missingFromCache = count - inDbFirst.Count;
                var inCacheSecond = await _cacheService.SortedSetRangeByScoreAsync<MessageMedia>(
                    mediaTimelineKey,
                    order: Order.Descending,
                    start: anchorInDb.MessageSequence - missingFromCache,
                    stop: anchorInDb.MessageSequence
                );

                var inCacheCorrectSecond = inCacheSecond
                    .Where(m =>
                        m.MessageSequence < anchorInDb.MessageSequence
                        || (
                            m.MessageSequence == anchorInDb.MessageSequence
                            && m.IndexInMessage < anchorInDb.IndexInMessage
                        )
                    )
                    .Take(missingFromCache)
                    .ToList();

                var dbIds = inDbFirst.Select(d => d.Id).ToHashSet();
                var mergedResult = new List<MessageMedia>(inDbFirst);
                mergedResult.AddRange(
                    inCacheCorrectSecond.Where(cacheMedia => !dbIds.Contains(cacheMedia.Id))
                );

                mergedResult.Reverse();
                return mergedResult;
            }

            // TrÆ°á»ng há»£p after (After case)
            var inDbFirstAfter = await _dbContext
                .MessageMedias.Where(m =>
                    m.Message.ConversationId == conversationId
                    && (
                        m.MessageSequence > anchorInDb.MessageSequence
                        || (
                            m.MessageSequence == anchorInDb.MessageSequence
                            && m.IndexInMessage > anchorInDb.IndexInMessage
                        )
                    )
                )
                .OrderBy(m => m.MessageSequence)
                .ThenBy(m => m.IndexInMessage)
                .Take(count)
                .ToListAsync();

            if (inDbFirstAfter.Count >= count)
                return inDbFirstAfter;

            var missingFromCacheAfter = count - inDbFirstAfter.Count;
            var inCacheSecondAfter = await _cacheService.SortedSetRangeByScoreAsync<MessageMedia>(
                mediaTimelineKey,
                order: Order.Ascending,
                start: anchorInDb.MessageSequence,
                stop: anchorInDb.MessageSequence + missingFromCacheAfter
            );

            var inCacheCorrectSecondAfter = inCacheSecondAfter
                .Where(m =>
                    m.MessageSequence > anchorInDb.MessageSequence
                    || (
                        m.MessageSequence == anchorInDb.MessageSequence
                        && m.IndexInMessage > anchorInDb.IndexInMessage
                    )
                )
                .Take(missingFromCacheAfter)
                .ToList();

            var dbIdsAfter = inDbFirstAfter.Select(d => d.Id).ToHashSet();
            var mergedResultAfter = new List<MessageMedia>(inDbFirstAfter);
            mergedResultAfter.AddRange(
                inCacheCorrectSecondAfter.Where(cacheMedia => !dbIdsAfter.Contains(cacheMedia.Id))
            );

            return mergedResultAfter;
        }
    }
}
