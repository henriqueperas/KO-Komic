using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlipAnimation : MonoBehaviour
{
    public Sprite newSprite;

    public void Flip()
    {
        gameObject.GetComponent<Image>().sprite = newSprite;
    }
}
