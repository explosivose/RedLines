﻿using UnityEngine;
using System.Collections;

public class Cleaner : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		Destroy(other.gameObject);
	}
}
