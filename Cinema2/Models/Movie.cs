namespace Cinema2.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; }
        public decimal TicketPrice { get; set; }
        public string MainImg { get; set; } = string.Empty;
        public long Traffic { get; set; }
        public DateTime DateTime { get; set; }
        public int CategoryId { get; set; }
        public int CiinemaId { get; set; }
        public Category category { get; set; } = null!;
        public Ciinema ciinema { get; set; } = null!;
        
        public List<ActorMovie> ActorMovies { get; set; }
        public List<MovieSubImage> subImages { get; set; }
    }
}
