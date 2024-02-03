using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.SceneManagement;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject exit;
    [SerializeField] GameObject floorPiece;
    [SerializeField] GameObject cardSpawner;
    int size;
    int findFreeTries;
    [SerializeField] int floorsBetweenPoints = 1;
    [SerializeField] float spawnChance = 0.01f;
    [SerializeField] Material floorMaterial;
    bool[,] maze;
    float floorScale = 3f;
    const int VERTEXLIMIT = 32000;
    int pieceIncrementer = 0;

    // Start is called before the first frame update
    void Start()
    {
        size = GameState.mazeSize;
        findFreeTries = size * size;

        Logger.Instance.Log("Maze Creation started.");

        maze = new bool[size, size];
        foreach (var path in MazePathsBuilder(RandomPointSeeder(size)))
            FloorLayer(path, floorPiece);

        BatchMeshes();

        PlacePlayerAndGoal();
        //Application.targetFrameRate = 120;
    }

    void PlacePlayerAndGoal()
    {
        Logger.Instance.Log("Entity Placement started.");



        List<Vector2Int> playerPositions = new List<Vector2Int>();
        for (int i = 0; i < size; i++)
            if (maze[i, 0])
                playerPositions.Add(new Vector2Int(i, 0));
        for (int i = 0; i < size; i++)
            if (maze[0, i])
                playerPositions.Add(new Vector2Int(0, i));

        List<Vector2Int> exitPositions = new List<Vector2Int>();
        for (int i = 0; i < size; i++)
            if (maze[i, size - 1])
                exitPositions.Add(new Vector2Int(i, size - 1));
        for (int i = 0; i < size; i++)
            if (maze[size - 1, i])
                exitPositions.Add(new Vector2Int(size - 1, i));



        Vector3 playerPosition = MazeToWorldSpace(playerPositions[Random.Range(0, playerPositions.Count)]) * (floorsBetweenPoints + 1) * floorScale;
        player.transform.position = playerPosition;
        Logger.Instance.Log("Player moved to " + player.transform.position.x + "/" + player.transform.position.z);

        Vector3 exitPosition = playerPosition;
        foreach (Vector2Int v in exitPositions)
        {
            Vector3 pos = MazeToWorldSpace(v) * (floorsBetweenPoints + 1) * floorScale;
            if ((playerPosition - pos).magnitude > (playerPosition - exitPosition).magnitude)
                exitPosition = pos;
        }
        Instantiate(exit, exitPosition, Quaternion.identity);



        Logger.Instance.Log("Entity Placement finished.");
    }

    public void VRSwitch()
    {
        Logger.Instance.Log("VR Enabled: Player moved to " + player.transform.position.x + "/" + player.transform.position.z);
        Logger.Instance.Log("VR Enabled: Camera moved to " + Camera.main.transform.position.x + "/" + Camera.main.transform.position.z);
    }

    void BatchMeshes()
    {
        Logger.Instance.Log("Batching started.");
        //Get all eligable GameObjects
        List<GameObject> gos = new List<GameObject>();

        foreach (Transform go in transform)
            if (go.tag == "Floor")
                gos.Add(go.gameObject);
        Logger.Instance.Log("Batching: GOs listed (" + gos.Count + " GOs)");

        //Sort them x distance to parent. this should create rectangular groups. Most efficient grouping i could think of.
        gos.Sort((child1, child2) =>
            child1.transform.localPosition.x
            .CompareTo(child2.transform.localPosition.x));


        Logger.Instance.Log("Batching: GOs sorted (" + gos.Count + " GOs)");

        while (gos.Count > 0)
        {
            List<CombineInstance> combined = new List<CombineInstance>();
            int vertices = 0;
            int i = 0;
            for (i = gos.Count - 1; i > 0; i--)
            {
                if (gos[i] == null)
                {
                    Logger.Instance.Log(i);
                    continue;
                }
                CombineInstance combineInstance = new CombineInstance();
                MeshFilter meshFilter = gos[i].GetComponent<MeshFilter>();
                combineInstance.mesh = meshFilter.sharedMesh;
                combineInstance.transform = meshFilter.transform.localToWorldMatrix;
                vertices += meshFilter.sharedMesh.vertexCount;

                combined.Add(combineInstance);
                gos[i].SetActive(false);
                Destroy(gos[i]);

                if (vertices >= VERTEXLIMIT)
                    break;
            }

            Logger.Instance.Log("Remove " + (gos.Count - i) + " Elements at Index " + i + " off " + gos.Count + " Elements total.");
            gos.RemoveRange(i, gos.Count - i);

            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combined.ToArray());

            GameObject renderer = new GameObject();
            renderer.transform.parent = transform;
            renderer.AddComponent<MeshFilter>().mesh = combinedMesh;
            renderer.AddComponent<MeshCollider>().sharedMesh = combinedMesh;
            renderer.GetComponent<MeshFilter>().mesh.Optimize();
            renderer.AddComponent<MeshRenderer>().material = floorMaterial;
            renderer.tag = "Floor";
        };

        //shouldnt do anything anymore but keep it here for future use.
        StaticBatchingUtility.Combine(gos.ToArray(), transform.gameObject);

        Logger.Instance.Log("Batching finished.");
    }

    void FloorLayer(List<Vector2Int> path, GameObject floor)
    {
        float scaleAdjust = (floorsBetweenPoints + 1) * floorScale;

        if (!maze[path[0].x, path[0].y])
            PlaceFloorPiece(MazeToWorldSpace(path[0]) * scaleAdjust, floor);

        for (int i = 1; i < path.Count; i++)
        {
            GameObject newFloorPiece = PlaceFloorPiece(MazeToWorldSpace(path[i]) * scaleAdjust, floor);

            if (Random.value < spawnChance)
            {
                GameObject spawner = Instantiate(cardSpawner, transform);
                spawner.transform.localPosition = newFloorPiece.transform.localPosition;
            }

            for (int j = 0; j < floorsBetweenPoints; j++)
            {
                newFloorPiece = Instantiate(floor, transform);
                newFloorPiece.name = "floor" + pieceIncrementer;
                pieceIncrementer++;

                Vector3 offset = MazeToWorldSpace(path[i - 1] - path[i]) / ((float)(floorsBetweenPoints + 1)) * (float)(floorsBetweenPoints - j);
                Vector3 position = (MazeToWorldSpace(path[i]) + offset) * scaleAdjust;

                PlaceFloorPiece(position, floorPiece);
            }
        }
    }

    Vector3 MazeToWorldSpace(Vector2Int vec2Int)
    {
        return new Vector3(vec2Int.x, 0, vec2Int.y);
    }

    GameObject PlaceFloorPiece(Vector3 position, GameObject floor)
    {
        GameObject newFloorPiece = Instantiate(floor, transform);
        newFloorPiece.transform.localPosition = position;
        newFloorPiece.name = "floor" + pieceIncrementer;
        pieceIncrementer++;
        return newFloorPiece;
    }

    List<Vector2Int> RandomPointSeeder(int points)
    {
        List<Vector2Int> seeds = new List<Vector2Int>();
        for (int i = 0; i < points; i++)
        {
            seeds.Add(new Vector2Int(Random.Range(1, size - 1), Random.Range(1, size - 1)));
        }
        return seeds;
    }

    List<List<Vector2Int>> MazePathsBuilder(List<Vector2Int> seeds)
    {
        Logger.Instance.Log("Paths Creation started.");



        List<List<Vector2Int>> paths = new List<List<Vector2Int>>();

        foreach (Vector2Int seed in seeds)
        {
            paths.Add(PathBuilder(seed));
        }



        int openPathExtentionAttempts = findFreeTries;
        while (openPathExtentionAttempts > 0)
        {
            int r1 = Random.Range(0, paths.Count);

            Vector2Int point;
            do
            {
                point = paths[r1][Random.Range(0, paths[r1].Count)];
                if (point.x != 0 && point.x != size - 1 && point.y != 0 && point.y != size - 1)
                    break;
            } while (true);

            paths.Add(PathBuilder(point));
            openPathExtentionAttempts--;
        }



        foreach (var path in paths)
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine((Vector2)path[i] * floorScale * (floorsBetweenPoints + 1), (Vector2)path[i + 1] * floorScale * (floorsBetweenPoints + 1), Color.white, 60f);
            }



        Logger.Instance.Log("Paths created.");
        return paths;
    }

    List<Vector2Int> PathBuilder(Vector2Int startPoint)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(startPoint);

        do
        {
            Vector2Int nextPoint = FindNextPoint(path[path.Count - 1]);
            if (nextPoint == -Vector2Int.one)
                break;
            path.Add(nextPoint);

            if (nextPoint.x == 0 || nextPoint.x == size - 1 || nextPoint.y == 0 || nextPoint.y == size - 1)
                break;

        } while (true);

        return path;
    }

    Vector2Int FindNextPoint(Vector2Int startPoint)
    {
        List<Vector2Int> possibleDirections = new List<Vector2Int>();
        if (!maze[startPoint.x + 1, startPoint.y])
            possibleDirections.Add(Vector2Int.right);
        if (!maze[startPoint.x - 1, startPoint.y])
            possibleDirections.Add(Vector2Int.left);
        if (!maze[startPoint.x, startPoint.y + 1])
            possibleDirections.Add(Vector2Int.up);
        if (!maze[startPoint.x, startPoint.y - 1])
            possibleDirections.Add(Vector2Int.down);

        if (possibleDirections.Count == 0)
            return -Vector2Int.one;

        Vector2Int nextPoint = startPoint + possibleDirections[Random.Range(0, possibleDirections.Count)];
        maze[nextPoint.x, nextPoint.y] = true;
        return nextPoint;
    }
}
