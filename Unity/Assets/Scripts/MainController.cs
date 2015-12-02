using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class MainController : MonoBehaviour {

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

    public const string SERVER_ADDRESS = "10.10.11.42:8088/battleship.php";

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
                switch (w.text)
                {
                    default:
                        print(w.text);
                        break;
                }
            }
            Waiting = false;
        }
    }
}
