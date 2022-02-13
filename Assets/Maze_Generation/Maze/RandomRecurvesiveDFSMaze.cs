using System.Linq;
using System.Text;
using System.Collections.Generic;

using UnityEngine;

using asim.unity.extensions;

/* 
 * Simple Random Depth-first search maze generation algorithm.
Start at a random cell.
Mark the current cell as visited, and get a list of its neighbors. 
For each neighbor, starting with a randomly selected neighbor:
If that neighbor hasn't been visited, 
remove the wall between this cell and that neighbor, 
and then repeat with that neighbor as the current cell.

Sourced from : http://rosettacode.org/wiki/Maze_generation#C.23
*/

namespace asim.unity.generation.maze
{
    public class RandomRecurvesiveDFSMaze : Maze
    {
        MazeCell[,] cells;

        public override void CreateMaze(int rowNum, int colNum)
        {
            if (rowNum == 0 || colNum == 0) return;

            //Step 1. Initialize a grid of cells
            cells = new MazeCell[rowNum, colNum];
            for (int i = 0; i < rowNum; i++)
            {
                for (int j = 0; j < colNum; j++)
                {
                    cells[i,j] = MazeCell.Initial;
                }
            }

            //Step 2. Set the starting (random) cell to be visited
            VisitCell(new Vector2(Random.Range(0, rowNum), Random.Range(0, colNum)));
        }

        void VisitCell(Vector2 cell)
        {
            cells[(int) cell.x,(int) cell.y] |= MazeCell.Visited; //Mark Cell as visited
            
            //Step 3. Foreach surrounding neighbours that are not visited yet, pick a random neighbour
            foreach (
                var p in
                GetNeighbours(new Vector2(cell.x, cell.y))
                    .Shuffle()
                    .Where(c => !(cells[(int) c.Neighbour.x,(int) c.Neighbour.y].HasFlag(MazeCell.Visited))))
            {
                //Step 3a. Remove the wall connecting the current cell, and the neighbour clel
                cells[(int) cell.x,(int) cell.y] -= p.Wall;
                cells[(int) p.Neighbour.x,(int) p.Neighbour.y] -= p.Wall.OppositeWall();

                //Recurvesively repeat with the current neighbour
                VisitCell(new Vector2(p.Neighbour.x, p.Neighbour.y));
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
            if (cell.y < cells.GetLength(1) - 1)
                yield return new RemoveWallAction { Neighbour = new Vector2(cell.x, cell.y + 1), Wall = MazeCell.RightWall };
            if (cell.x > 0)
                yield return new RemoveWallAction { Neighbour = new Vector2(cell.x - 1, cell.y), Wall = MazeCell.TopWall };
            if (cell.x < cells.GetLength(0) - 1)
                yield return new RemoveWallAction { Neighbour = new Vector2(cell.x + 1, cell.y), Wall = MazeCell.BottomWall };
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