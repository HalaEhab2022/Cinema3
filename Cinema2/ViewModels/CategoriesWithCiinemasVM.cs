namespace Cinema2.ViewModels

{
    public class CategoriesWithCiinemasVM
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Ciinema> Ciinemas { get; set; }
        public IEnumerable<Actor> Actors { get; set; }
        public IEnumerable<ActorMovie> ActorMovies { get; set; }
        public Movie? Movie { get; set; }
        public List<int> SelectedActorIds { get; set; }
    }
}
