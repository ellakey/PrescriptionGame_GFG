using UnityEngine;

/// <summary>
/// Holds all state that is scoped to a single match (created fresh each round).
/// Owned by GameGrid, written to by DragBehavior, read by Result.
/// Replaces the old static GameGrid.score and DragBehavior.itemCounts.
/// </summary>
public class MatchSession
{
    public int Score { get; private set; }
    public int[] ItemCounts { get; private set; }

    public MatchSession(int berryTypeCount)
    {
        Score = 0;
        ItemCounts = new int[berryTypeCount];
    }

    public void AddScore(int amount)
    {
        Score += amount;
    }
}
