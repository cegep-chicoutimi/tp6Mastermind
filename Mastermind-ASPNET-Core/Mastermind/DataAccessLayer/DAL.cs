using Mastermind.DataAccessLayer.Factories;

namespace Mastermind.DataAccessLayer
{
    public class DAL
    {
        private ConfigFactory? _configFact;
        private MemberFactory? _memberFact;
        private StatsMemberFactory? _statsMemberFact;

        public static string? ConnectionString { get; set; }

        public ConfigFactory ConfigFact
        {
            get
            {
                _configFact ??= new ConfigFactory();

                return _configFact;
            }
        }

        public MemberFactory MemberFact
        {
            get
            {
                _memberFact ??= new MemberFactory();

                return _memberFact;
            }
        }

        public StatsMemberFactory StatsMemberFact
        {
            get
            {
                _statsMemberFact ??= new StatsMemberFactory();  

                return _statsMemberFact;
            }
        }
    }
}
