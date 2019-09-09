using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    public List<GameObject> owned_pieces;
    public int team = 1;
    static int players = 0;
    int selected_piece = -1;
    GameObject Opponent;
    [SyncVar]
    bool myturn = false;
	public Camera cam;
    // Start is called before the first frame update
    void Awake()
    {
        if (players == 2) players = 0;
        team = ++players;
        foreach (GameObject pos_piece in GameObject.FindGameObjectsWithTag("Piece"))
        {
            if (pos_piece.GetComponent<PieceInfo>().team == team)
            {
                owned_pieces.Add(pos_piece);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (myturn && Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (selected_piece != -1 && hit.transform.gameObject.tag == "Square")
                    {
						//initiate move

						if (TryMovePiece(selected_piece, hit.transform.gameObject.GetComponent<SquareInfo>().x, hit.transform.gameObject.GetComponent<SquareInfo>().y))
						{
							Deselect();
							CmdEndTurn();
						}
						else Deselect();
                    }
                    else if (hit.transform.root.gameObject.tag == "Piece" && hit.transform.root.gameObject.GetComponent<PieceInfo>().team == team)
                    {
                        Deselect();
                        selected_piece = hit.transform.root.gameObject.GetComponent<PieceInfo>().id;
                        HighlightOptions();
                    }
					else if(selected_piece != -1 && hit.transform.root.gameObject.tag == "Piece" && hit.transform.root.gameObject.GetComponent<PieceInfo>().team != team)
					{
						if (TryMovePiece(selected_piece, hit.transform.root.gameObject.GetComponent<PiecePosition>().current_square.GetComponent<SquareInfo>().x, hit.transform.root.gameObject.GetComponent<PiecePosition>().current_square.GetComponent<SquareInfo>().y))
						{
							CmdEndTurn();
						}
						Deselect();
					}
                }
                else
                {
                    Deselect();
                }
            }
        }
        if (!(Opponent != null) && players == 2)
        {
            FindOpponent();
        }
    }
	bool TryMovePiece(int p, int x, int y)
	{
		print(selected_piece + " or " + p + " to " + x + "," + y);
		if (!GridInfo.pieces[p].GetComponent<PiecePosition>().IsValidMove(x, y)) return false;
		CmdMovePiece(p, x, y);
		return true;
	}
    [Command]
    void CmdMovePiece(int p, int x, int y)
    {
		GridInfo.pieces[p].GetComponent<PiecePosition>().RpcMoveTo(x, y);
    }
    [Command]
    void CmdEndTurn()
    {
        RpcEndTurn();
    }
    [ClientRpc]
    void RpcEndTurn()
    {
        print("Player" + team + "pass turn");
        myturn = false;
        Opponent.GetComponent<Player>().CmdBeginTurn();
    }
    [Command]
    void CmdBeginTurn()
    {
        RpcBeginTurn();
    }
    [ClientRpc]
    void RpcBeginTurn()
    {
        print("Player " +team + " turn begin");
        myturn = true;
        foreach (GameObject piece in owned_pieces)
        {
            piece.GetComponent<PiecePosition>().FindValidMoves();
        }
    }
    void FindOpponent()
    {
        if (Opponent != null) return;
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(player != gameObject)
            {
                Opponent = player;
                break;
            }
        }
        if (team == 1) CmdBeginTurn();
    }
    void HighlightOptions()
    {
        foreach (GameObject square in GridInfo.pieces[selected_piece].GetComponent<PiecePosition>().getValidMoves())
        {
            square.GetComponent<SpaceColor>().BecomeRed();
        }
    }
    void Deselect()
    {
        UnHighlight();
        selected_piece = -1;
    }
    void UnHighlight()
    {
        if(selected_piece != -1)
        {
            foreach (GameObject square in GridInfo.pieces[selected_piece].GetComponent<PiecePosition>().getValidMoves())
            {
                square.GetComponent<SpaceColor>().ResetColor();
            }
        }
    }

}
