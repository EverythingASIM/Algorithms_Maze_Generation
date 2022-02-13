using System;
using System.Text;
using System.Collections.Generic;

using asim.unity.extensions;

namespace asim.unity.generation.maze
{
    /// <summary>
    /// Randomize Prism based on a list of walls/edges
    /// </summary>
    public class RandomizePrimMaze : Maze
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

            //2. Start by choosing a random cell, mark it as visited and add its wall into a list;
            int randomrow = UnityEngine.Random.Range(0, rowNum);
            int randomcol = UnityEngine.Random.Range(0, colNum);
            cells[randomrow, randomcol] |= MazeCell.Visited;

            List<Tuple<int, int, MazeCell>> wallList = new List<Tuple<int, int, MazeCell>>();
            wallList.AddRange(GetCellWalls(randomrow, randomcol));

            //3. while theres still walls
            while(wallList.Count > 0)
            {
                //3a. pick a wall at random, 
                int randomWallI = UnityEngine.Random.Range(0, wallList.Count);
                Tuple<int, int, MazeCell> randomWall = wallList[randomWallI];

                //3b if the random picked wall has an unvisited celland set the cell to be visited and remove the wall to create a path
                int row = randomWall.Item1;
                int col = randomWall.Item2;
                MazeCell direction = randomWall.Item3;
                int row2 = row;
                int col2 = col;
                if (direction.hasflag(MazeCell.TopWall))
                {
                    row2 -= 1;
                }
                else if (direction.hasflag(MazeCell.BottomWall))
                {
                    row2 += 1;
                }
                else if (direction.hasflag(MazeCell.LeftWall))
                {
                    col2 -= 1;
                }
                else if (direction.hasflag(MazeCell.RightWall))
                {
                    col2 += 1;
                }

                if(!cells[row2, col2].hasflag(MazeCell.Visited))
                {
                    //3c. Remove walls, carve a path and Set wall as visited
                    cells[row, col] -= direction;
                    cells[row2, col2] -= direction.OppositeWall();
                    cells[row2, col2] |= MazeCell.Visited;

                    //3c. add the neighbouring walls fo the list
                    wallList.AddRange(GetCellWalls(row2, col2));
                }

                //3d. remove the current wall
                wallList.RemoveAt(randomWallI);
            }
        }

        List<Tuple<int, int, MazeCell>> GetCellWalls(int x,int y)
        {
            if (x < 0 || y < 0 || x > cells.GetLength(0) || y > cells.GetLength(1)) return null;

            List<Tuple<int, int, MazeCell>> walls = new List<Tuple<int, int, MazeCell>>();

            if (x > 0 && cells[x,y].hasflag(MazeCell.TopWall)) 
                walls.Add(new Tuple<int, int, MazeCell>(x, y, MazeCell.TopWall));

            if (x < cells.GetLength(0) - 1  && cells[x, y].hasflag(MazeCell.BottomWall)) 
                walls.Add(new Tuple<int, int, MazeCell>(x, y, MazeCell.BottomWall));

            if (y > 0 && cells[x, y].hasflag(MazeCell.LeftWall)) 
                walls.Add(new Tuple<int, int, MazeCell>(x, y, MazeCell.LeftWall));

            if (y < cells.GetLength(1) - 1 && cells[x, y].hasflag(MazeCell.RightWall)) 
                walls.Add(new Tuple<int, int, MazeCell>(x, y, MazeCell.RightWall));

            return walls;
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
    /// <summary>
    /// Randomize Prism based on a list of neighbours instead of list of walls/edges
    /// http://weblog.jamisbuck.org/2011/1/10/maze-generation-prim-s-algorithm
    /// </summary>
    public class RandomizePrimMaze2 : Maze
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

            //2. Start by choosing a random cell, mark it as visited and add its non visited neighbours into a list;
            int randomrow = UnityEngine.Random.Range(0, rowNum);
            int randomcol = UnityEngine.Random.Range(0, colNum);
            cells[randomrow, randomcol] |= MazeCell.Visited;

            List<Tuple<int, int>> FrontierSet = new List<Tuple<int, int>>();
            FrontierSet.AddRange(GetCellNonVisitedNeighbours(randomrow, randomcol));

            //3. while theres still non visited neighbours
            while (FrontierSet.Count > 0)
            {
                //3a. pick a random cell from frontier set
                int randomCellI = UnityEngine.Random.Range(0, FrontierSet.Count);
                Tuple<int, int> randomCell = FrontierSet[randomCellI];
                int row = randomCell.Item1;
                int col = randomCell.Item2;

                //3b pick a random neighbour from a list of that is visited of the random cell 
                List<Tuple<int, int>> neighbour = GetCellVisitedNeighbours(row, col);
                Tuple<int, int> randomneighbour = neighbour[UnityEngine.Random.Range(0, neighbour.Count)];
                int row2 = randomneighbour.Item1;
                int col2 = randomneighbour.Item2;

                //3c remove walls
                MazeCell direction = GetDirection(row, col, row2, col2);
            
                cells[row, col] -= direction;
                cells[row, col] |= MazeCell.Visited;
                cells[row2, col2] -= direction.OppositeWall();

                //3d. remove the current from set,add the neighbouring walls fo the list 
                FrontierSet.RemoveAt(randomCellI);
                foreach (var cell in GetCellNonVisitedNeighbours(row, col))
                {
                    if (!FrontierSet.Contains(cell))//we dont want to add duplicated cells
                        FrontierSet.Add(cell);
                }
            }
        }

        List<Tuple<int, int>> GetCellNonVisitedNeighbours(int x, int y)
        {
            if (x < 0 || y < 0 || x > cells.GetLength(0) || y > cells.GetLength(1)) return null;

            List<Tuple<int, int>> neighbours = new List<Tuple<int, int>>();

            if (x > 0)
            {
                if(!cells[x - 1, y].hasflag(MazeCell.Visited))
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

        List<Tuple<int, int>> GetCellVisitedNeighbours(int x, int y)
        {
            if (x < 0 || y < 0 || x > cells.GetLength(0) || y > cells.GetLength(1)) return null;

            List<Tuple<int, int>> neighbours = new List<Tuple<int, int>>();

            if (x > 0)
            {
                if (cells[x - 1, y].hasflag(MazeCell.Visited))
                    neighbours.Add(new Tuple<int, int>(x - 1, y));
            }
            if (x < cells.GetLength(0) - 1)
            {
                if (cells[x + 1, y].hasflag(MazeCell.Visited))
                    neighbours.Add(new Tuple<int, int>(x + 1, y));
            }
            if (y > 0)
            {
                if (cells[x, y - 1].hasflag(MazeCell.Visited))
                    neighbours.Add(new Tuple<int, int>(x, y - 1));

            }
            if (y < cells.GetLength(1) - 1)
            {
                if (cells[x, y + 1].hasflag(MazeCell.Visited))
                    neighbours.Add(new Tuple<int, int>(x, y + 1));
            }

            return neighbours;
        }
        MazeCell GetDirection(int x,int y, int x2,int y2)
        {
            MazeCell dir;
            if(x < x2)
            {
                dir = MazeCell.BottomWall;
            }
            else if ( x > x2)
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
