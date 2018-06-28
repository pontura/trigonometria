using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour {

	UISubMenu submenu;
	public GameObject ShapesMenu;
	public GameObject RedimenMenu;
	public GameObject buttonsContainer;
	public Text message;
	public Dropdown mechanicsSelector;
	bool isOn;

	void Start () {
		SetOff ();
		submenu = GetComponent<UISubMenu> ();
		Events.OnButtonClickd += OnButtonClickd;
		Events.OnMessageShow += ShowMessage;
	}
	void OnDestroy(){
		Events.OnButtonClickd -= OnButtonClickd;
		Events.OnMessageShow -= ShowMessage;
	}
	public void SetOff()
	{
		isOn = false;
		foreach (Button button in buttonsContainer.GetComponentsInChildren<Button>())
			button.interactable = false;
	}
	public void SetOn()
	{
		if (isOn)
			return;
		isOn = true;
		foreach (Button button in buttonsContainer.GetComponentsInChildren<Button>())
			button.interactable = true;
	}
	void OnButtonClickd(UIButton uiButton)
	{
		switch (uiButton.type) {
		case UIButton.types.TRANSLATE:
			Move();
			break;
		case UIButton.types.ROTATE:
			Rotate();
			break;
		case UIButton.types.BREAK:
			Break();
			break;
		case UIButton.types.DESTROY:
			DestroyShape();
			break;
		case UIButton.types.ZOOM:
			Zoom();
			break;
		case UIButton.types.ROTATE_CAMERA:
			RotateCamera();
			break;
		}
	}
	public void Move()
	{
		submenu.SetOn (UIButton.types.TRANSLATE);
	}
	public void Rotate()
	{
		submenu.SetOn (UIButton.types.ROTATE);
	}
	public void Break()
	{
		Game.Instance.board.BreakShape ();
		submenu.SetOff();
	}
	public void DestroyShape()
	{
		Game.Instance.board.DestroyShape ();
		submenu.SetOff();
	}
	bool up = true;
	float zoomvalue = 2;
	void Zoom()
	{
		if (zoomvalue == 1 && up)
			zoomvalue = 2;
		else if (zoomvalue == 2 && up) {
			zoomvalue = 3;
			up = false;
		} else if (zoomvalue == 3 && !up)
			zoomvalue = 2;
		else if (zoomvalue == 2 && !up) {
			zoomvalue = 1;
			up = true;
		}
		Events.OnZoom (zoomvalue);
	}
	void RotateCamera()
	{
		Events.OnCameraRotate ();
	}

	void ShowMessage(string s){
		message.text = s;
		Invoke ("CleanMessage", 5);
	}

	void CleanMessage(){
		message.text = "";
	}

	public void OnMechanicChange(){
		switch (mechanicsSelector.value) {
		case 0:
			ShapesMenu.SetActive (true);
			RedimenMenu.SetActive (false);
			submenu.SetOff ();
			Events.OnMechanicChange (Board.MechanicStates.INTEGRAR);
			break;
		case 1:
			ShapesMenu.SetActive (true);
			RedimenMenu.SetActive (false);
			submenu.SetOff ();
			Events.OnMechanicChange (Board.MechanicStates.COMBINAR);
			break;
		case 2:
			ShapesMenu.SetActive (false);
			RedimenMenu.SetActive (true);
			submenu.SetOff ();
			Events.OnMechanicChange (Board.MechanicStates.REDIMENSIONAR);
			break;
		}
	}
}
