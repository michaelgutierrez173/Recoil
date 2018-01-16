using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour {

    private static DontDestroy instanceRef;

    void Awake()
    {
        if (instanceRef == null)
        {
            instanceRef = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(SceneManager.GetActiveScene().name == "LobbySceneBetter"){
            Destroy(gameObject);
        }
	}
}
