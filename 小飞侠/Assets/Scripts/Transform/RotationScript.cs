using UnityEngine;
using System.Collections;

public class RotationScript : MonoBehaviour {

    public float rotationsPerMinute = 640f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 0, rotationsPerMinute * Time.deltaTime,
            Space.Self);
    }
}
