using Mastermind.GameModels;
using Mastermind.DataAccessLayer;
using Mastermind.Models;
using Mastermind.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using MySqlX.XDevAPI;
using System.Security.Claims;

namespace Mastermind.Controllers
{
    public class GameController : Controller
    {
        //une variable(une constante dans ce cas) session pour stocker l'état actuel du jeu 
        private const string SESSION_GAME_NAME = "CurrentGame";


        //La logique de cette méthode ne vient pas de mo...source: ChatGPT
        private int GetMemberIdFromClaims()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.Sid);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int memberId))
            {
                return memberId;
            }

            return 0;   //Pour un utilisateur non membre qui souhaite jouer
        }

        /// <summary>
        /// Cette méthode crée un nouveau jeu ou récupère le jeu en cours depuis la session ! 
        /// </summary>
        /// <returns></returns>
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Validate(IFormCollection collection)
        {
            Game? game = null;
            DAL dal = new DAL();

            //essaie de récupérer un jeu en session et le désérialise si trouvé
            string? currentJsonGame = HttpContext.Session.GetString("CurrentGame");
            if (currentJsonGame != null)
            {
                game = JsonSerializer.Deserialize<Game>(currentJsonGame);

                if (game != null)
                {
                    PlayerRow playerRow = new();
                    //Pour chaque position, on récupère la couleur sélectionnée par le joueur et on l'ajoute et
                    //on l'ajoute à la liste des couleurs constituant la tentative en cours du joueur
                    for (int position = 1; position <= game.NbPositions; position++)
                    {
                        int.TryParse(collection["color-index-" + position], out int color);
                        playerRow.Pawns.Add(new Pawn { Color = color });
                    }

                    game.Validate(playerRow);

                    HttpContext.Session.SetString("CurrentGame", JsonSerializer.Serialize(game));   //mise à jour de la session !
                }

                //On met à jour les stats du joueur après chaque validation
                int memberId = GetMemberIdFromClaims();
                dal.StatsMemberFact.UpdateStatistics(memberId, game, game.CurrentPlayingRow - 1);
            }

            game ??= CreateOrGetGame();

            return PartialView("PartialGame", game);
        }

        /// <summary>
        /// Pour recommencer une partie !
        /// </summary>
        /// <returns></returns>
        ///  [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Replay(IFormCollection collection)
        {
            

            //essaie de récupérer un jeu en session et le désérialise si trouvé
            string? currentJsonGame = HttpContext.Session.GetString("CurrentGame");
            if (currentJsonGame != null)
            {
                DAL dal = new DAL();
                Game? game = null;
                game = JsonSerializer.Deserialize<Game>(currentJsonGame);

                if (game != null)
                {
                    int memberId = GetMemberIdFromClaims();
                    if (memberId != 0)
                    {
                        StatsMember statsMember = dal.StatsMemberFact.GetByMember(memberId);

                        if (statsMember != null)
                        {
                            //Uniquement pour les joueurs membres  ayant déjà fait au moins une tentative
                            if (game.CurrentPlayingRow > 1)
                            {
                                statsMember.GamesAbandoned++;
                                dal.StatsMemberFact.save(statsMember);
                            }
                        }

                    }

                    HttpContext.Session.Remove(SESSION_GAME_NAME);  //On supprime l'état actuel du jeu en session 

                }
            }

                    return RedirectToAction("Index");   //pour créer un nouveau jeu éventuellement 
        }
    }
}
