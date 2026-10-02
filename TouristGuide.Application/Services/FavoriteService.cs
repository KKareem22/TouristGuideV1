using TouristGuide.Application.DTOs.Favorite;
using TouristGuide.Application.Interfaces;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Enums;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IUnitOfWork _uow;

        public FavoriteService(IUnitOfWork uow) => _uow = uow;

        public async Task<IReadOnlyList<UserFavoriteDto>> GetAllAsync(string userId, string? entityType = null)
        {
            var repo = _uow.GetRepository<UserFavorite, int>();
            var all = await repo.GetAllAsync();
            var filtered = all.Where(f => f.UserId == userId && !f.IsDeleted);
            if (!string.IsNullOrEmpty(entityType) && Enum.TryParse<FavoriteType>(entityType, true, out var et))
                filtered = filtered.Where(f => f.EntityType == et);
            return filtered.OrderByDescending(f => f.SavedAt)
                           .Select(f => new UserFavoriteDto
                           {
                               Id = f.Id, EntityType = f.EntityType.ToString(),
                               EntityId = f.EntityId, SavedAt = f.SavedAt
                           }).ToList();
        }

        public async Task<UserFavoriteDto> ToggleAsync(string userId, ToggleFavoriteDto dto)
        {
            if (!Enum.TryParse<FavoriteType>(dto.EntityType, true, out var et))
                throw new Exception($"Invalid entity type: {dto.EntityType}");

            // Resolve profile
            var profileRepo = _uow.GetRepository<TouristProfile, int>();
            var profiles = await profileRepo.GetAllAsync();
            var profile = profiles.FirstOrDefault(p => p.UserId == userId)
                ?? throw new Exception("Tourist profile not found.");

            var repo = _uow.GetRepository<UserFavorite, int>();
            var all = await repo.GetAllAsync();
            var existing = all.FirstOrDefault(f => f.UserId == userId
                                                && f.EntityType == et
                                                && f.EntityId == dto.EntityId
                                                && !f.IsDeleted);
            if (existing is not null)
            {
                existing.IsDeleted = true;
                repo.Update(existing);
                await _uow.SaveChangesAsync();
                return new UserFavoriteDto { Id = existing.Id, EntityType = existing.EntityType.ToString(), EntityId = existing.EntityId, SavedAt = existing.SavedAt };
            }

            var entity = new UserFavorite
            {
                UserId = userId, EntityType = et,
                EntityId = dto.EntityId, SavedAt = DateTime.UtcNow,
                TouristProfileId = profile.Id
            };
            repo.Add(entity);
            await _uow.SaveChangesAsync();
            return new UserFavoriteDto { Id = entity.Id, EntityType = entity.EntityType.ToString(), EntityId = entity.EntityId, SavedAt = entity.SavedAt };
        }

        public async Task<bool> IsFavoriteAsync(string userId, string entityType, int entityId)
        {
            if (!Enum.TryParse<FavoriteType>(entityType, true, out var et)) return false;
            var repo = _uow.GetRepository<UserFavorite, int>();
            var all = await repo.GetAllAsync();
            return all.Any(f => f.UserId == userId && f.EntityType == et && f.EntityId == entityId && !f.IsDeleted);
        }
    }
}
