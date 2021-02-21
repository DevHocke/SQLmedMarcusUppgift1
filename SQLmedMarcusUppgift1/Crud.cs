﻿using System.Collections.Generic;
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
        /// Kontrollerar om databasen redan finns.
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
        /// Skapar upp databasen ObamaTree om den inte redan finns.
        /// </summary>
        /// <param name="dBName"></param>
        public void CreateDatabase(string dBName)
        {
            string query = $"CREATE DATABASE {dBName}";
            ExecuteQuery(query);
            DatabaseName = dBName;
        }
        /// <summary>
        /// Skapar tabellen Obamas om den inte redan finns.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        internal void CreateTable(string tableName, string columns)
        {
            string query = $"CREATE TABLE {tableName} ({columns})";
            ExecuteQuery(query);
        }

        internal void FillObamaTree(string table, string[] lines)
        {
            foreach (string line in lines)
            {
                AddToTable(table, line);
            }
        }
        /// <summary>
        /// Tar emot en query och alternativa parametrar med params modifier.
        /// Kopplar upp sig mot databasen  och returnerar en datatable.
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
        /// kör queryn mot databasen och lägger till personen användaren matat in.
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

        internal List<List<string>> MissingData()
        {
            string query = "SELECT * FROM Obamas WHERE date_of_death = '0' OR mother_id = 0 OR father_id = 0";
            DataTable dataTable = GetDataTable(query);
            return GetListOfPersons(dataTable);
        }

        internal List<List<string>> GetAllObamas()
        {
            string query = "SELECT * FROM Obamas";
            DataTable dataTable = GetDataTable(query);
            return GetListOfPersons(dataTable);
        }

        public List<List<string>> SearchByYear(string year)
        {
            string query = "SELECT * FROM Obamas WHERE date_of_birth LIKE @year";
            DataTable dataTable = GetDataTable(query, ("@year", $"{year}%"));
            return GetListOfPersons(dataTable);
        }

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

        public List<List<string>> SearchbyYear(string year)
        {
            string query = "SELECT * FROM Obamas WHERE date_of_birth LIKE @year";
            DataTable dataTable = GetDataTable(query, ("@year", $"{year}%"));
            return GetListOfPersons(dataTable);
        }

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