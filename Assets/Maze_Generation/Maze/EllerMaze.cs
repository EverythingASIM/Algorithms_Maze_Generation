using System;
using System.Text;
using System.Collections.Generic;

using asim.unity.extensions;

namespace asim.unity.generation.maze
{
    /// <summary>
    /// http://weblog.jamisbuck.org/2010/12/29/maze-generation-eller-s-algorithm
    /// very unoptimized
    /// </summary>
    public class EllerMaze : Maze
    {
        MazeCell[,] cells;
        int currentSetIndex = -1;

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

            Dictionary<Tuple<int, int>, int> cellsList = new Dictionary<Tuple<int, int>, int>();
            Dictionary<int,List<Tuple<int, int>>> cellSetList = new Dictionary<int,List<Tuple<int, int>>>();

            //2. add each cell in the first row into sets
            for (int j = 0; j < colNum; j++)
            {
                Tuple<int, int> cell = new Tuple<int, int>(0, j);

                cellSetList[++currentSetIndex] = new List<Tuple<int, int>>() { cell };
                cellsList[cell] = currentSetIndex;
            }

            //for each row,
            for (int i = 0; i < rowNum - 1;)
            {
                //3. check each cell of what set it belongs to
                Tuple<int, int> currentcell = new Tuple<int, int>(i, 0);
                int currentsetIndex = cellsList[currentcell];
                for (int j = 0; j < colNum - 1; j++)
                {
                    //4. Randomly choose to connect to the next cell
                    bool connect = UnityEngine.Random.Range(0, 2) == 1 ? true: false;
                    Tuple<int, int> nextcell = new Tuple<int, int>(i, j + 1);
                    if (connect)
                    {
                        //4a. if adjacent cell does not belongs to the same set,
                        //Convert the next cell to be the same set and carve a path
                        if (cellsList[nextcell] != currentsetIndex)
                        {
                            cells[i, j] -= MazeCell.RightWall;
                            cells[i, j + 1] -= MazeCell.LeftWall;

                            var list = cellSetList[cellsList[nextcell]];
                            list.Remove(nextcell);
                            if (list.Count == 0) cellSetList.Remove(cellsList[nextcell]);

                            currentsetIndex = cellsList[nextcell];

                            cellSetList[cellsList[currentcell]].Add(nextcell);
                            cellsList[nextcell] = cellsList[currentcell];
                        }
                    }
                    else
                    {
                        currentsetIndex = cellsList[nextcell];
                    }
                    currentcell = nextcell;
                }

                //4. after randomly joining adjacent sets ,loop though each set list and 
                foreach (var key in cellSetList.Keys)
                {
                    var set = cellSetList[key];

                    //4a randomly choose a random number of cells within the set to extend down 
                    set.Shuffle();
                    int count = set.Count;
                    for (int l = 0; l < UnityEngine.Random.Range(1, count); l++)
                    {
                        int x = set[l].Item1;
                        int y = set[l].Item2;

                        cells[x, y] -= MazeCell.BottomWall;
                        cells[x + 1, y] -= MazeCell.TopWall;

                        Tuple<int, int> newCell = new Tuple<int, int>(x + 1, y);
                        cellSetList[key].Add(newCell);
                        cellsList[newCell] = key;
                    }
                }
                i++;

                //replace the current cellList with a new celllist with sets of the new row
                cellSetList = new Dictionary<int,List<Tuple<int, int>>>();
                for (int j = 0; j < colNum; j++)
                {
                    cellsList.Remove(new Tuple<int, int>(i - 1, j));
                    var cell = new Tuple<int, int>(i, j);
                    if (cellsList.ContainsKey(cell))
                    {
                        if(!cellSetList.ContainsKey(cellsList[cell]))
                            cellSetList[cellsList[cell]] = new List<Tuple<int, int>>();
                        cellSetList[cellsList[cell]].Add(cell);
                    }
                    else
                    {
                        cellSetList[++currentSetIndex] = new List<Tuple<int, int>>() { cell };
                        cellsList[cell] = currentSetIndex;
                    }
                }

                //if last row combine all sets
                if (i == rowNum - 1)
                {
                    int setIndex = cellsList[new Tuple<int, int>(i, 0)];
                    for (int j = 0; j < colNum - 1; j++)
                    {
                        //4a. if adjacent cell does not belongs to the same set,
                        //Convert the next cell to be the same set and carve a path
                        Tuple<int, int> nextcell = new Tuple<int, int>(i, j + 1);
                        if (cellsList[nextcell] != setIndex)
                        {
                            cells[i, j] -= MazeCell.RightWall;
                            cells[i, j + 1] -= MazeCell.LeftWall;

                            setIndex = cellsList[nextcell];
                        }
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