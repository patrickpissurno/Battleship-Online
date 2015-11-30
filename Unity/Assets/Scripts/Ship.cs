 using System.Collections;
using System.Collections.Generic;
public class Ship
{
    public int PieceAmount;
    public CellController[] Cells;
    public Ship(int pieceAmount)
    {
        this.PieceAmount = pieceAmount;
        Cells = new CellController[this.PieceAmount];
    }

    public static string Serialize(Ship[] ships)
    {
        string result = "";
        foreach (Ship s in ships)
            result += Serialize(s);
        return result;
    }

    public static string Serialize(Ship ship)
    {
        string result = "amount=" + ship.PieceAmount + "|pieces=";
        foreach (CellController cell in ship.Cells)
            result += cell.ID_j + ":" + cell.ID_i + (cell == ship.Cells[ship.Cells.Length-1] ? ";" : ",");
        return result;
    }
}

public static class ShipAmount
{
    public static Ship[] Types;
    public static int[] Amount;
    public static int TotalCellAmount;
    public static void Init()
    {
        Types = new Ship[]{new Ship(2), new Ship(3)};
        Amount = new int[]{1,1};

        TotalCellAmount = 0;
        for (int i = 0; i < Amount.Length; i++)
            TotalCellAmount += Types[i].PieceAmount * Amount[i];
    }
}