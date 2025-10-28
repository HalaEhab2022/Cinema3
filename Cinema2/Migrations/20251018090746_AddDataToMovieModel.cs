using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema2.Migrations
{
    /// <inheritdoc />
    public partial class AddDataToMovieModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Movies (Name, Description, Status, ticketPrice, MainImg, DateTime, CategoryId, CiinemaId) values ('A Beautiful Mind', 'Etiam faucibus cursus urna.', 1, 1659, 'Home Alone.jpg', '1997-03-10', 1, 1);insert into Movies (Name, Description, Status, ticketPrice, MainImg, DateTime, CategoryId, CiinemaId) values ('The Green Mile', 'Cras pellentesque volutpat dui. Maecenas tristique, est et tempus semper, est quam pharetra magna, ac consequat metus sapien ut nunc. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Mauris viverra diam vitae quam.', 1, 1216, 'Titanic.jpeg', '2024-03-01', 1, 1);insert into Movies (Name, Description, Status, ticketPrice, MainImg, DateTime, CategoryId, CiinemaId) values ('Titanic', 'Quisque ut erat. Curabitur gravida nisi at nibh.', 1, 3061, 'Me Before You.jpg', '1989-05-03', 1, 1);insert into Movies (Name, Description, Status, ticketPrice, MainImg, DateTime, CategoryId, CiinemaId) values ('Me Before You', 'Duis bibendum, felis sed interdum venenatis, turpis enim blandit mi, in porttitor pede justo eu massa. Donec dapibus. Duis at velit eu est congue elementum. In hac habitasse platea dictumst. Morbi vestibulum, velit id pretium iaculis, diam erat fermentum justo, nec condimentum neque sapien placerat ante. Nulla justo. Aliquam quis turpis eget elit sodales scelerisque.', 1, 1808, 'Me Before You.jpg', '1958-11-12', 1, 1);insert into Movies (Name, Description, Status, ticketPrice, MainImg, DateTime, CategoryId, CiinemaId) values ('Home Alone', 'Proin eu mi. Nulla ac enim.', 1, 3680, 'The Matrix.jpg', '1953-07-15', 1, 1);insert into Movies (Name, Description, Status, ticketPrice, MainImg, DateTime, CategoryId, CiinemaId) values ('Crazy Rich Asians', 'Integer aliquet, massa id lobortis convallis, tortor risus dapibus augue, vel accumsan tellus nisi eu orci. Mauris lacinia sapien quis libero. Nullam sit amet turpis elementum ligula vehicula consequat. Morbi a ipsum. Integer a nibh. In quis justo. Maecenas rhoncus aliquam lacus. Morbi quis tortor id nulla ultrices aliquet.', 1, 4108, 'The Green Mile.jpeg', '1999-02-25', 1, 1);insert into Movies (Name, Description, Status, ticketPrice, MainImg, DateTime, CategoryId, CiinemaId) values ('Saving Private Ryan', 'Pellentesque viverra pede ac diam. Cras pellentesque volutpat dui. Maecenas tristique, est et tempus semper, est quam pharetra magna, ac consequat metus sapien ut nunc. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Mauris viverra diam vitae quam.', 1, 350, 'Home Alone.jpg', '2023-03-01', 1, 1);insert into Movies (Name, Description, Status, ticketPrice, MainImg, DateTime, CategoryId, CiinemaId) values ('The Matrix', 'Vivamus tortor. Duis mattis egestas metus. Aenean fermentum. Donec ut mauris eget massa tempor convallis. Nulla neque libero, convallis eget, eleifend luctus, ultricies eu, nibh. Quisque id justo sit amet sapien dignissim vestibulum. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Nulla dapibus dolor vel est. Donec odio justo, sollicitudin ut, suscipit a, feugiat et, eros. Vestibulum ac est lacinia nisi venenatis tristique.', 1, 3456, 'Crazy Rich Asians.jpg', '1998-07-15', 1, 1);");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE Movies");
        }
    }
}
