// NICKED! Also rather shit.

// simple fading script
// A texture is stretched over the entire screen. The color of the pixel is set each frame until it reaches its target color.
 
 
using UnityEngine;
 
 
public class CameraFade : MonoBehaviour
{   
	private GUIStyle m_BackgroundStyle 			= new GUIStyle();		// Style for background tiling
	private Texture2D m_FadeTexture;									// 1x1 pixel texture used for fading
	private static Color m_CurrentScreenOverlayColor 	= new Color(0,0,0,0);	// default starting color: black and fully transparrent
	private static Color m_TargetScreenOverlayColor 	= new Color(0,0,0,0);	// default target color: black and fully transparrent
	private static Color m_DeltaColor 					= new Color(0,0,0,0);	// the delta-color is basically the "speed / second" at which the current color should change
	private int m_FadeGUIDepth 					= -10;				// make sure this texture is drawn on top of everything
 	private System.Action m_fadeCompleteHandler = null;
	private float m_fadeTimeDelta = 0.0f;
	
	// initialize the texture, background-style and initial color:
	private void Awake()
	{		
		m_FadeTexture = new Texture2D(1, 1);        
        m_BackgroundStyle.normal.background = m_FadeTexture;
 
		m_FadeTexture.SetPixel(0, 0, m_CurrentScreenOverlayColor);
		m_FadeTexture.Apply();
		m_fadeTimeDelta = Time.fixedDeltaTime;
	}
 
 
	// draw the texture and perform the fade:
	private void OnGUI()
    {   
		// if the current color of the screen is not equal to the desired color: keep fading!
		if (m_CurrentScreenOverlayColor != m_TargetScreenOverlayColor)
		{			
			// if the difference between the current alpha and the desired alpha is smaller than delta-alpha * deltaTime, then we're pretty much done fading:
			if (Mathf.Abs(m_CurrentScreenOverlayColor.a - m_TargetScreenOverlayColor.a) < Mathf.Abs(m_DeltaColor.a) * m_fadeTimeDelta)
			{
				m_CurrentScreenOverlayColor = m_TargetScreenOverlayColor;
				SetScreenOverlayColor(m_CurrentScreenOverlayColor);
				m_DeltaColor = new Color(0,0,0,0);
				
				Debug.Log("Fade complete, calling handler...");
				if(m_fadeCompleteHandler != null)
				{
					m_fadeCompleteHandler();	
				}
			}
			else
			{
				// fade!
				SetScreenOverlayColor(m_CurrentScreenOverlayColor + m_DeltaColor * m_fadeTimeDelta);
				if (m_CurrentScreenOverlayColor == m_TargetScreenOverlayColor)
				{
					Debug.Log("Fade complete, calling handler...");
					if(m_fadeCompleteHandler != null)
					{
						m_fadeCompleteHandler();	
					}
				}
			}
		}
	
 
		// only draw the texture when the alpha value is greater than 0:
		if (m_CurrentScreenOverlayColor.a > 0)
		{			
            GUI.depth = m_FadeGUIDepth;
            GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), m_FadeTexture, m_BackgroundStyle);
		}
    }
 
 
	// instantly set the current color of the screen-texture to "newScreenOverlayColor"
	// can be usefull if you want to start a scene fully black and then fade to opague
	public void SetScreenOverlayColor(Color newScreenOverlayColor)
	{
		m_CurrentScreenOverlayColor = newScreenOverlayColor;
		m_FadeTexture.SetPixel(0, 0, m_CurrentScreenOverlayColor);
		m_FadeTexture.Apply();
	}
 
 
	// initiate a fade from the current screen color (set using "SetScreenOverlayColor") towards "newScreenOverlayColor" taking "fadeDuration" seconds
	public void StartFade(Color newScreenOverlayColor, float fadeDuration, System.Action completeHandler)
	{
		Debug.Log("Fade requested");
		m_fadeCompleteHandler = completeHandler;
		
		bool instantFade = fadeDuration <= 0.0f;
		bool sameColour = newScreenOverlayColor == m_CurrentScreenOverlayColor;
			
		if (sameColour || instantFade)		// can't have a fade last -2455.05 seconds!
		{
			Debug.Log("Fade not needed, calling handler...");
			SetScreenOverlayColor(newScreenOverlayColor);
			if(m_fadeCompleteHandler != null)
			{
				m_fadeCompleteHandler();	
			}
		}
		else					// initiate the fade: set the target-color and the delta-color
		{
			m_TargetScreenOverlayColor = newScreenOverlayColor;
			m_DeltaColor = (m_TargetScreenOverlayColor - m_CurrentScreenOverlayColor) / fadeDuration;
		}
	}
}