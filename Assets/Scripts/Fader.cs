using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour {

	public bool IsFadeIn = false;

	private string panelFadeIn = "Panel Open";
	private string panelFadeOut = "Panel Close";
	private string styleExpand = "Expand";

	public Animator PanelAnimator;
	public Animator StyleAnimator;
	
	void Start ()
	{
		if (IsFadeIn) {
			PanelAnimator.Play(panelFadeOut);
			StyleAnimator.Play(styleExpand);
		}
	}

	public void LoadScene(int sceneNumber)
	{
		PanelAnimator.Play(panelFadeOut);
		StyleAnimator.Play(styleExpand);
	}
}