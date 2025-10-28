using Microsoft.EntityFrameworkCore;
namespace Cinema2.Models
{
    [PrimaryKey(nameof(movieId), nameof(Img))]
    public class MovieSubImage
    {
        public int movieId { get; set; }
        public string Img { get; set; } = string.Empty;
        public Movie movie { get; set; } = null!;
    }
}
