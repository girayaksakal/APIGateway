using Microsoft.AspNetCore.Mvc;
using SharedService;

namespace ListingService.Controllers {
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ListingsController : ControllerBase {
        private readonly SharedDbContext _dbContext;

        public ListingsController(SharedDbContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpPost("InsertListing")]
        public IActionResult InsertListing([FromBody] ListingRequest request) {
            var listing = new Listing {
                Title = request.Title,
                Description = request.Description,
                NoOfPeople = request.NoOfPeople,
                Country = request.Country,
                City = request.City,
                Price = request.Price
            };

            _dbContext.Listings.Add(listing);
            _dbContext.SaveChanges();

            return Ok(new { ListingId = listing.Id, Status = "Listing added successfully." });
        }

        public class ListingRequest {
            public required string Title { get; set; }
            public required string Description { get; set; }
            public int NoOfPeople { get; set; }
            public required string Country { get; set; }
            public required string City { get; set; }
            public decimal Price { get; set; }
        }
    }
}