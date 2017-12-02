using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMover : MonoBehaviour {

    void LateUpdate ()
    {
        transform.Translate(0f, 0f, -Time.deltaTime * 3f);
        //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (Time.deltaTime * 5f));
    }
}
