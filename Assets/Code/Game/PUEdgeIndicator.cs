
using UnityEngine;
using System.Xml;
using System.Collections;
using UnityEngine.UI;
using System.Security.Cryptography;

public class PUEdgeIndicator : PURawImage {

	public GameObject from;
	public GameObject to;

	public override void gaxb_final(XmlReader reader, object _parent, Hashtable args) {
		base.gaxb_final (reader, _parent, args);
		ScheduleForUpdate ();
	}

	// This is required for application-level subclasses
	public override void gaxb_init () {
		base.gaxb_init ();
		gaxb_addToParent();
	}

	public override void Update() {
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

		CheckCanvasGroup();

		Vector3 screenPos = Camera.main.WorldToScreenPoint (to.transform.position);
		if (screenPos.x < 0 || screenPos.x > Camera.main.pixelWidth || screenPos.y < 0 || screenPos.y > Camera.main.pixelHeight) {
			Ray ray = new Ray (from.transform.position, to.transform.position - from.transform.position);
			float bestDistance = 999999999.0f;
			bool foundPlane = false;

			foreach (Plane plane in planes) {
				float distanceToPlane;
				if (plane.Raycast (ray, out distanceToPlane)) {
					if (distanceToPlane < bestDistance) {
						bestDistance = distanceToPlane;
						foundPlane = true;
					}
				}
			}

			Vector3 planeIntersectPosition = ray.GetPoint (bestDistance - 80);
			screenPos = Camera.main.WorldToScreenPoint (planeIntersectPosition);
			Vector3 indicatorPos = rectTransform.parent.InverseTransformPoint (screenPos);

			rectTransform.anchoredPosition = indicatorPos;
			canvasGroup.alpha = 1;
		} else {
			canvasGroup.alpha = 0;
		}
	}


}
