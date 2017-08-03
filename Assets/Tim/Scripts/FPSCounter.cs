using UnityEngine;
using System.Collections;

[AddComponentMenu("Utilities/HUDFPS")]
public class FPSCounter : MonoBehaviour
{
    // Attach this to any object to make a frames/second indicator.
    //
    // It calculates frames/second over each updateInterval,
    // so the display does not keep changing wildly.
    //
    // It is also fairly accurate at very low FPS counts (<10).
    // We do this not by simply counting frames per interval, but
    // by accumulating FPS for each frame. This way we end up with
    // corstartRect overall FPS even if the interval renders something like
    // 5.5 frames.

    public Rect startRect = new Rect(10, 10, 75, 50); // The rect the window is initially displayed at.
    public bool updateColor = true; // Do you want the color to change if the FPS gets low
    public bool allowDrag = true; // Do you want to allow the dragging of the FPS window
    public float frequency = 0.5F; // The update frequency of the fps
    public int nbDecimal = 1; // How many decimal do you want to display

    private float accum = 0f; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private Color color = Color.white; // The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
    private string sFPS = ""; // The fps formatted into a string.
    private GUIStyle style; // The style the text will be displayed at, based en defaultSkin.label.


     
    float deltaTime = 0.0f;

    void Start()
    {
        QualitySettings.vSyncCount = 0; 
        Application.targetFrameRate = 20;


    }

    void Update()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 20;

        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        sFPS = fps.ToString(); // assign that value to any UI or 3D Text.

    }


    void OnGUI()
    {
        // Copy the default label skin, change the color and the alignement
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
        }

        GUI.color = updateColor ? color : Color.white;
        startRect = GUI.Window(0, startRect, DoMyWindow, "");
    }

    void DoMyWindow(int windowID)
    {
        GUI.Label(new Rect(0, 0, startRect.width, startRect.height), sFPS + " FPS", style);
        if (allowDrag) GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }
}