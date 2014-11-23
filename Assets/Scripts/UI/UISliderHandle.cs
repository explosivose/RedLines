using UnityEngine;
using System.Collections;

public class UISliderHandle : UIelement {

	public enum Action {
		MasterVolume,
		MusicVolume
	}
	
	public float value {
		get {
			float oldMax = parent.width - width;
			float oldMin = 0;
			float oldRange = oldMax - oldMin;
			float newRange = maxValue - minValue;
			return (((left - oldMin) * newRange) / oldRange) + minValue;
		}
		set {
			float oldRange = maxValue - minValue;
			float newMax = parent.width - width;
			float newMin = 0;
			float newRange = newMax - newMin;
			left = Mathf.RoundToInt((((value - minValue) * newRange) / oldRange) + newMin);
		}
	}
	
	public Action action;
	public float minValue = 0f;
	public float maxValue = 1f;
	
	private bool slide;
	private float startPos;
	private UIelement parent;
	private bool applicationIsQuitting = false;
	
	void OnApplicationQuit() {
		applicationIsQuitting = true;
	}
	
	protected override void Awake ()
	{
		base.Awake ();
		parent = transform.parent.GetComponent<UIelement>();
	}
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		switch(action) {
		default:
		case Action.MasterVolume:
			value = Options.masterVolume;
			break;
		case Action.MusicVolume:
			value = Options.musicVolume;
			break;
		}
	}
	
	protected override void OnDisable ()
	{
		base.OnDisable ();
		if (!applicationIsQuitting)
			Options.SaveSettings();
	}
	
	protected override void Update() {
		parent.text = value.ToString("F2");
		if (slide) {
			float move = startPos - Input.mousePosition.x;
			int nextPos = left - Mathf.RoundToInt(move);
			nextPos = Mathf.Min(nextPos, parent.width - width);
			nextPos = Mathf.Max(nextPos, 0);
			left = nextPos;
			startPos = Input.mousePosition.x;
			
			switch(action) {
			default:
			case Action.MasterVolume:
				Options.masterVolume = value;
				break;
			case Action.MusicVolume:
				Options.musicVolume = value;
				break;
			}
		}
		else {
			switch(action) {
			default:
			case Action.MasterVolume:
				value = Options.masterVolume;
				break;
			case Action.MusicVolume:
				value = Options.musicVolume;
				break;
			}
		}
	}
	
	protected override void OnMouseDown ()
	{
		slide = true;
		startPos = Input.mousePosition.x;
	}
	
	protected override void OnMouseUp ()
	{
		slide = false;
	}
	
	
}
