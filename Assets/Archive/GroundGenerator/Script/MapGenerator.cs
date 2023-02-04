using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] int width, height;
    [SerializeField] GameObject[] tiles;
    [SerializeField] Tilemap tileMap;
    [SerializeField] Camera mainCamera;

    Tile tileRef;

    World worldTile;

    float currentCamPos = 0;
    private Thread mapWatcher;

    struct Range
    {
        public int min;
        public int max;
    }
    struct RLEColumn
    {
        public List<Range> ranges;
    }
    struct World
    {
        public List<RLEColumn> columns; //running left to right
    }

    void Start()
    {
        tileRef = ScriptableObject.CreateInstance<Tile>();
        tileRef.gameObject = tiles[0];
        
        FirstGeneration();
        //DeleteCircle(50, 0, 10);        
        //DeleteCircle(120, 29, 20);
        //DeleteCircle(80, 40, 15);
        RefreshTiles();

        //mapWatcher = new Thread(mapCheck);
        //mapWatcher.Start();
        //StartCoroutine(mapCheck());
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            //GenerateNewLine();
            MoveDown();
        }
    }

    void FirstGeneration()
    {
        worldTile = new World();
        worldTile.columns = new List<RLEColumn>();

        for (int x = 0; x < width; x++)
        {
            /*int minHeight = height - 1;
            int maxHeight = height + 2;
            height = Random.Range(minHeight, maxHeight);*/
            
            Range range = new Range();
            range.min = 0;
            range.max = height;

            RLEColumn col = new RLEColumn();
            col.ranges = new List<Range>();
            col.ranges.Add(range);
            worldTile.columns.Add(col);
        }
    }

    void RefreshTiles()
    {
        tileMap.ClearAllTiles();

        for (int x = 0; x < worldTile.columns.Count; x++)
        {
            foreach (Range range in worldTile.columns[x].ranges)
            {
                for (int y = range.min; y <= range.max; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    tileMap.SetTile(pos, tileRef);
                }
            }
        }      
    }

    void MoveDown()
    {
        mainCamera.transform.Translate(0, -1, 0);
        currentCamPos -= 1;
        if (currentCamPos <= -5)
        {
            GenerateNewLines();        
            RefreshTiles();
            currentCamPos = 0;
        }
    }

    IEnumerator mapCheck()
    {
        while (true)
        {
            Debug.Log("T");
            if (currentCamPos <= -5)
            {
                GenerateNewLines();        
                RefreshTiles();
                currentCamPos = 0;
            }
        }
    }

    void GenerateNewLines()
    {
        for (int x = 0; x < worldTile.columns.Count; x++)
        {
            int min = int.MaxValue;
            int minI = -1;
            for (int i = 0; i < worldTile.columns[x].ranges.Count; i++)
            {
                Range range = worldTile.columns[x].ranges[i];
                if (range.min < min)
                {
                    min = range.min;
                    minI = i;
                }
            }

            Range rangeMin = worldTile.columns[x].ranges[minI];
            Range r1 = new Range();
            r1.min = min - 100;
            r1.max = rangeMin.max;

            for (int y = r1.min; y <= min; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                
                tileMap.SetTile(pos, tileRef);
            }

            worldTile.columns[x].ranges.RemoveAt(minI);
            worldTile.columns[x].ranges.Insert(minI, r1);
        }
    }

    public void DeleteCircle(int xc, int yc, int r)
    {
        int x = 0;
        int y = r;
        int d = 1 - r;
        int dx = 2 * x;
        int dy = 2 * y;

        while (x <= y)
        {            
            DeleteRange(x + xc, -y + yc, y + yc);
            DeleteRange(-x + xc, -y + yc, y + yc);
            DeleteRange(y + xc, -x + yc, x + yc);
            DeleteRange(-y + xc, -x + yc, x + yc);

            if(d >= 0)
            {
                y--;
                dy-=2;
                d = d - dy;
            }

            x++;
            dx += 2;

            d = d + dx + 1;
        }
    }

    void DeleteRange(int x, int yMin, int yMax)
    {
        for (int i = 0; i < worldTile.columns[x].ranges.Count; i++)
        {
            Range range = worldTile.columns[x].ranges[i];

            if (yMin <= range.min)
            {
                if (yMax >= range.min && yMax < range.max)
                {                    
                    Range r1 = new Range();
                    r1.min = yMax;
                    r1.max = range.max;
                    worldTile.columns[x].ranges.RemoveAt(i);
                    worldTile.columns[x].ranges.Insert(i, r1);
                }
                else if (yMax >= range.max)
                {
                    worldTile.columns[x].ranges.RemoveAt(i);
                    i--;
                }
            }
            else if (yMin > range.min && yMin < range.max)
            {
                if (yMax < range.max)
                {
                    Range r1 = new Range();
                    r1.min = range.min;
                    r1.max = yMin;

                    Range r2 = new Range();
                    r2.min = yMax;
                    r2.max = range.max;

                    worldTile.columns[x].ranges.Add(r1);
                    worldTile.columns[x].ranges.Add(r2);
                    worldTile.columns[x].ranges.RemoveAt(i);
                    i--;
                }
                else if (yMax >= range.max)
                {
                    Range r1 = new Range();
                    r1.min = range.min;
                    r1.max = yMin;
                    worldTile.columns[x].ranges.RemoveAt(i);
                    worldTile.columns[x].ranges.Insert(i, r1);
                }
            }
        }
    }
}
