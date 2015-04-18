
using UnityEngine;
using System.Text;

public class PlayerVisual : NotificationBehaviour {

	MeshHelper mesh;

	public void UpdateGeometry(Vector3 velocity) {
		if (mesh != null) {
			mesh.Clear ();
		}

		mesh = new MeshHelper (1, "Game/truck", "LD32/TruckBillboard");


		float tileX = 0.0f;
		float tileY = 0.0f;

		if (velocity.x > 0.1f) {
			tileX = 0.0f;
			tileY = 0.5f;
		} else if (velocity.x < -0.1f) {
			tileX = 0.5f;
			tileY = 0.0f;
		} else if (velocity.z > 0.1f) {
			tileX = 0.0f;
			tileY = 0.0f;
		} else{
			tileX = 0.5f;
			tileY = 0.5f;
		}




		MeshQuad quad = new MeshQuad (
			transform.position,
			transform.position,
			transform.position,
			transform.position,
			new Vector2 (tileX+0.5f, tileY+0.5f),
			new Vector2 (tileX+0.0f, tileY+0.5f),
			new Vector2 (tileX+0.5f, tileY+0.0f),
			new Vector2 (tileX+0.0f, tileY+0.0f),
			new Vector2 (128, 0),
			new Vector2 (128, 0),
			new Vector2 (128, 0),
			new Vector2 (128, 0),
			new Vector3 (-0.77f, -0.77f, 0),
			new Vector3 (0.77f, -0.77f, 0),
			new Vector3 (-0.77f, 0.77f, 0),
			new Vector3 (0.77f, 0.77f, 0),
			Color.white,
			Color.white,
			Color.white,
			Color.white
		);

		mesh.AddQuad (quad);

		mesh.Commit ();
	}
	
}
