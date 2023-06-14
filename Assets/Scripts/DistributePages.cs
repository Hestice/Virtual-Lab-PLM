using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistributePages : MonoBehaviour
{
    public float spacing = 0f;

    private void Start()
    {
        RectTransform parentRect = GetComponent<RectTransform>();
        float parentWidth = parentRect.rect.width;

        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform childRect = transform.GetChild(i).GetComponent<RectTransform>();

            float childWidth = childRect.rect.width;
            float xPos = ((parentWidth - childWidth) / 2f) + (i * (childWidth + spacing));

            childRect.anchoredPosition = new Vector2(xPos, childRect.anchoredPosition.y);
        }
    }
}
