using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseOver : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler {

    public Color onMouseOverColor;

    private TextMeshProUGUI text;
    private Color startColor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = onMouseOverColor;
        text.fontSize = 38;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = startColor;
        text.fontSize = 36;
    }

    // Use this for initialization
    void Start () {
        text = GetComponent<TextMeshProUGUI>();
        startColor = text.color;
	}
	
	// Update is called once per frame
	void Update () {
    }
}
