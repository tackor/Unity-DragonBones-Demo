
using UnityEngine;
using System;

[Serializable]
public class LevelItemRepresentation
{
	
	// 位置
	public Vector2 position;

	// 旋转
    public Vector3 rotation;

	// 缩放
    public Vector3 scale;

	// 预制体的名称
    public string prefabName;

	// 层级
	public string spriteLayer;

	// 显示次序
    public int spriteOrder;

	// 颜色
    public Color spriteColor;

}