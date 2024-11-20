using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    List<Sprite> sprites;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadSpritesWithDelay());
    }

    IEnumerator LoadSpritesWithDelay()
    {
        SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
        foreach (Sprite sprite in sprites)
        {
            spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds(0.1f); // 100 milliszekundum késleltetés
        }
        Destroy(this.gameObject);
    }
}
