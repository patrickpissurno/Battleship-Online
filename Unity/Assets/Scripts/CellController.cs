using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class CellController : MonoBehaviour {
    public static bool Locked = false;
    public Button button;
    public Image image;
    public int ID_i;
    public int ID_j;
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
        //Debug.Log(cell.gameObject.name);
    }
}