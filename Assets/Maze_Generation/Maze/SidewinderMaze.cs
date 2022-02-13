using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace asim.unity.generation.maze
{
    /// <summary>
    /// http://weblog.jamisbuck.org/2011/2/3/maze-generation-sidewinder-algorithm
    /// 1. Go though each row, starting with first cell in each row start the "run",
    /// 2. randomly choose to carve a path to the east cell
    /// 3. if carved, set the new cell to current and repeat step 2
    /// 4. if choosed not to carved or having reached the last col, from the list of cells visited in the run, choose one randomly to carve north.
    /// 5. set current to be the next available cell in the col and repeat the "run" 2-4
    /// 6. otherwise go to the next row and repeat the "run"
    /// </summary>
    public class SidewinderMaze : Maze
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

            for (int i = 0; i < rowNum; i++)
            {
                int currentCol = 0;
                for (int j = 0; j < colNum; j++)
                {
                    if(i > 0 && (j + 1 == colNum || UnityEngine.Random.Range(0,2) == 0))
                    {
                        // end current run and carve north
                        var randomCol = currentCol + UnityEngine.Random.Range(0,j - currentCol + 1);
                        cells[i, randomCol] -= MazeCell.TopWall;
                        cells[i-1, randomCol] -= MazeCell.BottomWall;
                        currentCol = j + 1;
                    }
                    else if(j + 1 < colNum)
                    {
                        //carve east
                        cells[i, j] -= MazeCell.RightWall;
                        cells[i, j+1] -= MazeCell.LeftWall;
                    }
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