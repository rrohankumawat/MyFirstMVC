using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace MyFirstMVCApp.Models
{
    public class User
    {
        [Key] 
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
