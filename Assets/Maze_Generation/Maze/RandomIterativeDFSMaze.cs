using System.Linq;
using System.Text;
using System.Collections.Generic;

using UnityEngine;

using asim.unity.extensions;

/* 
 * Similar to Recursive Simple Random Depth-first search Maze
 * but instead uses a stack instead of recursion
*/

namespace asim.unity.generation.maze
{
    public class RandomIterativeDFSMaze : Maze
    {
        MazeCell[,] cells;
        Stack<Vector2> visitedCellsToCheckNeightbour;

        int Mazeheight;
        int Mazewidth;

        public override void CreateMaze(int rowNum, int colNum)
        {
            if (rowNum == 0 || colNum == 0) return;

            //Step 1. Initialize a grid of cells
            Mazeheight = rowNum;
            Mazewidth = colNum;
            cells = new MazeCell[rowNum, colNum];
            for (int i = 0; i < rowNum; i++)
            {
                for (int j = 0; j < colNum; j++)
                {
                    cells[i,j] = MazeCell.Initial;
                }
            }

            //Step 2. Set the starting (random) cell to be visited
            Vector2 cell = new Vector2(Random.Range(0, rowNum), Random.Range(0, colNum));
            cells[(int)cell.x,(int)cell.y] |= MazeCell.Visited;

            //Step 3. Push the visited cell onto stack
            visitedCellsToCheckNeightbour = new Stack<Vector2>();
            visitedCellsToCheckNeightbour.Push(cell);
            
            //Step 4. Pop the stack until empty
            while (visitedCellsToCheckNeightbour.Count>0)
            {
                //Check the cell in the stack for its neighbour
                cell = visitedCellsToCheckNeightbour.Pop();
                CheckNeighbourCell(cell);
            }
        }

        void CheckNeighbourCell(Vector2 cell)
        {
            //Step 5. pick a random neighbour that is not visited
            foreach (
                var p in
                GetNeighbours(new Vector2(cell.x, cell.y))
                    .Shuffle()
                    .Where(c => !(cells[(int)c.Neighbour.x,(int)c.Neighbour.y].HasFlag(MazeCell.Visited))))
            {
                //Step 5a. Remove the wall connecting the current cell, and the neighbour cell
                cells[(int)cell.x,(int)cell.y] -= p.Wall;
                cells[(int)p.Neighbour.x,(int)p.Neighbour.y] -= p.Wall.OppositeWall();

                //Step 6, set the neighbour to be visited and add to stack
                cells[(int)p.Neighbour.x,(int)p.Neighbour.y] |= MazeCell.Visited;
                visitedCellsToCheckNeightbour.Push(new Vector2((int)p.Neighbour.x, (int)p.Neighbour.y));
            }
        }

        /// <summary>
        /// struct to Represent the neightbour position
        /// </summary>
        struct RemoveWallAction
        {
            public Vector2 Neighbour;
            public MazeCell Wall;
        }
        /// <summary>
        /// returns a list of all neightbour cell of the current and marked where the wall between current cell
        /// </summary>
        IEnumerable<RemoveWallAction> GetNeighbours(Vector2 cell)
        {
            //Note that cell x,y is refering to the index of row,cols and not in terms of positioning (common mistake)
            if (cell.y > 0)
                yield return new RemoveWallAction { Neighbour = new Vector2(cell.x, cell.y - 1), Wall = MazeCell.LeftWall };
            if (cell.y < Mazewidth - 1)
                yield return new RemoveWallAction { Neighbour = new Vector2(cell.x, cell.y + 1), Wall = MazeCell.RightWall };
            if (cell.x > 0)
                yield return new RemoveWallAction { Neighbour = new Vector2(cell.x - 1, cell.y), Wall = MazeCell.TopWall };
            if (cell.x < Mazeheight - 1)
                yield return new RemoveWallAction { Neighbour = new Vector2(cell.x + 1, cell.y), Wall = MazeCell.BottomWall };
        }

        public override char[][] GetMaze()
        {
            if (cells == null) return null;

            char[][] Mazedata = new char[Mazeheight * 2 + 1][];
            for (int i = 0; i < Mazeheight; i++)
            {
                StringBuilder sbTop = new StringBuilder();
                StringBuilder sbMid = new StringBuilder();
                for (int j = 0; j < Mazewidth; j++)
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
                    Mazedata[2 * Mazeheight] = sbTop.ToString().ToCharArray();
                }
            }
            return Mazedata;
        }
    }
}