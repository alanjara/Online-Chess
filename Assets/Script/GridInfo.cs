using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInfo : MonoBehaviour
{
    public static List<List<GameObject>> grid = new List<List<GameObject>>();
    public static List<GameObject> pieces = new List<GameObject>();
    // Start is called before the first frame update
    void Awake()
    {

        for (int x = 0; x < 8; x++)
        {
            grid.Add(new List<GameObject>());
            for (int y = 0; y < 8; y++)
            {
                GameObject t = new GameObject();
                grid[x].Add(t);
                Destroy(t);
            }
        }
        for (int x = 0; x < 32; x++)
        {
            GameObject t = new GameObject();
            pieces.Add(t);
            Destroy(t);
        }
    }
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
