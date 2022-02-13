using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace asim.unity.generation.maze
{
    /// <summary>
    /// http://weblog.jamisbuck.org/2011/2/1/maze-generation-binary-tree-algorithm
    /// </summary>
    public class BinaryTreeMaze : Maze
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

            //2. loop though all cells,
            for (int i = 0; i < rowNum; i++)
            {
                for (int j = 0; j < colNum; j++)
                {
                    //3. randomly pick either top or left direction to carve a path
                    List<MazeCell> dirs = new List<MazeCell>();
                    if (i > 0) dirs.Add(MazeCell.TopWall);
                    if (j > 0) dirs.Add(MazeCell.LeftWall);

                    if (dirs.Count == 0) continue;

                    var dir = dirs[UnityEngine.Random.Range(0, dirs.Count)];
                    int nextX = i; 
                    int nextY = j;
                    if (dir == MazeCell.TopWall) nextX -= 1;
                    else nextY -= 1;

                    cells[i, j] -= dir;
                    cells[nextX,nextY] -= dir.OppositeWall();
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