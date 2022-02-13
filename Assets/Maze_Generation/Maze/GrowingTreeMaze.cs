using System;
using System.Text;
using System.Collections.Generic;

namespace asim.unity.generation.maze
{
    /// <summary>
    /// http://weblog.jamisbuck.org/2011/1/27/maze-generation-growing-tree-algorithm
    /// </summary>
    public class GrowingTreeMaze : Maze
    {
        MazeCell[,] cells;

        public override void CreateMaze(int rowNum, int colNum)
        {
            if (rowNum == 0 || colNum == 0) return;

            //1. Initilize Cells and keep a seperate list of non visited cells
            cells = new MazeCell[rowNum, colNum];
            for (int i = 0; i < rowNum; i++)
            {
                for (int j = 0; j < colNum; j++)
                {
                    cells[i, j] = MazeCell.Initial;
                }
            }

            //1. keep a list of all visited cells
            List<Tuple<int, int>> VisitedCells = new List<Tuple<int, int>>();

            //2. select a random cell, and mark it as visited
            int randomX = UnityEngine.Random.Range(0, rowNum);
            int randomY = UnityEngine.Random.Range(0, colNum);
            cells[randomX, randomY] |= MazeCell.Visited;

            //3. add it to visited list 
            VisitedCells.Add(new Tuple<int, int>(randomX, randomY));

            //4. select a cell in visited cells, either randomly, by newest, or however variaition
            while (VisitedCells.Count > 0)
            {
                //select random cell
                Tuple<int, int> cell = VisitedCells[UnityEngine.Random.Range(0, VisitedCells.Count)];

                //select oldest cell 
                //Tuple<int, int> cell = VisitedCells[0];

                //select newest cell 
                //Tuple<int, int> cell = VisitedCells[0];

                int currentx = cell.Item1;
                int currenty = cell.Item2;


                List<Tuple<int, int>> nonVisitedNeighbours = GetCellNonVisitedNeighbours(currentx, currenty);
                if(nonVisitedNeighbours.Count > 0)
                {
                    //5. check for nonvisited neighbours, choose one random and carve a path and add it to visited list
                    Tuple<int, int> nextcell = nonVisitedNeighbours[UnityEngine.Random.Range(0, nonVisitedNeighbours.Count)];
                    int nextx = nextcell.Item1;
                    int nexty = nextcell.Item2;
                    MazeCell direction = GetDirection(currentx, currenty, nextx, nexty);

                    cells[currentx, currenty] -= direction;
                    cells[nextx, nexty] -= direction.OppositeWall();
                    cells[nextx, nexty] |= MazeCell.Visited;
                    VisitedCells.Add(nextcell);
                }
                else
                {
                    //6. no non visited cell, remove it from visited list
                    VisitedCells.Remove(new Tuple<int, int>(currentx, currenty));
                }
            }
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