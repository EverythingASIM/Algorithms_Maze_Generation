using System;
using System.Collections.Generic;
using System.Text;


namespace asim.unity.generation.maze
{
    /// <summary>
    /// http://weblog.jamisbuck.org/2011/1/24/maze-generation-hunt-and-kill-algorithm
    /// </summary>
    public class HuntKillMaze : Maze
    {
        MazeCell[,] cells;
        public override void CreateMaze(int rowNum, int colNum)
        {
            if (rowNum == 0 || colNum == 0) return;

            //1. initizlie starting cells
            cells = new MazeCell[rowNum, colNum];
            for (int i = 0; i < rowNum; i++)
            {
                for (int j = 0; j < colNum; j++)
                {
                    cells[i, j] = MazeCell.Initial;
                }
            }

            //2. choose a random cell and mark it as visited
            int currentX = UnityEngine.Random.Range(0, rowNum);
            int currentY = UnityEngine.Random.Range(0, colNum);
            cells[currentX, currentY] |= MazeCell.Visited;

            Tuple<int, int> firstCellWithUnivistedNeighbours = new Tuple<int, int>(currentX, currentY);
            do
            {
                currentX = firstCellWithUnivistedNeighbours.Item1;
                currentY = firstCellWithUnivistedNeighbours.Item2;

                //3. select a random non visited neighbour 
                List<Tuple<int, int>> nonVisitedNeighbours = GetCellNonVisitedNeighbours(currentX, currentY);
                do
                {
                    Tuple<int, int> nextwalk = nonVisitedNeighbours[UnityEngine.Random.Range(0, nonVisitedNeighbours.Count)];

                    //4. carve a path of current and the new cell and mark the new cell as visited
                    int nextX = nextwalk.Item1;
                    int nextY = nextwalk.Item2;
                    MazeCell direction = GetDirection(currentX, currentY, nextX, nextY);
                    cells[currentX, currentY] -= direction;
                    cells[nextX, nextY] -= direction.OppositeWall();
                    cells[nextX, nextY] |= MazeCell.Visited;

                    //5. set current as  the new cell
                    currentX = nextX;
                    currentY = nextY;

                    //6 repeat until there are no visited neighbours
                    nonVisitedNeighbours = GetCellNonVisitedNeighbours(currentX, currentY);

                } while (nonVisitedNeighbours.Count > 0);

                //Scan row by row to look for visited cells, that has unvisited neighbours or hunt()
                firstCellWithUnivistedNeighbours = hunt();

            } while (firstCellWithUnivistedNeighbours != null);
        }

        List<Tuple<int, int>> GetCellNonVisitedNeighbours(int x, int y)
        {
            if (x < 0 || y < 0 || x > cells.GetLength(0) || y > cells.GetLength(1)) return null;

            List<Tuple<int, int>> neighbours = new List<Tuple<int, int>>();

            if (x > 0)
            {
                if (!cells[x - 1, y].hasflag(MazeCell.Visited))
                    neighbours.Add(new Tuple<int, int>(x - 1, y));
            }
            if (x < cells.GetLength(0) - 1)
            {
                if (!cells[x + 1, y].hasflag(MazeCell.Visited))
                    neighbours.Add(new Tuple<int, int>(x + 1, y));
            }
            if (y > 0)
            {
                if (!cells[x, y - 1].hasflag(MazeCell.Visited))
                    neighbours.Add(new Tuple<int, int>(x, y - 1));

            }
            if (y < cells.GetLength(1) - 1)
            {
                if (!cells[x, y + 1].hasflag(MazeCell.Visited))
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
        Tuple<int, int> hunt()
        {
            Tuple<int, int> firstCellWithUnivistedNeighbours = null;
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    if (cells[i, j].hasflag(MazeCell.Visited))
                    {
                        var nonVisitedNeighbours = GetCellNonVisitedNeighbours(i, j);
                        if (nonVisitedNeighbours.Count > 0)
                        {
                            firstCellWithUnivistedNeighbours = new Tuple<int, int>(i, j);
                            break;
                        }
                    }
                }
            }
            return firstCellWithUnivistedNeighbours;
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
                    sbTop.Append(cells[i, j].hasflag(MazeCell.TopWall) ? "##" : "#-");
                    sbMid.Append(cells[i, j].hasflag(MazeCell.LeftWall) ? "#-" : "--");
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
   
