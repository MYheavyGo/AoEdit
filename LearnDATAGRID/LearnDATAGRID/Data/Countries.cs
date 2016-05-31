using LearnDATAGRID.Utils;
using System.Collections.ObjectModel;

namespace LearnDATAGRID
{
    public class Countries : ObservableCollection<Country>
    { 
        public Countries()
        {
            CreateCollection();
        }

        private void CreateCollection()
        {
            Add(new Country("Suisse", "Berne", "CHF", false));
            Add(new Country("France", "Paris", "EUR", true));
            Add(new Country("Allemagne", "Berlin", "EUR", true));
            Add(new Country("Grande-Bretagne", "Londres", "EUR", false));
            Add(new Country("Espagne", "Barcelone", "EUR", true));
        }
    }
}