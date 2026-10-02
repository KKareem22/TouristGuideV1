using TouristGuide.Application.DTOs.Guide;
using TouristGuide.Application.Interfaces;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Enums;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Application.Services
{
    public class TourBookingService : ITourBookingService
    {
        private readonly IUnitOfWork _uow;

        public TourBookingService(IUnitOfWork uow) => _uow = uow;

        public async Task<IReadOnlyList<TourBookingDto>> GetForTouristAsync(string userId)
        {
            var profileRepo = _uow.GetRepository<TouristProfile, int>();
            var profiles = await profileRepo.GetAllAsync();
            var profile = profiles.FirstOrDefault(p => p.UserId == userId)
                ?? throw new Exception("Tourist profile not found.");

            var repo = _uow.GetRepository<TourBooking, int>();
            var all = await repo.GetAllAsync();
            return all.Where(b => b.TouristProfileId == profile.Id && !b.IsDeleted)
                      .OrderByDescending(b => b.BookingDate)
                      .Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<TourBookingDto>> GetRequestsForGuideAsync(string userId)
        {
            var guideRepo = _uow.GetRepository<GuideProfile, int>();
            var guides = await guideRepo.GetAllAsync();
            var guide = guides.FirstOrDefault(g => g.UserId == userId && !g.IsDeleted)
                ?? throw new Exception("Guide profile not found.");

            var tourRepo = _uow.GetRepository<Tour, int>();
            var tours = await tourRepo.GetAllAsync();
            var tourIds = tours.Where(t => t.GuideProfileId == guide.Id).Select(t => t.Id).ToHashSet();

            var repo = _uow.GetRepository<TourBooking, int>();
            var all = await repo.GetAllAsync();
            return all.Where(b => tourIds.Contains(b.TourId) && !b.IsDeleted)
                      .OrderByDescending(b => b.BookingDate)
                      .Select(MapToDto).ToList();
        }

        public async Task<TourBookingDto> CreateAsync(string userId, CreateTourBookingDto dto)
        {
            var profileRepo = _uow.GetRepository<TouristProfile, int>();
            var profiles = await profileRepo.GetAllAsync();
            var profile = profiles.FirstOrDefault(p => p.UserId == userId)
                ?? throw new Exception("Tourist profile not found.");

            var tourRepo = _uow.GetRepository<Tour, int>();
            var tour = await tourRepo.GetByIdAsync(dto.TourId)
                ?? throw new Exception($"Tour {dto.TourId} not found.");

            var repo = _uow.GetRepository<TourBooking, int>();
            var entity = new TourBooking
            {
                BookingDate = dto.BookingDate, TravelerCount = dto.TravelerCount,
                TotalPrice = tour.PricePerPerson * dto.TravelerCount,
                SpecialRequests = dto.SpecialRequests,
                Status = BookingStatus.Pending,
                TouristProfileId = profile.Id, TourId = dto.TourId
            };
            repo.Add(entity);
            await _uow.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<TourBookingDto> AcceptAsync(int bookingId, string guideUserId)
            => await UpdateStatusAsync(bookingId, BookingStatus.Accepted);

        public async Task<TourBookingDto> DeclineAsync(int bookingId, string guideUserId)
            => await UpdateStatusAsync(bookingId, BookingStatus.Rejected);

        private async Task<TourBookingDto> UpdateStatusAsync(int bookingId, BookingStatus status)
        {
            var repo = _uow.GetRepository<TourBooking, int>();
            var booking = await repo.GetByIdAsync(bookingId)
                ?? throw new Exception($"Booking {bookingId} not found.");
            booking.Status = status;
            booking.UpdatedAt = DateTime.UtcNow;
            repo.Update(booking);
            await _uow.SaveChangesAsync();
            return MapToDto(booking);
        }

        private static TourBookingDto MapToDto(TourBooking b) => new()
        {
            Id = b.Id, BookingDate = b.BookingDate, TravelerCount = b.TravelerCount,
            TotalPrice = b.TotalPrice, Status = b.Status.ToString(),
            SpecialRequests = b.SpecialRequests, TouristProfileId = b.TouristProfileId,
            TourId = b.TourId
        };
    }
}
