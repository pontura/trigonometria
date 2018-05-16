using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShpaesManager : MonoBehaviour {

	UIMenu uiMenu;

	void Start () {
		uiMenu = GetComponent<UIMenu> ();
		Events.OnButtonClickd += OnButtonClickd;
	}	
	void OnButtonClickd(UIButton uiButton)
	{
		switch (uiButton.type) {
		case UIButton.types.ADD:
			uiMenu.SetOn ();
			OnShapeClicked(uiButton.id);
			break;
		}
	}
	public void OnShapeClicked(int id)
	{
		Game.Instance.board.AddNewShape (id);
	}

}
