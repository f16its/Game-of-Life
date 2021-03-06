using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;

namespace Games_of_Life
{
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();

        static int width = 96;
        static int height = 40;

        Cell[,] cells = new Cell[width, height];
        Cell[,] prevCells = new Cell[width, height];
        Rectangle[,] rectangles = new Rectangle[width, height];

        private const int size = 10;
        
        private const int marginX = 20;
        private const int marginY = 20;

        public MainWindow()
        {
            InitializeComponent();
            myCanvas.Focus();

            for (int i = 0; i < width; i++)//Creates and draws cells
            {
                for (int j = 0; j < height; j++)
                {
                    cells[i, j] =  new Cell(i * size + marginX, j * size + marginY);
                    rectangles[i, j] = new Rectangle
                    {
                        Stroke = Brushes.Gray,
                        Fill = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        Height = size,
                        Width = size
                    };
                    myCanvas.Children.Add(rectangles[i, j]);
                    Canvas.SetLeft(rectangles[i, j], cells[i, j].xLocation);
                    Canvas.SetTop(rectangles[i, j], cells[i, j].yLocation);
                }
            }

            gameTimer.Interval = TimeSpan.FromMilliseconds(200);
            gameTimer.Tick += GameLoop;

        }

        private void GameLoop(object sender, EventArgs e)
        {

            for (int i = 0; i < width; i++)//Creates a deep copy of the cells
                for (int j = 0; j < height; j++)
                    prevCells[i, j] = cells[i, j].DeepCopy();

            for (int i = 0; i < width; i++)//Checks for number of neighbors for each cell
            {
                for (int j = 0; j < height; j++)
                {

                    cells[i, j].liveNeighbors = 0;

                    if (prevCells[(i + width - 1) % width, (j + height - 1) % height].isAlive)
                        cells[i, j].liveNeighbors++;

                    if (prevCells[(i + width - 1) % width, j].isAlive)
                        cells[i, j].liveNeighbors++;

                    if (prevCells[(i + width - 1) % width, (j + 1) % height].isAlive)
                        cells[i, j].liveNeighbors++;

                    if (prevCells[i, (j + height - 1) % height].isAlive)
                        cells[i, j].liveNeighbors++;

                    if (prevCells[i, (j + 1) % height].isAlive)
                        cells[i, j].liveNeighbors++;

                    if (prevCells[(i + 1) % width, (j + height - 1) % height].isAlive)
                        cells[i, j].liveNeighbors++;

                    if (prevCells[(i + 1) % width, j].isAlive)
                        cells[i, j].liveNeighbors++;

                    if (prevCells[(i + 1) % width, (j + 1) % height].isAlive)
                        cells[i, j].liveNeighbors++;


                    if (cells[i, j].isAlive && (cells[i,j].liveNeighbors < 2 || cells[i, j].liveNeighbors > 3))//Kills cells that don't have 2 or 3 neighbors
                    {
                        cells[i, j].isAlive = false;
                        rectangles[i, j].Fill = Brushes.White;
                    }
                    if (!(cells[i, j].isAlive) && cells[i, j].liveNeighbors == 3)//Revives cells with 3 neighbors
                    {
                        cells[i, j].isAlive = true;
                        rectangles[i, j].Fill = Brushes.Black;
                    }

                }
            }
            for (int i = 0; i < width; i++)//Updates cells copy
            {
                for (int j = 0; j < height; j++)
                {
                    prevCells[i, j] = cells[i, j];
                }
            }

            Cell.Generation++;
            generationLabel.Content = $"Generation: {Cell.Generation}";
        }

        private void ToggleLife(object sender, MouseButtonEventArgs e)//Adds or removes life on click
        {
            //Gets mouse position and converts it to grid position
            int x = Convert.ToInt32(e.GetPosition(this).X - marginX);
            int y = Convert.ToInt32(e.GetPosition(this).Y - marginY);
            x -= Mod(x, size);
            x /= size;
            y -= Mod(y, size);
            y /= size;

            if (x >= 0 && x < width && y >=0 && y < height)//Checks if click is made within grid boundries and toggles life accordingly
            {
                if (cells[x,y].isAlive)
                {
                    cells[x, y].isAlive = false;
                    rectangles[x, y].Fill = Brushes.White;
                }
                else
                {
                    cells[x, y].isAlive = true;
                    rectangles[x, y].Fill = Brushes.Black;
                }
            }
        }

        private void GameButton_Click(object sender, RoutedEventArgs e)//Starts or stops game timer
        {
            if ((string)gameButton.Content == "Start")
            {
                gameTimer.Start();
                gameButton.Content = "Stop";
            }
            else
            {
                gameTimer.Stop();
                gameButton.Content = "Start";
            }
        }

        private void RandomSeedButton_Click(object sender, RoutedEventArgs e)//Randomly adds or removes life
        {
            Random random = new Random();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (random.Next(2) == 0)
                    {
                        cells[i, j].isAlive = true;
                        rectangles[i, j].Fill = Brushes.Black;
                    }
                    else
                    {
                        cells[i, j].isAlive = false;
                        rectangles[i, j].Fill = Brushes.White;
                    }

                }
            }

        }

        private void ClearCanvas(object sender, RoutedEventArgs e)//Kills all life and stops timer
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    cells[i, j].isAlive = false;
                    rectangles[i, j].Fill = Brushes.White;

                }
            }
            Cell.Generation = 0;
            generationLabel.Content = "Generation: 0";
            if ((string)gameButton.Content == "Stop")
            {
                gameTimer.Stop();
                gameButton.Content = "Start";
            }
        }

        private int Mod(int dividend, int divisor)//True modulo function
        {
            if (dividend >= 0)
                return dividend % divisor;
            else
                return divisor - (Math.Abs(dividend) % divisor);
        }


        private void SpeedChange(object sender, RoutedPropertyChangedEventArgs<double> e)//Slider logic for changing speed
        {
            switch (speedSlider.Value)
            {
                case 0:
                    gameTimer.Interval = TimeSpan.FromMilliseconds(350);
                    break;
                case 1:
                    gameTimer.Interval = TimeSpan.FromMilliseconds(300);
                    break;
                case 2:
                    gameTimer.Interval = TimeSpan.FromMilliseconds(250);
                    break;
                case 3:
                    gameTimer.Interval = TimeSpan.FromMilliseconds(200);
                    break;
                case 4:
                    gameTimer.Interval = TimeSpan.FromMilliseconds(150);
                    break;
                case 5:
                    gameTimer.Interval = TimeSpan.FromMilliseconds(100);
                    break;
                case 6:
                    gameTimer.Interval = TimeSpan.FromMilliseconds(50);
                    break;
            }
        }
    }
}
