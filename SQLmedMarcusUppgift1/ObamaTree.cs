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
            bool keepGoing = true;
            while (keepGoing)
            {
                Console.Clear();
                Crud crud = new Crud();
                Console.WriteLine("What do you want to search for?");
                Console.WriteLine("1. Search after first or last name.");
                Console.WriteLine("2. Search for Obamas with missing data.");
                Console.WriteLine("3. List all the Obamas in the database.");
                Console.WriteLine("4. Search Obamas born a certain year.");
                Console.WriteLine("5. Go back.");
                int.TryParse(Console.ReadLine(), out int choice);
                List<List<string>> persons = new List<List<string>>();
                switch (choice)
                {
                    case 1:
                        Console.Write("Enter a name: ");
                        persons = crud.SearchByName(Console.ReadLine());
                        break;
                    case 2:
                        persons = crud.MissingData();
                        break;
                    case 3:
                        persons = crud.GetAllObamas();
                        break;
                    case 4:
                        Console.Write("Enter a year: ");
                        persons = crud.SearchByYear(Console.ReadLine());
                        break;
                    case 5:
                        MainMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid choice, try again! Press any key to continue.");
                        Console.ReadKey();
                        break;
                }

                if (persons.Count > 0)
                {
                    while (true)
                    {
                        Console.Clear();
                        int counter = 1;
                        foreach (List<string> person in persons)
                        {
                            Console.WriteLine($"{counter++}. {person[1]} {person[2]} {person[3]}");
                        }
                        Console.WriteLine($"{counter}. Go back");
                        Console.Write("Make a choice: ");
                        int.TryParse(Console.ReadLine(), out choice);
                        if (choice > 0 && choice <= persons.Count)
                        {
                            SelectedPerson(persons[choice - 1]);
                        }
                        else if (choice == counter)
                        {
                            SearchObamaTree();
                        }
                        else
                        {
                            Console.WriteLine("Invalid choice, try again! Press any key to continue.");
                            Console.ReadKey();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No Obama match your search! Try again.");
                    Console.ReadKey();
                }
            }
        }

        private void SelectedPerson(List<string> person)
        {

            bool keepGoing = true;
            while (keepGoing)
            {
                Console.Clear();
                DisplayInfo(person);
                Console.WriteLine("What do you want to do?");
                Console.WriteLine("1. Edit person. ");
                Console.WriteLine("2. Delete Person.");
                Console.WriteLine("3. Show siblings.");
                Console.WriteLine("4. Go back.");
                int.TryParse(Console.ReadLine(), out int choice);
                switch (choice)
                {
                    case 1:
                        EditPerson(person);
                        break;
                    case 2:
                        DeletePerson(person);
                        break;
                    case 3:
                        ShowSiblings(person);
                        break;
                    case 4:
                        SearchObamaTree();
                        break;
                    default:
                        Console.WriteLine("Invalid choice, try again! Press any key to continue.");
                        Console.ReadKey();
                        break;
                }

            }
        }

        private void ShowSiblings(List<string> person)
        {
            throw new NotImplementedException();
        }

        private void DeletePerson(List<string> person)
        {
            Crud crud = new Crud();
            Console.WriteLine($"Do you really want to delete {person[1]} {person[2]}? (y/n) ");
            string choice = Console.ReadLine().ToLower();
            if (choice == "y")
            {
                crud.DeletePerson(person);
                Console.WriteLine($"{person[1]} {person[2]} was deleted");
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
                SearchObamaTree();
            }
        }

        private void EditPerson(List<string> person)
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                Console.Clear();
                DisplayInfo(person);
                Console.WriteLine("What do you want to update?");
                Console.WriteLine("1. First name.");
                Console.WriteLine("2. Last name.");
                Console.WriteLine("3. Date of birth.");
                Console.WriteLine("4. Date of Death.");
                Console.WriteLine("5. Mother.");
                Console.WriteLine("6. Father.");
                Console.WriteLine("7. Go back.");
                int.TryParse(Console.ReadLine(), out int choice);
                Crud crud = new Crud();
                switch (choice)
                {
                    case 1:
                        Console.Write("First name: ");
                        person[1] = Console.ReadLine();
                        break;
                    case 2:
                        Console.Write("Last name: ");
                        person[2] = Console.ReadLine();
                        break;
                    case 3:
                        Console.Write("Date of birth: ");
                        person[3] = Console.ReadLine();
                        break;
                    case 4:
                        Console.Write("Date of death: ");
                        person[4] = Console.ReadLine();
                        break;
                    case 5:
                        Console.Write("Enter mother's name: ");
                        string mother = Console.ReadLine();
                        List<List<string>> people = crud.SearchByName(mother);
                        if (people.Count > 0)
                        {
                            int counter = 1;
                            foreach (List<string> peopleM in people)
                            {
                                Console.WriteLine($"{counter++} {peopleM[1]} {peopleM[2]} {peopleM[3]}");
                            }
                            Console.WriteLine($"{counter}. None of the above.");
                            int.TryParse(Console.ReadLine(), out choice);
                            if (choice > 0 && choice <= people.Count)
                            {
                                person[5] = people[choice - 1][0];
                            }
                            else if (choice == counter)
                            {
                                person[5] = CreateParent(mother, "her");
                            }
                            else
                            {
                                Console.WriteLine("Invalid choice, try again! Press any key to continue.");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            person[5] = CreateParent(mother, "her");
                        }
                        break;
                    case 6:
                        Console.Write("Enter father's name: ");
                        string father = Console.ReadLine();
                        people = crud.SearchByName(father);
                        if (people.Count > 0)
                        {
                            int counter = 1;
                            foreach (List<string> peopleF in people)
                            {
                                Console.WriteLine($"{counter++} {peopleF[1]} {peopleF[2]} {peopleF[3]}");
                            }
                            Console.WriteLine($"{counter}. None of the above.");
                            int.TryParse(Console.ReadLine(), out choice);
                            if (choice > 0 && choice <= people.Count)
                            {
                                person[6] = people[choice - 1][0];
                            }
                            else if (choice == counter)
                            {
                                person[6] = CreateParent(father, "him");
                            }
                        }
                        else
                        {
                            person[6] = CreateParent(father, "him");
                        }
                        break;
                    case 7:
                        keepGoing = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice, try again! Press any key to continue.");
                        Console.ReadKey();
                        break;
                }
                crud.UpdatePerson(person);
            } 
        }

        private string CreateParent(string parent, string type)
        {
            Console.Write($"{parent} does not exist in the database, do you want to add {type}? (y/n) ");
            string yesNo1 = Console.ReadLine().ToLower();
            if (yesNo1 == "y")
            {
                AddPerson();
                return GetLastAddedId();
            }
            else
            {
                return "0";
            }
        }

        private string GetLastAddedId()
        {
            throw new NotImplementedException();
        }

        private void DisplayInfo(List<string> person)
        {
            throw new NotImplementedException();
        }

        public static void AddToList(string question, List<string> person)
        {
            Console.Write(question);
            person.Add(Console.ReadLine());
        }

        private void AddPerson()
        {
            Console.Clear();
            List<string> person = new List<string>();
            AddToList("Enter the persons first name: ", person);
            AddToList("Enter the persons last name: ", person);
            AddToList("Enter the persons date of birth: ", person);
            for (int i = 0; i < 3; i++)
            {
                person.Add("0");
            }
            Crud crud = new Crud();
            crud.CreatePerson(person);
            Console.WriteLine(person[0] + " " + person[1] + "was added to the ObamaTree!");
        }
        
    }
}
