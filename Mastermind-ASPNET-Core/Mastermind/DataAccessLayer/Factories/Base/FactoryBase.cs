using Mastermind.Models.Base;
using MySql.Data.MySqlClient;
using Mastermind.DataAccessLayer;

namespace Demo_ASPNET_Core.DataAccessLayer.Factories.Base
{
    /* Il s'agit ici d'une Factory de base(générique) qui fournit aux usines spécifiques des méthodes pour
     * creer et manipuler des objets de type spécifiques (T); sous certaines contraintes.
     */
    public abstract class FactoryBase<T> where T : ModelBase //le type générique T doit dériver de la classe "ModelBase"
    {
        protected abstract T CreateFromReader(MySqlDataReader reader);

        public abstract T CreateEmpty();

        /// <summary>
        ///  Récupère le premier objet de type T à partir d'une commande SQL et des paramètres spécifiés.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        protected T? GetFirstModel(string commandText, string paramName, object paramValue)
        {
            List<T> objs = GetAllModels(commandText, paramName, paramValue);

            //retourne "null" si la liste objs est vide, si non le premier élément
            return objs.Count > 0 ? objs[0] : null;
        }


        /// <summary>
        /// Version surchargée de la méthode précédente prenant une liste de paramètres SQL en option.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        protected T? GetFirstModel(string commandText, List<MySqlParameter>? sqlParams = null)
        {
            List<T> objs = GetAllModels(commandText, sqlParams);

            //retourne "null" si la liste objs est vide, si non le premier élément
            return objs.Count > 0 ? objs[0] : null;
        }

        /// <summary>
        /// Récupère tous les objets de type T à partir d'une commande SQL et des paramètres spécifiés.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        protected List<T> GetAllModels(string commandText, string paramName, object paramValue)
            => GetAllModels(commandText, new List<MySqlParameter> { new MySqlParameter(paramName, paramValue) });


        /// <summary>
        /// Récupère tous les objets de type T à partir d'une commande SQL et unr liste(pouvant etre nulle) e paramètres SQL en option.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="sqlParams"></param>
        /// <returns></returns>
        protected List<T> GetAllModels(string commandText, List<MySqlParameter>? sqlParams = null)
        {
            List<T> objs = new();

            MySqlConnection? cnn = null;
            MySqlDataReader? reader = null;

            try
            {
                cnn = new MySqlConnection(DAL.ConnectionString);
                cnn.Open();

                MySqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = commandText;

                if (sqlParams != null && sqlParams.Count > 0)
                {
                    cmd.Parameters.AddRange(sqlParams.ToArray());
                }

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    objs.Add(CreateFromReader(reader));
                }
            }
            finally
            {
                reader?.Close();
                cnn?.Close();
            }

            return objs;
        }


        /// <summary>
        /// Méthode pour exécuter des commandes SQL non query (INSERT/UPDATE/DELETE/CREATE/ALTER) prenant des paramètres spécifiés et n'utilisant pas d'objet.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        protected void ExecuteNonQuery(string commandText, string paramName, object paramValue)
            => ExecuteNonQuery(null, commandText, new List<MySqlParameter> { new MySqlParameter(paramName, paramValue) });


        /// <summary>
        /// Méthode pour exécuter des commandes SQL non query (INSERT/UPDATE/DELETE/CREATE/ALTER) prenant une liste de paramètres SQL en option et n'utilisant pas d'objet.
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="sqlParams"></param>
        protected void ExecuteNonQuery(string commandText, List<MySqlParameter>? sqlParams = null)
            => ExecuteNonQuery(null, commandText, sqlParams);

        /// <summary>
        /// Méthode pour exécuter des commandes SQL non query (INSERT/UPDATE/DELETE/CREATE/ALTER) prenant des paramètres spcécifiés.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="commandText"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        protected void ExecuteNonQuery(T? obj, string commandText, string paramName, object paramValue)
            => ExecuteNonQuery(obj, commandText, new List<MySqlParameter> { new MySqlParameter(paramName, paramValue) });


        /// <summary>
        /// Méthode pour exécuter des commandes SQL non query (INSERT/UPDATE/DELETE/CREATE/ALTER) prenant une liste(null par défaut!) de paramètres SQL en option.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="commandText"></param>
        /// <param name="sqlParams"></param>
        protected void ExecuteNonQuery(T? obj, string commandText, List<MySqlParameter>? sqlParams = null)
        {
            MySqlConnection? cnn = null;

            try
            {
                cnn = new MySqlConnection(DAL.ConnectionString);
                cnn.Open();

                MySqlCommand cmd = cnn.CreateCommand();
                cmd.CommandText = commandText;

                if (sqlParams != null && sqlParams.Count > 0)
                {
                    cmd.Parameters.AddRange(sqlParams.ToArray());
                }

                cmd.ExecuteNonQuery();

                if (obj != null && obj.Id == 0) //Si c'est un INSERT
                {
                    obj.Id = (int)cmd.LastInsertedId;
                }
            }
            finally
            {
                cnn?.Close();
            }
        }
    }
}
