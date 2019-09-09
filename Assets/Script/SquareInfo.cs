using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class SquareInfo : NetworkBehaviour
{
    [SyncVar]
    public int x = 0;
    [SyncVar]
    public int y = 0;
    [SyncVar]
    public int occupant = -1;
    // Start is called before the first frame update
    private void Start()
    {
        GridInfo.grid[x][y] = gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetOccupant()
    {
        return occupant;
    }
    public void Occupy(int new_occupant)
    {
        if (occupant != -1) GridInfo.pieces[occupant].GetComponent<PiecePosition>().Die();
        occupant = new_occupant;
    }
    public void Abandon()
    {
        occupant = -1;
    }
}
