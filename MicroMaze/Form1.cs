using System;
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

        private CancellationTokenSource cts = new CancellationTokenSource();
        private CancellationTokenSource resetCts = new CancellationTokenSource();


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
            mazeWidth.Text = "56";
            mazeHeight.Text = "33";


            List<string> mazeTypes = new List<string>();
            mazeTypes.Add("Recursive Backtracker");
            mazeTypes.Add("Prims Algorithm");

            for (int i = 0; i < mazeTypes.Count; i++)
            {
                cboxMazeTypes.Items.Add(mazeTypes[i]);
            }

            List<string> solveMethods = new List<string>();
            solveMethods.Add("DFS");
            solveMethods.Add("BFS");

            for (int i = 0; i < solveMethods.Count; i++)
            {
                cboxSolveMethod.Items.Add(solveMethods[i]);
            }



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

        private int[,] GenerateMazePrim(int width, int height)
        {
            // Create a maze full of walls
            int[,] maze = new int[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    maze[y, x] = 1;
                }
            }

            // Pick a random starting cell
            int startX = rand.Next(width);
            int startY = rand.Next(height);

            // Mark the starting cell as part of the maze
            maze[startY, startX] = 0;

            // Create a list to store the frontier cells
            List<(int, int)> frontier = new List<(int, int)>();
            AddFrontierCells(frontier, maze, startX, startY, width, height);

            // While there are frontier cells
            while (frontier.Count > 0)
            {
                // Pick a random frontier cell
                int randomIndex = rand.Next(frontier.Count);
                (int fx, int fy) = frontier[randomIndex];
                frontier.RemoveAt(randomIndex);

                // Find the neighboring cells
                List<(int, int)> neighbors = GetNeighbors(fx, fy, width, height);

                // Count the number of cells in the maze
                int mazeCellCount = 0;
                foreach ((int nx, int ny) in neighbors)
                {
                    if (maze[ny, nx] == 0)
                    {
                        mazeCellCount++;
                    }
                }

                // If the frontier cell has exactly one neighboring cell in the maze
                if (mazeCellCount == 1)
                {
                    // Add the frontier cell to the maze
                    maze[fy, fx] = 0;

                    // Add the neighboring cells to the frontier list
                    AddFrontierCells(frontier, maze, fx, fy, width, height);
                }
            }

            return maze;
        }

        private void AddFrontierCells(List<(int, int)> frontier, int[,] maze, int x, int y, int width, int height)
        {
            List<(int, int)> neighbors = GetNeighbors(x, y, width, height);
            foreach ((int nx, int ny) in neighbors)
            {
                if (maze[ny, nx] == 1)
                {
                    frontier.Add((nx, ny));
                    maze[ny, nx] = 2; // Mark frontier cells as "2" temporarily
                }
            }
        }

        private List<(int, int)> GetNeighbors(int x, int y, int width, int height)
        {
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { -1, 0, 1, 0 };

            List<(int, int)> neighbors = new List<(int, int)>();
            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i], ny = y + dy[i];
                if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                {
                    neighbors.Add((nx, ny));
                }
            }

            return neighbors;
        }

        /***************************************************************************************/
        /***************************************************************************************/


        /*********************************AI MOVEMENT*******************************************/
        /***************************************************************************************/

        private async void MoveMouseAI()
        {
            // Reset the player's position
            panelMouse.Location = new Point(mouseX * CELL_SIZE, mouseY * CELL_SIZE);

            switch (cboxSolveMethod.Text)
            {
                case "DFS":

                    // Create a visited matrix
                    bool[,] visited = new bool[maze.GetLength(0), maze.GetLength(1)];

                    // The path stack
                    Stack<Tuple<int, int>> path = new Stack<Tuple<int, int>>();

                    // Call DFS from player's position
                    await DFS(mouseX, mouseY, visited, path);

                    break;

                case "BFS":

                    // Call BFS from player's position
                    await BFS(mouseX, mouseY);

                    break;
            }
        }

        private async Task<bool> DFS(int x, int y, bool[,] visited, Stack<Tuple<int, int>> path)
        {
            if (resetCts.Token.IsCancellationRequested)
            {
                // If reset is requested, don't resume movement
                return true;
            }

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

        private async Task BFS(int startX, int startY)
        {
            // Create a visited matrix
            bool[,] visited = new bool[maze.GetLength(0), maze.GetLength(1)];

            // Create a dictionary to keep track of paths
            Dictionary<Tuple<int, int>, Tuple<int, int>> comeFrom = new Dictionary<Tuple<int, int>, Tuple<int, int>>();

            // Create a queue for BFS and enqueue start position
            Queue<Tuple<int, int>> queue = new Queue<Tuple<int, int>>();
            queue.Enqueue(new Tuple<int, int>(startX, startY));

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var x = current.Item1;
                var y = current.Item2;

                // Check if out of bounds or visited or is a wall
                if (x < 0 || y < 0 || x >= maze.GetLength(1) || y >= maze.GetLength(0) || visited[y, x] || maze[y, x] == 1)
                {
                    continue;
                }

                // Mark as visited
                visited[y, x] = true;

                // Update the player position and delay 
                panelMouse.Location = new Point(x * CELL_SIZE, y * CELL_SIZE);
                await Task.Delay(100); // adjust delay as needed

                // Check if reached exit
                if (x == exitX && y == exitY)
                {
                    // If the exit is found, backtrack along the path and update the player position
                    var path = new Stack<Tuple<int, int>>();
                    var node = current;

                    while (node != null)
                    {
                        path.Push(node);
                        comeFrom.TryGetValue(node, out node);
                    }

                    while (path.Count > 0)
                    {
                        var pos = path.Pop();
                        panelMouse.Location = new Point(pos.Item1 * CELL_SIZE, pos.Item2 * CELL_SIZE);
                        await Task.Delay(100); // adjust delay as needed
                    }

                    return;
                }

                // Enqueue neighbors
                var neighbors = new List<Tuple<int, int>>()
                {
                    new Tuple<int, int>(x + 1, y), // right
                    new Tuple<int, int>(x, y + 1), // down
                    new Tuple<int, int>(x - 1, y), // left
                    new Tuple<int, int>(x, y - 1), // up
                };

                foreach (var neighbor in neighbors)
                {
                    if (!visited[neighbor.Item2, neighbor.Item1] && maze[neighbor.Item2, neighbor.Item1] != 1)
                    {
                        comeFrom[neighbor] = current;
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        /***************************************************************************************/
        /***************************************************************************************/


        /***************************************BUTTONS*****************************************/
        /***************************************************************************************/

        private async void btnNewMaze_Click(object sender, EventArgs e)
        {
            int width = Int32.Parse(mazeWidth.Text);
            int height = Int32.Parse(mazeHeight.Text);

            // Generate the maze

            switch (cboxMazeTypes.Text)
            {
                case "Recursive Backtracker":
                    maze = await Task.Run(() => GenerateMazeRecursiveBacktracker(width, height));
                    break;

                case "Prims Algorithm":
                    maze = await Task.Run(() => GenerateMazePrim(width, height));
                    break;

                default:
                    maze = await Task.Run(() => GenerateMazeRecursiveBacktracker(width, height));
                    break;
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

        private void btnStart_Click(object sender, EventArgs e)
        {

            // Reset the reset token
            resetCts = new CancellationTokenSource();

            MoveMouseAI();
        }


        private void btnReset_Click(object sender, EventArgs e)
        {
            resetCts.Cancel();

            // Stop AI movement
            cts.Cancel();

            // Reset CancellationTokenSource
            cts = new CancellationTokenSource();

            // Set mouse starting position
            do
            {
                mouseX = rand.Next(maze.GetLength(1));
                mouseY = rand.Next(maze.GetLength(0));
            } while (maze[mouseY, mouseX] == 1);

            // Move mouse back to the start position
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
