using System;
using System.Text;
using System.Collections.Generic;

using asim.unity.datastructures;
using asim.unity.extensions;

namespace asim.unity.generation.maze
{
    /// <summary>
    /// http://weblog.jamisbuck.org/2011/1/3/maze-generation-kruskal-s-algorithm
    /// </summary>
    public class RandomizedKruskalMaze : Maze
    {
        MazeCell[,] cells;
        DisjointSet sets;
        public override void CreateMaze(int rowNum, int colNum)
        {
            if (rowNum == 0 || colNum == 0) return;

            //1. Create the maze cell
            cells = new MazeCell[rowNum, colNum];
            for (int i = 0; i < rowNum; i++)
            {
                for (int j = 0; j < colNum; j++)
                {
                    cells[i,j] = MazeCell.Initial;
                }
            }

            //2. For each cell, create a disjointset
            sets = new DisjointSet(rowNum * colNum);

            //3. Build a list of walls idenified by the row & col of the cell and the wall direction
            List<Tuple<int, int, MazeCell>> walls = new List<Tuple<int, int, MazeCell>>();
            for (int x = 0; x < rowNum; x++)
            {
                for (int y = 0; y < colNum; y++)
                {
                    if (x > 0) walls.Add(new Tuple<int, int, MazeCell>(x, y, MazeCell.TopWall));
                    if (y > 0) walls.Add(new Tuple<int, int, MazeCell>(x, y, MazeCell.LeftWall));
                }
            }

            //4. Select each wall from the list in random
            walls.Shuffle();
            for (int i = 0; i < walls.Count; i++)
            {
                //4a. Calculate which sets the wall seperates
                int rowIndex = walls[i].Item1;
                int colIndex = walls[i].Item2;
                MazeCell direction = walls[i].Item3;

                int rowIndex2 = rowIndex;
                int colIndex2 = colIndex;
                if(direction.hasflag(MazeCell.TopWall))
                {
                    rowIndex2 -= 1;
                }
                else
                {
                    colIndex2 -= 1;
                }
                int set1Index = rowIndex * colNum + colIndex;
                int set2Index = rowIndex2 * colNum + colIndex2;

                //4b. See if the sets has different parent root nodes, union them together
                if (sets.FindSet(set1Index) != sets.FindSet(set2Index))
                {
                    sets.Union(set1Index, set2Index);
                    //4c. Remove walls of the connecting cells
                    cells[rowIndex,colIndex] -= direction;
                    cells[rowIndex2,colIndex2] -= direction.OppositeWall();
                }
            }
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
   