using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour  {

	public types type;
	public int id;

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
		ADD
	}
	void Start()
	{
		GetComponent<Button>().onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick()
	{
		Events.OnButtonClickd (this);
	}
}
