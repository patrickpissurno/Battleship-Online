using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour {
    public GameController gameController;
    public List<List<CellController>> Cells;
    public const float CELL_SIZE = 60f;
    public const int GRID_SIZE = 5;
    public static readonly string[] LETTERS = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
    public GameObject CellPrefab;
    private GameObject canvas;

	void Start () {
        gameController = GetComponent<GameController>();
        Cells = new List<List<CellController>>();
        canvas = GameObject.Find("Canvas");
        GenerateMap();
	}

    void GenerateMap()
    {
        for (int i = 0; i < GRID_SIZE + 1; i++)
        {
            Cells.Add(new List<CellController>());
            for (int j = 0; j < GRID_SIZE + 1; j++)
            {
                if (!(i == 0 && j == 0))
                {
                    CellPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(CELL_SIZE, CELL_SIZE);
                    GameObject o = Instantiate(CellPrefab, new Vector2((j - (GRID_SIZE + 1) / 2) * CELL_SIZE, (-i + (GRID_SIZE + 1) / 2) * CELL_SIZE), Quaternion.identity) as GameObject;
                    o.transform.SetParent(canvas.transform, false);
                    if (i > 0 && j > 0)
                    {
                        
                        CellController c = o.AddComponent<CellController>();
                        c.ID_i = i - 1;
                        c.ID_j = j - 1;
                        o.name = c.ID_i + ":" + c.ID_j;
                        Cells[c.ID_i].Add(c);
                        Destroy(o.transform.Find("Text").gameObject);
                    }
                    else
                    {
                        //HEADER
                        Text t = o.transform.Find("Text").GetComponent<Text>();
                        t.text = i == 0 ? LETTERS[j - 1] : i.ToString();
                        t.color = Color.white;
                        o.name = "Header-" + t.text;
                        Destroy(o.GetComponent<Image>());
                        Destroy(o.GetComponent<Button>());
                    }
                }
            }
        }
    }

    public int ValidateMap()
    {
        gameController.ships.Clear();
        ShipAmount.Init();
        List<CellController> availableCells = CellController.SelectedCells.ToList();
        foreach (CellController cell in CellController.SelectedCells)
        {
            if (availableCells.IndexOf(cell) != -1)
            {
                availableCells.Remove(cell);
                Debug.Log(cell.name);

                List<CellController> shipCells = new List<CellController>();

                for (int i = cell.ID_i - 1; i >= 0; i--)
                {
                    if (Cells[i][cell.ID_j].state != CellController.CellState.Selected)
                        break;

                    if(!CheckAvailability(shipCells.Count()))
                        break;

                    if (availableCells.IndexOf(Cells[i][cell.ID_j]) != -1)
                    {
                        CellController c = Cells[i][cell.ID_j];
                        Debug.Log(c.name);
                        availableCells.Remove(c);
                        shipCells.Add(c);
                    }
                }

                for (int i = cell.ID_i + 1; i < GRID_SIZE; i++)
                {
                    if (Cells[i][cell.ID_j].state != CellController.CellState.Selected)
                        break;

                    if(!CheckAvailability(shipCells.Count()))
                        break;

                    if (availableCells.IndexOf(Cells[i][cell.ID_j]) != -1)
                    {
                        CellController c = Cells[i][cell.ID_j];
                        Debug.Log(c.name);
                        availableCells.Remove(c);
                        shipCells.Add(c);
                    }
                }

                if (shipCells.Count == 0)
                {
                }

                shipCells.Add(cell);

                bool invalidFormation = true;
                for (int x = ShipAmount.Types.Count() - 1; x >= 0; x--)
                {
                    Debug.Log("Type: " + ShipAmount.Types[x].PieceAmount + " => " + ShipAmount.Amount[x]);
                    if (ShipAmount.Amount[x] > 0 && ShipAmount.Types[x].PieceAmount == shipCells.Count)
                    {
                        ShipAmount.Amount[x]--;
                        invalidFormation = false;
                        break;
                    }
                }

                if (invalidFormation)
                    return 404;
                Ship ship = new Ship(shipCells.Count);
                ship.Cells = shipCells.ToArray();
                gameController.ships.Add(ship);
            }
        }

        //Debug.Log("SHIPS:");
        //foreach (Ship _s in gameController.ships)
        //    Debug.Log(_s.PieceAmount);

        if (CellController.SelectedCells.Count() < ShipAmount.TotalCellAmount)
            return 100;
        return -1;
    }

    bool CheckAvailability(int cellCount)
    {
        for (int x = ShipAmount.Types.Count() - 1; x >= 0; x--)
        {
            if (ShipAmount.Amount[x] >= 1)
            {
                if (cellCount + 1 < ShipAmount.Types[x].PieceAmount)
                    return true;
                else
                    return false;
            }
        }
        return false;
    }
}
