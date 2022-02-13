using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace asim.unity.generation.maze
{
    /// <summary>
    // http://weblog.jamisbuck.org/2011/1/20/maze-generation-wilson-s-algorithm
    /// </summary>
    public class WilsonMaze : Maze
    {
        MazeCell[,] cells;

        public override void CreateMaze(int rowNum, int colNum)
        {
            if (rowNum == 0 || colNum == 0) return;

            //1. Initilize Cells and keep a seperate list of non visited cells
            cells = new MazeCell[rowNum, colNum];
            List<Tuple<int, int>> nonVisitedCells = new List<Tuple<int, int>>();
            for (int i = 0; i < rowNum; i++)
            {
                for (int j = 0; j < colNum; j++)
                {
                    cells[i, j] = MazeCell.Initial;
                    nonVisitedCells.Add(new Tuple<int, int>(i, j));
                }
            }

            int randomX = UnityEngine.Random.Range(0, rowNum);
            int randomY = UnityEngine.Random.Range(0, colNum);

            //1. select a random cell, and mark it as visited
            cells[randomX, randomY] |= MazeCell.Visited;
            nonVisitedCells.Remove(new Tuple<int, int>(randomX, randomY));

            int remaining = rowNum * colNum - 1;
            while (remaining > 0)
            {
                //2. Select a random non visited cell
                Tuple<int, int> randomNonVisitedCell = nonVisitedCells[UnityEngine.Random.Range(0, nonVisitedCells.Count)];

                //3. start walking to a random neighbour, keep track of walked cells using a list 
                // keep track of the starting cell
                Dictionary<Tuple<int, int>, MazeCell> walkedCells = new Dictionary<Tuple<int, int>, MazeCell>();
                int startingX = randomNonVisitedCell.Item1;
                int startingY = randomNonVisitedCell.Item2;
                int currentX = startingX;
                int currentY = startingY;

                //4. add the random neighbour to the walked list, and if the neighbour is part of the maze, we stop
                int nextX;
                int nextY;
                do
                {
                    List<Tuple<int, int>> neighbours = GetCellNeighbours(currentX, currentY);
                    Tuple<int, int> nextWalkCell = neighbours[UnityEngine.Random.Range(0, neighbours.Count)];
                    nextX = nextWalkCell.Item1;
                    nextY = nextWalkCell.Item2;

                    MazeCell direction = GetDirection(currentX, currentY, nextX, nextY);
                    walkedCells[new Tuple<int, int>(currentX, currentY)] = direction;

                    currentX = nextX;
                    currentY = nextY;
                }
                while (!cells[nextX, nextY].hasflag(MazeCell.Visited));
                walkedCells[new Tuple<int, int>(nextX, nextY)] = 0;

                currentX = startingX;
                currentY = startingY;
                MazeCell walkedTo = walkedCells[new Tuple<int, int>(currentX, currentY)];
                while (walkedTo != 0)
                {
                    nextX = currentX;
                    nextY = currentY;
                    if (walkedTo == MazeCell.LeftWall)
                    {
                        nextY--;
                    }
                    else if (walkedTo == MazeCell.RightWall)
                    {
                        nextY++;
                    }
                    if (walkedTo == MazeCell.TopWall)
                    {
                        nextX--;
                    }
                    else if (walkedTo == MazeCell.BottomWall)
                    {
                        nextX++;
                    }

                    cells[currentX, currentY] -= walkedTo;
                    cells[nextX, nextY] -= walkedTo.OppositeWall();
                    cells[currentX, currentY] |= MazeCell.Visited;
                    nonVisitedCells.Remove(new Tuple<int, int>(currentX, currentY));
                    remaining--;

                    currentX = nextX;
                    currentY = nextY;

                    walkedTo = walkedCells[new Tuple<int, int>(currentX, currentY)];
                }
             }
        }

        List<Tuple<int, int>> GetCellNeighbours(int x, int y)
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