
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GridBuilder : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject prefabSnapPoint;
    //GameObject[,] gridObjects;

    public int width;
    public int height;
    public int cellSize = 1;

    //int howManyWillDinamicShow = 9;

    List<GameObject> gridSnaps;
    
    List<Plane> planes;
    void Start()
    {
        gridSnaps = new List<GameObject>();

        Renderer renderer_ref = gameObject.GetComponent<Renderer>();
        width = (int)Math.Round(renderer_ref.bounds.size.x / cellSize);
        height = (int)Math.Round(renderer_ref.bounds.size.z / cellSize);

        renderer_ref.material.mainTextureScale = new Vector2(width/2, height/2);

        planes = new List<Plane>
        {
            new Plane(Vector3.up, Vector3.zero)
        };
    }
    
    // Update is called once per frame
    void Update()
    {

    }

    public Vector3 GetClosestPositionToGrid(Ray rayCast)
    {
        float distance = -1;
        Plane hitedPlane = planes.FirstOrDefault<Plane>(m_plane => m_plane.Raycast(rayCast, out distance) && distance>0);
        if (distance > 0)
        {
            Vector3 worldPosition = rayCast.GetPoint(distance);
            Vector3 gridPosition = GetClosestPointToGrid(worldPosition);

            GerencyGridSnaps(gridPosition);
            
            return gridPosition;
        }        
        return Vector3.zero;
    }


    private void GerencyGridSnaps(Vector3 gridPosition)
    {
        var snapPositions = GetNeighbors(gridPosition);
        snapPositions.Add(gridPosition);


        snapPositions.ForEach(snapPos => {
            if (!gridSnaps.Select(gb => gb.transform.position).Contains(snapPos))
            {
                var obj = Instantiate(prefabSnapPoint, snapPos, Quaternion.identity);
                    
                gridSnaps.Add(obj);

            }
        });
        
        gridSnaps.RemoveAll(gb =>
        {
            bool toRemove = false;
            var socket = gb.GetComponent<XRSocketInteractor>();

            toRemove = !snapPositions.Contains(gb.transform.position) && socket.interactablesSelected.Count == 0;
            if (toRemove)
                GameObject.Destroy(gb);

            return toRemove;
        });

        gridSnaps.ForEach(gb => { gb.GetComponent<Renderer>().material.color = Color.white; });
        gridSnaps.Find(gb=> gb.transform.position == gridPosition).GetComponent<Renderer>().material.color = Color.blue;


    }

    private Vector3 GetClosestPointToGrid(Vector3 position)
    {
        var x = Mathf.Round(position.x / cellSize)*cellSize;
        var z = Mathf.Round(position.z / cellSize)*cellSize;
        return new Vector3(x,0,z);
    }

    private List<Vector3> GetNeighbors(Vector3 origin)
    {
        var vec = new List<Vector3>();
        vec.Add(origin - new Vector3(cellSize ,0, cellSize));
        vec.Add(origin - new Vector3(0        ,0, cellSize));
        vec.Add(origin - new Vector3(-cellSize,0, cellSize));

        vec.Add(origin - new Vector3(-cellSize,0, 0));
        vec.Add(origin - new Vector3(0, 0, cellSize));

        vec.Add(origin - new Vector3(-cellSize, 0, -cellSize));
        vec.Add(origin - new Vector3(0, 0, -cellSize));
        vec.Add(origin - new Vector3(cellSize, 0, -cellSize));
        return vec;
    }
}
