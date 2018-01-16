using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyIfScene : MonoBehaviour {

    public string sceneToDestroyOn;
	
	// Update is called once per frame
	void Update () {
        if(SceneManager.GetActiveScene().name == sceneToDestroyOn){
            Destroy(gameObject);
        }
	}
}
