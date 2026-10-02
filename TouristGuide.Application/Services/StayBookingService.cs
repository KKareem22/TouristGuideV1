using TouristGuide.Application.DTOs.Stay;
using TouristGuide.Application.Interfaces;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Enums;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Application.Services
{
    public class StayBookingService : IStayBookingService
    {
        private readonly IUnitOfWork _uow;

        public StayBookingService(IUnitOfWork uow) => _uow = uow;

        public async Task<IReadOnlyList<StayBookingDto>> GetForTouristAsync(string userId)
        {
            var profileRepo = _uow.GetRepository<TouristProfile, int>();
            var profiles = await profileRepo.GetAllAsync();
            var profile = profiles.FirstOrDefault(p => p.UserId == userId)
                ?? throw new Exception("Tourist profile not found.");

            var repo = _uow.GetRepository<StayBooking, int>();
            var all = await repo.GetAllAsync();
            return all.Where(b => b.TouristProfileId == profile.Id && !b.IsDeleted)
                      .OrderByDescending(b => b.CheckInDate)
                      .Select(MapToDto).ToList();
        }

        public async Task<StayBookingDto> CreateAsync(string userId, CreateStayBookingDto dto)
        {
            var profileRepo = _uow.GetRepository<TouristProfile, int>();
            var profiles = await profileRepo.GetAllAsync();
            var profile = profiles.FirstOrDefault(p => p.UserId == userId)
                ?? throw new Exception("Tourist profile not found.");

            var roomRepo = _uow.GetRepository<Room, int>();
            var room = await roomRepo.GetByIdAsync(dto.RoomId)
                ?? throw new Exception($"Room {dto.RoomId} not found.");

            var totalNights = (dto.CheckOutDate - dto.CheckInDate).Days;
            if (totalNights <= 0) throw new Exception("Check-out must be after check-in.");

            var repo = _uow.GetRepository<StayBooking, int>();
            var entity = new StayBooking
            {
                CheckInDate = dto.CheckInDate, CheckOutDate = dto.CheckOutDate,
                TotalNights = totalNights, GuestCount = dto.GuestCount,
                TotalPrice = room.PricePerNight * totalNights,
                SpecialRequests = dto.SpecialRequests,
                Status = BookingStatus.Pending,
                TouristProfileId = profile.Id,
                RoomId = dto.RoomId, AccommodationId = dto.AccommodationId
            };
            repo.Add(entity);
            await _uow.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<StayBookingDto> CancelAsync(int bookingId, string userId)
        {
            var repo = _uow.GetRepository<StayBooking, int>();
            var booking = await repo.GetByIdAsync(bookingId)
                ?? throw new Exception($"Booking {bookingId} not found.");
            booking.Status = BookingStatus.Cancelled;
            booking.UpdatedAt = DateTime.UtcNow;
            repo.Update(booking);
            await _uow.SaveChangesAsync();
            return MapToDto(booking);
        }

        private static StayBookingDto MapToDto(StayBooking b) => new()
        {
            Id = b.Id, CheckInDate = b.CheckInDate, CheckOutDate = b.CheckOutDate,
            TotalNights = b.TotalNights, GuestCount = b.GuestCount,
            TotalPrice = b.TotalPrice, Status = b.Status.ToString(),
            SpecialRequests = b.SpecialRequests,
            AccommodationId = b.AccommodationId, RoomId = b.RoomId
        };
    }
}
