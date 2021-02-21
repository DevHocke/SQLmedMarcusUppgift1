﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SQLmedMarcusUppgift1
{
    class Crud
    {
        public string ConnectionString { get; set; } = @"Data Source=.\SQLExpress;Integrated Security=true; database={0}";
        public string DatabaseName { get; set; } = "ObamaTree";

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

        public DataTable GetDataTable(string query, params (string name, string value)[] parameters) // name = @, value = variablen vi skyddat med key.
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

        internal void FillObamaTree(string table, string[] lines)
        {
            foreach (string line in lines)
            {
                AddToTable(table, line);
            }
        }

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

        internal void CreateTable(string tableName, string columns)
        {
            string query = $"CREATE TABLE {tableName} ({columns})";
            ExecuteQuery(query);
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

        private void UpdatePerson(List<string> pers)
        {
            throw new NotImplementedException();
        }

        private List<List<string>> GetListOfPersons(DataTable dataTable)
        {
            throw new NotImplementedException();
        }
    }
}
