using Demo_ASPNET_Core.DataAccessLayer.Factories.Base;
using Mastermind.Models;
using MySql.Data.MySqlClient;
using System.Text;

namespace Mastermind.DataAccessLayer.Factories
{
    public class MemberFactory : FactoryBase<Member>
    {
        private static bool _membersLoaded = false;
        private static readonly object _membersLoadingLock = new();
        private static readonly Dictionary<int, Member> _members = new();

        protected override Member CreateFromReader(MySqlDataReader reader)
        {
            int id = (int)reader["Id"];
            string fullName = reader["FullName"].ToString() ?? string.Empty;
            string email = reader["Email"].ToString() ?? string.Empty;
            string username = reader["Username"].ToString() ?? string.Empty;
            string password = reader["Password"].ToString() ?? string.Empty;
            string role = reader["Role"].ToString() ?? string.Empty;
            string imagePath = reader["ImagePath"].ToString() ?? string.Empty;
            //string imagePath = reader["ImagePath"].ToString() ?? string.Empty;


            return new Member(id, fullName, email, username, password, role, imagePath);
        }

        public override Member CreateEmpty()
        {
            return new Member(0, "", "", "", "", "", "");
        }

        public List<Member> GetAll()
        {
            string commandText = "SELECT * FROM tp6_members;";

            return GetAllModels(commandText);   
        }

        public Member? Get(int id)
        {
            string query = "SELECT * FROM tp6_members where Id = @Id;";

            return GetFirstModel(query, "@Id", id);

        }

        public Member? Get(string sid)
        {
            if (int.TryParse(sid, out int id))
            {
                return Get(id);
            }

            return null;
        }

        public Member? GetByEmail(string email)
        {
           
            return _members.Values.FirstOrDefault(m => m.Email == email);
        }

        public Member? GetByUsername(string username)
        {
            string query = "SELECT * FROM tp6_members where Username = @Username;";

            return GetFirstModel(query, "@Username", username);
        }

        public void Save(Member member)
        {
            //Je me défini une liste de paramètres qui seront envoyés et la commandeText Associée
            string commandText = null;
            List<MySqlParameter>? sqlParams = new List<MySqlParameter>();

            if (member.Id == 0)
            {   
                 commandText = "INSERT INTO `tp6_members` (`FullName`, `Email`, `Username`, `Password`, `Role`, `ImagePath`) " +
                    "VALUES (@FullName, @Email, @Username, @Password, @Role,  @ImagePath)";
            }
            else
            {
                commandText = "UPDATE tp6_members " +
                    "SET FullName=@FullName, Email=@Email, Username=@Username, Password=@Password, Role=@Role, ImagePath=@ImagePath WHERE Id=@Id";

                sqlParams.Add(new MySqlParameter("@Id", member.Id));
            }

            sqlParams.Add(new MySqlParameter("@FullName", member.FullName));
            sqlParams.Add(new MySqlParameter("@Username", member.Username));
            sqlParams.Add(new MySqlParameter("@Email", member.Email));
            sqlParams.Add(new MySqlParameter("@Password", member.Password));
            sqlParams.Add(new MySqlParameter("@Role", member.Role));
            sqlParams.Add(new MySqlParameter("@ImagePath", member.ImagePath));

            ExecuteNonQuery(member, commandText, sqlParams);
        }

        public void Delete( int id)
        {
            Member member = Get(id);
            string commandText = "Delete from tp6_members where Id=@Id";
            List<MySqlParameter>? sqlParams = new List<MySqlParameter>();
            sqlParams.Add(new MySqlParameter("@Id", id));

            ExecuteNonQuery(commandText, sqlParams);
        }
    }
        
}
