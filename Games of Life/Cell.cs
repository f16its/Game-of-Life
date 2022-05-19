using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Games_of_Life
{
    public class Cell
    {
        public static int Generation = 0;

        public bool isAlive;
        public int liveNeighbors;
        public int xLocation;
        public int yLocation;

        public Cell(int xLocation, int yLocation)
        {
            this.isAlive = false;
            this.liveNeighbors = 0;
            this.xLocation = xLocation;
            this.yLocation = yLocation;
        }

        public Cell DeepCopy()
        {
            Cell copy = new Cell(this.xLocation, this.yLocation)
            {
                isAlive = this.isAlive,
                liveNeighbors = this.liveNeighbors
            };
            return copy;
        }
    }
}
