
using UnityEngine;
using System.Text;
using System.Collections.Generic;

public partial class GameController : MonoBehaviour, IPUCode {


	public void Update() {

		// Solve the distance graph; this is use by the AI to know how to move towards the player
		SolveDistanceGraphForPlayer ();

		// Does the player collide with an enemy?
		Vector3 playerPos = player.gameObject.transform.position;

		foreach (GameObject enemy in enemies) {
			Vector3 enemyPos = enemy.gameObject.transform.position;

			if (Vector3.Distance (playerPos, enemyPos) < 78) {
				Debug.Log ("GAME OVER!");
			}


			foreach (GameObject spill in new List<GameObject>(ketchupSpills)) {
				float d = Vector3.Distance (spill.gameObject.transform.position, enemyPos);
				if (d < 78) {
					enemy.SendMessage ("SpinOut");

					ketchupSpills.Remove (spill);
					LeanTween.alpha (spill, 0.0f, 1.0f).setDestroyOnComplete (true);
					Debug.Log ("Spin out!!");
				}
			}
		}

	}

}
