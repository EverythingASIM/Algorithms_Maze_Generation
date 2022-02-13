using System.Text;

using UnityEngine;

namespace asim.unity.generation.maze
{
    /// <summary>
    /// http://weblog.jamisbuck.org/2011/1/12/maze-generation-recursive-division-algorithm
    /// From one large space, divide into multiple sections with single wall but with a small passage, making two smaller space
    /// recursively divide the two smaller spaces
    /// This version divides using a single wall, but there are other versions that divides using two walls see :
    /// https://en.wikipedia.org/wiki/Maze_generation_algorithm#Recursive_implementation
    /// </summary>
    public class RecursiveDivisioMaze : Maze
    {
        const int HORIZONTAL = 1;
        const int VERTICAL = 0;

        MazeCell[,] cells;

        public override void CreateMaze(int rowNum, int colNum)
        {
            if (rowNum == 0 || colNum == 0) return;

            //1. Initilize Cells as empty (no walls) except the edges
            cells = new MazeCell[rowNum, colNum];
            for (int i = 0; i < rowNum; i++)
            {
                for (int j = 0; j < colNum; j++)
                {
                    cells[i, j] = 0;

                    if (i == 0) cells[i, j] |= MazeCell.TopWall;
                    else if (i == rowNum-1) cells[i, j] |= MazeCell.BottomWall;

                    if (j == 0) cells[i, j] |= MazeCell.LeftWall;
                    else if (j == colNum - 1) cells[i, j] |= MazeCell.RightWall;
                }
            }

            divide(0, 0, colNum, rowNum, choose_orientation(colNum, rowNum));
        }

        int choose_orientation(int width,int height)
        {
            if (width < height) return HORIZONTAL;
            else if (height < width) return VERTICAL;
            else return Random.Range(0, 2);
        }
        void divide(int startrowIndex,int startcolIndex,int width,int height,int orientation)
        {
            if (width < 2 || height < 2) return;

            bool horizontal = orientation == HORIZONTAL;

            //1. From the selected orientation to slipt, randomly select a starting cell to start drawing the wall accross,
            var currentWallrowIndex = startrowIndex + (horizontal ? Random.Range(0, height - 1) : 0); //if drawing horizontal, pick a random row index
            var currentWallcolIndex = startcolIndex + (horizontal ? 0 : Random.Range(0, width - 1)); // if drawing vertical, pick a random col index

            //2. randomly select a row/col index to be the passage in the list of walls be to drawn accross
            var passageWallrowIndex = currentWallrowIndex + (horizontal ? 0 : Random.Range(0, height)); //if drawing vertical, pick a random row index
            var passageWallcolIndex = currentWallcolIndex + (horizontal ? Random.Range(0, width) : 0); //if drawing horizontal, pick a random col index

            //3. what direction to place the wall of the cell
            MazeCell walldir = horizontal ? MazeCell.BottomWall : MazeCell.RightWall;

            //4. Contstruct the walls to divide, but skip the passage
            int wallLength = horizontal ? width : height;
            for (int i = 0; i < wallLength; i++)
            {
                if (currentWallrowIndex != passageWallrowIndex || currentWallcolIndex != passageWallcolIndex)
                {
                    if (horizontal)
                    {
                        cells[currentWallrowIndex, currentWallcolIndex] |= walldir;
                        cells[currentWallrowIndex + 1, currentWallcolIndex] |= walldir.OppositeWall();
                    }
                    else
                    {
                        cells[currentWallrowIndex, currentWallcolIndex] |= walldir;
                        cells[currentWallrowIndex, currentWallcolIndex + 1] |= walldir.OppositeWall();
                    }
                }
                if (horizontal) currentWallcolIndex++;
                else currentWallrowIndex++;
            }

            //Ones the space has been divided, recurisvely call divide on the two new child spaces
            var newSpaceRowIndex = startrowIndex;
            var newSpaceColIndex = startcolIndex;
            var newSpaceWidth = horizontal ? width: currentWallcolIndex - startcolIndex + 1;
            var newSpaceHeight = horizontal ? currentWallrowIndex - startrowIndex + 1 : height;
            divide(newSpaceRowIndex, newSpaceColIndex, newSpaceWidth, newSpaceHeight, choose_orientation(newSpaceWidth, newSpaceHeight));

            var newSpaceRowIndex2 = horizontal ? currentWallrowIndex + 1: startrowIndex;
            var newSpaceColIndex2 = horizontal ? startcolIndex : currentWallcolIndex + 1;
            var newSpaceWidth2 = horizontal ? width : startcolIndex + width - currentWallcolIndex - 1;
            var newSpaceHeight2 = horizontal ? startrowIndex + height - currentWallrowIndex - 1 : height;
            divide(newSpaceRowIndex2, newSpaceColIndex2, newSpaceWidth2, newSpaceHeight2, choose_orientation(newSpaceWidth2, newSpaceHeight2));
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