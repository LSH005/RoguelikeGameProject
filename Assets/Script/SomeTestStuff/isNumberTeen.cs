using UnityEngine;

public class isNumberTeen : MonoBehaviour
{
    [Range(1, 100)]
    public int Number = 10;

    void Start()
    {
        if (Number <= 1 || Number >= 100)
        {
            return;
        }

        if (Number % 10 == 0)
        {
            Debug.Log($"It's {Number}");
        }
        else
        {
            Debug.Log("No");
        }
    }
}