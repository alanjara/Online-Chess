using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class PieceInfo : NetworkBehaviour
{
    public string piece_type = "pawn";
    public float piece_value = 0;
    public int team = 0;
    static int num_pieces = 0;
    [SyncVar]
    public int id;
    // Start is called before the first frame update
    void Awake()
    {
        id = num_pieces++;
        if (num_pieces > 32) num_pieces = 0;
    }
    private void Start()
    {
        GridInfo.pieces[id] = gameObject;
    }
}
