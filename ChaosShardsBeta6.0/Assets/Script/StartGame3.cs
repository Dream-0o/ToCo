﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartGame3 : MonoBehaviour {

    public GameObject NewSceneManager;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void OnClick()
    {
        Debug.Log("第3关");
        NewSceneManager.GetComponent<NewSceneName>().NewScene("dialogues2");
        SceneManager.LoadScene("talk");
    }
}
