
using UnityEngine;
using System.Text;
using System.Collections.Generic;

public partial class GameController : MonoBehaviour, IPUCode {
	
	public PUGameObject End;
	public PURawImage EndVignette;
	public PURawImage EndLogo;
	public PURawImage ClickToTryAgain;
	public PUText EndScoreField;


	public void UserWantsToTryAgain() {
		GameController.StartNewGame ();
	}

	public void GameOver() {
		if (GameIsOver == false) {
			GameIsOver = true;

			Debug.Log ("GameIsOver!!!!");

			End.gameObject.SetActive (true);

			EndVignette.CheckCanvasGroup ();
			EndVignette.canvasGroup.alpha = 0.0f;
			LeanTween.alpha (EndVignette.gameObject, 1.0f, 0.64f);

			Vector3 pos = EndLogo.rectTransform.anchoredPosition;
			EndLogo.rectTransform.anchoredPosition = new Vector3 (pos.x, pos.y+1000, pos.z);
			LeanTween.moveLocalY (EndLogo.rectTransform, pos.y, 2.0f).setEase (LeanTweenType.easeInOutBounce);

			EndScoreField.CheckCanvasGroup ();
			EndScoreField.canvasGroup.alpha = 0.0f;
			LeanTween.alpha (EndScoreField.gameObject, 1.0f, 0.64f);
			EndScoreField.text.text = PlanetUnityStyle.ReplaceStyleTags(string.Format ("[b3]Score:[/b3] [p3]{0}[/p3]", PlayerScore));

			ClickToTryAgain.CheckCanvasGroup ();
			LeanTween.value (ClickToTryAgain.gameObject, (t, obj) => {
				ClickToTryAgain.canvasGroup.alpha = t;
			}, 0.4f, 1.0f, 0.47f).setEase (LeanTweenType.easeOutCubic).setLoopPingPong ();
		}
	}


}
