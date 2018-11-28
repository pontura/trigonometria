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
		Events.OnSliderValueChange += OnSliderValueChange;
		Events.CloseSubMenu += SetOff;
	}

	void OnDestroy(){
		Events.OnButtonClickd -= OnButtonClickd;
		Events.OnSliderValueChange -= OnSliderValueChange;
		Events.CloseSubMenu -= SetOff;
	}

	void OnSliderValueChange(UIScaleSlider uiSlider){
		switch (uiSlider.type) {
		case UIScaleSlider.types.SCALE_X:
			Game.Instance.redimensionManager.ScaleX (uiSlider.slider.value, uiSlider.slider.minValue, uiSlider.slider.maxValue);
			break;
		case UIScaleSlider.types.SCALE_Y:
			Game.Instance.redimensionManager.ScaleY (uiSlider.slider.value, uiSlider.slider.minValue, uiSlider.slider.maxValue);
			break;
		case UIScaleSlider.types.SCALE_Z:
			Game.Instance.redimensionManager.ScaleZ (uiSlider.slider.value, uiSlider.slider.minValue, uiSlider.slider.maxValue);
			break;
		}
	}
	void OnButtonClickd(UIButton uiButton)
	{
		switch (uiButton.type) {
		case UIButton.types.MOVE_X_IN:
			Game.Instance.shapeMove.Move (Quaternion.Euler(Game.Instance.board.CameraRot)*new Vector3 (1, 0, 0));			
			translateButton.Select();
			break;
		case UIButton.types.MOVE_X_OUT:
			Game.Instance.shapeMove.Move (Quaternion.Euler(Game.Instance.board.CameraRot)*new Vector3 (-1, 0, 0));
			translateButton.Select();
			break;
		case UIButton.types.MOVE_Z_IN:
			Game.Instance.shapeMove.Move (Quaternion.Euler(Game.Instance.board.CameraRot)*new Vector3 (0, 0, 1));
			translateButton.Select();
			break;
		case UIButton.types.MOVE_Z_OUT:
			Game.Instance.shapeMove.Move (Quaternion.Euler(Game.Instance.board.CameraRot)*new Vector3 (0, 0, -1));
			translateButton.Select();
			break;

		case UIButton.types.ROTATE_UP:
			if(Game.Instance.board.selectedShape!=null)
			Game.Instance.shapeMove.Rotate (90);
			rotateButton.Select();
			break;
		case UIButton.types.ROTATE_DOWN:
			if(Game.Instance.board.selectedShape!=null)
			Game.Instance.shapeMove.Rotate (-90);
			rotateButton.Select();
			break;
		case UIButton.types.MOVE_Y_UP:
			if(Game.Instance.board.selectedShape!=null)
			Game.Instance.shapeMove.Move (Quaternion.Euler(Game.Instance.board.CameraRot)*new Vector3 (0, 1, 0));
			translateButton.Select();
			break;
		case UIButton.types.MOVE_Y_DOWN:
			if(Game.Instance.board.selectedShape!=null)
			Game.Instance.shapeMove.Move (Quaternion.Euler(Game.Instance.board.CameraRot)*new Vector3 (0, -1, 0));
			translateButton.Select();
			break;
		}
	}
	public void SetOff()
	{
		rotatePanel.SetActive (false);
		translatePanel.SetActive (false);
	}
	public void SetOff(UIButton.types type)
	{
		if(type!=UIButton.types.ROTATE)rotatePanel.SetActive (false);
		if(type!=UIButton.types.TRANSLATE)translatePanel.SetActive (false);
	}
	public void SetOn(UIButton.types type)
	{
		SetOff (type);

		switch (type) {
		case UIButton.types.TRANSLATE:
				translatePanel.SetActive (!translatePanel.activeSelf);
				break;
		case UIButton.types.ROTATE:
			rotatePanel.SetActive (!rotatePanel.activeSelf);
			break;
		}
	}
}
