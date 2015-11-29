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