using Microsoft.EntityFrameworkCore;

namespace Cinema2.Models
{
    [PrimaryKey(nameof(ActorId), nameof(MovieId))]
    public class ActorMovie
    {
        public int ActorId { get; set; }
        public Actor Actor { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
