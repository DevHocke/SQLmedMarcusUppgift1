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
            bool isDatabase = crud.DoesDatabaseExist(dbName);
            if (isDatabase == false)
            {
                crud.CreateDatabase(dbName);
                crud.CreateTable(table,
                    "ID int PRIMARY KEY IDENTITY (1,1) NOT NULL, " +
                    "first_name nvarchar(50) NOT NULL, " +
                    "last_name nvarchar(50) NOT NULL, " +
                    "date_of_birth nvarchar(50) NULL, " +
                    "date_of_death nvarchar(50) NULL, " +
                    "mother_id int NULL, " +
                    "father_id int NULL");
                string[] lines = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\Släktträdet.txt"));
                crud.FillObamaTree(table, lines);
            }
        }

        public void MainMenu()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                Console.Clear();
                Console.WriteLine("Welcome to the ObamaTree! What do you want to do? ");
                Console.WriteLine("1. Add a person: ");
                Console.WriteLine("2. Search ObamaTree: ");
                Console.WriteLine("3. Exit program");
                int.TryParse(Console.ReadLine(), out int choice);
                switch (choice)
                {
                    case 1:
                        AddPerson();
                        break;
                    case 2:
                        SearchObamaTree();
                        break;
                    case 3:
                        Console.WriteLine("Bye bye. . .");
                        Thread.Sleep(2000);
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice, try again! Press any key to continue.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void SearchObamaTree()
        {
            throw new NotImplementedException();
        }

        private void AddPerson()
        {
            throw new NotImplementedException();
        }
    }
}
