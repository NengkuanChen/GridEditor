using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Grids;
public class RandomEditButton : MonoBehaviour
{
    TextMeshProUGUI text;
    private Image image;
    public Color onColor, offColor;
    TestGridScript gridScript;
    public bool on = false;

    public void Clicked()
    {
        on = !on;
        if (on)
        {
            text.text = "Random Edit: On";
            image.color = onColor;
        }
        else
        {
            text.text = "Random Edit: Off";
            image.color = offColor;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        image = GetComponent<Image>();
        Clicked();
        GetComponent<Button>().onClick.AddListener(Clicked);
        gridScript = FindObjectOfType<TestGridScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (on && Input.GetMouseButtonDown(0))
        {
            var pos = gridScript.WorldToGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            gridScript.RandomChangeTile(pos.x, pos.y);
        }
    }
}
