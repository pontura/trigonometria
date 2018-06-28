using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScaleSlider : MonoBehaviour {

	public types type;
	public Image[] toColirize;
	public Slider slider;

	public enum types
	{
		SCALE_X,
		SCALE_Y,
		SCALE_Z
	}
	void Start()
	{
		slider = GetComponent<Slider>();
		slider.onValueChanged.AddListener(OnSliderValueChange);
	}

	void OnSliderValueChange(float val)
	{
		Events.OnSliderValueChange (this);
	}
}
