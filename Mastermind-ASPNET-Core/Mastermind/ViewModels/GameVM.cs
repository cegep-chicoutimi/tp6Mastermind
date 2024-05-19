using Mastermind.GameModels;
using Mastermind.Models;

namespace Mastermind.ViewModels
{
    public class GameVM
    {
        public Game Game { get; set; }
        
        public StatsMember StatsMember { get; set; }

        public GameVM(Game game, StatsMember statsMember)
        {
            Game = game;
            StatsMember = statsMember;
        }
    }
}
