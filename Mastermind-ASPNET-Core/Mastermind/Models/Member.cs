using Mastermind.Models.Base;
using Mastermind.Resources;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace Mastermind.Models
{
    public class Member : ModelBase
    {
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_STANDARD = "Standard";

        [Display(Name = "FullName", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "ModelRequired", ErrorMessageResourceType = typeof(Resource))]
        [StringLength(30, ErrorMessageResourceName = "ModelLengthLessThan", ErrorMessageResourceType = typeof(Resource))]
        public string FullName { get; set; } = string.Empty;

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email", ResourceType = typeof(Resource))]
        [EmailAddress(ErrorMessageResourceName = "InvalidEmailFormat", ErrorMessageResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "ModelRequired", ErrorMessageResourceType = typeof(Resource))]
        [StringLength(50, ErrorMessageResourceName = "ModelLengthLessThan", ErrorMessageResourceType = typeof(Resource))]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Username", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "ModelRequired", ErrorMessageResourceType = typeof(Resource))]
        [StringLength(20, MinimumLength = 3, ErrorMessageResourceName = "ModelLengthBetween", ErrorMessageResourceType = typeof(Resource))]
        [Remote("MemberUsernameNotExists", "Member", "Admin", AdditionalFields = "Id", ErrorMessageResourceName = "UsernameAlreadyExists", ErrorMessageResourceType = typeof(Resource))]
        public string Username { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "ModelRequired", ErrorMessageResourceType = typeof(Resource))]
        [StringLength(20, MinimumLength = 5, ErrorMessageResourceName = "ModelLengthBetween", ErrorMessageResourceType = typeof(Resource))]
        public string Password { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public Guid ActivationCode { get; set; } = Guid.Empty;
        public string Role { get; set; } = ROLE_STANDARD;


        [Display(Name = "ImagePath", ResourceType = typeof(Resource))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "ModelRequired", ErrorMessageResourceType = typeof(Resource))]
        public string ImagePath { get; set; } = string.Empty;

        // Constructeur vide requis pour la désérialisation
        public Member()
        {
        }

        public Member(int id, string fullName, string email, string username, string password, string role, string imagePath) : base(id)
        {
            FullName = fullName;
            Email = email;
            Username = username;
            Password = password;
            Role = role;
            ImagePath = imagePath;
        }
    }
}
