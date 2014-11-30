using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	public Transform laserProjectilePrefab;
	public float sprayrate;
	public float maxTime = 3f;
	
	public AudioClip[] audioFire;
	public AudioClip[] audioLoop;
	public AudioClip[] audioStop;
	
	bool firing;
	float startTime;
	
	public void Fire() {
		if (!firing) StartCoroutine(FireRoutine());
	}

	private IEnumerator FireRoutine() {
		firing = true;
		startTime = Time.time;
				
		PlayRandomSound(audioFire, transform.position);
		
		int i = Random.Range(0, audioLoop.Length);
		audio.clip = audioLoop[i];
		audio.loop = true;
		audio.Play();
		
		Transform projectiletr = Instantiate(
			laserProjectilePrefab,
			transform.position,
			transform.rotation) as Transform;
		LaserProjectile projectile = projectiletr.GetComponent<LaserProjectile>();
		
		// ensure at least two points are fired
		projectile.AddPoint(transform.position);
		ScreenShake.Instance.Shake(0.2f, 1.5f);
		yield return new WaitForSeconds(1f/sprayrate);
		projectile.AddPoint(transform.position);
		ScreenShake.Instance.Shake(0.2f, 1.5f);
		
		// continue adding points while button is held down;
		while (Input.GetButton("Fire1") && GameManager.Instance.IsPlaying && Time.time < startTime + maxTime) {
			yield return new WaitForSeconds(1f/sprayrate);
			ScreenShake.Instance.Shake(0.2f, 1.5f);
			projectile.AddPoint(transform.position);
		}
		audio.Stop();
		yield return new WaitForSeconds(1f);
		PlayRandomSound(audioStop, transform.position);
		firing = false;
	}
	
	private float PlayRandomSound(AudioClip[] library, Vector3 position)
	{
		int i = Random.Range(0, library.Length);
		AudioSource.PlayClipAtPoint(library[i], position);
		return library[i].length;
	}

}
