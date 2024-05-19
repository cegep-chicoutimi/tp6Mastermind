using Mastermind.Areas.Admin.ViewModels;
using Mastermind.DataAccessLayer;
using Mastermind.GameModels;
using Mastermind.Models;
using Mastermind.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mastermind.GameModels;
using System.Net.NetworkInformation;
using System.Text.Json;
using MySqlX.XDevAPI;
using System.Security.Claims;

namespace Mastermind.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Member.ROLE_ADMIN)]
    public class HomeController : Controller
    {
        //une variable(une constante dans ce cas) session pour stocker l'état actuel du jeu 
        private const string SESSION_GAME_NAME = "CurrentGame";

        private Game CreateOrGetGame()
        {
            Game? game = null;

            string? currentJsonGame = HttpContext.Session.GetString(SESSION_GAME_NAME);
            if (currentJsonGame != null)
                game = JsonSerializer.Deserialize<Game>(currentJsonGame);   //l'état du jeu tel qu'il était avant la requête actuelle.

            //Le jeu n'a pas encore commencé ou que la session a expirée
            if (game == null)
            {
                //une nouvelle instance de jeu est crée avec ses config initiales(qui sont récupérées)
                Dictionary<string, Config> configByKey = new DAL().ConfigFact.GetAll();

                int.TryParse(configByKey[Config.NB_COLORS].Value, out int nbColors);
                int.TryParse(configByKey[Config.NB_POSITIONS].Value, out int nbPositions);
                int.TryParse(configByKey[Config.NB_ATTEMPTS].Value, out int nbAttempts);

                game = new(nbColors, nbPositions, nbAttempts);

                //Le jeu nouvellement créé est ensuite stocké en session.
                HttpContext.Session.SetString(SESSION_GAME_NAME, JsonSerializer.Serialize(game));
            }

            return game;
        }

        private int GetMemberIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.Sid);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int memberId))
            {
                return memberId;
            }

            return 0;   //Pour un utilisateur non membre qui souhaite jouer
        }

        public IActionResult Index()
        {
            DAL dal = new DAL();
            Game game = CreateOrGetGame();  //récupère l'instance du jeu en session ou en créé un nouveau

            int memberId = GetMemberIdFromClaims();
            StatsMember statsMember = dal.StatsMemberFact.GetByMember(memberId);

            if (statsMember == null)    //Pour un utilisateur non membre qui souhaite jouer
            {
                statsMember = dal.StatsMemberFact.CreateEmpty();
                statsMember.MemberId = memberId;
            }


            GameVM viewModel = new(game, statsMember);

            return View(viewModel);
        }
    }
}
