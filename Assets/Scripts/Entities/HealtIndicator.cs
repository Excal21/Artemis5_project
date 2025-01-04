using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealtIndicator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private List<GameObject> miniatures;
    public void Start(){
        Vector3 screenPosition = new Vector3(Screen.width - 3, 3, Camera.main.nearClipPlane);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);

        // Beállítjuk a miniatures objektumok pozícióját
        for (int i = 0; i < miniatures.Count; i++)
        {
            miniatures[i].transform.position = new Vector3(worldPosition.x - 0.5f - i * 1f, worldPosition.y + 0.5f, transform.position.z);
            miniatures[i].SetActive(false);
        }
    }
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
    private IEnumerator FadeIn(){
        for (int i = 0; i < miniatures.Count; i++)
        {
            yield return new WaitForSeconds(0.3f);
            miniatures[i].SetActive(true);
        }
    }
    public void Show(){
        StartCoroutine(FadeIn());
    }
}
