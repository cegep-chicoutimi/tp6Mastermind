using Mastermind.Models.Base;

namespace Mastermind.Models
{
    public class StatsMember : ModelBase
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int GamesAbandoned { get; set; } 

        public int BestPerformance { get; set; }
        public int NombreCoups { get; set; }

        public StatsMember (int id, int memberId, int gamesWon, int gamesLost, int gamesAbandoned, int bestPerformance, int moyenneCoups)
        {
            Id = id;
            MemberId = memberId;
            GamesWon = gamesWon;
            GamesLost = gamesLost;
            GamesAbandoned = gamesAbandoned;    
            BestPerformance = bestPerformance;
            NombreCoups = moyenneCoups;
        } 
        
        public double GetAverage()
        {
            double average = 0; 

            average = GamesWon / (GamesLost + GamesWon);

            return average;
        }
    }
}
