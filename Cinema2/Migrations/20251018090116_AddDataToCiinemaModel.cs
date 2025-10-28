using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema2.Migrations
{
    /// <inheritdoc />
    public partial class AddDataToCiinemaModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Ciinemas (Name, Description, Status, Img) values ('Century', 'Morbi quis tortor id nulla ultrices aliquet. Maecenas leo odio, condimentum id, luctus nec, molestie sed, justo.', 1, 'century-cinema.jpg');insert into Ciinemas (Name, Description, Status, Img) values ('Galaxy', 'Phasellus in felis. Donec semper sapien a libero. Nam dui. Proin leo odio, porttitor id, consequat in, consequat ut, nulla. Sed accumsan felis. Ut at dolor quis odio consequat varius. Integer ac leo. Pellentesque ultrices mattis odio. Donec vitae nisi.', 1, 'Galaxy.jpg');insert into Ciinemas (Name, Description, Status, Img) values ('IMAX-Americana', 'Phasellus sit amet erat. Nulla tempus.', 1, 'imax.jpg');insert into Ciinemas (Name, Description, Status, Img) values ('Mayami', 'Donec ut dolor. Morbi vel lectus in quam fringilla rhoncus. Mauris enim leo, rhoncus sed, vestibulum sit amet, cursus id, turpis.', 0, 'mayami.png');insert into Ciinemas (Name, Description, Status, Img) values ('Metro', 'In tempor, turpis nec euismod scelerisque, quam turpis adipiscing lorem, vitae mattis nibh ligula nec sem. Duis aliquam convallis nunc.', 1, 'metro.jpg');insert into Ciinemas (Name, Description, Status, Img) values ('Palace', 'Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Mauris viverra diam vitae quam. Suspendisse potenti. Nullam porttitor lacus at turpis. Donec posuere metus vitae ipsum. Aliquam non mauris. Morbi non lectus. Aliquam sit amet diam in magna bibendum imperdiet. Nullam orci pede, venenatis non, sodales sed, tincidunt eu, felis.', 1, 'palace.jpg');insert into Ciinemas (Name, Description, Status, Img) values ('Radio', 'Donec semper sapien a libero. Nam dui. Proin leo odio, porttitor id, consequat in, consequat ut, nulla. Sed accumsan felis. Ut at dolor quis odio consequat varius. Integer ac leo. Pellentesque ultrices mattis odio. Donec vitae nisi. Nam ultrices, libero non mattis pulvinar, nulla pede ullamcorper augue, a suscipit nulla elit ac nulla. Sed vel enim sit amet nunc viverra dapibus.', 1, 'radio.jpeg');insert into Ciinemas (Name, Description, Status, Img) values ('Rio', 'Fusce lacus purus, aliquet at, feugiat non, pretium quis, lectus. Suspendisse potenti. In eleifend quam a odio. In hac habitasse platea dictumst. Maecenas ut massa quis augue luctus tincidunt. Nulla mollis molestie lorem. Quisque ut erat. Curabitur gravida nisi at nibh. In hac habitasse platea dictumst. Aliquam augue quam, sollicitudin vitae, consectetuer eget, rutrum at, lorem.', 0, 'rio.jpeg');insert into Ciinemas (Name, Description, Status, Img) values ('Vox', 'Morbi porttitor lorem id ligula. Suspendisse ornare consequat lectus. In est risus, auctor sed, tristique in, tempus sit amet, sem. Fusce consequat. Nulla nisl.', 1, 'vox.jpg');");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("TRUNCATE TABLE Ciinemas");
        }
    }
}
