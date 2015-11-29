using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
    public PlayerController playerController;
    public List<Ship> ships;
	void Start () {
        ships = new List<Ship>();
        playerController = GetComponent<PlayerController>();
        ShipAmount.Init();
	}
	
	void Update () {
	
	}

    public void Finish()
    {
        switch (playerController.ValidateMap())
        {
            case 404:
                Debug.LogError("Formação inválida");
                break;
            case 100:
                Debug.LogError("Posicione todos os navios");
                break;
            default:
                Debug.Log("Mapa OK");
                break;
        }
    }
}
