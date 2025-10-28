namespace Cinema2.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Img { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public List<ActorMovie> ActorMovies { get; set; }
    }
}
