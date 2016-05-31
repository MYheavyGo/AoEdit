using System;

namespace LearnDATAGRID
{
    public class Country
    {
        private static int id;

        public static int Id
        {
            get
            {
                try
                {
                    int index = 0;
                    foreach(Country c in ((MainWindow)App.Current.MainWindow).Countries)
                    {
                        if (index != c.ID)
                        {
                            return index;
                        }
                        index++;
                    }
                    return index;
                }
                catch {}
                return id++;
            }
            set { id = value; }
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Capital { get; set; }
        public string Currency { get; set; }
        public bool DriveRight { get; set; }

        public Country(bool isInstance = true)
        {
            ID = Id;
            Name = "NO NAME";
            Capital = "NO CAPITAL";
            Currency = "NO CURRENCY";
            DriveRight = false;
        }

        public Country(string name, string capital, string currency, bool right) : this()
        {
            Name = name;
            Capital = capital;
            Currency = currency;
            DriveRight = right;
        }
    }
}
