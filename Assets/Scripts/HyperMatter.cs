﻿using UnityEngine;
using System.Collections;

public class HyperMatter : MonoBehaviour 
{
	public Transform hyperDustPrefab;
	public Transform hyperMatterHintPrefab;
	public Transform explosion;
	public float minSpeedFactor = 0.75f;
	public float maxSpeedFactor = 1.25f;
	public AudioClip[] audioExplosions;
	private float speed;
	private Vector3 initialScale;
	private Transform hyperMatterHint;
	private bool provideHint = false;
	
	void Start()
	{
		float cubeSpeed = CubeMaster.Instance.cubeSpeed;
		speed = Random.Range(minSpeedFactor * cubeSpeed, maxSpeedFactor * cubeSpeed);
		initialScale = transform.localScale;
		Destroy(this.gameObject, 30f);
		
		if (GameManager.Instance.ShowHints)
		{
			hyperMatterHint = Instantiate(hyperMatterHintPrefab) as Transform;
			provideHint = true;
		}
		transform.parent.audio.Play();
	}
	
	void Update()
	{
		float hyperfactor = 1f;
		if (CubeMaster.Instance.HyperJump) hyperfactor = 10f;
		transform.parent.position += Vector3.back * speed * hyperfactor * Time.deltaTime;
		Vector3 scale = initialScale * (Mathf.Sin (Time.time*4f)/4f + 1f);
		transform.parent.localScale = scale;
		transform.parent.Rotate(Vector3.one * hyperfactor);
		if (provideHint)
		{
			hyperMatterHint.position = transform.parent.position + Vector3.back * transform.parent.localScale.x;
		}
	}
	
	public void Explode()
	{
		Instantiate(explosion, transform.parent.position, Random.rotation);
		ScoreBoard.CurrentScore += 1000 * Player.Instance.HyperMultiplier;
		int x = Random.Range(0, audioExplosions.Length);
		AudioSource.PlayClipAtPoint(audioExplosions[x], transform.parent.position);
		
		for (int i = 0; i < Random.Range(6, 12); i++)
		{
			Vector3 pos = transform.parent.position + Random.insideUnitSphere;
			Transform dust = Instantiate(hyperDustPrefab, pos, Quaternion.identity) as Transform;
			//dust.parent = transform;
		}
		
		if (provideHint)
		{
			provideHint = false;
			Destroy(hyperMatterHint.gameObject);
		}
		
		renderer.enabled = false;
		transform.parent.collider.enabled = false;
		transform.parent.FindChild("Hologram").renderer.enabled = false;
		transform.parent.audio.Stop();
	}
	
}
