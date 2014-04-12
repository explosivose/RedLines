using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour 
{
	public Vector3 offset;
	public Vector3 defaultRotation;
	public Material linemat;
	private Transform player;
	private Player playerScript;
	
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerScript = player.GetComponent<Player>();
	}
	
	
	void Update () 
	{
		if (Player.isDead)
		{
			Quaternion rotation = Quaternion.LookRotation(player.position - transform.position);
			transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 4f);
		}
		else
		{
			switch(playerScript.controlMode)
			{
			case Player.ControlMode.mouseFromCenter:
				transform.position = player.position + offset;
				break;
			case Player.ControlMode.mouseRotation:
				Vector3 target = player.position + offset;
				transform.position = Vector3.Lerp (transform.position, target, Time.deltaTime * 8f);
				break;
			}
			
		}

	}
	void OnPostRender()
	{
		if (playerScript.controlMode == Player.ControlMode.mouseFromCenter)
		{
			Vector3 center = Vector3.one/2f;
			Vector3 mouse = new Vector3(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height);
			
			GL.PushMatrix();
			linemat.SetPass(0);
			GL.LoadOrtho();
			GL.Begin (GL.LINES);
			GL.Color (Color.red);
			GL.Vertex(center);
			GL.Vertex(mouse);
			GL.End ();
			GL.PopMatrix();
		}
	}
}
