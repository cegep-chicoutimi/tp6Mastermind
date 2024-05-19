namespace Mastermind.GameModels
{
    public class Game
    {
        public  enum GameState
        {
            ComputerWin,
            PlayerWin,
            Running
        }

        public int NbColors { get; set; }
        public int NbPositions { get; set; }
        public int NbAttempts { get; set; }     //nombre maximum de tentatives pour deviner la combinaison secrète !

        public GameState State { get; set; } = GameState.Running;
        public int CurrentPlayingRow { get; set; } = 1;   //numéro de tentatives(ou de coups) actuel
        public ComputerRow ComputerRow { get; set; } = new ComputerRow();   //séquence de l'ordinateur
        public List<PlayerRow> PlayerRows { get; set; } = new List<PlayerRow>();    //Liste des tentatives du joueur !


        /// <summary>
        /// initialise une nouvelle partie avec un nombre donné de couleurs/positions/tentatives
        /// </summary>
        /// <param name="nbColors"></param>
        /// <param name="nbPositions"></param>
        /// <param name="nbAttempts"></param>
        public Game(int nbColors, int nbPositions, int nbAttempts)
        {
            Random rnd = new();

            NbColors = nbColors;
            NbPositions = nbPositions;
            NbAttempts = nbAttempts;

            for (int i = 0; i < NbPositions; i++)
            {
                /*Pour chaque position dans la séquence/combinaison une couleur est choisie aléatoirement parmi les 
                 couleurs possibles à ajoutée !*/
                ComputerRow.PawnColors.Add(rnd.Next(1, NbColors + 1));
            }
        }
        
        /// <summary>
        /// Cette méthode vérifie la tentative du joueur et met à jour l'état du jeu 
        /// </summary>
        /// <param name="playerRow"></param>
        public void Validate(PlayerRow playerRow)
        {
            if (State == GameState.Running)
            {
                List<int> computerColors = new(ComputerRow.PawnColors); // Création d'une copie de la séquence de l'ordinateur !

                // Vérification pour les pions valides de la bonne couleur à la bonne position
                for (int position = 0; position < NbPositions; position++)
                {
                    Pawn pawn = playerRow.Pawns[position];

                    //si la couleur du pion du joueur correspond à la couleur du pion de l'ordinateur à la même position
                    if (pawn.Color == ComputerRow.PawnColors[position])
                    {
                        pawn.Mark = Pawn.MarkState.Black;
                        //Cette supressio garantit que cette couleur ne sera pas à nouveau considérée pour un marquage "White".
                        computerColors.Remove(pawn.Color);
                    }
                }

                // Après avoir vérifier toutes les positions(au sorti de la boucle)
                if (playerRow.NbBlackMarks == NbPositions)
                {
                    //si tous les pions sont bien placés, on modifie l'état du jeu en conséquence !
                    State = GameState.PlayerWin;
                }
                else
                {
                    // Vérification pour les pions qui sont de la bonne couleur mais hors position
                    for (int position = 0; position < NbPositions; position++)
                    {
                        Pawn pawn = playerRow.Pawns[position];

                        if (pawn.Mark == Pawn.MarkState.None && computerColors.Contains(pawn.Color))
                        {
                            pawn.Mark = Pawn.MarkState.White;
                            computerColors.Remove(pawn.Color);
                        }
                    }

                    if (CurrentPlayingRow == NbAttempts)
                    {
                        State = GameState.ComputerWin;
                    }
                }

                //la tentative du joueur est ajoutée à la liste des tentatives.
                PlayerRows.Add(playerRow);
                CurrentPlayingRow++; //On passe à la ligne suivante |
            }
        }
    }
}
