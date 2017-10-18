using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {
    public GameObject door;
    public GameObject pivot;

    bool touched = false;
    string touchedBy;

    //The starting rotation of the door in the Y direction
    float startingRotation;
    float openInwardsTo;
    float openOutwardsTo;

    enum DoorStates { closed, open };
    DoorStates doorState;
    // Use this for initialization
    void Start ()
    {
        startingRotation = door.transform.rotation.y;
        openOutwardsTo = startingRotation - 0.3f;
        openInwardsTo = startingRotation + 0.3f;
        
        doorState = DoorStates.closed;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (touched)
        {
            if(doorState == DoorStates.closed)
            {
                if (door.transform.rotation.y < openInwardsTo)
                {
                    door.transform.RotateAround(pivot.transform.position, Vector3.up, -40 * Time.deltaTime);
                }
                else
                {
                    touched = false;
                    doorState = DoorStates.open;
                }
            }
            if (doorState == DoorStates.open)
            {
                if (door.transform.rotation.y  > startingRotation)
                {
                    door.transform.RotateAround(pivot.transform.position, Vector3.up, 40 * Time.deltaTime);
                }
                else
                {
                    touched = false;
                    doorState = DoorStates.closed;
                }
            }
        }
    }

    void ChangeDoorState()
    {

        touched = true;
    }
}
