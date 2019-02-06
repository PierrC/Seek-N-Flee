using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        Screen.SetResolution(600, 450, false);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartDemo() {
        SceneManager.LoadScene(1);
    }

    public void LinkToGithub() {
        Application.OpenURL("https://github.com/PierrC/HTN_Demo");
    }
}
