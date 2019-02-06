using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

    public int number = 1;
    public int inputedNumber = 3;

	// Use this for initialization
	void Start () {
        Debug.Log(number);
	}

    public void PrintInputNumber() {
        Debug.Log("Inputed Number:" + inputedNumber);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
