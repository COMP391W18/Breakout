using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorModifier : MonoBehaviour {

    // The color mode
    public enum ColorMode { FIXED, GRADIENT, TIME_BASED }

    // The current color of the show
    public Color ShapeColor;
    public ColorMode ModifierMode;
    public Gradient ColorGradier;

    GameController GameControllerRef;

    SpriteRenderer SpriteComponent;
   
    // 
    float CurrentAngle;
    Color CurrentColor;

	// Use this for initialization
	void Start ()
    {
        // Cache the sprite component
        SpriteComponent = GetComponentInParent<SpriteRenderer>();
        
        // Random start value
        CurrentAngle = Random.Range(0, 360);

        // Cache the game controller component
        GameControllerRef = GameObject.Find("GameController").GetComponent<GameController>();
    }
	
    public void SetColor(float NewColor)
    {
        CurrentAngle = NewColor - 5f;

        // Update the color value
        CurrentColor = Color.HSVToRGB(CurrentAngle / 360f, 1, 1);
    }
    public void SetColor(Color NewColor)
    {
        ShapeColor = NewColor;
    }
    public void SetColor(Gradient NewColor)
    {
        ColorGradier = NewColor;
    }

    public Color GetCurrentColor()
    {
        return CurrentColor;
    }

    // Update is called once per frame
    void Update ()
    {
        // Based the 
        switch (ModifierMode)
        {
            case ColorMode.TIME_BASED:
                // Update the current angle
                CurrentAngle = (CurrentAngle > 360) ? 0 : CurrentAngle + 5;

                // Update the color value
                CurrentColor = Color.HSVToRGB(CurrentAngle / 360f, 1, 1);
                break;

            case ColorMode.GRADIENT:
                // Update the current angle
                CurrentAngle = (CurrentAngle > 360) ? 0 : CurrentAngle + 1 * Time.deltaTime;

                // Update the color value
                CurrentColor = ColorGradier.Evaluate(CurrentAngle / 360f);
                break;

            case ColorMode.FIXED:
                CurrentColor = ShapeColor;
                break;
        }
                
        if (SpriteComponent)
            SpriteComponent.color = CurrentColor;
    }
}
