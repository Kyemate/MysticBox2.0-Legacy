using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MysticBox.Models
{
    /// <summary>
    /// The user data and profile for our application
    /// </summary>
    public class AppUser : IdentityUser<int>
    {
        [MaxLength(16)]
        public string UploadKey { get; set; }

    }

    public class AppRole : IdentityRole<int> { }
}
