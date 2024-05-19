using Mastermind.Resources;
using System.ComponentModel.DataAnnotations;

namespace Mastermind.Areas.Admin.ViewModels
{
    public class ConfigEditVM
    {
        [Display(Name = "numberColors", ResourceType = typeof(Resource))]
        [Range(6, 8, ErrorMessageResourceName = "modelNumberColorsBetween", ErrorMessageResourceType = typeof(Resource))]
        public int NbColors { get; set; }

        [Display(Name = "numberPositions", ResourceType = typeof(Resource))]
        [Range(4, 5, ErrorMessageResourceName = "modelNumberPositionsBetween", ErrorMessageResourceType = typeof(Resource))]
        public int NbPositions { get; set; }

        [Display(Name = "numberAttempts", ResourceType = typeof(Resource))]
        [Range(6, 12, ErrorMessageResourceName = "modelNumberAttemptsBetween", ErrorMessageResourceType = typeof(Resource))]
        public int NbAttempts { get; set; }

        // Constructeur vide requis pour la désérialisation
        public ConfigEditVM()
        {
        }

        public ConfigEditVM(int nbColor, int nbPositions, int nbAttempts)
        {
            NbColors = nbColor;
            NbPositions = nbPositions;
            NbAttempts = nbAttempts;
        }
    }
}
