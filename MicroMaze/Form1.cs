﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace MicroMaze
{
    public partial class Form1 : Form
    {

        private int[,] maze;
        Random rand = new Random();
        private int mouseX = 0;
        private int mouseY = 0;
        private int exitX = 0;
        private int exitY = 0;
        private const int MOUSE_SIZE = 30;
        const int CELL_SIZE = 30; // Adjust size as needed
        const int LINE_THICKNESS = 2;
        Panel panelMouse = new Panel();
        Panel panelExit = new Panel();

        private CancellationTokenSource resetCts;
        private Dictionary<string, Func<int, int, int[,]>> mazeGenerationMethods;
        private Dictionary<string, Func<int, int, bool[,], Task>> mazeSolvingMethods;



        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        public Form1()
        {
            InitializeComponent();
            setStartingValues();
            this.panelMaze.Paint += new PaintEventHandler(panelMaze_Paint);

            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }
        }

        private void setStartingValues()
        {
            mazeWidth.Text = "20";
            mazeHeight.Text = "20";

            //Maz Generation Methods
            mazeGenerationMethods = new Dictionary<string, Func<int, int, int[,]>>();
            mazeGenerationMethods.Add("Kruskal's Algorithm", GenerateMazeKruskal);
            mazeGenerationMethods.Add("Recursive Backtracker", GenerateMazeRecursiveBacktracker);
            mazeGenerationMethods.Add("Prims Algorithm", GenerateMazePrims);
            cboxMazeTypes.Items.AddRange(mazeGenerationMethods.Keys.ToArray());


            //maze solve methods
            mazeSolvingMethods = new Dictionary<string, Func<int, int, bool[,], Task>>();
            mazeSolvingMethods.Add("A*", AStar);  // Assumes AStar has signature: (int, int, bool[,]) => Task
            mazeSolvingMethods.Add("DFS", DFS);  // Assumes DFS has signature: (int, int, bool[,]) => Task
            mazeSolvingMethods.Add("BFS", BFS);  // Assumes BFS has signature: (int, int, bool[,]) => Task
            cboxSolveMethod.Items.AddRange(mazeSolvingMethods.Keys.ToArray());



            cboxSolveMethod.SelectedIndex = 0;
            cboxMazeTypes.SelectedIndex = 0;
        }




        /***********************************DRAW MAZE*******************************************/
        /***************************************************************************************/
        private void DrawMaze(Graphics g, int[,] maze)
        {
            int numRows = maze.GetLength(0);
            int numCols = maze.GetLength(1);

            using (Pen pen = new Pen(Color.Black, LINE_THICKNESS))
            {
                for (int row = 0; row < numRows; row++)
                {
                    for (int col = 0; col < numCols; col++)
                    {
                        int topLeftX = col * CELL_SIZE;
                        int topLeftY = row * CELL_SIZE;

                        // If this cell is blocked, fill it
                        if (maze[row, col] == 1)
                        {
                            g.FillRectangle(Brushes.Black, topLeftX, topLeftY, CELL_SIZE, CELL_SIZE);
                        }

                        // Draw grid lines for clarity
                        g.DrawRectangle(pen, topLeftX, topLeftY, CELL_SIZE, CELL_SIZE);
                    }
                }
            }
        }

        private void panelMaze_Paint(object sender, PaintEventArgs e)
        {
            if (maze != null) // Make sure maze is not null
            {
                DrawMaze(e.Graphics, maze);
            }
        }

        /***************************************************************************************/
        /***************************************************************************************/





        /********************************Generate Maze Alogirthms*******************************/
        /***************************************************************************************/

        /***************************************************************************************/
        /*Recursive Backtracker*/
        /***************************************************************************************/

        private int[,] GenerateMazeRecursiveBacktracker(int width, int height)
        {
            // Create a maze full of walls
            int[,] maze = new int[height, width];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    maze[y, x] = 1;

            // Pick a random starting cell
            int startX = rand.Next(width);
            int startY = rand.Next(height);

            // Start the recursive method
            GenerateCell(maze, startX, startY, width, height);

            return maze;
        }

        private void GenerateCell(int[,] maze, int x, int y, int width, int height)
        {
            // Mark the current cell as visited
            maze[y, x] = 0;

            // Direction offsets for: up, right, down, left
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { -1, 0, 1, 0 };

            // Randomly order the directions
            int[] directions = new int[4];
            for (int i = 0; i < 4; i++)
            {
                int j = rand.Next(i + 1);
                if (i != j)
                    directions[i] = directions[j];
                directions[j] = i;
            }

            // Visit each direction
            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[directions[i]], ny = y + dy[directions[i]];
                int wx = nx + dx[directions[i]], wy = ny + dy[directions[i]];

                if (wx >= 0 && wy >= 0 && wx < width && wy < height && maze[wy, wx] == 1)
                {
                    maze[ny, nx] = 0;
                    GenerateCell(maze, wx, wy, width, height);
                }
            }
        }


        /***************************************************************************************/
        /*Prims Algorithm*/
        /***************************************************************************************/

        private int[,] GenerateMazePrims(int width, int height)
        {
            // Create a maze full of walls
            int[,] maze = new int[height, width];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    maze[y, x] = 1;

            // Pick a random starting cell
            int startX = rand.Next(width);
            int startY = rand.Next(height);

            // List of all frontier cells
            List<Tuple<int, int>> frontiers = new List<Tuple<int, int>>();

            // Mark the starting cell as visited and add its frontier cells
            maze[startY, startX] = 0;
            AddFrontiers(maze, startX, startY, width, height, frontiers);

            while (frontiers.Count > 0)
            {
                // Pick a random frontier cell
                var idx = rand.Next(frontiers.Count);
                var frontier = frontiers[idx];
                frontiers.RemoveAt(idx);

                // List all neighbors of the frontier cell that are in the maze
                List<Tuple<int, int>> neighbors = new List<Tuple<int, int>>();
                if (frontier.Item2 > 1 && maze[frontier.Item2 - 2, frontier.Item1] == 0)
                    neighbors.Add(new Tuple<int, int>(0, -1));
                if (frontier.Item2 < height - 2 && maze[frontier.Item2 + 2, frontier.Item1] == 0)
                    neighbors.Add(new Tuple<int, int>(0, 1));
                if (frontier.Item1 > 1 && maze[frontier.Item2, frontier.Item1 - 2] == 0)
                    neighbors.Add(new Tuple<int, int>(-1, 0));
                if (frontier.Item1 < width - 2 && maze[frontier.Item2, frontier.Item1 + 2] == 0)
                    neighbors.Add(new Tuple<int, int>(1, 0));

                if (neighbors.Count > 0)
                {
                    // Connect the frontier cell to a random neighbor
                    var dir = neighbors[rand.Next(neighbors.Count)];
                    maze[frontier.Item2 + dir.Item2, frontier.Item1 + dir.Item1] = 0;
                    maze[frontier.Item2, frontier.Item1] = 0;

                    // Add new frontier cells
                    AddFrontiers(maze, frontier.Item1, frontier.Item2, width, height, frontiers);
                }
            }

            return maze;
        }

        private void AddFrontiers(int[,] maze, int x, int y, int width, int height, List<Tuple<int, int>> frontiers)
        {
            if (y > 1 && maze[y - 2, x] == 1)
                frontiers.Add(new Tuple<int, int>(x, y - 2));
            if (y < height - 2 && maze[y + 2, x] == 1)
                frontiers.Add(new Tuple<int, int>(x, y + 2));
            if (x > 1 && maze[y, x - 2] == 1)
                frontiers.Add(new Tuple<int, int>(x - 2, y));
            if (x < width - 2 && maze[y, x + 2] == 1)
                frontiers.Add(new Tuple<int, int>(x + 2, y));
        }

        /***************************************************************************************/
        /*Kruskal's Algorithm*/
        /***************************************************************************************/

        public int[,] GenerateMazeKruskal(int width, int height)
        {
            // Initialize maze with walls
            int[,] maze = new int[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    maze[y, x] = 1;
                }
            }

            // Initialize disjoint set
            int cellCount = width * height;
            DisjointSet disjointSet = new DisjointSet(cellCount);

            // Initialize wall list and shuffle it
            List<Tuple<int, int, int, int>> walls = new List<Tuple<int, int, int, int>>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y > 0)
                    {
                        walls.Add(Tuple.Create(y, x, y - 1, x));  // vertical wall
                    }
                    if (x > 0)
                    {
                        walls.Add(Tuple.Create(y, x, y, x - 1));  // horizontal wall
                    }
                }
            }
            Shuffle(walls);

            // Iterate over walls
            foreach (var wall in walls)
            {
                // If the cells divided by the wall belong to distinct sets
                if (disjointSet.Find(wall.Item1 * width + wall.Item2) != disjointSet.Find(wall.Item3 * width + wall.Item4))
                {
                    // Remove the wall and join the sets
                    maze[(wall.Item1 + wall.Item3) / 2, (wall.Item2 + wall.Item4) / 2] = 0;
                    disjointSet.Union(wall.Item1 * width + wall.Item2, wall.Item3 * width + wall.Item4);
                }
            }

            return maze;
        }

        private void Shuffle<T>(List<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public class DisjointSet
        {
            private int[] parent;
            private int[] rank;

            public DisjointSet(int count)
            {
                parent = new int[count];
                rank = new int[count];

                for (int i = 0; i < count; i++)
                {
                    parent[i] = i;
                    rank[i] = 0;
                }
            }

            public int Find(int i)
            {
                if (i != parent[i])
                {
                    parent[i] = Find(parent[i]);  // path compression
                }
                return parent[i];
            }

            public void Union(int i, int j)
            {
                int ri = Find(i), rj = Find(j);

                if (ri != rj)
                {
                    // Make the root of the smaller rank point to the root of the larger rank
                    if (rank[ri] < rank[rj])
                    {
                        parent[ri] = rj;
                    }
                    else if (rank[ri] > rank[rj])
                    {
                        parent[rj] = ri;
                    }
                    else  // ranks are equal
                    {
                        parent[ri] = rj;
                        rank[rj]++;
                    }
                }
            }
        }


        /***************************************************************************************/
        /***************************************************************************************/


        /*********************************AI MOVEMENT*******************************************/
        /***************************************************************************************/

        private async void MoveMouseAI()
        {
            // Reset the player's position
            panelMouse.Location = new Point(mouseX * CELL_SIZE, mouseY * CELL_SIZE);

            // Reset the CancellationTokenSource
            resetCts?.Cancel();
            resetCts = new CancellationTokenSource();


            //common configs
            bool[,] visited = new bool[maze.GetLength(0), maze.GetLength(1)];

            string selectedMethod = cboxSolveMethod.Text;
            if (mazeSolvingMethods.TryGetValue(selectedMethod, out Func<int, int, bool[,], Task> solver))
            {
                await solver(mouseX, mouseY, visited);
            }
            else
            {
                // Handle the case where no valid method was selected
                // For example, use a default solver
                await DFS(mouseX, mouseY, visited);
            }
        }

        private async Task AStar(int startX, int startY, bool[,] visited)
        {
            // Priority queue to select the next node with the smallest cost
            var openSet = new SortedDictionary<double, Tuple<int, int>>();
            var cameFrom = new Dictionary<Tuple<int, int>, Tuple<int, int>>();
            var gScore = new Dictionary<Tuple<int, int>, double>();
            var fScore = new Dictionary<Tuple<int, int>, double>();

            var start = new Tuple<int, int>(startX, startY);
            openSet[0] = start;
            gScore[start] = 0;
            fScore[start] = HeuristicCostEstimate(startX, startY, exitX, exitY);

            while (openSet.Count > 0)
            {
                var current = openSet.First().Value;
                if (current.Item1 == exitX && current.Item2 == exitY)
                {
                    await ReconstructPath(cameFrom, current);
                    return;
                }

                openSet.Remove(openSet.First().Key);
                visited[current.Item2, current.Item1] = true;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        int newX = current.Item1 + i;
                        int newY = current.Item2 + j;
                        if (newX >= 0 && newY >= 0 && newX < maze.GetLength(1) && newY < maze.GetLength(0) && Math.Abs(i + j) == 1)
                        {
                            if (maze[newY, newX] != 1)
                            {
                                var neighbor = new Tuple<int, int>(newX, newY);
                                double tentativeGScore = gScore[current] + 1;

                                if (visited[newY, newX] && tentativeGScore >= gScore[neighbor])
                                {
                                    continue;
                                }

                                if (!visited[newY, newX] || tentativeGScore < gScore[neighbor])
                                {
                                    cameFrom[neighbor] = current;
                                    gScore[neighbor] = tentativeGScore;
                                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(newX, newY, exitX, exitY);
                                    if (!visited[newY, newX])
                                    {
                                        openSet[fScore[neighbor]] = neighbor;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private double HeuristicCostEstimate(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        private async Task ReconstructPath(Dictionary<Tuple<int, int>, Tuple<int, int>> cameFrom, Tuple<int, int> current)
        {
            var totalPath = new List<Tuple<int, int>>();
            totalPath.Add(current);
            while (cameFrom.Keys.Contains(current))
            {
                current = cameFrom[current];
                totalPath.Add(current);

                // Update the player position and delay 
                panelMouse.Location = new Point(current.Item1 * CELL_SIZE, current.Item2 * CELL_SIZE);
                await Task.Delay(100); // adjust delay as needed
            }
        }

        private async Task DFS(int x, int y, bool[,] visited)
        {
            Stack<Tuple<int, int>> path = new Stack<Tuple<int, int>>();
            await DFS(x, y, visited, path);
        }

        private async Task<bool> DFS(int x, int y, bool[,] visited, Stack<Tuple<int, int>> path)
        {
            // Check if out of bounds or visited or is a wall
            if (x < 0 || y < 0 || x >= maze.GetLength(1) || y >= maze.GetLength(0) || visited[y, x] || maze[y, x] == 1)
            {
                return false;
            }

            // Mark as visited
            visited[y, x] = true;

            // Add position to the path
            path.Push(new Tuple<int, int>(x, y));

            // Update the player position and delay 
            panelMouse.Location = new Point(x * CELL_SIZE, y * CELL_SIZE);
            await Task.Delay(100); // adjust delay as needed

            //check if reset is requested
            if (resetCts.Token.IsCancellationRequested)
            {
                // If reset is requested, don't resume movement
                return false;
            }

            // Check if reached exit
            if (x == exitX && y == exitY)
            {
                return true;
            }

            // Move in each direction
            if (await DFS(x + 1, y, visited, path) // right
                || await DFS(x, y + 1, visited, path) // down
                || await DFS(x - 1, y, visited, path) // left
                || await DFS(x, y - 1, visited, path)) // up
            {
                return true;
            }

            // Check again if reset is requested before backtracking
            if (resetCts.Token.IsCancellationRequested)
            {
                // If reset is requested, don't resume movement
                return false;
            }

            // If none of the directions leads to the exit, remove the position from the path (backtrack) and update the player's position
            path.Pop();
            if (path.Count > 0)
            {
                var prevPos = path.Peek();
                panelMouse.Location = new Point(prevPos.Item1 * CELL_SIZE, prevPos.Item2 * CELL_SIZE);
                await Task.Delay(100); // adjust delay as needed
            }
            return false;
            
        }

        private async Task BFS(int x, int y, bool[,] visited)
        {
            Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
            await BFS(x, y, visited, queue);
        }

        private async Task BFS(int x, int y, bool[,] visited, Queue<Tuple<int, int>> queue)
        {
            // The directions array
            int[,] dirs = { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } }; // right, down, left, up

            // Add the starting position to the queue and mark it as visited
            queue.Enqueue(new Tuple<int, int>(x, y));
            visited[y, x] = true;

            while (queue.Count > 0)
            {
                // Check if reset is requested
                if (resetCts.Token.IsCancellationRequested)
                {
                    // If reset is requested, don't resume movement
                    return;
                }

                // Dequeue the current cell
                var cell = queue.Dequeue();

                // Update the player position and delay 
                panelMouse.Location = new Point(cell.Item1 * CELL_SIZE, cell.Item2 * CELL_SIZE);
                await Task.Delay(100); // adjust delay as needed

                // If the exit is found
                if (cell.Item1 == exitX && cell.Item2 == exitY)
                {
                    return;
                }

                // Enqueue all valid and unvisited neighbors
                for (int i = 0; i < 4; i++)
                {
                    int nx = cell.Item1 + dirs[i, 0];
                    int ny = cell.Item2 + dirs[i, 1];

                    if (nx >= 0 && ny >= 0 && nx < maze.GetLength(1) && ny < maze.GetLength(0) && !visited[ny, nx] && maze[ny, nx] != 1)
                    {
                        queue.Enqueue(new Tuple<int, int>(nx, ny));
                        visited[ny, nx] = true;
                    }
                }
            }
        }

        /***************************************************************************************/
        /***************************************************************************************/


        /***************************************OTHERS******************************************/
        /***************************************************************************************/

        private bool validateStartMazeFields()
        {
            bool valid = true;

            //validation tasks list


            return valid;
        }

        private bool validateGenerateMazeFields()
        {
            bool valid = true;

            //validation tasks list


            return valid;
        }


        /***************************************BUTTONS*****************************************/
        /***************************************************************************************/

        private async void btnNewMaze_Click(object sender, EventArgs e)
        {

            if (validateGenerateMazeFields() == true)
            {
                int width = Int32.Parse(mazeWidth.Text);
                int height = Int32.Parse(mazeHeight.Text);

                // Generate the maze

                string selectedMethod = cboxMazeTypes.Text;
                if (mazeGenerationMethods.TryGetValue(selectedMethod, out Func<int, int, int[,]> generator))
                {
                    maze = await Task.Run(() => generator(width, height));
                }
                else
                {
                    // Handle the case where no valid method was selected
                    // For example, use a default generator
                    maze = await Task.Run(() => GenerateMazeRecursiveBacktracker(width, height));
                }

                do
                {
                    mouseX = rand.Next(width);
                    mouseY = rand.Next(height);
                } while (maze[mouseY, mouseX] == 1);

                // Find a random empty cell for the exit
                do
                {
                    exitX = rand.Next(width);
                    exitY = rand.Next(height);
                } while (maze[exitY, exitX] == 1 || (exitX == mouseX && exitY == mouseY));

                // Add the mouse panel to the maze panel
                panelMouse.BackColor = Color.Red;
                panelMouse.Size = new Size(CELL_SIZE, CELL_SIZE);
                panelMouse.Location = new Point(mouseX * CELL_SIZE, mouseY * CELL_SIZE);
                panelMaze.Controls.Add(panelMouse);

                // Add the exit panel to the maze panel
                panelExit.BackColor = Color.Green;
                panelExit.Size = new Size(CELL_SIZE, CELL_SIZE);
                panelExit.Location = new Point(exitX * CELL_SIZE, exitY * CELL_SIZE);
                panelMaze.Controls.Add(panelExit);

                // Redraw the maze
                panelMaze.Invalidate();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (validateStartMazeFields() == true)
            {

                // Reset the reset token
                //resetCts = new CancellationTokenSource();


                // Start the AI movement
                MoveMouseAI();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Cancel the ongoing BFS/DFS
            resetCts?.Cancel();

            // Reset the player's position
            panelMouse.Location = new Point(mouseX * CELL_SIZE, mouseY * CELL_SIZE);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        /***************************************************************************************/
        /***************************************************************************************/
    }
}
