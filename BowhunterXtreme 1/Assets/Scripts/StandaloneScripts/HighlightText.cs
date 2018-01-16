using UnityEngine;
using UnityEngine.EventSystems; //required for Event data
using UnityEngine.UI;

public class HighlightText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Text t;

    void Start()
    {
        t = this.gameObject.GetComponentInChildren<Text>();
    }

    GameObject currentHover;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            currentHover = eventData.pointerCurrentRaycast.gameObject;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        currentHover = null;
    }

    void Update()
    {
        if (currentHover)
            t.fontStyle = FontStyle.Bold;
        else
        {
            t.fontStyle = FontStyle.Normal;
        }
    }
}

