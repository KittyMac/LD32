
using UnityEngine;
using System.Collections;

public partial class IntroController : MonoBehaviour, IPUCode {

	public PURawImage ClickToStart;
	public PURawImage Vignette;
	public PUTextButton SkipButton;
	public PUColor BlackCover;
	public PUSwitcher BubbleSwitcher;

	public void Start() {
		ClickToStart.CheckCanvasGroup ();
		LeanTween.value (ClickToStart.gameObject, (t, obj) => {
			ClickToStart.canvasGroup.alpha = t;
		}, 0.4f, 1.0f, 0.47f).setEase (LeanTweenType.easeOutCubic).setLoopPingPong ();
	}

	public void StartIntro(Hashtable args) {

		PUClearButton sender = args ["sender"] as PUClearButton;
		sender.unload ();

		ClickToStart.unload ();

		// Play intro animation / story
		Vignette.CheckCanvasGroup();
		Vignette.canvasGroup.alpha = 0.0f;
		Vignette.gameObject.SetActive (true);
		LeanTween.alpha (Vignette.gameObject, 1.0f, 2.0f);

		float t = 2.0f;
		LeanTween.delayedCall (t, () => {
			BubbleSwitcher.SwitchTo (0);
		});

		t += 2.0f;
		LeanTween.delayedCall (t, () => {
			BubbleSwitcher.SwitchTo (1);
		});

		t += 5.0f;
		LeanTween.delayedCall (t, () => {
			BubbleSwitcher.SwitchTo (2);

			SkipButton.gameObject.SetActive(true);
		});

		t += 3.0f;
		LeanTween.delayedCall (t, () => {
			BubbleSwitcher.SwitchTo (3);
		});

		t += 3.0f;
		LeanTween.delayedCall (t, () => {
			BubbleSwitcher.SwitchTo (4);
		});

		t += 3.0f;
		LeanTween.delayedCall (t, () => {
			BubbleSwitcher.SwitchTo (5);
		});

		t += 3.0f;
		LeanTween.delayedCall (t, () => {
			BubbleSwitcher.SwitchTo (6);
		});

		t += 3.0f;
		LeanTween.delayedCall (t, () => {
			BubbleSwitcher.SwitchTo (7);
		});

		t += 3.0f;
		LeanTween.delayedCall (t, () => {
			BubbleSwitcher.SwitchTo (8);
		});

		t += 3.0f;
		LeanTween.delayedCall (t, () => {

			SkipIntro();
		});
	}

	public void SkipIntro() {
		LeanTween.cancelAll (false);

		BlackCover.CheckCanvasGroup();
		BlackCover.canvasGroup.alpha = 0.0f;
		BlackCover.gameObject.SetActive (true);
		LeanTween.alpha (BlackCover.gameObject, 1.0f, 1.0f).setOnComplete(() => {
			GameController.StartNewGame();
		});
	}
	
}
