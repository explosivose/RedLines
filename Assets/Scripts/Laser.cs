using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	public Transform laserProjectilePrefab;
	public float sprayrate;
	public float maxTime = 3f;
	
	bool firing;
	float startTime;
	
	public void Fire() {
		if (!firing) StartCoroutine(FireRoutine());
	}

	private IEnumerator FireRoutine() {
		firing = true;
		startTime = Time.time;
		
		Transform projectiletr = Instantiate(
			laserProjectilePrefab,
			transform.position,
			transform.rotation) as Transform;
		LaserProjectile projectile = projectiletr.GetComponent<LaserProjectile>();
		while (Input.GetButton("Fire1") && GameManager.Instance.IsPlaying && Time.time < startTime + maxTime) {
			ScreenShake.Instance.Shake(0.2f, 1.5f);
			projectile.AddPoint(transform.position);
			yield return new WaitForSeconds(1f/sprayrate);
		}
		firing = false;
	}

}
