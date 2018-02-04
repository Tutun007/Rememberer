using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.IO;

namespace Rememberer
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] connectionString;
            MyDbConnector connection=null;
            try
            {
                connectionString = File.ReadLines(@"connection.cfg").ToArray();
                connection = new MyDbConnector(connectionString[0], connectionString[1], connectionString[2], connectionString[3], connectionString[4]);
            }
            catch(Exception Ex) {
                Console.WriteLine("Oh no, i think there's something wrong with your connection.cfg.\n" +
                    "Here's the error:" + Ex.Message);
                Console.ReadLine();
                return;
            }

            List<Record> localData = new List<Record>();
            int currentPosition;
            try
            {
                localData = connection.SelectRecords();
                PrintList(localData);
                Console.SetCursorPosition(0, 0);
                Console.Write('§');
                Console.CursorLeft = 0;
                while (true)
                {
                    currentPosition = Console.CursorTop;
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.DownArrow:
                            if (localData.Count == 0)
                                break;
                            Console.SetCursorPosition(0, (currentPosition + 1) % localData.Count);
                            break;
                        case ConsoleKey.UpArrow:
                            if (localData.Count == 0)
                                break;
                            Console.SetCursorPosition(0, (currentPosition - 1 + localData.Count) % localData.Count);
                            break;
                        case ConsoleKey.Delete:
                            if (localData.Count == 0)
                                break;
                            if (localData[currentPosition].Status != Status.New)
                                localData[currentPosition].Status = Status.Del;
                            else
                                localData.RemoveAt(currentPosition);
                            Console.Clear();
                            PrintList(localData);
                            Console.SetCursorPosition(0, 0);
                            break;
                        case ConsoleKey.Enter:
                            if (localData.Count == 0)
                                break;
                            Console.Clear();
                            Console.WriteLine("Edit your record and press Enter:");
                            SendKeys.SendWait(localData[currentPosition].Text);
                            localData[currentPosition].Text = Console.ReadLine();
                            if (localData[currentPosition].Status != Status.New)
                                localData[currentPosition].Status = Status.Edit;
                            Console.Clear();
                            PrintList(localData);
                            Console.SetCursorPosition(0, 0);
                            break;
                        case ConsoleKey.Spacebar:
                            Console.Clear();
                            Console.WriteLine("Create your record and press Enter:");
                            localData.Add(new Record(Console.ReadLine(), Status.New));
                            Console.Clear();
                            PrintList(localData);
                            Console.SetCursorPosition(0, 0);
                            break;
                        case ConsoleKey.L:
                            localData = connection.SelectRecords();
                            Console.Clear();
                            PrintList(localData);
                            Console.SetCursorPosition(0, 0);
                            break;
                        case ConsoleKey.S:
                            if (localData.Count == 0)
                                break;
                            connection.DeleteRecords(localData.Where(x => x.Status == Status.Del).ToList());
                            connection.UpdateRecords(localData.Where(x => x.Status == Status.Edit).ToList());
                            connection.InsertRecords(localData.Where(x => x.Status == Status.New).ToList());
                            localData = connection.SelectRecords();
                            Console.Clear();
                            PrintList(localData);
                            Console.SetCursorPosition(0, 0);
                            break;
                        case ConsoleKey.R:
                            Console.Clear();
                            PrintList(localData);
                            Console.SetCursorPosition(0, 0);
                            break;
                        case ConsoleKey.Escape:
                            return;
                            break;
                        case ConsoleKey.F1:
                            Console.Clear();
                            Console.WriteLine("-=KEYS=-\n" +
                                "Up and Down arrows  :Navigation\n" +
                                "Enter               :Edit record\n" +
                                "Spacebar            :Create new record\n" +
                                "Delete              :Delete new record or mark DB record\n" +
                                "S                   :Save changes to DB\n" +
                                "L                   :Override local data from DB\n" +
                                "R                   :Refresh window\n" +
                                "Esc                 :Leave this horrible place\n\n" +
                                "DB   - record, loaded from remote database\n" +
                                "Edit - DB record, which was changed in local storage\n" +
                                "Del  - DB record, which was deleted in local storage\n" +
                                "New  - record, which was created in local storage\n\n" +
                                "Press any key");
                            Console.ReadKey();
                            Console.Clear();
                            PrintList(localData);
                            Console.SetCursorPosition(0, 0);
                            break;
                        default:
                            Console.CursorLeft = 0;
                            break;
                    }
                    Console.Write('§');
                    Console.CursorLeft = 0;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Oops, something went wrong.\n" +
                    "Here's the error: " + ex.Message);
                Console.ReadLine();
                return;
            }
            //connection.Close();

            
        }

        private static void PrintList(List<Record> list)
        {
            foreach (var a in list/*.Where(x=>x.Status!=Status.Deleted)*/)
            {
                Console.WriteLine(" " + a);
            }
            Console.WriteLine("\nPress F1 for help");
        }
    }
    
}
