using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Infrastructure.Data
{
    /// <summary>
    /// Seeds realistic Egyptian tourist data into the database the first time
    /// the application starts.  All checks are idempotent — re-running never
    /// creates duplicate rows.
    ///
    /// Seeding order (respects FK constraints):
    ///   1. GuideProfile       (parent of Tour)
    ///   2. TouristPlaces      (5 Egyptian landmarks)
    ///   3. Activities         (child of TouristPlace)
    ///   4. Tours              (5 Egypt-based tours, child of GuideProfile)
    ///   5. TourItinerarySteps (child of Tour)
    ///   6. Accommodations     (5 real hotels in Egypt)
    ///   7. AccommodationAmenities (child of Accommodation)
    ///   8. Rooms              (child of Accommodation)
    /// </summary>
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext context, ILogger logger)
        {
            // Run any pending migrations automatically so seeding never fails
            // on a freshly cloned repo.
            await context.Database.MigrateAsync();

            // ── 1. Seed system GuideProfile ───────────────────────────────────
            // Tours require a GuideProfileId FK.  We create one "system" guide
            // whose UserId is a deterministic placeholder (not a real Identity user).
            const string systemGuideUserId = "SEED-GUIDE-00000000-0000-0000-0000-000000000001";

            GuideProfile guide;
            if (!await context.GuideProfiles.AnyAsync(g => g.UserId == systemGuideUserId))
            {
                guide = new GuideProfile
                {
                    UserId         = systemGuideUserId,
                    Bio            = "Expert Egyptologist and certified tour guide with over 15 years exploring Egypt's ancient wonders.",
                    Location       = "Cairo, Egypt",
                    Languages      = "English|Arabic|French",
                    Specialties    = "Ancient History|Desert Safari|Nile Cruises|Diving",
                    LicenseNumber  = "EG-GUIDE-001",
                    IsVerified     = true,
                    ExperienceYears = 15,
                    HappyTravelers = 1240,
                    AverageRating  = 4.9m,
                    ReviewCount    = 312,
                    PricePerPerson = 85m,
                    ProfileImageUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400",
                    CoverImageUrl   = "https://images.unsplash.com/photo-1539768942893-daf53e448371?w=1200"
                };
                context.GuideProfiles.Add(guide);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded system GuideProfile (Id={Id})", guide.Id);
            }
            else
            {
                guide = await context.GuideProfiles
                    .FirstAsync(g => g.UserId == systemGuideUserId);
                logger.LogInformation("System GuideProfile already exists (Id={Id}), skipping", guide.Id);
            }

            // ── 2. Seed TouristPlaces ─────────────────────────────────────────
            if (!await context.TouristPlaces.AnyAsync())
            {
                var places = new List<TouristPlace>
                {
                    new()
                    {
                        Name        = "Pyramids of Giza",
                        Description = "The last surviving wonder of the ancient world. The Great Pyramid of Khufu, built around 2560 BC, " +
                                      "stands 138 metres tall alongside the pyramids of Khafre and Menkaure. The enigmatic Great Sphinx " +
                                      "guards the plateau — an unmissable icon of human civilisation.",
                        City        = "Giza",
                        Country     = "Egypt",
                        Category    = PlaceCategory.Historic,
                        EntryFee    = 20m,
                        OpeningTime = new TimeSpan(8, 0, 0),
                        ClosingTime = new TimeSpan(17, 0, 0),
                        ImageUrl    = "https://images.unsplash.com/photo-1539768942893-daf53e448371?w=800",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1539768942893-daf53e448371?w=400",
                        Tags        = "Ancient|Wonder|Historic|Desert",
                        AverageRating = 4.9m,
                        ReviewCount = 8420,
                        IsFeatured  = true,
                        Latitude    = 29.9792,
                        Longitude   = 31.1342,
                        LocationUrl = "https://maps.google.com/?q=Pyramids+of+Giza"
                    },
                    new()
                    {
                        Name        = "Karnak Temple Complex",
                        Description = "The largest religious building ever constructed, Karnak covers 200 acres in ancient Thebes (Luxor). " +
                                      "Its Hypostyle Hall — 134 towering columns — is one of the most awe-inspiring spaces in Egypt. " +
                                      "Construction spanned 2,000 years across 30 pharaohs.",
                        City        = "Luxor",
                        Country     = "Egypt",
                        Category    = PlaceCategory.Historic,
                        EntryFee    = 15m,
                        OpeningTime = new TimeSpan(6, 0, 0),
                        ClosingTime = new TimeSpan(17, 30, 0),
                        ImageUrl    = "https://images.unsplash.com/photo-1562679299-8f22b84e0e47?w=800",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1562679299-8f22b84e0e47?w=400",
                        Tags        = "Temple|Pharaoh|Ancient|UNESCO",
                        AverageRating = 4.8m,
                        ReviewCount = 5310,
                        IsFeatured  = true,
                        Latitude    = 25.7188,
                        Longitude   = 32.6573,
                        LocationUrl = "https://maps.google.com/?q=Karnak+Temple+Luxor"
                    },
                    new()
                    {
                        Name        = "Dahab Blue Hole",
                        Description = "One of the world's most famous diving sites, the Blue Hole is a 130-metre deep underwater sinkhole " +
                                      "off the coast of Dahab on the Red Sea. The surrounding coral reef is alive with vibrant marine life " +
                                      "accessible even to snorkellers at the surface.",
                        City        = "Dahab",
                        Country     = "Egypt",
                        Category    = PlaceCategory.Nature,
                        EntryFee    = 5m,
                        OpeningTime = new TimeSpan(7, 0, 0),
                        ClosingTime = new TimeSpan(18, 0, 0),
                        ImageUrl    = "https://images.unsplash.com/photo-1682687220063-4742bd7fd538?w=800",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1682687220063-4742bd7fd538?w=400",
                        Tags        = "Diving|Snorkelling|Red Sea|Nature",
                        AverageRating = 4.7m,
                        ReviewCount = 3180,
                        IsFeatured  = true,
                        Latitude    = 28.5710,
                        Longitude   = 34.5394,
                        LocationUrl = "https://maps.google.com/?q=Dahab+Blue+Hole"
                    },
                    new()
                    {
                        Name        = "Valley of the Kings",
                        Description = "Carved into the limestone hills on the west bank of Luxor, the Valley of the Kings contains " +
                                      "63 royal tombs, including the legendary tomb of Tutankhamun discovered by Howard Carter in 1922. " +
                                      "Vivid painted walls depict scenes from the afterlife in extraordinary detail.",
                        City        = "Luxor",
                        Country     = "Egypt",
                        Category    = PlaceCategory.Historic,
                        EntryFee    = 25m,
                        OpeningTime = new TimeSpan(6, 0, 0),
                        ClosingTime = new TimeSpan(17, 0, 0),
                        ImageUrl    = "https://images.unsplash.com/photo-1568322445389-f64ac2515022?w=800",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1568322445389-f64ac2515022?w=400",
                        Tags        = "Pharaoh|Tombs|UNESCO|Ancient",
                        AverageRating = 4.8m,
                        ReviewCount = 4650,
                        IsFeatured  = false,
                        Latitude    = 25.7402,
                        Longitude   = 32.6014,
                        LocationUrl = "https://maps.google.com/?q=Valley+of+the+Kings+Luxor"
                    },
                    new()
                    {
                        Name        = "Khan el-Khalili Bazaar",
                        Description = "Cairo's most iconic souk, Khan el-Khalili has been the heart of Islamic Cairo since the 14th century. " +
                                      "Its narrow labyrinthine lanes are packed with gold, spices, perfumes, lanterns and textiles. " +
                                      "The adjacent Al-Azhar Mosque and the bustling coffeehouses make this a cultural feast.",
                        City        = "Cairo",
                        Country     = "Egypt",
                        Category    = PlaceCategory.Cultural,
                        EntryFee    = 0m,
                        OpeningTime = new TimeSpan(9, 0, 0),
                        ClosingTime = new TimeSpan(23, 0, 0),
                        ImageUrl    = "https://images.unsplash.com/photo-1553913861-c0fddf2619ee?w=800",
                        ThumbnailUrl = "https://images.unsplash.com/photo-1553913861-c0fddf2619ee?w=400",
                        Tags        = "Bazaar|Shopping|Culture|Islamic Cairo",
                        AverageRating = 4.6m,
                        ReviewCount = 6720,
                        IsFeatured  = false,
                        Latitude    = 30.0478,
                        Longitude   = 31.2625,
                        LocationUrl = "https://maps.google.com/?q=Khan+el-Khalili+Cairo"
                    }
                };

                context.TouristPlaces.AddRange(places);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} TouristPlaces", places.Count);

                // ── 3. Seed Activities (children of TouristPlaces) ────────────
                var pyramidsId = places[0].Id;
                var karnakId   = places[1].Id;
                var dahabId    = places[2].Id;
                var valleyId   = places[3].Id;
                var khanId     = places[4].Id;

                var activities = new List<Activity>
                {
                    new() { TouristPlaceId = pyramidsId, Name = "Camel Ride at Sunset",     Description = "Ride a camel around the plateau with the pyramids silhouetted against the golden sky.",     Price = 25m,  DurationInMinutes = 60,  Badge = "LOCAL FAVORITE" },
                    new() { TouristPlaceId = pyramidsId, Name = "Inside the Great Pyramid",  Description = "Descend into the narrow passage to the King's Chamber — a truly unforgettable experience.", Price = 30m,  DurationInMinutes = 45 },
                    new() { TouristPlaceId = karnakId,   Name = "Sound & Light Show",        Description = "An evening spectacular projecting the full story of Karnak onto its illuminated columns.",   Price = 20m,  DurationInMinutes = 75,  Badge = "MUST SEE" },
                    new() { TouristPlaceId = karnakId,   Name = "Guided Hypostyle Walk",     Description = "Expert Egyptologist leads you through the 134-column forest with full hieroglyphic translation.", Price = 15m, DurationInMinutes = 90 },
                    new() { TouristPlaceId = dahabId,    Name = "Intro Scuba Dive",           Description = "Try your first dive in the crystal-clear Red Sea — no certification required.",               Price = 55m,  DurationInMinutes = 120, Badge = "POPULAR" },
                    new() { TouristPlaceId = dahabId,    Name = "Coral Reef Snorkelling",    Description = "Float above the vibrant reef teeming with parrotfish, lionfish, and sea turtles.",            Price = 15m,  DurationInMinutes = 90 },
                    new() { TouristPlaceId = valleyId,   Name = "Tutankhamun's Tomb Tour",   Description = "Priority access and a specialist guide to the boy king's legendary golden burial chamber.",   Price = 35m,  DurationInMinutes = 60,  Badge = "LOCAL FAVORITE" },
                    new() { TouristPlaceId = khanId,     Name = "Street Food Tasting Walk",  Description = "Sample koshari, ful medames, feteer, and freshly ground coffee with a local foodie guide.", Price = 20m,  DurationInMinutes = 150, Badge = "POPULAR" },
                };

                context.Activities.AddRange(activities);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} Activities", activities.Count);
            }
            else
            {
                logger.LogInformation("TouristPlaces already seeded, skipping");
            }

            // ── 4. Seed Tours ─────────────────────────────────────────────────
            if (!await context.Tours.AnyAsync())
            {
                var tours = new List<Tour>
                {
                    new()
                    {
                        GuideProfileId  = guide.Id,
                        Title           = "Full-Day Pyramids & Sphinx Experience",
                        Description     = "The definitive Cairo day trip. Stand before the last surviving ancient wonder, learn the secrets " +
                                          "of pyramid construction from your Egyptologist guide, enter the Great Pyramid itself, and end " +
                                          "with a panoramic camel ride as the sun sets behind Khafre.",
                        ExperienceType  = "SMALL GROUP EXPERIENCE",
                        PricePerPerson  = 85m,
                        DurationInMinutes = 480,
                        MaxGroupSize    = 10,
                        Languages       = "English|Arabic",
                        CoverImageUrl   = "https://images.unsplash.com/photo-1539768942893-daf53e448371?w=1200",
                        AverageRating   = 4.9m,
                        ReviewCount     = 245,
                        Status          = TourStatus.Active
                    },
                    new()
                    {
                        GuideProfileId  = guide.Id,
                        Title           = "Luxor Temples by Night",
                        Description     = "As dusk falls over the Nile, journey to Karnak for the legendary Sound & Light Show, then walk " +
                                          "the Avenue of Sphinxes to Luxor Temple, dramatically floodlit against the night sky. A magical " +
                                          "evening that makes ancient Egypt feel alive.",
                        ExperienceType  = "EVENING EXPERIENCE",
                        PricePerPerson  = 65m,
                        DurationInMinutes = 240,
                        MaxGroupSize    = 12,
                        Languages       = "English|Arabic|French",
                        CoverImageUrl   = "https://images.unsplash.com/photo-1562679299-8f22b84e0e47?w=1200",
                        AverageRating   = 4.8m,
                        ReviewCount     = 189,
                        Status          = TourStatus.Active
                    },
                    new()
                    {
                        GuideProfileId  = guide.Id,
                        Title           = "Dahab Dive & Snorkel Day",
                        Description     = "Spend a full day exploring the Red Sea's finest underwater scenery. Beginners snorkel the " +
                                          "coral gardens while certified divers explore the famous Blue Hole arch. Freshly grilled fish " +
                                          "lunch on the beach is included.",
                        ExperienceType  = "OUTDOOR ADVENTURE",
                        PricePerPerson  = 95m,
                        DurationInMinutes = 360,
                        MaxGroupSize    = 8,
                        Languages       = "English|Arabic",
                        CoverImageUrl   = "https://images.unsplash.com/photo-1682687220063-4742bd7fd538?w=1200",
                        AverageRating   = 4.9m,
                        ReviewCount     = 134,
                        Status          = TourStatus.Active
                    },
                    new()
                    {
                        GuideProfileId  = guide.Id,
                        Title           = "Valley of the Kings Private Tour",
                        Description     = "Skip the crowds with this intimate private tour of the royal necropolis. Your guide unlocks " +
                                          "tombs rarely opened to tour groups, including Seti I's tomb with its extraordinary astronomical " +
                                          "ceiling. Includes Hatshepsut's temple and the Colossi of Memnon.",
                        ExperienceType  = "PRIVATE EXPERIENCE",
                        PricePerPerson  = 120m,
                        DurationInMinutes = 300,
                        MaxGroupSize    = 6,
                        Languages       = "English|Arabic|French|German",
                        CoverImageUrl   = "https://images.unsplash.com/photo-1568322445389-f64ac2515022?w=1200",
                        AverageRating   = 5.0m,
                        ReviewCount     = 97,
                        Status          = TourStatus.Active
                    },
                    new()
                    {
                        GuideProfileId  = guide.Id,
                        Title           = "Islamic Cairo Food & Culture Walk",
                        Description     = "Lose yourself in the living medieval city. Explore Al-Azhar Mosque, haggle in Khan el-Khalili " +
                                          "with your guide's help, taste street foods through the spice market, and finish with mint tea " +
                                          "in a 500-year-old coffeehouse.",
                        ExperienceType  = "FOOD & CULTURE WALK",
                        PricePerPerson  = 45m,
                        DurationInMinutes = 210,
                        MaxGroupSize    = 10,
                        Languages       = "English|Arabic",
                        CoverImageUrl   = "https://images.unsplash.com/photo-1553913861-c0fddf2619ee?w=1200",
                        AverageRating   = 4.7m,
                        ReviewCount     = 208,
                        Status          = TourStatus.Active
                    }
                };

                context.Tours.AddRange(tours);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} Tours", tours.Count);

                // ── 5. Seed TourItinerarySteps ────────────────────────────────
                var steps = new List<TourItineraryStep>
                {
                    // Tour 1 — Pyramids
                    new() { TourId = tours[0].Id, StepNumber = 1, Title = "Hotel pick-up",        Description = "Air-conditioned minivan collects you from your Cairo hotel at 8:00 AM." },
                    new() { TourId = tours[0].Id, StepNumber = 2, Title = "Enter the Great Pyramid", Description = "Descend the Grand Gallery to the King's Chamber — limited daily slots." },
                    new() { TourId = tours[0].Id, StepNumber = 3, Title = "Meet the Sphinx",       Description = "Stand face-to-face with the 73-metre limestone sentinel." },
                    new() { TourId = tours[0].Id, StepNumber = 4, Title = "Sunset camel ride",     Description = "Ride around the plateau as the Sahara glows orange." },

                    // Tour 2 — Luxor Nights
                    new() { TourId = tours[1].Id, StepNumber = 1, Title = "Karnak Sound & Light",  Description = "Dramatic narrated light show across the Hypostyle Hall." },
                    new() { TourId = tours[1].Id, StepNumber = 2, Title = "Avenue of Sphinxes",    Description = "Walk the 2.7 km ceremonial road linking Karnak and Luxor temples." },
                    new() { TourId = tours[1].Id, StepNumber = 3, Title = "Luxor Temple floodlit", Description = "Marvel at the colossal statues of Ramesses II after dark." },

                    // Tour 3 — Dahab
                    new() { TourId = tours[2].Id, StepNumber = 1, Title = "Morning snorkel",       Description = "Explore the shallow coral garden at the Blue Hole lagoon." },
                    new() { TourId = tours[2].Id, StepNumber = 2, Title = "Dive the arch",         Description = "Certified divers swim through the famous 26-metre arch." },
                    new() { TourId = tours[2].Id, StepNumber = 3, Title = "Beach fish lunch",      Description = "Grilled catch of the day at a beachside Bedouin restaurant." },

                    // Tour 4 — Valley of Kings
                    new() { TourId = tours[3].Id, StepNumber = 1, Title = "Tutankhamun's tomb",    Description = "Priority entry to the boy-king's lavishly decorated burial chamber." },
                    new() { TourId = tours[3].Id, StepNumber = 2, Title = "Seti I's tomb",         Description = "The finest painted tomb in Egypt — rarely opened to groups." },
                    new() { TourId = tours[3].Id, StepNumber = 3, Title = "Hatshepsut's temple",   Description = "The terraced mortuary temple of Egypt's most powerful female pharaoh." },

                    // Tour 5 — Islamic Cairo
                    new() { TourId = tours[4].Id, StepNumber = 1, Title = "Al-Azhar Mosque",       Description = "One of the world's oldest universities and a living centre of Islamic learning." },
                    new() { TourId = tours[4].Id, StepNumber = 2, Title = "Spice market tasting",  Description = "Sample cumin, saffron, hibiscus, and black seed with the market vendors." },
                    new() { TourId = tours[4].Id, StepNumber = 3, Title = "Mint tea & shisha",     Description = "Relax at Fishawi's coffeehouse, open 24 hours since 1773." },
                };

                context.TourItinerarySteps.AddRange(steps);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} TourItinerarySteps", steps.Count);
            }
            else
            {
                logger.LogInformation("Tours already seeded, skipping");
            }

            // ── 6. Seed Accommodations ────────────────────────────────────────
            if (!await context.Accommodations.AnyAsync())
            {
                var accommodations = new List<Accommodation>
                {
                    new()
                    {
                        Name          = "Four Seasons Hotel Cairo at Nile Plaza",
                        Description   = "Towering above the Nile on Garden City's waterfront, this legendary five-star hotel " +
                                        "offers the finest views of the river alongside unmatched dining and a rooftop pool. " +
                                        "The Egyptian Museum is a 10-minute drive away.",
                        City          = "Cairo",
                        Country       = "Egypt",
                        Type          = AccommodationType.Hotel,
                        StarRating    = 5,
                        PricePerNight = 320m,
                        Currency      = "USD",
                        AverageRating = 4.9m,
                        ReviewCount   = 2140,
                        ImageUrl      = "https://images.unsplash.com/photo-1582719508461-905c673771fd?w=800",
                        ThumbnailUrl  = "https://images.unsplash.com/photo-1582719508461-905c673771fd?w=400",
                        PhoneNumber   = "+20 2 2791 7000",
                        Email         = "reservations.caironp@fourseasons.com",
                        WebsiteUrl    = "https://www.fourseasons.com/caironp/",
                        CheckInTime   = new TimeSpan(15, 0, 0),
                        CheckOutTime  = new TimeSpan(12, 0, 0),
                        IsActive      = true,
                        Latitude      = 30.0429,
                        Longitude     = 31.2295
                    },
                    new()
                    {
                        Name          = "Sofitel Winter Palace Luxor",
                        Description   = "A palatial Victorian hotel built in 1886 for visiting dignitaries, the Winter Palace " +
                                        "sits in four acres of tropical gardens on the east bank of the Nile. Agatha Christie " +
                                        "wrote part of 'Death on the Nile' in Room 100.",
                        City          = "Luxor",
                        Country       = "Egypt",
                        Type          = AccommodationType.Hotel,
                        StarRating    = 5,
                        PricePerNight = 180m,
                        Currency      = "USD",
                        AverageRating = 4.7m,
                        ReviewCount   = 1580,
                        ImageUrl      = "https://images.unsplash.com/photo-1455587734955-081b22074882?w=800",
                        ThumbnailUrl  = "https://images.unsplash.com/photo-1455587734955-081b22074882?w=400",
                        PhoneNumber   = "+20 95 238 0422",
                        Email         = "h1661@sofitel.com",
                        WebsiteUrl    = "https://www.sofitel-luxor-winterpalace.com/",
                        CheckInTime   = new TimeSpan(14, 0, 0),
                        CheckOutTime  = new TimeSpan(12, 0, 0),
                        IsActive      = true,
                        Latitude      = 25.6989,
                        Longitude     = 32.6421
                    },
                    new()
                    {
                        Name          = "Rixos Premium Seagate Sharm El-Sheikh",
                        Description   = "An ultra-all-inclusive beachfront resort on the crystal waters of Nabq Bay. " +
                                        "Six restaurants, four pools, a private beach, and direct reef access make this " +
                                        "the benchmark luxury resort on the Red Sea Riviera.",
                        City          = "Sharm El-Sheikh",
                        Country       = "Egypt",
                        Type          = AccommodationType.Resort,
                        StarRating    = 5,
                        PricePerNight = 260m,
                        Currency      = "USD",
                        AverageRating = 4.8m,
                        ReviewCount   = 3420,
                        ImageUrl      = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800",
                        ThumbnailUrl  = "https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=400",
                        PhoneNumber   = "+20 69 360 1234",
                        Email         = "info.seagate@rixos.com",
                        WebsiteUrl    = "https://www.rixos.com/en/hotels-resorts/rixos-premium-seagate/",
                        CheckInTime   = new TimeSpan(15, 0, 0),
                        CheckOutTime  = new TimeSpan(12, 0, 0),
                        IsActive      = true,
                        Latitude      = 27.9974,
                        Longitude     = 34.3977
                    },
                    new()
                    {
                        Name          = "Basata Eco Lodge Nuweiba",
                        Description   = "Egypt's original eco-lodge, Basata ('Simplicity') has been welcoming free spirits " +
                                        "to the Gulf of Aqaba since 1986. Thatched beach huts open directly onto the sea, " +
                                        "stars blaze overhead, and the house rules keep it plastic-free and peaceful.",
                        City          = "Nuweiba",
                        Country       = "Egypt",
                        Type          = AccommodationType.Villa,
                        StarRating    = 3,
                        PricePerNight = 65m,
                        Currency      = "USD",
                        AverageRating = 4.6m,
                        ReviewCount   = 870,
                        ImageUrl      = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800",
                        ThumbnailUrl  = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=400",
                        PhoneNumber   = "+20 69 350 0481",
                        Email         = "info@basata.com",
                        WebsiteUrl    = "https://www.basata.com/",
                        CheckInTime   = new TimeSpan(14, 0, 0),
                        CheckOutTime  = new TimeSpan(11, 0, 0),
                        IsActive      = true,
                        Latitude      = 28.9739,
                        Longitude     = 34.6613
                    },
                    new()
                    {
                        Name          = "Steigenberger Pure Lifestyle Hurghada",
                        Description   = "An adults-only boutique resort on the Red Sea with a minimalist aesthetic, " +
                                        "infinity pool, and house reef for snorkelling. Rooftop bar, spa, and curated " +
                                        "diving packages through the on-site dive centre.",
                        City          = "Hurghada",
                        Country       = "Egypt",
                        Type          = AccommodationType.Boutique,
                        StarRating    = 5,
                        PricePerNight = 145m,
                        Currency      = "USD",
                        AverageRating = 4.8m,
                        ReviewCount   = 1240,
                        ImageUrl      = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800",
                        ThumbnailUrl  = "https://images.unsplash.com/photo-1566073771259-6a8506099945?w=400",
                        PhoneNumber   = "+20 65 344 1234",
                        Email         = "reservations.hurghada@steigenberger.com",
                        WebsiteUrl    = "https://www.steigenberger.com/en/hotels/all-hotels/egypt/hurghada/steigenberger-pure-lifestyle",
                        CheckInTime   = new TimeSpan(15, 0, 0),
                        CheckOutTime  = new TimeSpan(12, 0, 0),
                        IsActive      = true,
                        Latitude      = 27.2574,
                        Longitude     = 33.8116
                    }
                };

                context.Accommodations.AddRange(accommodations);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} Accommodations", accommodations.Count);

                // ── 7. Seed Amenities ─────────────────────────────────────────
                var amenities = new List<AccommodationAmenity>
                {
                    // Four Seasons Cairo
                    new() { AccommodationId = accommodations[0].Id, Name = "Free WiFi",       IconCode = "wifi" },
                    new() { AccommodationId = accommodations[0].Id, Name = "Rooftop Pool",    IconCode = "pool" },
                    new() { AccommodationId = accommodations[0].Id, Name = "Spa & Wellness",  IconCode = "spa" },
                    new() { AccommodationId = accommodations[0].Id, Name = "Nile View Rooms", IconCode = "view" },
                    new() { AccommodationId = accommodations[0].Id, Name = "Concierge",       IconCode = "concierge" },
                    // Sofitel Winter Palace Luxor
                    new() { AccommodationId = accommodations[1].Id, Name = "Free WiFi",       IconCode = "wifi" },
                    new() { AccommodationId = accommodations[1].Id, Name = "Outdoor Pool",    IconCode = "pool" },
                    new() { AccommodationId = accommodations[1].Id, Name = "Tropical Gardens",IconCode = "garden" },
                    new() { AccommodationId = accommodations[1].Id, Name = "Historic Bar",    IconCode = "bar" },
                    // Rixos Sharm
                    new() { AccommodationId = accommodations[2].Id, Name = "Free WiFi",       IconCode = "wifi" },
                    new() { AccommodationId = accommodations[2].Id, Name = "Private Beach",   IconCode = "beach" },
                    new() { AccommodationId = accommodations[2].Id, Name = "4 Pools",         IconCode = "pool" },
                    new() { AccommodationId = accommodations[2].Id, Name = "All Inclusive",   IconCode = "food" },
                    new() { AccommodationId = accommodations[2].Id, Name = "Kids Club",       IconCode = "kids" },
                    // Basata Nuweiba
                    new() { AccommodationId = accommodations[3].Id, Name = "Beach Access",    IconCode = "beach" },
                    new() { AccommodationId = accommodations[3].Id, Name = "Eco-Friendly",    IconCode = "leaf" },
                    new() { AccommodationId = accommodations[3].Id, Name = "Communal Kitchen",IconCode = "kitchen" },
                    // Steigenberger Hurghada
                    new() { AccommodationId = accommodations[4].Id, Name = "Free WiFi",       IconCode = "wifi" },
                    new() { AccommodationId = accommodations[4].Id, Name = "Infinity Pool",   IconCode = "pool" },
                    new() { AccommodationId = accommodations[4].Id, Name = "House Reef",      IconCode = "diving" },
                    new() { AccommodationId = accommodations[4].Id, Name = "Adults Only",     IconCode = "adult" },
                    new() { AccommodationId = accommodations[4].Id, Name = "Rooftop Bar",     IconCode = "bar" },
                };

                context.AccommodationAmenities.AddRange(amenities);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} AccommodationAmenities", amenities.Count);

                // ── 8. Seed Rooms ─────────────────────────────────────────────
                var rooms = new List<Room>
                {
                    // Four Seasons Cairo — 3 room types
                    new() { AccommodationId = accommodations[0].Id, RoomNumber = "101", RoomType = RoomType.Double,  PricePerNight = 320m,  MaxOccupancy = 2, BedCount = 1, Description = "City-view double with marble bathroom.",    ImageUrl = "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=600" },
                    new() { AccommodationId = accommodations[0].Id, RoomNumber = "501", RoomType = RoomType.Suite,   PricePerNight = 750m,  MaxOccupancy = 3, BedCount = 2, Description = "Nile-view junior suite with private balcony.", ImageUrl = "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=600" },
                    new() { AccommodationId = accommodations[0].Id, RoomNumber = "201", RoomType = RoomType.Deluxe,  PricePerNight = 450m,  MaxOccupancy = 2, BedCount = 1, Description = "Deluxe room overlooking Garden City.",         ImageUrl = "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=600" },
                    // Sofitel Luxor — 2 room types
                    new() { AccommodationId = accommodations[1].Id, RoomNumber = "110", RoomType = RoomType.Double,  PricePerNight = 180m,  MaxOccupancy = 2, BedCount = 1, Description = "Classic garden room in the historic wing.",   ImageUrl = "https://images.unsplash.com/photo-1566665797739-1674de7a421a?w=600" },
                    new() { AccommodationId = accommodations[1].Id, RoomNumber = "100", RoomType = RoomType.Suite,   PricePerNight = 380m,  MaxOccupancy = 3, BedCount = 2, Description = "The legendary Room 100 where Agatha Christie stayed.", ImageUrl = "https://images.unsplash.com/photo-1566665797739-1674de7a421a?w=600" },
                    // Rixos Sharm — 3 room types
                    new() { AccommodationId = accommodations[2].Id, RoomNumber = "201", RoomType = RoomType.Double,  PricePerNight = 260m,  MaxOccupancy = 2, BedCount = 1, Description = "Sea-view standard room with direct beach access.", ImageUrl = "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=600" },
                    new() { AccommodationId = accommodations[2].Id, RoomNumber = "301", RoomType = RoomType.Family,  PricePerNight = 380m,  MaxOccupancy = 4, BedCount = 2, Description = "Family room with two bedrooms and pool view.",  ImageUrl = "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=600" },
                    new() { AccommodationId = accommodations[2].Id, RoomNumber = "401", RoomType = RoomType.Suite,   PricePerNight = 620m,  MaxOccupancy = 4, BedCount = 2, Description = "Penthouse suite with 180° Red Sea panorama.",   ImageUrl = "https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=600" },
                    // Basata Nuweiba — 2 room types
                    new() { AccommodationId = accommodations[3].Id, RoomNumber = "H01", RoomType = RoomType.Single,  PricePerNight = 65m,   MaxOccupancy = 2, BedCount = 1, Description = "Beachfront hut with bamboo walls and sea breeze.", ImageUrl = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=600" },
                    new() { AccommodationId = accommodations[3].Id, RoomNumber = "H05", RoomType = RoomType.Twin,    PricePerNight = 90m,   MaxOccupancy = 3, BedCount = 2, Description = "Larger beach chalet with two beds and terrace.", ImageUrl = "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=600" },
                    // Steigenberger Hurghada — 2 room types
                    new() { AccommodationId = accommodations[4].Id, RoomNumber = "201", RoomType = RoomType.Double,  PricePerNight = 145m,  MaxOccupancy = 2, BedCount = 1, Description = "Sea-facing room with infinity pool access.",      ImageUrl = "https://images.unsplash.com/photo-1616594039964-ae9021a400a0?w=600" },
                    new() { AccommodationId = accommodations[4].Id, RoomNumber = "301", RoomType = RoomType.Suite,   PricePerNight = 280m,  MaxOccupancy = 2, BedCount = 1, Description = "Rooftop suite with private plunge pool.",         ImageUrl = "https://images.unsplash.com/photo-1616594039964-ae9021a400a0?w=600" },
                };

                context.Rooms.AddRange(rooms);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} Rooms", rooms.Count);
            }
            else
            {
                logger.LogInformation("Accommodations already seeded, skipping");
            }

            logger.LogInformation("DatabaseSeeder completed successfully");
        }
    }
}
