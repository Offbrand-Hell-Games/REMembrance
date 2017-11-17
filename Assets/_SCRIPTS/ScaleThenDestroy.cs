using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author: Corwin Belser
/// Attach to a GameObject to expand it until reaching a max distance
public class ScaleThenDestroy : MonoBehaviour {

    public float MAX_DISTANCE = 15f; /* Max size the GameObject should reach before being destroyed */
    public float EXPANSION_SPEED = 10f; /* Speed in m/s the GameObject should expand at */
	
    /// <summary>
    /// Called every frame. Destroys the GameObject if the size exceeds MAX_DISTANCE. Otherwise scales the GameObject
    /// </summary>
	void FixedUpdate () {
        Vector3 scale = this.transform.localScale;
        if (scale.magnitude > MAX_DISTANCE * 2)
            GameObject.Destroy(this.gameObject);
        else
        {
            Debug.Log("Magnitude: " + this.transform.localScale.magnitude + ", scale.x: " + scale.x);
            float increase = EXPANSION_SPEED * Time.deltaTime;
            this.transform.localScale = new Vector3(scale.x + increase, scale.y + increase, scale.z + increase);
        }
	}
}
