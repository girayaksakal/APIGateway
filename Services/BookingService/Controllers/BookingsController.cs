using Microsoft.AspNetCore.Mvc;
using SharedService;

namespace BookingService.Controllers {
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BookingsController : ControllerBase {
        private readonly SharedDbContext _dbContext;

        public BookingsController(SharedDbContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpGet("QueryListings")]
        public IActionResult QueryListings(DateTime from, DateTime to, int noOfPeople) {
            var listings = _dbContext.Listings
                .Where(l => l.NoOfPeople >= noOfPeople)
                .Select(l => new {
                    l.Id,
                    l.Title,
                    l.Description,
                    l.Price,
                    l.Country,
                    l.City,
                })
                .ToList();
            
            return Ok(listings);
        }

        [HttpPost("BookAStay")]
        public IActionResult BookAStay([FromBody] BookingRequest request) {
            var listing = _dbContext.Listings.FirstOrDefault(l => l.Id == request.ListingId);
            if (listing == null) {
                return NotFound("Listing with ID {request.ListingId} not found.");
            }

            var booking = new Booking {
                ListingId = request.ListingId,
                GuestNames = request.GuestNames,
                From = request.From,
                To = request.To
            };

            _dbContext.Bookings.Add(booking);
            _dbContext.SaveChanges();

            return Ok(new { BookingId = booking.Id ,Status = "Booking confirmed." });
        }
    }
    public class BookingRequest {
        public int ListingId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public required List<string> GuestNames { get; set; }
    }
}