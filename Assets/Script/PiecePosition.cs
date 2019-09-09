using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PiecePosition : NetworkBehaviour
{
	public GameObject current_square;
	float move_speed = 0.05f;
	public int ID;
	[SyncVar]
	bool moving = true;
	bool dead = false;
	List<GameObject> valid_moves = new List<GameObject>();
	// Start is called before the first frame update
	void Start()
	{
		current_square.GetComponent<SquareInfo>().occupant = GetComponent<PieceInfo>().id;
	}
	void FixedUpdate()
	{
		if (moving)
		{
			if (transform.position.x != current_square.transform.position.x || transform.position.z != current_square.transform.position.z)
			{
				if (transform.position.y < current_square.transform.position.y + 2)
				{
					Vector3 new_pos = transform.position;
					new_pos.y = new_pos.y + move_speed;
					transform.SetPositionAndRotation(new_pos, transform.rotation);
				}
				else MoveXZ();
			}
			else if (transform.position.y > current_square.transform.position.y + 1)
			{
				Vector3 new_pos = transform.position;
				new_pos.y = new_pos.y - move_speed;
				if (new_pos.y < current_square.transform.position.y + 1) new_pos.y = current_square.transform.position.y + 1;
				transform.SetPositionAndRotation(new_pos, transform.rotation);
			}
			else moving = false;
		}
		if (dead && transform.position.y < 9999)
		{
			Vector3 new_pos = transform.position;
			new_pos.y = new_pos.y + move_speed;
			transform.SetPositionAndRotation(new_pos, transform.rotation);
		}
		//Debug.DrawRay(transform.position, Vector3.forward);
	}
	[ClientRpc]
	public void RpcMoveTo(int x, int y)
	{
		current_square.GetComponent<SquareInfo>().Abandon();
		GridInfo.grid[x][y].GetComponent<SquareInfo>().Occupy(gameObject.GetComponent<PieceInfo>().id);
		current_square = GridInfo.grid[x][y];
		moving = true;
	}

	void MoveXZ()
	{
		Vector3 new_pos = transform.position;
		if (transform.position.x != current_square.transform.position.x)
		{
			float x_diff = current_square.transform.position.x - transform.position.x;
			if (Mathf.Abs(x_diff) > move_speed) x_diff = move_speed * x_diff / Mathf.Abs(x_diff);
			new_pos.x = new_pos.x + x_diff;
		}
		if (transform.position.z != current_square.transform.position.z)
		{
			float z_diff = current_square.transform.position.z - transform.position.z;
			if (Mathf.Abs(z_diff) > move_speed) z_diff = move_speed * z_diff / Mathf.Abs(z_diff);
			new_pos.z = new_pos.z + z_diff;
		}
		transform.SetPositionAndRotation(new_pos, transform.rotation);
	}
	/// <summary>
	/// returns true if space is empty and false if not empty. Either way will add space to valid moves if it is valid
	/// </summary>
	bool CheckMovePossible(int x, int y)
	{
		if (x > 7 || x < 0 || y > 7 || y < 0) return false;
		GameObject target_square = GridInfo.grid[x][y];
		int occupant_id = target_square.GetComponent<SquareInfo>().GetOccupant();
		if (GridInfo.grid[x][y].GetComponent<SquareInfo>().GetOccupant() != -1)
		{
			if (GetComponent<PieceInfo>().piece_type == "pawn" && current_square.GetComponent<SquareInfo>().x == x) return false;
			if (GetComponent<PieceInfo>().team != GridInfo.pieces[occupant_id].GetComponent<PieceInfo>().team) valid_moves.Add(target_square);
			return false;
		}
		if (GetComponent<PieceInfo>().piece_type == "pawn" && current_square.GetComponent<SquareInfo>().x != x) return true;
		valid_moves.Add(target_square);
		return true;
	}
	void BishopMoveCheck()
	{
		int x = current_square.GetComponent<SquareInfo>().x;
		int y = current_square.GetComponent<SquareInfo>().y;
		//up and right
		for (int r = x + 1, s = y + 1; r < 8 && s < 8; r++, s++) if (!CheckMovePossible(r, s)) break;
		//down and left
		for (int r = x - 1, s = y - 1; r > -1 && s > -1; r--, s--) if (!CheckMovePossible(r, s)) break;
		//down and right
		for (int r = x + 1, s = y - 1; r > -1 && s > -1; r++, s--) if (!CheckMovePossible(r, s)) break;
		//up and left
		for (int r = x - 1, s = y + 1; r > -1 && s > -1; r--, s++) if (!CheckMovePossible(r, s)) break;
	}
	void RookMoveCheck()
	{
		int x = current_square.GetComponent<SquareInfo>().x;
		int y = current_square.GetComponent<SquareInfo>().y;
		//check up
		for (int n = y + 1; n < 8; n++) if (!CheckMovePossible(x, n)) break;
		//check down
		for (int n = y - 1; n > -1; n--) if (!CheckMovePossible(x, n)) break;
		//check left
		for (int n = x - 1; n > -1; n--) if (!CheckMovePossible(n, y)) break;
		//check right
		for (int n = x + 1; n < 8; n++) if (!CheckMovePossible(n, y)) break;
	}
	public void FindValidMoves()
	{
		valid_moves.Clear();
		int x = current_square.GetComponent<SquareInfo>().x;
		int y = current_square.GetComponent<SquareInfo>().y;
		if (GetComponent<PieceInfo>().piece_type == "rook") RookMoveCheck();
		else if (GetComponent<PieceInfo>().piece_type == "bishop") BishopMoveCheck();
		else if (GetComponent<PieceInfo>().piece_type == "queen")
		{
			RookMoveCheck();
			BishopMoveCheck();
		}
		else if (GetComponent<PieceInfo>().piece_type == "knight")
		{
			CheckMovePossible(x + 2, y + 1);
			CheckMovePossible(x + 2, y - 1);
			CheckMovePossible(x - 2, y + 1);
			CheckMovePossible(x - 2, y - 1);
			CheckMovePossible(x + 1, y + 2);
			CheckMovePossible(x + 1, y - 2);
			CheckMovePossible(x - 1, y + 2);
			CheckMovePossible(x - 1, y - 2);
		}
		else if (GetComponent<PieceInfo>().piece_type == "king")
		{
			CheckMovePossible(x - 1, y - 1);
			CheckMovePossible(x, y - 1);
			CheckMovePossible(x + 1, y - 1);
			CheckMovePossible(x - 1, y);
			CheckMovePossible(x + 1, y);
			CheckMovePossible(x - 1, y + 1);
			CheckMovePossible(x, y + 1);
			CheckMovePossible(x + 1, y + 1);
		}
		else if (GetComponent<PieceInfo>().piece_type == "pawn")
		{
			if (GetComponent<PieceInfo>().team == 1)
			{
				CheckMovePossible(x - 1, y + 1);
				CheckMovePossible(x, y + 1);
				CheckMovePossible(x + 1, y + 1);
			}
			else
			{
				CheckMovePossible(x - 1, y - 1);
				CheckMovePossible(x, y - 1);
				CheckMovePossible(x + 1, y - 1);
			}
		}
	}
	public List<GameObject> getValidMoves()
	{
		return valid_moves;
	}
	public void Die()
	{
		dead = true;
		moving = false;
		GetComponent<PieceInfo>().team = 0;
	}
	public bool IsValidMove(int x, int y)
	{
		return valid_moves.Contains(GridInfo.grid[x][y].gameObject);
	}
}
