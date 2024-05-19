using Mastermind.Areas.Admin.ViewModels;
using Mastermind.DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Mastermind.Models;
using Mastermind.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Demo_ASPNET_Core.Areas.Admin.ViewModels;
using Mastermind.Resources;
using Mastermind.ViewModels;
using System.Security.Claims;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Mastermind.Controllers
{
    public class MemberController : Controller
    {
        public IActionResult Create()
        {
           Member member = new Member();

            return View("CreateEdit", member);
        }

        [HttpPost]
        public IActionResult Create(Member? member, IFormFile uploadfile)
        {
            if (member != null && uploadfile != null && uploadfile.Length > 0)
            {
                DAL dal = new DAL();
                Member existingMember = dal.MemberFact.GetByUsername(member.Username);  

                if(existingMember != null)  //Je pense que le "remote" le fait déja !
                {
                    return View("SiteMessage", new SiteMessageVM
                    {
                        Message = Resource.UsernameAlreadyExists
                    });
                }


                string extension = Path.GetExtension(uploadfile.FileName).ToLower();
                string filename = string.Format("{0}{1}", Guid.NewGuid().ToString(), extension);
                string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\Members", filename);

                using FileStream stream = System.IO.File.Create(pathToSave);
                uploadfile.CopyTo(stream);

                member.ImagePath = filename;

                ModelStateEntry? imagePathModelState = ModelState["ImagePath"];
                if (imagePathModelState != null)
                {
                    imagePathModelState.ValidationState = ModelValidationState.Valid;
                }
            }

            if (ModelState.IsValid)
            {
                member.Role = Member.ROLE_STANDARD; //Ne donner aucun choix pour le rôle : obligatoirement « Standard ».

                member.Password = CryptographyHelper.HashPassword(member.Password);

                //TODO: J'ai plus besoin des ExistingMember pour verifier le Username, car le Remote le fait déja !

                new DAL().MemberFact.Save(member);

                //Directement l'utilisateur est invité à se loguer avec son nouveau compte !
                return RedirectToAction("Login", "Auth", new { Area = "" });
            }
            return View("CreateEdit", member);
        }

        [AuthorizeLoggedIn] //Filtre uniquement pour les utilisateurs connectés
        public IActionResult Filter()
        {
            string userId = User.FindFirst(ClaimTypes.Sid)?.Value;

            int id = 0;

            if(int.TryParse(userId, out id))
            {
                DAL dal = new();
                Member? memberExisting = dal.MemberFact.Get(id);

                if (memberExisting != null)
                {
                    return View("CreateEdit", memberExisting);
                }
            }

            return View("SiteMessage", new SiteMessageVM
            {
                Message = Resource.Idwrong
            });


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Member member, IFormFile uploadfile)
        {
            if (member != null)
            {
                DAL dal = new();

                Member? existingMember = dal.MemberFact.Get(member.Id);
                if (existingMember != null)
                {
                    // Déconnexion de l'utilisateur
                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                    member.Password = CryptographyHelper.HashPassword(member.Password);

                    dal.MemberFact.Save(member);

                    //Directement l'utilisateur est invité à se loguer avec son nouveau compte !
                    return RedirectToAction("Login", "Auth", new { Area = "" });
                }
                else
                {
                    return View("SiteMessage", new SiteMessageVM
                    {
                        Message = Resource.Idwrong
                    });
                }
            }

            return RedirectToAction("List");
        }
    }
}
