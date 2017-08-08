using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

	// Use this for initialization
	void Start () {

		List<string> btnsName = new List<string>();  
//		btnsName.Add("Button1");  
		btnsName.Add("Button2");  
//		btnsName.Add("Button3");

		foreach(string btnName in btnsName)  
		{  
			GameObject btnObj = GameObject.Find(btnName);  
			Button btn = btnObj.GetComponent<Button>();  
			btn.onClick.AddListener(delegate() {  
				this.OnClick(btnObj);   
			});  
		} 
	}
	
	public void OnClick(GameObject sender)  
	{  

		switch (sender.name)  
		{  
		case "Button1":  
			break;  
		case "Button2":
			SceneManager.LoadScene ("Game");
			break;  
		case "Button3":  
			break;  
		default:  
			Debug.Log("none");  
			break;  
		}  
	}  
}
