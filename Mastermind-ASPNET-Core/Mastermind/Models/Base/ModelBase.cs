namespace Mastermind.Models.Base
{
    public abstract class ModelBase
    {
        public int Id { get; set; } = 0;

        // Constructeur vide requis pour la désérialisation
        public ModelBase()
        {
        }

        public ModelBase(int id)
        {
            Id = id;
        }
    }
}
