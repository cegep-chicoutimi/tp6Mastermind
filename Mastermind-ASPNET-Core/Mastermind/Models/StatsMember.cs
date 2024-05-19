using Mastermind.Models.Base;

namespace Mastermind.Models
{
    public class StatsMember : ModelBase
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }

        public int BestPerformance { get; set; }

        public StatsMember (int id, int memberId, int gamesWon, int gamesLost, int bestPerformance)
        {
            Id = id;
            MemberId = memberId;
            GamesWon = gamesWon;
            GamesLost = gamesLost;
            BestPerformance = bestPerformance;
        }   
    }
}
