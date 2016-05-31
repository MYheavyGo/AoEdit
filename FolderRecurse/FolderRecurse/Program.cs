using System;
using System.Collections.Generic;
using System.IO;

namespace FolderRecurse
{
    class Program
    {
        static List<string> files = new List<string>();
        static void Main(string[] args)
        {

            Console.WriteLine(args[0]);
            Console.WriteLine(BrowseFolder(args[0]));
            Console.ReadLine();
        }

        static private string BrowseFolder(string path)
        {
            string tmp = "";
            try
            {
                foreach (string s in Directory.GetDirectories(path))
                {
                    tmp += "\"D\" - " + s + "\n";
                    foreach (string f in Directory.GetFiles(s))
                    {
                        files.Add(f);
                        tmp += "\"F\" - " + f + "\n";
                    }
                    tmp += BrowseFolder(s);
                }
            }
            catch { }
            return tmp;
        }
    }
}
