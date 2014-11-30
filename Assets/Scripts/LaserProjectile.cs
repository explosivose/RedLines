using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LaserProjectile : MonoBehaviour {

	public float speed = 10f;
	public Transform linePointPrefab;
	public Transform laserHitPrefab;
	// keep track of our points
	private List<Transform> _points = new List<Transform>();
	private LineRenderer _line;
	
	void Awake() {
		_line = GetComponent<LineRenderer>();
		Color startC = new Color(1f, Random.value, Random.value);
		Color endC = new Color(1f, Random.value, Random.value);
		_line.SetColors(startC, endC);
		StartCoroutine(CleanUp());
	}
	
	public void AddPoint(Vector3 position, Quaternion rotation) {
		Transform instance = Instantiate(
			linePointPrefab, 
			position, 
			rotation) as Transform;
		instance.parent = transform;
		instance.light.color = new Color(1f, Random.value, Random.value);
		_points.Add(instance);
		_line.SetVertexCount(_points.Count);
		for(int i = 1; i < _points.Count; i++) {
			_line.SetPosition(i, _points[i].position);
			_line.SetPosition(i-1, _points[i-1].position);
		}
	}
	
	// move point rigidbodies
	void FixedUpdate() {
		foreach(Transform p in _points) {
			p.rigidbody.velocity = speed * p.forward + Random.insideUnitSphere;
		}
	}
	
	// draw line between points and explore hypermatter along the line
	void Update() {
		RaycastHit hit;
		for(int i = 1; i < _points.Count; i++) {
			_line.SetPosition(i, _points[i].position);
			_line.SetPosition(i-1, _points[i-1].position);
			if (Physics.Linecast(_points[i].position, _points[i-1].position, out hit)) {
				if (hit.transform.tag == "HyperMatter")
					hit.transform.BroadcastMessage("Explode");
				Instantiate(laserHitPrefab, hit.point, Quaternion.LookRotation(hit.normal));
			}
		}
	}
	
	IEnumerator CleanUp() {
		yield return new WaitForSeconds(10f);
		foreach(Transform p in _points)
			Destroy(p.gameObject);
		Destroy(this.gameObject);
	}
}
