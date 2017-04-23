using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
    {
    public float rotationRate = 2.0f;

    void Update()
        {
        foreach (Touch touch in Input.touches)
            {
            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                Debug.Log("Touching at: " + touch.position);

                if (touch.phase == TouchPhase.Began)
                    {
                    Debug.Log("Touch phase began at: " + touch.position);
                    }
                else if (touch.phase == TouchPhase.Moved)
                    {
                    Debug.Log("Touch phase Moved");
                    transform.Rotate(touch.deltaPosition.y * rotationRate,
                                        -touch.deltaPosition.x * rotationRate, 0, Space.World);
                    }
                else if (touch.phase == TouchPhase.Ended)
                    {
                    Debug.Log("Touch phase Ended");
                    }
                }
            }
        }
    }