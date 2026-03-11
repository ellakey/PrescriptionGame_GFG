using UnityEngine;
using TMPro;

public class GameGrid : MonoBehaviour
{
    [SerializeField] BerryHolder holder;
    [SerializeField] BerryMover mover;
    [SerializeField] Transform parent;
    [SerializeField] Progression progression;
    GameObject[,] board;
    [SerializeField] int width;
    [SerializeField] int height;
    int[] berries;

    public MatchSession Session { get; private set; }
    public TextMeshProUGUI scoretext;

    public int Width
    {
        get => width;
        set => width = value;
    }

    public int Height
    {
        get => height;
        set => height = value;
    }

    private void Start()
    {
        Session = new MatchSession(holder.Size);

        PatientInfo.SetId(PatientInfo.MedId);
        berries = progression.GetProgression();
        board = new GameObject[width, height];
        FillBoard(berries);
    }

    void FillBoard(int[] ids)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                if (board[x, y] == null)
                {
                    board[x, y] = Instantiate(holder.GetBerry(ids[Random.Range(0, ids.Length)]), new Vector2(mover.ConvertX(x), (mover.ConvertY(y)) + 400), Quaternion.identity, parent);
                    board[x, y].GetComponent<RectTransform>().localScale = new Vector2(mover.Scale, mover.Scale);
                    board[x, y].GetComponent<RectTransform>().localPosition = new Vector2(mover.ConvertX(x), mover.ConvertY(y) + 800 + Random.Range(0, 50));
                    board[x, y].GetComponent<Berry>().locX = x;
                    board[x, y].GetComponent<Berry>().locY = y;
                }
            }
        }
    }

    void Gravity()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                if (board[x, y] == null)
                {
                    for (int i = y - 1; i >= 0; i--)
                    {
                        if (board[x, i] != null)
                        {
                            board[x, y] = board[x, i];
                            board[x, y].GetComponent<Berry>().SetPosition(x, y);
                            board[x, i] = null;
                            break;
                        }
                    }
                }
            }
        }
    }

    int Remove(int x, int y)
    {
        // Remove berry from list and return what was removed
        GameObject removed = board[x, y];
        int id = removed.GetComponent<Berry>().Id;
        Destroy(removed);
        board[x, y] = null;
        return id;
    }

    // Loc is an array that contains the x and y position on the board of which to be removed.
    // Returns the amount of each berry removed by index of ID.
    public int[] RemoveGroup(int[,] loc)
    {
        int[] removed = new int[holder.Size];
        for (int i = 0; i < loc.GetLength(0); i++)
        {
            removed[Remove(loc[i, 0], loc[i, 1])]++;
        }
        Gravity();
        FillBoard(berries);
        AddScore((int)(removed.Length * (removed.Length * 0.8)));
        return removed;
    }

    public void AddScore(int amount)
    {
        Session.AddScore(amount);
        scoretext.text = Session.Score.ToString("D6");
    }

    public GameObject Get(int x, int y)
    {
        return board[x, y];
    }
}