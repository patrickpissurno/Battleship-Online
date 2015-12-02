using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class MainController : MonoBehaviour {

    private static MainController instance;

    [HideInInspector]
    public string User;
    [HideInInspector]
    public string Pass;
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

    public const string SERVER_ADDRESS = "127.0.0.1/battleship.php";

    void Awake()
    {
        instance = this;
    }

    void GetComp()
    {
        gameController = GetComponent<GameController>();
        playerController = GetComponent<PlayerController>();
    }
	void Start () {
        GetComp();
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

    public void Play()
    {
        ScreenController.Change("Matchmaking");
        StartCoroutine(FindMatch());
        //gameController.enabled = true;
        //playerController.enabled = true;
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
                        }
                        else
                            StartCoroutine(WaitForPlayer());
                        Player2 = keypair[1];
                        break;
                }
            }
        }
    }

    public IEnumerator WriteMap()
    {
        yield return new WaitForSeconds(0f);
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

    public IEnumerator SetReady()
    {
        yield return new WaitForSeconds(.2f);
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

    public static void SendMap()
    {
        instance.StartCoroutine(instance.WriteMap());
    }

    public IEnumerator WaitForReady()
    {
        yield return new WaitForSeconds(.5f);
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
                        if(!IsPlayer1)
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
                CellController.ClearSelection();
                ScreenController.Change("MainGame");
                CellController.Locked = false;
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
}
