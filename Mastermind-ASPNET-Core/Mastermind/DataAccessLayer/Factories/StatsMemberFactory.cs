using Demo_ASPNET_Core.DataAccessLayer.Factories.Base;
using Mastermind.Models;
using MySql.Data.MySqlClient;
using System.Text;
using Mastermind.GameModels;
using System.Diagnostics.Metrics;

namespace Mastermind.DataAccessLayer.Factories
{
    public class StatsMemberFactory : FactoryBase<StatsMember>
    {
        protected override StatsMember CreateFromReader(MySqlDataReader reader)
        {
            int id = (int)reader["Id"];
            int memberId = (int)reader["MemberId"];
            int gamesWon = (int)reader["GamesWon"];
            int gamesLost = (int)reader["GamesLost"];
            int bestPerformance = (int)reader["BestPerformance"];

            return new StatsMember(id, memberId, gamesWon, gamesLost, bestPerformance);
        }

        public override StatsMember CreateEmpty()
        {
            return new StatsMember(0, 0, 0, 0, 0);
        }

        public StatsMember? GetByMember(int memberId)
        {
            string query = "SELECT * FROM tp6_statsmembre where MemberId = @memberId;";

            return GetFirstModel(query, "@memberId", memberId);

        }

        public void save(StatsMember statsMember)
        {
            string commandText = null;
            List<MySqlParameter>? sqlParams = new List<MySqlParameter>();

            if(statsMember.Id == 0)
            {
                commandText = "INSERT INTO `tp6_statsmembre` (`MemberId`, `GamesWon`, `GamesLost`, `BestPerformance`) " +
                    "VALUES (@MemberId, @GameWon, @GameLost, @BestPerfomance)";
            }
            else
            {
                commandText = "update tp6_statsmembre " +
                    "SET MemberId = @MemberId, GamesWon = @GamesWon, GamesLost = @GamesLost, BestPerformance = @BestPerformance where Id = @Id";

                sqlParams.Add(new MySqlParameter("@Id", statsMember.Id));
            }

            sqlParams.Add(new MySqlParameter("@MemberId", statsMember.MemberId));
            sqlParams.Add(new MySqlParameter("@GameWon", statsMember.GamesWon));
            sqlParams.Add(new MySqlParameter("@GameLost", statsMember.GamesLost));
            sqlParams.Add(new MySqlParameter("@BestPerformance", statsMember.BestPerformance));

            ExecuteNonQuery(statsMember, commandText, sqlParams);
        }




        public void UpdateStatistics(int memberId, Game game, int nombreTentatives)
        {
            StatsMember statsMember = GetByMember(memberId);
            if(statsMember == null) //Pour un utilisateur non membre qui souhaite jouer
            {
                DAL dal = new DAL();
                statsMember = dal.StatsMemberFact.CreateEmpty();
                statsMember.MemberId = memberId;
            }

           

            if(statsMember != null)
            {
                if (game.State == Game.GameState.PlayerWin)
                {
                    statsMember.GamesWon++;

                    if (statsMember.BestPerformance == null || statsMember.BestPerformance == 0 || nombreTentatives < statsMember.BestPerformance)
                    {
                        statsMember.BestPerformance = nombreTentatives;
                    }
                }
                else
                {
                    statsMember.GamesLost++;
                }

                save(statsMember);  
               
            }
        }

    }
}
