using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class CellController : MonoBehaviour {
    public static bool Locked = false;
    public Button button;
    public Image image;
    public enum CellState
    {
        None,
        Selected
    }
    public CellState state = CellState.None;

	void Start () {
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
                this.state = CellState.Selected;
                break;
            case CellState.Selected:
                this.state = CellState.None;
                break;
        }
        this.Invalidate();
        Debug.Log(cell.gameObject.name);
    }
}