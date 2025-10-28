namespace Cinema2.ViewModels
{
    public record FilterMovieVM(string name, decimal? minPrice, decimal? maxPrice, int? categoryId, int? ciinemaId);
    
    
}
