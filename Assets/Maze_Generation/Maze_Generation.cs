using UnityEngine;

using asim.unity.generation.maze;
using asim.unity.managers.tilemanager;

using System.IO;

/// <summary>
/// Generate Maze and Move Camera In view
/// </summary>
public class Maze_Generation : MonoBehaviour
{
    [SerializeField] new Camera camera;
    [SerializeField] SimpleTile_TileManager Tilemanager;

    public int MazeRowSize = 10;
    public int MazeColSize = 10;

    void Start()
    {
        GenerateRDFSNewMaze(MazeRowSize, MazeColSize);

        FitCameraToMaze();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            GenerateRDFSNewMaze(MazeRowSize, MazeColSize);

            FitCameraToMaze();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GenerateIDFSNewMaze(MazeRowSize, MazeColSize);

            FitCameraToMaze();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GenerateKruskalNewMaze(MazeRowSize, MazeColSize);

            FitCameraToMaze();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GeneratePrimNewMaze(MazeRowSize, MazeColSize);

            FitCameraToMaze();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            GenerateRecursiveDivisionNewMaze(MazeRowSize, MazeColSize);

            FitCameraToMaze();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            GenerateAldousBroderNewMaze(MazeRowSize, MazeColSize);

            FitCameraToMaze();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            GenerateWilsonNewMaze(MazeRowSize, MazeColSize);

            FitCameraToMaze();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            GenerateHuntKillNewMaze(MazeRowSize, MazeColSize);

            FitCameraToMaze();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            GenerateEllerNewMaze(MazeRowSize, MazeColSize);

            FitCameraToMaze();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            GenerateBinaryTreeNewMaze(MazeRowSize, MazeColSize);

            FitCameraToMaze();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            GenerateGrowingTreeNewMaze(MazeRowSize, MazeColSize);

            FitCameraToMaze();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            GenerateSidewinderNewMaze(MazeRowSize, MazeColSize);

            FitCameraToMaze();
        }
    }
    void GenerateRDFSNewMaze(int rows, int cols)
    {
        RandomRecurvesiveDFSMaze maze = new RandomRecurvesiveDFSMaze();
        maze.CreateMaze(rows, cols);

        Tilemanager.Removetiles();
        Tilemanager.SpawnTilesetFromCharArray(maze.GetMaze());
    }

    void GenerateIDFSNewMaze(int rows, int cols)
    {
        RandomIterativeDFSMaze maze = new RandomIterativeDFSMaze();
        maze.CreateMaze(rows, cols);

        Tilemanager.Removetiles();
        Tilemanager.SpawnTilesetFromCharArray(maze.GetMaze());
    }
    void GenerateKruskalNewMaze(int rows, int cols)
    {
        RandomizedKruskalMaze maze = new RandomizedKruskalMaze();
        maze.CreateMaze(rows, cols);

        Tilemanager.Removetiles();
        Tilemanager.SpawnTilesetFromCharArray(maze.GetMaze());
    }
    bool primtoggle = false;
    void GeneratePrimNewMaze(int rows, int cols)
    {
        if(primtoggle)
        {
            RandomizePrimMaze maze = new RandomizePrimMaze();
            maze.CreateMaze(rows, cols);

            Tilemanager.Removetiles();
            Tilemanager.SpawnTilesetFromCharArray(maze.GetMaze());
        }
        else
        {
            RandomizePrimMaze2 maze = new RandomizePrimMaze2();
            maze.CreateMaze(rows, cols);

            Tilemanager.Removetiles();
            Tilemanager.SpawnTilesetFromCharArray(maze.GetMaze());
        }
        primtoggle = !primtoggle;
    }
    void GenerateRecursiveDivisionNewMaze(int rows, int cols)
    {
        RecursiveDivisioMaze maze = new RecursiveDivisioMaze();
        maze.CreateMaze(rows, cols);

        Tilemanager.Removetiles();
        Tilemanager.SpawnTilesetFromCharArray(maze.GetMaze());
    }
    void GenerateAldousBroderNewMaze(int rows, int cols)
    {
        AldousBroderMaze maze = new AldousBroderMaze();
        maze.CreateMaze(rows, cols);

        Tilemanager.Removetiles();
        Tilemanager.SpawnTilesetFromCharArray(maze.GetMaze());
    }
    void GenerateWilsonNewMaze(int rows, int cols)
    {
        WilsonMaze maze = new WilsonMaze();
        maze.CreateMaze(rows, cols);

        Tilemanager.Removetiles();
        Tilemanager.SpawnTilesetFromCharArray(maze.GetMaze());
    }
    void GenerateHuntKillNewMaze(int rows, int cols)
    {
        HuntKillMaze maze = new HuntKillMaze();
        maze.CreateMaze(rows, cols);

        Tilemanager.Removetiles();
        Tilemanager.SpawnTilesetFromCharArray(maze.GetMaze());
    }
    void GenerateEllerNewMaze(int rows, int cols)
    {
        EllerMaze maze = new EllerMaze();
        maze.CreateMaze(rows, cols);

        Tilemanager.Removetiles();
        Tilemanager.SpawnTilesetFromCharArray(maze.GetMaze());
    }
    void GenerateBinaryTreeNewMaze(int rows, int cols)
    {
        BinaryTreeMaze maze = new BinaryTreeMaze();
        maze.CreateMaze(rows, cols);

        Tilemanager.Removetiles();
        Tilemanager.SpawnTilesetFromCharArray(maze.GetMaze());
    }
    void GenerateGrowingTreeNewMaze(int rows, int cols)
    {
        GrowingTreeMaze maze = new GrowingTreeMaze();
        maze.CreateMaze(rows, cols);

        Tilemanager.Removetiles();
        Tilemanager.SpawnTilesetFromCharArray(maze.GetMaze());
    }
    
    void GenerateSidewinderNewMaze(int rows, int cols)
    {
        SidewinderMaze maze = new SidewinderMaze();
        maze.CreateMaze(rows, cols);

        Tilemanager.Removetiles();
        Tilemanager.SpawnTilesetFromCharArray(maze.GetMaze());
    }

    void FitCameraToMaze()
    {
        camera.transform.position = new Vector3(MazeColSize, -MazeRowSize, -5);
        camera.orthographicSize = Mathf.Max(1,Mathf.Max(MazeRowSize, MazeColSize) * 1.2f);
    }

    void SaveMaze(char[][] maze,string pathname)
    {
        StreamWriter sw = File.CreateText(pathname);
        for (int i = 0; i < maze.Length; i++)
        {
            char[] row = maze[i];
            for (int j = 0; j < row.Length; j++)
            {
                sw.WriteLine(new string(row));
            }
        }
        sw.Close();
    }
}
