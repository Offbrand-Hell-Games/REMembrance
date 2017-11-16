using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author: John Harvey
/// Controls whether a blinking
/// Updated 11/15/17
public class Blink_light : MonoBehaviour
{

    //The Light gameObject to which the script is attached to
    public Light spot_Light;
    //parameters to control blink rate
    public float minWaitTime;
    public float maxWaitTime;

    void Start()
    {
        //grabbing the gameobjects light component
        spot_Light = GetComponent<Light>();
        //Blinking coroutine
        StartCoroutine(Blinking());
    }

    IEnumerator Blinking()
    {
        while (true)
        {
            //This waits for a random assigned range of seconds
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            //turns light on and off
            spot_Light.enabled = !spot_Light.enabled;

        }
    }
}