using UnityEngine;

public class ScreenController : MonoBehaviour {

    private static ScreenController instance;
    public Screen[] Screens;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Change(Screens[0].Name);
    }
	
    public static void Change(string screen)
    {
        foreach(Screen s in instance.Screens)
        {
            bool a = s.Name == screen;
            foreach (GameObject o in s.Objects)
            {
                bool bl = o.name == "Map" && (s.Name == "MainGame" || s.Name == "MapSetup");
                o.SetActive(a || bl);
            }
        }
    }
}

[System.Serializable]
public class Screen
{
    public string Name;
    public GameObject[] Objects;
}
