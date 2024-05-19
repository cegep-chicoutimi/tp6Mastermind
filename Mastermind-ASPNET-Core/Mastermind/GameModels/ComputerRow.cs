namespace Mastermind.GameModels
{
    public class ComputerRow
    {
        //combinaison générée par l'ordinateur(qui est sensée être aléatoire!)
        public List<int> PawnColors { get; set; } = new List<int>();

        public ComputerRow()
        {
        }
    }
}
