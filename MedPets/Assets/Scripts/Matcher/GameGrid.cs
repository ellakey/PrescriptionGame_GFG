using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class GameGrid : MonoBehaviour
{
    //need fixing
    //Replace Object with Berry Class
    [SerializeField] BerryHolder holder;
    [SerializeField] BerryMover mover;
    [SerializeField] Transform parent;
    [SerializeField] Progression progression;
    GameObject[,] board;
    [SerializeField] int width;
    [SerializeField] int height;
    int[] berries;

    public static int score;
    public TextMeshProUGUI scoretext;

    public GameGrid(int w, int h)
    {
        //Initialize the Board with size width, height. Height is at the bottom 0 is at the top.
        board = new GameObject[width, height];
        width = w;
        height = h;
    }

    private void Start()
    {
        //INITIAL SETUP FOR BOARD SIZE AND FILLING
        PatientInfo.setId(PatientInfo.medId);
        berries = progression.getProgression();
        board = new GameObject[width, height];
        fillBoard(berries);
    }

    void fillBoard(int[] ids)
    {
        for(int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                if (board[x, y] == null)
                {
                    board[x, y] = Instantiate(holder.getBerry(ids[Random.Range(0, ids.Length)]), new Vector2(mover.conX(x), (mover.conY(y)) + 400), Quaternion.identity, parent);
                    board[x, y].GetComponent<RectTransform>().localScale = new Vector2(mover.getScale(), mover.getScale());
                    board[x, y].GetComponent<RectTransform>().localPosition = new Vector2(mover.conX(x), mover.conY(y) + 800 + Random.Range(0, 50));
                    board[x, y].GetComponent<Berry>().locX = x;
                    board[x, y].GetComponent<Berry>().locY = y;
                }
            }
        }
    }


    void gravity()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                if (board[x, y] == null)
                {
                    for(int i = y - 1; i >= 0; i--)
                    {
                        if (board[x, i] != null)
                        {
                            board[x, y] = board[x, i];
                            board[x, y].GetComponent<Berry>().setPosition(x, y);
                            //board[x, y].GetComponent<Rigidbody2D>().gravityScale = 1;
                            board[x, i] = null;
                            break;
                        }
                    }
                }
            }
        }
    }

    int remove(int x, int y)
    {
        //Remove berry from list and return what was removed
        GameObject removed = board[x, y];
        int id = removed.GetComponent<Berry>().getId();
        Destroy(removed);
        board[x, y] = null;
        return id;
    }

    //Loc is an array that contains the x and y position on the board of which to be removed
    //Method returns the ammount of each berry removed by index of ID.
    public int[] removeGroup(int[,] loc)
    {
        int[] removed = new int[holder.getSize()];
        for(int i = 0; i < loc.GetLength(0); i++)
        {
            removed[remove(loc[i, 0], loc[i, 1])]++;
        }
        gravity();
        fillBoard(berries);
        addScore((int)(removed.Length * (removed.Length * 0.8)));
        return removed;

    }

    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }

    public void setHeight(int h)
    {
        height = h;
    }

    public void setWidth(int w)
    {
        width = w;
    }

    public void addScore(int x){
        score += x;
        scoretext.text = score.ToString("D6");
    }

    public GameObject get(int x, int y)
    {
        return board[x, y];
    }
}
