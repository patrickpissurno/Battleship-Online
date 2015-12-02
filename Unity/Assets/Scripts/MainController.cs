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
                            }
                            Player1 = keypair[1];
                            break;
                        case "player2":
                            if (keypair[1].Equals(User))
                            {
                                ScreenController.Change("MapSetup");
                                gameController.enabled = true;
                                playerController.enabled = true;
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
            print(w.text);
        }
    }

    public static void SendMap()
    {
        instance.StartCoroutine(instance.WriteMap());
    }
}
