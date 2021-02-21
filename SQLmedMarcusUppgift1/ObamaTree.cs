using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace SQLmedMarcusUppgift1
{
    class ObamaTree
    {
        public void SetUp()
        {
            Crud crud = new Crud();
            string dbName = "ObamaTree";
            string table = "Obamas";
            bool isDatabase = Crud.DoesDatabaseExist(dbName);
            if (isDatabase == false)
            {
                crud.CreateDatabase(dbName);
                crud.CreateTable(table,
                    "ID int PRIMARY KEY IDENTITY (1,1) NOT NULL," +
                    "first_name nvarchar(50) NOT NULL," +
                    "last_name nvarchar(50) NOT NULL, " +
                    "date_of_birth nvarchar(50) NULL, " +
                    "date_of_death nvarchar(50) NULL, " +
                    "mother_id int NULL, " +
                    "father_id int NULL");
                string[] lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\Släktträdet.txt"));
                crud.FillObamaTree(table, lines);
            }
        }

        internal void MainMenu()
        {
            throw new NotImplementedException();
        }
    }
}
