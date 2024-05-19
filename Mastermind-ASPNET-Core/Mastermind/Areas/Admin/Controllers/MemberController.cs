using Demo_ASPNET_Core.Areas.Admin.ViewModels;
using Mastermind.Areas.Admin.ViewModels;
using Mastermind.DataAccessLayer;
using Mastermind.Helpers;
using Mastermind.Models;
using Mastermind.Resources;
using Mastermind.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Data;
using System.Diagnostics.Metrics;
using System.IO;

namespace Mastermind.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Member.ROLE_ADMIN)]
    public class MemberController : Controller
    {
        /*On autorise seulement les Admin, car la section Membre se trouve cette fois ci uniquement dans la section
         Administration et pas dans le siteWeb*/
        public IActionResult List()
        {
            List<Member> members = new DAL().MemberFact.GetAll();

            return View(members);
        }

        public IActionResult Create()
        {
            DAL dal = new DAL();    

            MemberCreateEditVM viewModel = new MemberCreateEditVM(dal.MemberFact.CreateEmpty(),
                new List<string>() { "Admin", "Standard" });

            return View("CreateEdit", viewModel);
        }

        [HttpPost]
        public IActionResult Create(Member member, IFormFile uploadfile)
        {
            DAL dal = new DAL();
            if (member != null && uploadfile != null && uploadfile.Length >0)
            {
               
                Member existingMember = dal.MemberFact.GetByUsername(member.Username);

                if(existingMember == null)
                {
                    string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\Members", uploadfile.FileName);

                    if (!System.IO.File.Exists(pathToSave))     //Si le fichier n'existe pas pour le path spécifié il est crée
                    {
                        using FileStream stream = System.IO.File.Create(pathToSave);
                        uploadfile.CopyTo(stream);
                    }
                    member.ImagePath = uploadfile.FileName;

                    ModelStateEntry? imagePathModelState = ModelState["member.ImagePath"];
                    if (imagePathModelState != null)
                    {
                        imagePathModelState.ValidationState = ModelValidationState.Valid;
                    }
                }

                if (ModelState.IsValid) //si toutes les validations du modèle réussissent
                {
                    //member.Role = viewModel.LesRoles[0];
                    member.Password = CryptographyHelper.HashPassword(member.Password);

                    new DAL().MemberFact.Save(member);

                    return RedirectToAction("List", "Member");
                }
            }
            return View("CreateEdit", new MemberCreateEditVM(dal.MemberFact.CreateEmpty(),
                new List<string>() { "Admin", "Standard" }));
        }

        public IActionResult Edit(int id)
        {
            if (id > 0)
            {
                DAL dal = new();
                Member? memberExisting = dal.MemberFact.Get(id);

                if (memberExisting != null)
                {
                    MemberCreateEditVM viewModel = new MemberCreateEditVM(memberExisting,
                         new List<string>() { "Admin", "Standard" });


                    return View("CreateEdit", viewModel);
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

                //Le chemin de l'ancienne image
                string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\Members", existingMember.ImagePath);

                //Comparer l'anciène et la nouvelle image et decider de supression ou pas 

                if (existingMember != null)
                {
                    dal.MemberFact.Save(member);
                }
                else
                {
                    return View("SiteMessage", new SiteMessageVM
                    {
                        Message = Resource.Idwrong
                    }) ;
                }
            }

            return RedirectToAction("List");
        }


        public IActionResult Delete(int id)
        {
            if (id > 0)
            {
                DAL dal = new();
                Member? member = dal.MemberFact.Get(id);

                if (member != null)
                {
                    return View(member);
                }
            }

            return View("AdminMessage", new AdminMessageVM
            {
                Message = string.Format(Resource.ItemNotFoundControllerMsgM, Resource.Member.ToLower()),
                Controller = "Member",
                Action = "List",
                BackToMsg = Resource.BackToMemberList
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, IFormCollection collection)
        {
            if (id > 0)
            {
                DAL dal = new DAL();
                Member? existingMember = dal.MemberFact.Get(id);

                string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\Members", existingMember.ImagePath);

                //On supprime l'image losr de l'effacement d'un membre !
                if(System.IO.File.Exists(pathToSave))
                {
                    System.IO.File.Delete(pathToSave);
                }

                new DAL().MemberFact.Delete(id);
            }

            return RedirectToAction("List");
        }


        /// <summary>
        ///  Il faudrait s'assurer que le nom d'utilisateur est unique avec l'attribut [Remote] du modèle pour la vérification AJAX.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public JsonResult MemberUsernameNotExists(Member member)
        {
            Member? existingMember = new DAL().MemberFact.GetByUsername(member.Username);
            bool exists = existingMember != null && existingMember.Id != member.Id;

            return Json(!exists);
        }
    }
}
