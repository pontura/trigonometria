using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISubMenu : MonoBehaviour {

	public Button rotateButton;
	public Button translateButton;

	public GameObject rotatePanel;
	public GameObject translatePanel;

	void Start () {
		SetOff ();
		Events.OnButtonClickd += OnButtonClickd;
	}
	void OnButtonClickd(UIButton uiButton)
	{
		switch (uiButton.type) {
		case UIButton.types.MOVE_X_IN:
			Game.Instance.board.Move (new Vector3 (1, 0, 0));
			translateButton.Select();
			break;
		case UIButton.types.MOVE_X_OUT:
			Game.Instance.board.Move (new Vector3 (-1, 0, 0));
			translateButton.Select();
			break;
		case UIButton.types.MOVE_Z_IN:
			Game.Instance.board.Move (new Vector3 (0, 0, 1));
			translateButton.Select();
			break;
		case UIButton.types.MOVE_Z_OUT:
			Game.Instance.board.Move (new Vector3 (0, 0, -1));
			translateButton.Select();
			break;

		case UIButton.types.ROTATE_UP:
			Game.Instance.board.Rotate (90);
			rotateButton.Select();
			break;
		case UIButton.types.ROTATE_DOWN:
			Game.Instance.board.Rotate (-90);
			rotateButton.Select();
			break;
		}
	}
	public void SetOff()
	{
		rotatePanel.SetActive (false);
		translatePanel.SetActive (false);
	}
	public void SetOn(UIButton.types type)
	{
		SetOff ();

		switch (type) {
		case UIButton.types.TRANSLATE:
			translatePanel.SetActive (true);
			break;
		case UIButton.types.ROTATE:
			rotatePanel.SetActive (true);
			break;
		}
	}
}
