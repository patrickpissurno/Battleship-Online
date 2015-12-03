﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class MainController : MonoBehaviour {

    private static MainController instance;

    [HideInInspector]
    public static string User = null;
    [HideInInspector]
    public static string Pass = null;
    [HideInInspector]
    public bool Signed = false;
    [HideInInspector]
    public bool Waiting = false;

    [HideInInspector]
    public GameController gameController;
    [HideInInspector]
    public PlayerController playerController;
    public InputField user;
    public InputField pass;

    public string Player1;
    public string Player2;
    public string MatchId;
    public bool IsPlayer1 = false;

    public Text MainGameTitle;
    private Text MainGameMessage;

    public Text MapSetupMessage;

    public int Attacks = 3;

    public bool GameEnded = false;

    public const string SERVER_ADDRESS = "127.0.0.1/battleship.php";

    void Awake()
    {
        instance = this;
    }

    void GetComp()
    {
        gameController = GetComponent<GameController>();
        playerController = GetComponent<PlayerController>();
        MainGameMessage = MainGameTitle.transform.Find("ErrorMessage").GetComponent<Text>();
    }
	void Start () {
        GetComp();
        if(User != null && Pass != null)
            StartCoroutine(Login(User, Pass));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void LoginButton()
    {
        StartCoroutine(Login(user.text, pass.text));
    }
    public IEnumerator Login(string user, string pass)
    {
        yield return new WaitForSeconds(0f);
        if (!Waiting)
        {
            Waiting = true;
            WWWForm form = new WWWForm();
            form.AddField("user", user);
            form.AddField("pass", pass);

            WWW w = new WWW(SERVER_ADDRESS + "?act=check_pass", form);
            yield return w;
            if (!string.IsNullOrEmpty(w.error))
            {
                print(w.error);
            }
            else
            {
                switch (w.text)
                {
                    case "1":
                        User = user;
                        Pass = pass;
                        Signed = true;
                        print("Signed-in");
                        StartCoroutine(WriteOnline());
                        ScreenController.Change("MainMenu");
                        break;
                    case "0":
                        User = "";
                        Pass = "";
                        Signed = false;
                        print("Invalid Pass");
                        break;
                    case "-1":
                        User = "";
                        Pass = "";
                        Signed = false;
                        print("User not fount");
                        break;

                    default:
                        print(w.text);
                        break;
                }
            }
            Waiting = false;
        }
    }

    private IEnumerator WriteOnline()
    {
        yield return new WaitForSeconds(.25f);
        WWWForm form = new WWWForm();
        form.AddField("user", User);

        WWW w = new WWW(SERVER_ADDRESS + "?act=write_user_online", form);
        yield return w;
        StartCoroutine(WriteOnline());
    }

    public void Play()
    {
        GameEnded = false;
        MainGameMessage.text = "";
        MapSetupMessage.text = "";
        CellController.ClearSelection();
        CellController.Locked = false;
        CellController.Attack = false;
        ScreenController.Change("Matchmaking");
        StartCoroutine(FindMatch());
    }

    public IEnumerator FindMatch()
    {
        yield return new WaitForSeconds(0f);
        if (!Waiting)
        {
            Waiting = true;
            WWWForm form = new WWWForm();
            form.AddField("user", User);

            WWW w = new WWW(SERVER_ADDRESS + "?act=matchmaking", form);
            yield return w;
            if (!string.IsNullOrEmpty(w.error))
            {
                print(w.error);
            }
            else
            {
                print(w.text);
                string[] datas = w.text.Split(';');
                foreach (string data in datas)
                {
                    string[] keypair = data.Split('=');
                    switch (keypair[0])
                    {
                        case "player1":
                            if (keypair[1].Equals(User))
                            {
                                ScreenController.Change("WaitingForPlayer");
                                StartCoroutine(WaitForPlayer());
                                IsPlayer1 = true;
                            }
                            Player1 = keypair[1];
                            break;
                        case "player2":
                            if (keypair[1].Equals(User))
                            {
                                ScreenController.Change("MapSetup");
                                gameController.enabled = true;
                                playerController.enabled = true;
                                IsPlayer1 = false;
                                StartCoroutine(ReadMatchStatus());
                            }
                            Player2 = keypair[1];
                            break;
                        case "matchid":
                            MatchId = keypair[1];
                            break;
                    }
                }
            }
            Waiting = false;
        }
    }

    private IEnumerator WaitForPlayer()
    {
        yield return new WaitForSeconds(.5f);
        if (!GameEnded)
        {
            WWWForm form = new WWWForm();
            form.AddField("id", MatchId);

            WWW w = new WWW(SERVER_ADDRESS + "?act=read_match", form);
            yield return w;
            if (!string.IsNullOrEmpty(w.error))
            {
                print(w.error);
            }
            else
            {
                print(w.text);
                string[] datas = w.text.Split(';');
                foreach (string data in datas)
                {
                    string[] keypair = data.Split('=');
                    switch (keypair[0])
                    {
                        case "player2":
                            if (!string.IsNullOrEmpty(keypair[1]))
                            {
                                ScreenController.Change("MapSetup");
                                gameController.enabled = true;
                                playerController.enabled = true;
                                StartCoroutine(ReadMatchStatus());
                            }
                            else
                                StartCoroutine(WaitForPlayer());
                            Player2 = keypair[1];
                            break;
                    }
                }
            }
        }
    }

    public IEnumerator WriteMap()
    {
        yield return new WaitForSeconds(0f);
        if (!GameEnded)
        {
            WWWForm form = new WWWForm();
            form.AddField("id", MatchId);
            form.AddField("user", User);
            form.AddField("map", Ship.Serialize(gameController.ships.ToArray()));

            WWW w = new WWW(SERVER_ADDRESS + "?act=write_match_map", form);
            yield return w;
            if (!string.IsNullOrEmpty(w.error))
            {
                print(w.error);
            }
            else
            {
                if (!w.text.Equals("-1"))
                {
                    StartCoroutine(SetReady());
                }
            }
        }
    }

    public IEnumerator SetReady()
    {
        yield return new WaitForSeconds(.2f);
        if (!GameEnded)
        {
            WWWForm form = new WWWForm();
            form.AddField("id", MatchId);
            form.AddField("user", User);

            WWW w = new WWW(SERVER_ADDRESS + "?act=write_match_ready", form);
            yield return w;
            if (!string.IsNullOrEmpty(w.error))
            {
                print(w.error);
            }
            else
            {
                StartCoroutine(WaitForReady());
            }
        }
    }

    public static void SendMap()
    {
        instance.StartCoroutine(instance.WriteMap());
    }

    public IEnumerator WaitForReady()
    {
        yield return new WaitForSeconds(.5f);
        if (!GameEnded)
        {
            WWWForm form = new WWWForm();
            form.AddField("id", MatchId);

            WWW w = new WWW(SERVER_ADDRESS + "?act=read_match", form);
            yield return w;
            if (!string.IsNullOrEmpty(w.error))
            {
                print(w.error);
            }
            else
            {
                print(w.text);
                string[] datas = w.text.Split(';');
                bool p1_ready = false;
                bool p2_ready = false;
                string map = null;
                foreach (string data in datas)
                {
                    string[] keypair = data.Split('=');
                    switch (keypair[0])
                    {
                        case "p1_ready":
                            p1_ready = keypair[1] == "1" ? true : false;
                            break;
                        case "p2_ready":
                            p2_ready = keypair[1] == "1" ? true : false;
                            break;
                        case "map1":
                            if (!IsPlayer1)
                                map = keypair[1].Replace('$', '=').Replace('&', ';');
                            break;
                        case "map2":
                            if (IsPlayer1)
                                map = keypair[1].Replace('$', '=').Replace('&', ';');
                            break;
                    }
                }
                if (p1_ready && p2_ready)
                {
                    GameEnded = false;
                    CellController.ClearSelection();
                    ScreenController.Change("MainGame");
                    MainGameTitle.text = IsPlayer1 ? "Seu turno" : "Turno do oponente";
                    if (!IsPlayer1)
                        StartCoroutine(WaitForTurn());
                    CellController.Locked = !IsPlayer1;
                    CellController.Attack = true;
                    if (map != null)
                    {
                        LoadMap(map);
                    }
                }
                else
                {
                    StartCoroutine(WaitForReady());
                }
            }
        }
    }

    public void LoadMap(string str)
    {
        gameController.ships.Clear();
        string[] ships = str.Split(';');
        foreach (string ship in ships)
        {
            string[] ship_datas = ship.Split('|');
            Ship newShip = new Ship(0);
            List<CellController> cellList = new List<CellController>();
            foreach (string ship_data in ship_datas)
            {
                string[] keypair = ship_data.Split('=');
                switch (keypair[0])
                {
                    case "amount":
                        newShip.PieceAmount = int.Parse(keypair[1]);
                        break;
                    case "pieces":
                        string[] pieces = keypair[1].Split(',');
                        foreach (string piece in pieces)
                        {
                            string[] p = piece.Split(':');
                            CellController c = playerController.Cells[int.Parse(p[1])][int.Parse(p[0])];
                            c.EnemyCell = true;
                            cellList.Add(c);
                        }
                        newShip.Cells = cellList.ToArray();
                        break;
                }
            }
            gameController.ships.Add(newShip);
        }
    }

    IEnumerator WaitForTurn()
    {
        print("Executed");
        yield return new WaitForSeconds(.5f);
        if (!GameEnded)
        {
            WWWForm form = new WWWForm();
            form.AddField("id", MatchId);

            WWW w = new WWW(SERVER_ADDRESS + "?act=read_match", form);
            yield return w;
            if (!string.IsNullOrEmpty(w.error))
            {
                print(w.error);
            }
            else
            {
                print(w.text);
                string[] datas = w.text.Split(';');
                foreach (string data in datas)
                {
                    string[] keypair = data.Split('=');
                    switch (keypair[0])
                    {
                        case "turn":
                            if (keypair[1].Equals(User))
                            {
                                if (MainGameTitle.text != "Seu turno")
                                {
                                    Attacks = 3;
                                    CellController.Locked = false;
                                    MainGameTitle.text = "Seu turno";
                                }
                            }
                            else
                                StartCoroutine(WaitForTurn());
                            break;
                        case "winner":
                            if (!string.IsNullOrEmpty(keypair[1]))
                            {
                                CellController.Locked = true;
                                GameEnded = true;
                                MainGameTitle.text = "Você perdeu!";
                                StartCoroutine(ReturnToMenu());
                            }
                            break;
                    }
                }
            }
        }
    }

    public static void DoAttack()
    {
        if (!instance.GameEnded)
        {
            if (instance.Attacks > 1)
            {
                instance.Attacks--;
                //Check if ship length > 0, to declare victory
                instance.StartCoroutine(instance.CheckVictory());
            }
            else
            {
                CellController.Locked = true;
                instance.MainGameTitle.text = "Turno do oponente";
                instance.StartCoroutine(instance.NextTurn());
            }
        }
    }

    public IEnumerator CheckVictory()
    {
        yield return new WaitForSeconds(0);
        if (instance.gameController.ships.Count == 1)
        {
            CellController.Locked = true;
            GameEnded = true;
            instance.MainGameTitle.text = "Você ganhou!";
            instance.StartCoroutine(instance.DeclareVictory());
            StartCoroutine(ReturnToMenu());
        }
    }

    public IEnumerator NextTurn()
    {
        yield return new WaitForSeconds(.2f);
        if (!GameEnded)
        {
            WWWForm form = new WWWForm();
            form.AddField("id", MatchId);
            form.AddField("user", User);

            WWW w = new WWW(SERVER_ADDRESS + "?act=write_match_endturn", form);
            yield return w;
            if (!string.IsNullOrEmpty(w.error))
            {
                print(w.error);
            }
            else
            {
                StartCoroutine(WaitForTurn());
            }
        }
    }

    public IEnumerator DeclareVictory(bool quit = false)
    {
        yield return new WaitForSeconds(.2f);
        WWWForm form = new WWWForm();
        form.AddField("id", MatchId);
        form.AddField("user", !quit ? User : (IsPlayer1 ? Player2 : Player1));

        WWW w = new WWW(SERVER_ADDRESS + "?act=write_match_winner", form);
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
        {
            print(w.error);
        }
        else
            Debug.LogWarning(w.text);
    }

    private IEnumerator ReadMatchStatus()
    {
        yield return new WaitForSeconds(1f);
        WWWForm form = new WWWForm();
        form.AddField("id", MatchId);

        WWW w = new WWW(SERVER_ADDRESS + "?act=read_match", form);
        yield return w;
        if (!string.IsNullOrEmpty(w.error))
        {
            print(w.error);
        }
        else
        {
            string[] datas = w.text.Split(';');
            foreach (string data in datas)
            {
                string[] keypair = data.Split('=');
                switch (keypair[0])
                {
                    case "status":
                        if (keypair[1] == "closed" && !GameEnded)
                        {
                            //Match Error
                            CellController.Locked = true;
                            ScreenController.Change("MainGame");
                            GameObject.Find("Map").SetActive(false);
                            instance.MainGameTitle.text = "Empate (desconexão)";
                            StartCoroutine(ReturnToMenu());
                        }
                        else
                            StartCoroutine(ReadMatchStatus());
                        break;
                }
            }
        }
    }

    private IEnumerator ReturnToMenu()
    {
        if (!Waiting)
        {
            Waiting = true;
            for (int i = 3; i > 0; i--)
            {
                MainGameMessage.text = "Voltando ao menu em " + i;
                yield return new WaitForSeconds(1f);
            }
            ScreenController.Change("MainMenu");
            Application.LoadLevel(0);
            Waiting = false;
        }
    }
}
