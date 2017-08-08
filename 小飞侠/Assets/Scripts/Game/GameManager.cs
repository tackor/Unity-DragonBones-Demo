using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;
using UnityEditor;

public class GameManager : MonoBehaviour
{

	public static GameManager instance;

	private string selectedLevel;

	void Awake ()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
	}

	// Use this for initialization
	void Start () {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public void RestartLevel (float delay)
	{
		StartCoroutine (RestartLevelDelay (delay));
	}

	private IEnumerator RestartLevelDelay (float delay)
	{
		yield return new WaitForSeconds (delay);
		SceneManager.LoadScene ("Game");
	}

	void Update ()
	{
//        if (Input.GetKeyDown(KeyCode.Escape))
//        {
//            SceneManager.LoadScene("Menu");
//        }
	}

	public void SaveLevel ()
	{
		
		var levelRoot = GameObject.Find ("Level");
		var ldr = new LevelDataRepresentation ();
		var levelItems = new List<LevelItemRepresentation> ();

		foreach (Transform t in levelRoot.transform) {
			var sr = t.GetComponent<SpriteRenderer> ();
			var li = new LevelItemRepresentation () {
				position = t.position,
				rotation = t.rotation.eulerAngles,
				scale = t.localScale
			};
			if (t.name.Contains (" ")) {
				li.prefabName = t.name.Substring (0, t.name.IndexOf (" "));
			} else {
				li.prefabName = t.name;
			}
			if (sr != null) {
				li.spriteLayer = sr.sortingLayerName;
				li.spriteColor = sr.color;
				li.spriteOrder = sr.sortingOrder;
			}
			levelItems.Add (li);
		}

		ldr.levelItems = levelItems.ToArray ();
		ldr.playerStartPosition = GameObject.Find ("Swordsman").transform.position;

		var currentCamSettings = FindObjectOfType<CameraLerpToTransform> ();
		if (currentCamSettings != null) {
			ldr.cameraSettings = new CameraSettingsRepresentation () {
				cameraTrackTarget = currentCamSettings.camTarget.name,
				cameraZDepth = currentCamSettings.cameraZDepth,
				minX = currentCamSettings.minX,
				minY = currentCamSettings.minY,
				maxX = currentCamSettings.maxX,
				maxY = currentCamSettings.maxY,
				trackingSpeed = currentCamSettings.trackingSpeed
			};
		}

		var levelDataToJson = JsonUtility.ToJson (ldr);
		var savePath = System.IO.Path.Combine (Application.dataPath, Application.loadedLevelName + ".json");
		System.IO.File.WriteAllText (savePath, levelDataToJson);
		Debug.Log ("Level saved to " + savePath);

	}

	// 当场景被加载时执行该方法
	private void OnSceneLoaded (Scene scene, LoadSceneMode loadsceneMode)
	{

		Debug.Log ("111");

		// 当前只有一个 .json 文件
		var levelFiles = Directory.GetFiles (Application.dataPath, "*.json");

		if (levelFiles.Length > 0) {
			
			selectedLevel = levelFiles [0];
		} else {

			selectedLevel = null;
		}

		// 如果 selectedLevel 存在, 并且该场景是 Game 那么就恢复之前保存的数据
		if (!string.IsNullOrEmpty (selectedLevel) && scene.name == "Game") {
			Debug.Log ("Loading level content for: " + selectedLevel);
			LoadLevelContent ();
		}

		// 如果是 Menu 场景, 就设置UI
//		if (scene.name == "Menu") DiscoverLevels();
	}

	// 加载数据
	private void LoadLevelContent ()
	{
		Debug.Log ("level name = " + selectedLevel);

		var existingLevelRoot = GameObject.Find ("Level");
		Destroy (existingLevelRoot);

		var levelRoot = new GameObject ("Level");
		var levelFileJsonContent = File.ReadAllText (selectedLevel);
		var levelData = JsonUtility.FromJson<LevelDataRepresentation> (levelFileJsonContent);
		foreach (var li in levelData.levelItems) {
			
			var pieceResource = Resources.Load (@"Prefabs/" + li.prefabName);
			if (pieceResource == null) {
				Debug.LogError ("Cannot find resource: " + li.prefabName);
			}

			var piece = (GameObject)Instantiate (pieceResource, li.position, Quaternion.identity);
			var pieceSprite = piece.GetComponent<SpriteRenderer> ();
			if (pieceSprite != null) {
				pieceSprite.sortingOrder = li.spriteOrder;
				pieceSprite.sortingLayerName = li.spriteLayer;
				pieceSprite.color = li.spriteColor;
			}
			piece.transform.parent = levelRoot.transform;
			piece.transform.position = li.position;
			piece.transform.rotation = Quaternion.Euler (li.rotation.x, li.rotation.y, li.rotation.z);
			piece.transform.localScale = li.scale;
		}

		var Swordsman = GameObject.Find ("Swordsman");
		Swordsman.transform.position = levelData.playerStartPosition;
		Debug.Log ("位置: " + levelData.playerStartPosition);

		Camera.main.transform.position = new Vector3 (
			Swordsman.transform.position.x, 
			Swordsman.transform.position.y,
			Camera.main.transform.position.z);

		var camSettings = FindObjectOfType<CameraLerpToTransform> ();

		if (camSettings != null) {
			camSettings.cameraZDepth = levelData.cameraSettings.cameraZDepth;
			camSettings.camTarget = GameObject.Find (levelData.cameraSettings.cameraTrackTarget).transform;
			camSettings.maxX = levelData.cameraSettings.maxX;
			camSettings.maxY = levelData.cameraSettings.maxY;
			camSettings.minX = levelData.cameraSettings.minX;
			camSettings.minY = levelData.cameraSettings.minY;
			camSettings.trackingSpeed = levelData.cameraSettings.trackingSpeed;
		}
	}

}
