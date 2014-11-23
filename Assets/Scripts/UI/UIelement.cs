using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class UIelement : MonoBehaviour {

	public bool useLocalizedString = true;
	public Strings.guiIndex localizedString;
	public string finalText;
	public bool typeEffect;
	public bool overrideTextColor;
	public Color textColor;
	public Color textHoverColor;
	
	public int width {
		get {
			return Mathf.RoundToInt(boxCollider.size.x * 100f);
		}
	}
	
	public int height {
		get {
			return Mathf.RoundToInt(boxCollider.size.y * 100f);
		}
	}
	
	// zero means top edge meets the top of the CTRL
	public int top {
		get {
			return Mathf.RoundToInt(-transform.localPosition.y * 100f);
		}
		set {
			Vector3 localpos = transform.localPosition;
			localpos.y = -(float)value/100f;
			transform.localPosition = localpos;
		}
	}
	
	public int left {
		get {
			return Mathf.RoundToInt(transform.localPosition.x * 100f);
		}
		set {
			Vector3 localpos = transform.localPosition;
			localpos.x = (float)value/100f;
			transform.localPosition = localpos;
		}
	}
	
	public string text {
		get {
			return textMesh.text;
		}
		set {
			textMesh.text = value;
		}
	}
	
	private Vector3 initialPosition;
	private BoxCollider boxCollider;
	protected TextMesh textMesh {get; private set;}
	protected bool typing; 
	
	protected virtual void Awake()
	{
		if (useLocalizedString)
			finalText = Strings.guiTable[(int)localizedString];
		boxCollider = GetComponent<BoxCollider>();
		initialPosition = transform.localPosition;
		textMesh = transform.Find("text").GetComponent<TextMesh>();
		textMesh.font = UI.font;
		if (!overrideTextColor)
		{
			textColor = UI.fontColor;
			textHoverColor = UI.fontHighlightColor;
		}
	}
	
	protected virtual void Start()
	{
	
	}
	
	protected virtual void Update()
	{
	
	}
	
	protected virtual void OnEnable()
	{
		textMesh.color = textColor;
		if (typeEffect) StartCoroutine( TypeText() );
		else text = finalText;
	}
	
	protected virtual void OnDisable() 
	{
		transform.localPosition = initialPosition;
	}
	
	protected virtual void OnMouseEnter() 
	{
		textMesh.color = textHoverColor;
	}
	
	protected virtual void OnMouseExit() 
	{
		textMesh.color = textColor;
	}
	
	protected virtual void OnMouseDown() 
	{
		transform.localPosition = initialPosition - Vector3.forward * 0.1f;
		//MouseLook.freeze = true;
	}
	
	protected virtual void OnMouseUp() 
	{
		transform.localPosition = initialPosition;
		//MouseLook.freeze = false;
	}
	
	protected virtual void OnMouseUpAsButton()
	{
	
	}
	
	protected IEnumerator TypeText() 
	{
		int _typeIndex = 0;
		text = "";
		typing = true;
		while(typing && _typeIndex < finalText.Length) {
			text += finalText[_typeIndex++];
			yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
		}
		typing = false;
	}
}
