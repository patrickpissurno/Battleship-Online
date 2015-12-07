using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class CellController : MonoBehaviour {
    public static bool Attack = false;
    public static bool Locked = false;
    public Button button;
    public Image image;
    public int ID_i;
    public int ID_j;
    public bool EnemyCell = false;
    public enum CellState
    {
        None,
        Selected
    }
    public CellState state = CellState.None;
    public static List<CellController> SelectedCells;

	void Start () {
        SelectedCells = new List<CellController>();
        this.button = this.GetComponent<Button>();
        this.image = this.GetComponent<Image>();
        this.button.onClick.AddListener(() => { Clicked(this); });
        this.Invalidate();
	}

    public void Invalidate()
    {
        switch (this.state)
        {
            case CellState.None:
                this.image.color = Color.white;
                break;
            case CellState.Selected:
                this.image.color = Color.black;
                break;
        }
    }

    public void Clicked(CellController cell)
    {
        if (!Attack)
        {
            switch (this.state)
            {
                case CellState.None:
                    if (SelectedCells.Count < ShipAmount.TotalCellAmount)
                    {
                        this.state = CellState.Selected;
                        SelectedCells.Add(this);
                    }
                    break;
                case CellState.Selected:
                    this.state = CellState.None;
                    SelectedCells.Remove(this);
                    break;
            }
            this.Invalidate();
        }
        else
        {
            if (!Locked)
            {
                if (EnemyCell)
                {
                    state = CellState.Selected;
                    Invalidate();
                    ShipAmount.TotalCellAmount--;
                }
                MainController.DoAttack();
            }
        }
    }

    public static void ClearSelection()
    {
        if (SelectedCells != null)
        {
            foreach (CellController c in SelectedCells)
            {
                c.state = CellState.None;
                c.Invalidate();
            }
            SelectedCells.Clear();
        }
    }
}