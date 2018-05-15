using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShpaesManager : MonoBehaviour {


	public void OnShapeClicked(int id)
	{
		Game.Instance.board.AddNewShape (id);
	}
	public void Rotate(int qty)
	{
		Game.Instance.board.Rotate (qty);
	}
	public void MoveInX(int qty)
	{
		Game.Instance.board.Move (new Vector3(qty,0,0));
	}
	public void MoveInZ(int qty)
	{
		Game.Instance.board.Move (new Vector3(0,0,qty));
	}

}
