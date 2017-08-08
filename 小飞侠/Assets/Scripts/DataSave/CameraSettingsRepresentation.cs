using System;

/**
 * 相机设置
 */ 
[Serializable]
public class CameraSettingsRepresentation
{
    public string cameraTrackTarget;  // 目标
    public float trackingSpeed;       // 追踪目标的速度
    public float cameraZDepth;        // 相机深度
    public float minX;                // 允许移动的水平最左点
	public float minY;                // 允许移动的垂直最上点
	public float maxX;                // 允许移动的水平最右点
	public float maxY;                // 允许移动的垂直最下点
}