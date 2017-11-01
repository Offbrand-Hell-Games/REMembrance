using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour {
    public GameObject door;
    public GameObject pivot;
    

    bool touched = false;
    string touchedBy;
    enum DoorStates { closed, open };

    

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
	void FixedUpdate ()
    {
        if(touched)
        {
            float currentRotation = door.transform.localEulerAngles.y;
            if (touched)
            {
                if (doorState == DoorStates.closed)
                {
                    door.transform.RotateAround(pivot.transform.position, Vector3.up, 1f);
                    
                    if (Mathf.Floor(currentRotation) == openRotation)
                    {
                        touched = false;
                        doorState = DoorStates.open;
                    }
                    //
                }
                if (doorState == DoorStates.open)
                {
                    door.transform.RotateAround(pivot.transform.position, Vector3.up, -1f);
                    if (Mathf.Floor(currentRotation) == closedRotation)
                    {
                        touched = false;
                        doorState = DoorStates.closed;
                    }
                    //
                }

            }
        }
    }

    void ChangeDoorState()
    {
        touched = true;
    }
}
