using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Author: Joseph Lipinski
/// Controls whether a door iss open or closed
/// Updated 11/13/17

public class DoorController : MonoBehaviour {
    //The Door gameObject that the to which the script is attached
    public GameObject door;
    //The child gameObject of the door gameObject
    public GameObject pivot;
    
    //A boolean to control the door state
    bool touched = false;
    string touchedBy;
    
    enum DoorStates { closed, open };
	[SerializeField]
	public float doorOpenSpeed = 10f;

    //Two floats
    //closedRotation is the angle of the door intially
    //openRotation is the angle of the door after it's been swung open
    float closedRotation, openRotation;
    DoorStates doorState = DoorStates.closed;
    // Use this for initialization
    void Start ()
    {
        closedRotation = door.transform.localEulerAngles.y;
        if(closedRotation > 270f)
        {
            openRotation = 360 - closedRotation;
        }
        else
        {
            openRotation = closedRotation + 90f;
        }
	}
	
	// Update is called once per frame
    // Simply handles opening and closing the door
	void FixedUpdate ()
    {
        if(touched)
        {
            float currentRotation = door.transform.localEulerAngles.y;
            if (touched)
            {
                Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Door"), true);
                if (doorState == DoorStates.closed)
                {
                    door.transform.RotateAround(pivot.transform.position, Vector3.up, doorOpenSpeed);
                    
                    if (Mathf.Floor(currentRotation) == openRotation)
                    {
                        touched = false;
                        doorState = DoorStates.open;
                        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Door"), false);
                    }
                    //
                }
                if (doorState == DoorStates.open)
                {
                    door.transform.RotateAround(pivot.transform.position, Vector3.up, -doorOpenSpeed);
                    if (Mathf.Floor(currentRotation) == closedRotation)
                    {
                        touched = false;
                        doorState = DoorStates.closed;
                        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Door"), false);
                    }
                }

            }
        }
    }


    /// <summary> A sendMessage reciever for openin and closing doors </summary>
    void ChangeDoorState()
    {
        Debug.Log("Door: " + name + " recieved instruction to open.");
        touched = true;
    }
}
