using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredSection : MonoBehaviour
{
    public Color sectionColor;
    void Start()
    {
        // Assign a random color to this colored section when it is instantiated
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();  // Get the SpriteRenderer component
        renderer.color = GetRandomColor();  // Set the random color
        sectionColor = renderer.color;
    }

    // Function to return a random color from red, green, or yellow
    private Color GetRandomColor()
    {
        Color[] colors = { Color.red, Color.green, Color.yellow };  // Array of colors
        return colors[Random.Range(0, colors.Length)];  // Return a random color
    }
}
