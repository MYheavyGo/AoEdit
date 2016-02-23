using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoEdit.GUI
{
    class Board
    {

        public List<Volume> Cubes { get; set; }
        int nbrWidth;
        int nbrHeight;

        public Board()
        {
            Cubes = new List<Volume>();
            nbrWidth = 16;
            nbrHeight = 16;


            CreateBoard();
        }

        private void CreateBoard()
        {
            for(int i = 0; i < nbrHeight; i++)
            {
                for(int j = 0; j < nbrWidth; j++)
                {
                    Cubes.Add(new Cube());
                }
            }
        }
    }
}
