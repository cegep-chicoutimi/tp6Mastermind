namespace Mastermind.GameModels
{
    //Cette classe représente une tentative d'un joueur
    public class PlayerRow
    {
        //Cette liste représente les pions placés par un joueur dans une tentative
        public List<Pawn> Pawns { get; set; } = new List<Pawn>();

        public int NbBlackMarks
        {
            get
            {
                //nombre de pions qui sont bien placés
                return Pawns.Count(pawn => pawn.Mark == Pawn.MarkState.Black);
            }
        }

        public int NbWhiteMarks
        {
            get
            {
                //nombre de pions qui sont mal placés
                return Pawns.Count(pawn => pawn.Mark == Pawn.MarkState.White);
            }
        }

        public PlayerRow() { }
    }
}
