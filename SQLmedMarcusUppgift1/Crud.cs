using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SQLmedMarcusUppgift1
{
    internal class Crud
    {
        public string ConnectionString { get; set; } = @"Data Source=.\SQLExpress;Integrated Security=true; database={0}";
        public string DatabaseName { get; set; } = "ObamaTree";

        /// <summary>
        /// Checks if the database allready exists.
        /// </summary>
        /// <param name="dbname"></param>
        /// <returns></returns>
        public bool DoesDatabaseExist(string dbname)
        {
            string query = "SELECT name FROM sys.databases";
            DatabaseName = "master";
            DataTable dataTable = GetDataTable(query);
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (dataRow.ItemArray[0].ToString() == dbname)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Creates the database ObamaTree if it does not exist.
        /// </summary>
        /// <param name="dBName"></param>
        public void CreateDatabase(string dBName)
        {
            string query = $"CREATE DATABASE {dBName}";
            ExecuteQuery(query);
            DatabaseName = dBName;
        }

        /// <summary>
        /// Creates the table Obamas if it does not exist.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        internal void CreateTable(string tableName, string columns)
        {
            string query = $"CREATE TABLE {tableName} ({columns})";
            ExecuteQuery(query);
        }

        /// <summary>
        /// Fills the ObamaTree database with table content.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="lines"></param>
        internal void FillObamaTree(string table, string[] lines)
        {
            foreach (string line in lines)
            {
                AddToTable(table, line);
            }
        }

        /// <summary>
        /// Takes a query and alt parameters using params modifier.
        /// Connects to the database, does magic and then returns a datatable.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable GetDataTable(string query, params (string name, string value)[] parameters)
        {
            DataTable dataTable = new DataTable();
            string connectionString = string.Format(ConnectionString, DatabaseName);
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    foreach (var parameter in parameters)
                    {
                        sqlCommand.Parameters.AddWithValue(parameter.name, parameter.value);
                    }

                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dataTable);
                    }
                }
            }
            return dataTable;
        }

        /// <summary>
        /// Executes a query and adds the users inputs to the table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="line"></param>
        public void AddToTable(string table, string line)
        {
            string query = $"INSERT INTO {table} (first_name, last_name, " +
                $"date_of_birth, date_of_death, mother_id, father_id)" +
                $"VALUES (@fName, @lName, @dob, @dod, @mId, @fId)";
            string[] info = line.Split(", ");
            (string, string)[] parameters = new (string, string)[]
            {
                ("@fName", info[0]),
                ("@lName", info[1]),
                ("@dob", info[2]),
                ("@dod", info[3]),
                ("@mId", info[4]),
                ("@fId", info[5])
            };
            ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// This method is used to execute querys depending on what the user wants to do
        /// with the database.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        private void ExecuteQuery(string query, params (string name, string value)[] parameters)
        {
            string connectionString = string.Format(ConnectionString, DatabaseName);
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    foreach (var parameter in parameters)
                    {
                        sqlCommand.Parameters.AddWithValue(parameter.name, parameter.value);
                    }
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// This query checks for persons with missing data and displays them to the user.
        /// </summary>
        /// <returns></returns>
        internal List<List<string>> MissingData()
        {
            string query = "SELECT * FROM Obamas WHERE date_of_death = '0' OR mother_id = 0 OR father_id = 0";
            DataTable dataTable = GetDataTable(query);
            return GetListOfPersons(dataTable);
        }

        /// <summary>
        /// This query displays everyone in the list to the user.
        /// </summary>
        /// <returns></returns>
        internal List<List<string>> GetAllObamas()
        {
            string query = "SELECT * FROM Obamas";
            DataTable dataTable = GetDataTable(query);
            return GetListOfPersons(dataTable);
        }

        /// <summary>
        /// This query displays every person matching the users search terms.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<List<string>> SearchByYear(string year)
        {
            string query = "SELECT * FROM Obamas WHERE date_of_birth LIKE @year";
            DataTable dataTable = GetDataTable(query, ("@year", $"{year}%"));
            return GetListOfPersons(dataTable);
        }

        /// <summary>
        /// This query displays perosns matching the users coice of names to search for.
        /// If present in the database the person / persons are displayed for the user.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<List<string>> SearchByName(string name)
        {
            string query = "SELECT * FROM Obamas WHERE ";
            (string, string)[] parameters;
            if (name.Contains(" "))
            {
                string[] names = name.Split(" ");
                query += "first_name LIKE @fName OR last_name LIKE @lName";
                parameters = new (string, string)[]
                {
                    ("@fName", $"{names[0]}%"),
                    ("@lName", $"{names[1]}%")
                };
            }
            else
            {
                query += "first_name LIKE @name OR last_name LIKE @name";
                parameters = new (string, string)[]
                {
                    ("@name", $"{name}%")
                };
            }
            DataTable dataTable = GetDataTable(query, parameters);
            return GetListOfPersons(dataTable);
        }

        /// <summary>
        /// This query takes the users inputs for a created person and adds them to the database.
        /// </summary>
        /// <param name="person"></param>
        internal void CreatePerson(List<string> person)
        {
            string query = "INSERT INTO Obamas (first_name, last_name, date_of_birth, date_of_death, mother_id, father_id)" +
                "VALUES (@fName, @lName, @dob, @dod, @mId, @fId)";
            (string, string)[] parameters = new (string, string)[]
            {
                ("@fName", person[0]),
                ("@lName", person[1]),
                ("@dob", person[2]),
                ("@dod", person[3]),
                ("@mId", person[4]),
                ("@fId", person[5])
            };
            ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// This query deletes the users choosen person to delete from the database.
        /// </summary>
        /// <param name="person"></param>
        public void DeletePerson(List<string> person)
        {
            string query = "DELETE FROM Obamas WHERE ID = @id";
            ExecuteQuery(query, ("@id", person[0]));
            query = "SELECT * FROM Obamas";
            DataTable dataTable = GetDataTable(query);
            List<List<string>> people = GetListOfPersons(dataTable);
            foreach (List<string> pers in people)
            {
                if (pers[5] == person[0])
                {
                    pers[5] = "0";
                    UpdatePerson(pers);
                }
                else if (pers[6] == person[0])
                {
                    pers[6] = "0";
                    UpdatePerson(pers);
                }
            }
        }

        /// <summary>
        /// This query searches the mother and father ID´s to find siblings of a person.
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public List<List<string>> GetSiblings(List<string> person)
        {
            string query = "SELECT * FROM Obamas WHERE (mother_id = @mId AND mother_id > 0) OR (father_id = @fId AND father_id > 0)";
            (string, string)[] parameters = new (string, string)[]
            {
                ("@mId", person[5]),
                ("@fId", person[6])
            };
            DataTable dataTable = GetDataTable(query, parameters);
            return GetListOfPersons(dataTable).Where(s => s[0] != person[0]).ToList();
        }

        /// <summary>
        /// Sends the query and paremeters to the method ExecuteQuery
        /// where the person UpdatePerson() takes in is updated.
        /// </summary>
        /// <param name="person"></param>
        public void UpdatePerson(List<string> person)
        {
            string query = "UPDATE Obamas SET first_name = @fName, last_name = @lName, " +
                "date_of_birth = @dob, date_of_death = @dod, mother_id = @mId, father_id = @fId " +
                "WHERE ID = @id";
            (string, string)[] parameters = new (string, string)[]
            {
                ("@id", person[0]),
                ("@fName", person[1]),
                ("@lName", person[2]),
                ("@dob", person[3]),
                ("@dod", person[4]),
                ("@mId", person[5]),
                ("@fId", person[6])
            };
            ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Takes in a datatable and returns the list of persons in it.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private List<List<string>> GetListOfPersons(DataTable dataTable)
        {
            List<List<string>> persons = new List<List<string>>();
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    persons.Add(RowToList(row));
                }
            }
            return persons;
        }

        /// <summary>
        /// This query searches the users choice of birth date.
        /// If a person with the birthday exists the person is displayed in the console.
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<List<string>> SearchbyYear(string year)
        {
            string query = "SELECT * FROM Obamas WHERE date_of_birth LIKE @year";
            DataTable dataTable = GetDataTable(query, ("@year", $"{year}%"));
            return GetListOfPersons(dataTable);
        }

        /// <summary>
        /// Checks if a person exists and if so displays the person to the user in the console.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal List<string> SearchById(string id)
        {
            string query = "SELECT * FROM Obamas WHERE ID = @id";
            DataTable datatable = GetDataTable(query, ("@id", id));
            if (datatable.Rows.Count > 0)
            {
                return RowToList(datatable.Rows[0]);
            }
            return new List<string>();
        }

        /// <summary>
        /// Takes in a datarows content and stores the info in a List of strings wich is then returned.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private List<string> RowToList(DataRow row)
        {
            List<string> person = new List<string>();
            person.Add(row["ID"].ToString());
            person.Add(row["first_name"].ToString());
            person.Add(row["last_name"].ToString());
            person.Add(row["date_of_birth"].ToString());
            person.Add(row["date_of_death"].ToString());
            person.Add(row["mother_id"].ToString());
            person.Add(row["father_id"].ToString());

            return person;
        }
    }
}