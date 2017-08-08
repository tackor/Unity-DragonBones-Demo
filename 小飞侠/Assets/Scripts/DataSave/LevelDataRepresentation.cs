using UnityEngine;
using System;

/**
 * 等级数据
 */ 
[Serializable]
public class LevelDataRepresentation
{
	// 玩家的位置
    public Vector2 playerStartPosition;

	// 相机的设置
    public CameraSettingsRepresentation cameraSettings;

	// 场景中其他非生命物体的数据
    public LevelItemRepresentation[] levelItems;
}