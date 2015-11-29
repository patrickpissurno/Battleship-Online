using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public const float CELL_SIZE = 60f;
    public const int GRID_SIZE = 5;
    public static readonly string[] LETTERS = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
    public GameObject CellPrefab;
    private GameObject canvas;
	void Start () {
        canvas = GameObject.Find("Canvas");
        GenerateMap();
	}
	
	void Update () {
	
	}

    void GenerateMap()
    {
        for (int i = 0; i < GRID_SIZE + 1; i++)
        {
            for (int j = 0; j < GRID_SIZE + 1; j++)
            {
                if (!(i == 0 && j == 0))
                {
                    CellPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(CELL_SIZE, CELL_SIZE);
                    GameObject o = Instantiate(CellPrefab, new Vector2((j - (GRID_SIZE + 1) / 2) * CELL_SIZE, (-i + (GRID_SIZE + 1) / 2) * CELL_SIZE), Quaternion.identity) as GameObject;
                    o.transform.SetParent(canvas.transform, false);
                    if (i > 0 && j > 0)
                        o.name = (i) + ":" + LETTERS[j - 1];
                    else
                    {
                        //HEADER
                        Text t = o.transform.Find("Text").GetComponent<Text>();
                        t.text = i == 0 ? LETTERS[j - 1] : i.ToString();
                        t.color = Color.white;
                        Destroy(o.GetComponent<Image>());
                    }
                }
            }
        }
    }
}
