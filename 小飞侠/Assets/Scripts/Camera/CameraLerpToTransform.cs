
using UnityEngine;
using System.Collections;

public class CameraLerpToTransform : MonoBehaviour
{

	public Transform camTarget;
	public float trackingSpeed;
	public float cameraZDepth = -10f;
	public float minX;
	public float minY;
	public float maxX;
	public float maxY;

	void FixedUpdate ()
	{
		if (camTarget != null) {
			var newPos = Vector2.Lerp (
				                      transform.position,
				                      camTarget.position,
				                      Time.deltaTime * trackingSpeed);
			
			var camPosition = new Vector3 (newPos.x, newPos.y, -10f);
			var v3 = camPosition;
			var clampX = Mathf.Clamp (v3.x, minX, maxX);
			var clampY = Mathf.Clamp (v3.y, minY, maxY);

			transform.position = new Vector3 (clampX, clampY, -10f);
		}
	}
}
