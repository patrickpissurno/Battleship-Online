using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
    public static GameController instance;
    public Text ErrorMsg;
    [HideInInspector]
    public PlayerController playerController;
    public List<Ship> ships;

    void Awake()
    {
        instance = this;
    }

	void Start () {
        CellController.Locked = false;
        CellController.Attack = false;
        ships = new List<Ship>();
        playerController = GetComponent<PlayerController>();
        ShipAmount.Init();
	}

    public void Finish()
    {
        switch (playerController.ValidateMap())
        {
            case 404:
                ErrorMsg.text = "Formação inválida";
                CellController.ClearSelection();
                break;
            case 100:
                ErrorMsg.text = "Posicione todos os navios";
                break;
            default:
                ErrorMsg.text = "Aguardando o outro jogador...";
                MainController.SendMap();
                CellController.Locked = true;
                break;
        }
    }
}
