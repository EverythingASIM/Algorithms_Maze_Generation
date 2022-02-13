using static asim.unity.generation.maze.Maze;

namespace asim.unity.generation.maze
{
    public abstract class Maze
    {
        [System.Flags]
        public enum MazeCell
        {
            TopWall = 1,
            RightWall = 2,
            BottomWall = 4,
            LeftWall = 8,
            Visited = 128,
            Initial = TopWall | RightWall | BottomWall | LeftWall,
        }

        public abstract void CreateMaze(int rows, int cols);
        public abstract char[][] GetMaze();

    }
    public static class MazeExtensions
    {
        public static MazeCell OppositeWall(this MazeCell orig)
        {
            return (MazeCell)(((int)orig >> 2) | ((int)orig << 2)) & MazeCell.Initial;
        }

        /// <summary>
        /// Note that c# system.enum hasflag implenetation is known to be slower
        /// </summary>
        public static bool hasflag(this MazeCell cs, MazeCell flag)
        {
            return ((int)cs & (int)flag) != 0;
        }
    }
}