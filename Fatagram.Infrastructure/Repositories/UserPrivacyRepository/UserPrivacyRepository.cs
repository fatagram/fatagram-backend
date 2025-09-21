using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.UserPrivacyRepository.Interface;
using Fatagram.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Repositories.UserPrivacyRepository
{
    /// <summary>
    /// Repository for managing user privacy settings.
    /// </summary>
    public class UserPrivacyRepository : IUserPrivacyRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserPrivacyRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public UserPrivacyRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets the privacy levels for the specified fields of a user asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="field">The fields for which to get the privacy levels.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a dictionary of field names and their privacy levels.</returns>
        public async Task<Dictionary<string, PrivacyLevel>> GetAsync(Guid userId, IEnumerable<string> field)
        {
            var userPrivacy = new Dictionary<string, PrivacyLevel>();
            foreach (var f in field)
            {
                var privacy = await _dbContext.UserPrivacies.FirstOrDefaultAsync(up => up.UserId == userId && up.Field == f);
                if (privacy != null)
                {
                    userPrivacy.Add(privacy.Field, privacy.PrivacyLevel);
                }
                else userPrivacy.Add(f, PrivacyLevel.Public);
            }
            return userPrivacy;
        }


        /// <summary>
        /// Sets all privacy levels of a user to public asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating the success of the operation.</returns>
        public async Task InitialAsync(Guid userId)
        {
            var fields = await _dbContext.UserPrivacies.Where(up => up.UserId == userId).ToListAsync();
            foreach (var field in fields)
            {
                _dbContext.UserPrivacies.Remove(field);
            }
            await _dbContext.SaveChangesAsync();
        }


        /// <summary>
        /// Sets the privacy level for a user asynchronously.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="userPrivacy">The user privacy settings to set.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating the success of the operation.</returns>
        public async Task UpdateAsync(UserPrivacy userPrivacy)
        {
            var privacy = await _dbContext.UserPrivacies.FirstOrDefaultAsync(up => up.UserId == userPrivacy.UserId && up.Field == userPrivacy.Field);
            if (privacy != null)
            {
                privacy.PrivacyLevel = userPrivacy.PrivacyLevel;
                _dbContext.UserPrivacies.Update(privacy);
            }
            else
            {
                await _dbContext.UserPrivacies.AddAsync(userPrivacy);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
