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
            int gamesAbandoned = (int)reader["GamesAbandoned"];
            int bestPerformance = (int)reader["BestPerformance"];
            int NombreCoups = (int)reader["NombreCoups"];

            return new StatsMember(id, memberId, gamesWon, gamesLost, gamesAbandoned, bestPerformance, NombreCoups);
        }

        public override StatsMember CreateEmpty()
        {
            return new StatsMember(0, 0, 0, 0, 0, 0, 0);
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
                commandText = "INSERT INTO `tp6_statsmembre` (`MemberId`, `GamesWon`, `GamesLost`, `GamesAbandoned`, `NombreCoups`,`BestPerformance`) " +
                    "VALUES (@MemberId, @GameWon, @GameLost, @GamesAbandoned, @NombreCoups, @BestPerformance)";
            }
            else
            {
                commandText = "update tp6_statsmembre " +
                    "SET MemberId = @MemberId, GamesWon = @GamesWon, GamesLost = @GamesLost," +
                    " GamesAbandoned = @GamesAbandoned, NombreCoups = @NombreCoups, BestPerformance = @BestPerformance where Id = @Id";

                sqlParams.Add(new MySqlParameter("@Id", statsMember.Id));
            }

            sqlParams.Add(new MySqlParameter("@MemberId", statsMember.MemberId));
            sqlParams.Add(new MySqlParameter("@GamesWon", statsMember.GamesWon));
            sqlParams.Add(new MySqlParameter("@GamesLost", statsMember.GamesLost));
            sqlParams.Add(new MySqlParameter("@GamesAbandoned", statsMember.GamesAbandoned));
            sqlParams.Add(new MySqlParameter("@NombreCoups", statsMember.NombreCoups));
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
                        statsMember.BestPerformance = nombreTentatives;     //Si on a pour ce membre de une nouvelle meilleure performance
                    }
                }
                else if (game.State == Game.GameState.ComputerWin)
                {
                    statsMember.GamesLost++;
                   
                }

                if(statsMember.MemberId != 0)   //On sauvegarde uniquement les stats des joueurs présents dans la BD comme "membre"
                {
                    if (game.CurrentPlayingRow == 10)
                        save(statsMember);
                    else
                        game.CurrentPlayingRow++;
                }
                
            }
        }

    }
}
