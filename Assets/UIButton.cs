using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour  {

	public types type;
	public int id;
	public Image[] toColirize;

	public enum types
	{
		TRANSLATE,
		ROTATE,
		BREAK,
		DESTROY,
		MOVE_X_IN,
		MOVE_X_OUT,
		MOVE_Z_IN,
		MOVE_Z_OUT,
		ROTATE_UP,
		ROTATE_DOWN,
		ADD,
		ZOOM,
		ROTATE_CAMERA,
		MOVE_Y_UP,
		MOVE_Y_DOWN
	}
	void Start()
	{
		Events.OnShapeSelected += OnShapeSelected;
		GetComponent<Button>().onClick.AddListener(TaskOnClick);
	}
	void OnShapeSelected(ShapeAsset sa)
	{
		foreach (Image t in toColirize) {
			if((Game.Instance.board.selectedShape.childs.Count==0 && type == types.BREAK)||sa == null)
				t.color = Color.grey;			
			else
				t.color = sa.color;
		}

	}

	void TaskOnClick()
	{
		Events.OnButtonClickd (this);
	}
}
