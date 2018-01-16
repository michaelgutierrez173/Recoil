using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Prototype.NetworkLobby
{
    public class ChooseMap : NetworkBehaviour
    {
        
        public LobbyManager lm;
        public string[] scenes;
        public Dropdown d;

        [SyncVar]
        private int index = 0;

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            d.value = index;
            if (!isServer){
                d.enabled = false;
            }
        }

        public void OnDropdownChange(int map){
            Debug.Log("Dropdown change to map: " + scenes[map]);
            lm.playScene = scenes[map];
            index = map;
        }
    }
}
