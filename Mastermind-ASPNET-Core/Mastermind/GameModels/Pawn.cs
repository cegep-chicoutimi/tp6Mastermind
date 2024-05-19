namespace Mastermind.GameModels
{
    public class Pawn
    {
        /* Marque du pion qui indique s'il est bien placé/mal placé ou non marqué
         */
        public enum MarkState
        {
            None,
            Black, // bien placé: le pion a la bonne couleur et est à la bonne position
            White  //mal placé: le pion a la bonne couleur mais n'est pas à la bonne position 
        }

        public int Color { get; set; } = 0; //sa couleur
        public MarkState Mark { get; set; } = MarkState.None;

        public Pawn() { }
    }
}
