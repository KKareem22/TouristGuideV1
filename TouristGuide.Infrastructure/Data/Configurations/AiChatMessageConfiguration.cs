using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class AiChatMessageConfiguration : IEntityTypeConfiguration<AiChatMessage>
    {
        public void Configure(EntityTypeBuilder<AiChatMessage> builder)
        {
            builder.Property(m => m.Content)
                   .IsRequired();

            builder.HasOne(m => m.Session)
                   .WithMany(s => s.Messages)
                   .HasForeignKey(m => m.SessionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
