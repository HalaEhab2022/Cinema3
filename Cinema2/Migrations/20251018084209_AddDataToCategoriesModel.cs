using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema2.Migrations
{
    /// <inheritdoc />
    public partial class AddDataToCategoriesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Categories (Name, Description, Status) values ('Action', 'Etiam justo. Etiam pretium iaculis justo.', 1);insert into Categories (Name, Description, Status) values ('Drama', 'Suspendisse ornare consequat lectus. In est risus, auctor sed, tristique in, tempus sit amet, sem. Fusce consequat.', 1);insert into Categories (Name, Description, Status) values ('Comedy', 'Aenean sit amet justo. Morbi ut odio. Cras mi pede, malesuada in, imperdiet et, commodo vulputate, justo.', 1);insert into Categories (Name, Description, Status) values ('Romance', 'Nulla justo. Aliquam quis turpis eget elit sodales scelerisque.', 0);insert into Categories (Name, Description, Status) values ('Horror', 'Aliquam augue quam, sollicitudin vitae, consectetuer eget, rutrum at, lorem.', 1);insert into Categories (Name, Description, Status) values ('Thriller', 'Aliquam erat volutpat. In congue. Etiam justo. Etiam pretium iaculis justo. In hac habitasse platea dictumst. Etiam faucibus cursus urna.', 1);insert into Categories (Name, Description, Status) values ('Adventure', 'Nam congue, risus semper porta volutpat, quam pede lobortis ligula, sit amet eleifend pede libero quis orci. Nullam molestie nibh in lectus.', 1);insert into Categories (Name, Description, Status) values ('Science Fiction', 'Nam congue, risus semper porta volutpat, quam pede lobortis ligula, sit amet eleifend pede libero quis orci. Nullam molestie nibh in lectus. Pellentesque at nulla. Suspendisse potenti. Cras in purus eu magna vulputate luctus. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vivamus vestibulum sagittis sapien. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.', 1);insert into Categories (Name, Description, Status) values ('Fantasy', 'Nulla mollis molestie lorem. Quisque ut erat. Curabitur gravida nisi at nibh. In hac habitasse platea dictumst.', 1);insert into Categories (Name, Description, Status) values ('Crime', 'Aliquam sit amet diam in magna bibendum imperdiet. Nullam orci pede, venenatis non, sodales sed, tincidunt eu, felis. Fusce posuere felis sed lacus. Morbi sem mauris, laoreet ut, rhoncus aliquet, pulvinar sed, nisl.', 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE Categories");
        }
    }
}
