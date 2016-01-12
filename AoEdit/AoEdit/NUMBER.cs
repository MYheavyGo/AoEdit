using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoEdit
{
    class NUMBER
    {
        public string Hexadecimal { get; set; }
        public int Decimal { get; set; }
        public char Caractere { get; set; }

        public NUMBER(byte nbr)
        {
            Hexadecimal = Convert.ToString(nbr, 16);
            Decimal = int.Parse(Hexadecimal, NumberStyles.AllowHexSpecifier);
            Caractere = (char)Decimal;
            if (Decimal == 0)
                Caractere = '.';
        }
    }
}
