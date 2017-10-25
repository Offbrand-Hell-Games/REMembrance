using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {
    public GameObject door;
    public GameObject pivot;

    bool touched = false;
    string touchedBy;

    //The starting rotation of the door in the Y direction
    //float startingRotation;
    //float openInwardsTo;
    //float openOutwardsTo;
    enum DoorStates { closed, open };
    DoorStates doorState;
    // Use this for initialization
    void Start ()
    {
        //startingRotation = door.transform.localEulerAngles.y;
        //openOutwardsTo = startingRotation - 90f;
        //openInwardsTo = startingRotation - 90f;
        
        doorState = DoorStates.closed;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (touched)
        {
            if(doorState == DoorStates.closed)
            {
                door.transform.RotateAround(pivot.transform.position, Vector3.up, -50f * Time.deltaTime);
                if((door.transform.localEulerAngles.y != 0) && (door.transform.localEulerAngles.y < 270))
                {
                    touched = false;
                    doorState = DoorStates.open;
                    print(door.transform.localEulerAngles.y);
                }
                //
            }
            if (doorState == DoorStates.open)
            {
                door.transform.RotateAround(pivot.transform.position, Vector3.up, 50f * Time.deltaTime);
                if ((door.transform.localEulerAngles.y != 0) && (door.transform.localEulerAngles.y < 270))
                {
                    touched = false;
                    doorState = DoorStates.closed;
                    print(door.transform.localEulerAngles.y);
                }
                //
            }

        }
    }

    void ChangeDoorState()
    {

        touched = true;
    }
}
