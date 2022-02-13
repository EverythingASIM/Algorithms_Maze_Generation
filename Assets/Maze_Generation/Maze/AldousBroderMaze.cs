using System;
using System.Text;
using System.Collections.Generic;

using asim.unity.extensions;

namespace asim.unity.generation.maze
{
    /// <summary>
    /// http://weblog.jamisbuck.org/2011/1/17/maze-generation-aldous-broder-algorithm
    /// </summary>
    public class AldousBroderMaze : Maze
    {
        MazeCell[,] cells;

        public override void CreateMaze(int rowNum, int colNum)
        {
            if (rowNum == 0 || colNum == 0) return;

            //1. Initilize Cells
            cells = new MazeCell[rowNum, colNum];
            for (int i = 0; i < rowNum; i++)
            {
                for (int j = 0; j < colNum; j++)
                {
                    cells[i, j] = MazeCell.Initial;
                }
            }

            //2. Choose a random cell and set cell to be visited
            int currentX = UnityEngine.Random.Range(0, rowNum);
            int currentY = UnityEngine.Random.Range(0, colNum);
            cells[currentX, currentY] |= MazeCell.Visited;

            int cellsLeftToVisit = rowNum * colNum - 1;
            while (cellsLeftToVisit > 0)
            {
                //3. From the current cell select a random neighbour cell
                List<Tuple<int, int>> neighbours = GetNeighbours(currentX, currentY);
                int randomI = UnityEngine.Random.Range(0, neighbours.Count);
                int nextX = neighbours[randomI].Item1;
                int nextY = neighbours[randomI].Item2;
                MazeCell randomNeighbour = cells[nextX, nextY];

                //4. if unvisited, carve a path and mark as visited
                if (!randomNeighbour.hasflag(MazeCell.Visited))
                {
                    MazeCell direction = GetDirection(currentX, currentY, nextX, nextY);
                    cells[currentX, currentY] -= direction;
                    cells[nextX, nextY] -= direction.OppositeWall();
                    cells[nextX, nextY] |= MazeCell.Visited;

                    cellsLeftToVisit--;
                }

                //5. walk to the neighbouring cell
                currentX = nextX;
                currentY = nextY;
            }
        }

        List<Tuple<int,int>> GetNeighbours(int x,int y)
        {
            if (x < 0 || y < 0 || x > cells.GetLength(0) || y > cells.GetLength(1)) return null;

            List<Tuple<int, int>> neighbours = new List<Tuple<int, int>>();

            if (x > 0)
            {
                neighbours.Add(new Tuple<int, int>(x - 1, y));
            }
            if (x < cells.GetLength(0) - 1)
            {
                 neighbours.Add(new Tuple<int, int>(x + 1, y));
            }
            if (y > 0)
            {
                neighbours.Add(new Tuple<int, int>(x, y - 1));

            }
            if (y < cells.GetLength(1) - 1)
            {
                neighbours.Add(new Tuple<int, int>(x, y + 1));
            }

            return neighbours;
        }
        MazeCell GetDirection(int x, int y, int x2, int y2)
        {
            MazeCell dir;
            if (x < x2)
            {
                dir = MazeCell.BottomWall;
            }
            else if (x > x2)
            {
                dir = MazeCell.TopWall;
            }
            else if (y < y2)
            {
                dir = MazeCell.RightWall;
            }
            else// if (y > y2)
            {
                dir = MazeCell.LeftWall;
            }
            return dir;
        }

        public override char[][] GetMaze()
        {
            if (cells == null) return null;

            char[][] Mazedata = new char[cells.GetLength(0) * 2 + 1][];
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                StringBuilder sbTop = new StringBuilder();
                StringBuilder sbMid = new StringBuilder();
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    sbTop.Append(cells[i,j].hasflag(MazeCell.TopWall) ? "##" : "#-");
                    sbMid.Append(cells[i,j].hasflag(MazeCell.LeftWall) ? "#-" : "--");
                }
                sbTop.Append("#");
                sbMid.Append("#");
                Mazedata[2 * i] = sbTop.ToString().ToCharArray();
                Mazedata[1 + 2 * i] = sbMid.ToString().ToCharArray();
                if (i == 0)
                {
                    Mazedata[2 * cells.GetLength(0)] = sbTop.ToString().ToCharArray();
                }
            }
            return Mazedata;
        }
    }
}