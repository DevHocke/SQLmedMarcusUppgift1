using System;
using System.Collections.Generic;
using System.Data;
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

        private DataTable GetDataTable(string query)
        {
            throw new NotImplementedException();
        }
    }
}
