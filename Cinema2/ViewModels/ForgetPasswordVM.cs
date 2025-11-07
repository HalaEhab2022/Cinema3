using System.ComponentModel.DataAnnotations;

namespace Cinema2.ViewModels
{
    public class ForgetPasswordVM
    {
        public int Id { get; set; }
        [Required]
        public string UserNameOrEmail { get; set; }
    }
}
