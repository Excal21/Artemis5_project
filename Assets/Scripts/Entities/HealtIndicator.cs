using System.Collections.Generic;
using UnityEngine;

public class HealtIndicator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private List<GameObject> miniatures;

    public void UpdateHealth(int health)
    {
        for (int i = 0; i < miniatures.Count; i++)
        {
            if (i < health)
            {
                miniatures[i].SetActive(true);
            }
            else
            {
                miniatures[i].SetActive(false);
            }
        }
    }
}
