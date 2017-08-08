using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransationScript : MonoBehaviour
{
	//swan的移动速度.
	public float _speed_H = 0f;
	public float _speed_W = 0f;

	const float FINTIME = 2.5f;
	private float _delTime = FINTIME;
	private float isUp = 1;
	
	// Update is called once per frame
	void Update ()
	{
		_delTime -= Time.deltaTime;


		if (_delTime >= 0) {
			
			isUp = 1;
		} else if (_delTime >= -FINTIME) {
			isUp = -1;
		}
		else {
			_delTime = FINTIME;
		}

		gameObject.transform.Translate (new Vector3 (_speed_H * isUp, _speed_W * isUp, 0), Space.World);

	}
}
