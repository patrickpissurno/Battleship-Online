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
                o.SetActive(a);
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
