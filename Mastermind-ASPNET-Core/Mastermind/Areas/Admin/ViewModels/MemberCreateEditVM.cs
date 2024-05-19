using Mastermind.Models;

namespace Mastermind.Areas.Admin.ViewModels
{
    public class MemberCreateEditVM
    {
        /* Puisque lors de l'édition d'un membre, il va falloir proposer une liste déroulantre de choix de role, 
         * cette nouvelle ViewModel est pour concilier cela !
         */

        public Member Member { get; set; } = new Member();
        public List<string> LesRoles { get; set; }

        public MemberCreateEditVM() { }     //Constructeur vide requis pour la désérialisation

        public MemberCreateEditVM(Member member, List<string> lesRoles)
        {
            Member = member;
            LesRoles = lesRoles;
        }
    }
}
