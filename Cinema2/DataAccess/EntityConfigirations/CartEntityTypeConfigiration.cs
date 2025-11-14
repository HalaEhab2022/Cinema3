using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema2.DataAccess.EntityConfigirations
{
    public class CartEntityTypeConfigiration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(e => new { e.MovieId, e.ApplicationUserId });
        }
    }
}
