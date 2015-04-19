using UnityEngine;
using System.Security.Cryptography;

public class MeshQuad {

	public Vector3 a, b, c, d;
	public Vector2 uvA, uvB, uvC, uvD;
	public Vector2 uv2A, uv2B, uv2C, uv2D;
	public Color32 colorA, colorB, colorC, colorD;
	public Vector3 normA, normB, normC, normD;

	public void UpdateSizeAndOrigin(Vector2 size, Vector3 origin) {
		a = new Vector3 (origin.x - size.x / 2.0f, origin.y, origin.z - size.y / 2.0f);
		b = new Vector3 (origin.x - size.x / 2.0f, origin.y, origin.z + size.y / 2.0f);
		c = new Vector3 (origin.x + size.x / 2.0f, origin.y, origin.z - size.y / 2.0f);
		d = new Vector3 (origin.x + size.x / 2.0f, origin.y, origin.z + size.y / 2.0f);
	}


	public void CreateQuadGeometryFromSizeAndOrigin(Vector2 size, Vector3 origin, Color color, Vector4 uvCoords, bool flippedXY) {
		UpdateSizeAndOrigin (size, origin);

		if (flippedXY) {
			uvD = new Vector2 (uvCoords.z, uvCoords.w);
			uvC = new Vector2 (uvCoords.z, uvCoords.y);
			uvB = new Vector2 (uvCoords.x, uvCoords.w);
			uvA = new Vector2 (uvCoords.x, uvCoords.y);
		} else {
			uvA = new Vector2 (uvCoords.z, uvCoords.w);
			uvC = new Vector2 (uvCoords.z, uvCoords.y);
			uvB = new Vector2 (uvCoords.x, uvCoords.w);
			uvD = new Vector2 (uvCoords.x, uvCoords.y);
		}

		colorA = colorB = colorC = colorD = color;

		normA = normB = normC = normD = Vector3.zero;
	}

	public MeshQuad(Vector2 size, Vector3 origin, Vector4 uvCoords, Color32 color, bool flippedXY) {
		CreateQuadGeometryFromSizeAndOrigin (size, origin, color, uvCoords, flippedXY);
	}

	public MeshQuad(Vector2 size) {
		CreateQuadGeometryFromSizeAndOrigin (size, Vector3.zero, Color.white, new Vector4 (0, 0, 1, 1), false);
	}

	public MeshQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color color) {
		this.a = a;
		this.b = b;
		this.c = c;
		this.d = d;

		uvA = new Vector2 (1, 1);
		uvC = new Vector2 (1, 0);
		uvB = new Vector2 (0, 1);
		uvD = new Vector2 (0, 0);

		colorA = colorB = colorC = colorD = color;

		normA = normB = normC = normD = Vector3.zero;
	}

	public MeshQuad(
		Vector3 a, Vector3 b, Vector3 c, Vector3 d, 
		Vector2 uvA, Vector2 uvB, Vector2 uvC, Vector2 uvD,
		Vector2 uv2A, Vector2 uv2B, Vector2 uv2C, Vector2 uv2D,
		Vector3 normA, Vector3 normB, Vector3 normC, Vector3 normD, 
		Color colorA, Color colorB, Color colorC, Color colorD) {
		this.a = a;
		this.b = b;
		this.c = c;
		this.d = d;

		this.uvA = uvA;
		this.uvB = uvB;
		this.uvC = uvC;
		this.uvD = uvD;

		this.uv2A = uv2A;
		this.uv2B = uv2B;
		this.uv2C = uv2C;
		this.uv2D = uv2D;

		this.colorA = colorA;
		this.colorB = colorB;
		this.colorC = colorC;
		this.colorD = colorD;

		this.normA = normA;
		this.normB = normB;
		this.normC = normC;
		this.normD = normD;
	}
}

public class MeshHelper {
	public GameObject gameObject;
	public Mesh mesh;
	public MeshQuad[] quads;
	int numQuads = 0;

	protected Vector3[] vertices;
	protected Vector3[] normals;
	protected Vector2[] uv;
	protected Vector2[] uv2;
	protected Color32[] colors32;
	protected int[] triangles;

	public MeshHelper(int maxQuads, string texturePath, string shaderName) {

		quads = new MeshQuad[maxQuads];

		mesh = new Mesh ();
		mesh.name = texturePath;

		mesh.subMeshCount = 0;

		vertices = new Vector3[maxQuads * 4];
		normals = new Vector3[maxQuads * 4];
		uv = new Vector2[maxQuads * 4];
		uv2 = new Vector2[maxQuads * 4];
		colors32 = new Color32[maxQuads * 4];
		triangles = new int[maxQuads * 6];

		for (int i = 0; i < maxQuads; i++) {
			triangles [i * 6 + 0] = i * 4 + 0;
			triangles [i * 6 + 1] = i * 4 + 1;
			triangles [i * 6 + 2] = i * 4 + 2;
			triangles [i * 6 + 3] = i * 4 + 2;
			triangles [i * 6 + 4] = i * 4 + 1;
			triangles [i * 6 + 5] = i * 4 + 3;
		}

		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uv;
		mesh.uv2 = uv2;
		mesh.colors32 = colors32;
		mesh.triangles = triangles;

		gameObject = new GameObject (mesh.name, typeof(MeshRenderer), typeof(MeshFilter));

		MeshFilter filter = (MeshFilter)gameObject.GetComponent (typeof(MeshFilter));
		filter.mesh = mesh;

		gameObject.GetComponent<Renderer>().material = new Material (Shader.Find (shaderName));
		gameObject.GetComponent<Renderer>().material.color = new Color (1, 1, 1, 1);
		gameObject.GetComponent<Renderer>().material.mainTexture = PlanetUnityResourceCache.GetTexture (texturePath);
	}
		

	public MeshQuad AddQuad(Vector2 size, Vector3 origin, Vector4 uvCoords, Color32 color, bool flippedXY) {
		MeshQuad quad = new MeshQuad(size, origin, uvCoords, color, flippedXY);
		quads [numQuads] = quad;
		numQuads++;
		return quad;
	}

	public void AddQuad(Vector2 size, Vector3 origin) {
		AddQuad (size, origin, new Vector4 (0, 0, 1, 1), new Color32 (255, 255, 255, 255), false);
	}

	public void SetQuad(int idx, Vector2 size, Vector3 origin) {
		if (idx >= numQuads) {
			return;
		}
		quads [idx].UpdateSizeAndOrigin (size, origin);
	}

	public void SetQuadColor(int idx, Color color) {
		if (idx >= numQuads) {
			return;
		}
		quads [idx].colorA = color;
		quads [idx].colorB = color;
		quads [idx].colorC = color;
		quads [idx].colorD = color;
	}


	public MeshQuad AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color color) {
		MeshQuad quad = new MeshQuad(a, b, c, d, color);
		quads [numQuads] = quad;
		numQuads++;
		return quad;
	}

	public MeshQuad AddQuad(MeshQuad quad) {
		quads [numQuads] = quad;
		numQuads++;
		return quad;
	}



	public void AddXZLine(Vector3 pointA, Vector3 pointB, float width, Color color) {

		Vector3 baseVector = (pointB - pointA).normalized.RotateLeftAboutY() * width;

		Vector3 a = pointA + baseVector;
		Vector3 b = pointA - baseVector;
		Vector3 c = pointB + baseVector;
		Vector3 d = pointB - baseVector;

		MeshQuad quad = new MeshQuad (
			                a, b, c, d, 
			                new Vector2 (0, 0),
			                new Vector2 (1, 0),
			                new Vector2 (0, 1),
			                new Vector2 (1, 1),
			                Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero,
			                Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero,
			                color, color, color, color);

		AddQuad(quad);

	}

	public void AddAntsLine(Vector3 pointA, Vector3 pointB, float width, Color color) {

		Vector3 baseVector = (pointB - pointA).normalized.RotateLeftAboutY() * width;
		float l = (pointB - pointA).magnitude / 10.0f;

		Vector3 a = pointA + baseVector;
		Vector3 b = pointA - baseVector;
		Vector3 c = pointB + baseVector;
		Vector3 d = pointB - baseVector;

		MeshQuad quad = new MeshQuad (
			a, b, c, d, 
			new Vector2 (0, 0),
			new Vector2 (1, 0),
			new Vector2 (0, l),
			new Vector2 (1, l),
			Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero,
			Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero,
			color, color, color, color);

		AddQuad(quad);

	}

	public void AddXZCricle(float radius, Vector3 origin, Color color, float lineWidth, int segments) {
		// circle around the base...
		float radianDelta = (Mathf.PI * 2.0f) / segments;
		for (int i = 0; i < segments; i++) {
			float delta = i * radianDelta;
			Vector2 baseVector1 = new Vector2 (radius - (lineWidth / 2), 0.0f);
			Vector2 baseVector2 = new Vector2 (radius + (lineWidth / 2), 0.0f);
			Vector2 a = baseVector1.RotateZ (delta);
			Vector2 b = baseVector1.RotateZ (delta + radianDelta);
			Vector2 c = baseVector2.RotateZ (delta);
			Vector2 d = baseVector2.RotateZ (delta + radianDelta);

			// projectiles are drawn at the origin along the reverse vector
			AddQuad (
				new Vector3(a.x, 0, a.y)+origin, 
				new Vector3(b.x, 0, b.y)+origin,
				new Vector3(c.x, 0, c.y)+origin,
				new Vector3(d.x, 0, d.y)+origin,
				color);
		}
	}


	public void AddXZCricle(float radius, Vector3 center, float a, float aw, int segs, Color c) {
		float coef = aw/segs;

		Vector3 p0, p1, p2;

		p0 = p1 = p2 = center;

		for(int i = 0; i < segs; i++)
		{
			float rads = i*coef;
			float rads2 = (i+1)*coef;

			//z' = z*cos q - x*sin q
			//x' = z*sin q + x*cos q

			p1.x = (radius * Mathf.Cos(rads + a)) + center.x;
			p1.z = (radius * Mathf.Sin(rads + a)) + center.z;

			p2.x = (radius * Mathf.Cos(rads2 + a)) + center.x;
			p2.z = (radius * Mathf.Sin(rads2 + a)) + center.z;

			AddQuad (p0, p1, p2, p0, c);
		}
	}



	public void Commit(bool infiniteBounds = false) {

		mesh.Clear ();

		int i = 0;
		foreach (MeshQuad quad in quads) {
			if (quad == null) {
				continue;
			}

			vertices [i * 4 + 0] = quad.a;
			vertices [i * 4 + 1] = quad.b;
			vertices [i * 4 + 2] = quad.c;
			vertices [i * 4 + 3] = quad.d;

			uv [i * 4 + 0] = quad.uvA;
			uv [i * 4 + 1] = quad.uvB;
			uv [i * 4 + 2] = quad.uvC;
			uv [i * 4 + 3] = quad.uvD;

			uv2 [i * 4 + 0] = quad.uv2A;
			uv2 [i * 4 + 1] = quad.uv2B;
			uv2 [i * 4 + 2] = quad.uv2C;
			uv2 [i * 4 + 3] = quad.uv2D;
				
			colors32 [i * 4 + 0] = quad.colorA;
			colors32 [i * 4 + 1] = quad.colorB;
			colors32 [i * 4 + 2] = quad.colorC;
			colors32 [i * 4 + 3] = quad.colorD;

			normals [i * 4 + 0] = quad.normA;
			normals [i * 4 + 1] = quad.normB;
			normals [i * 4 + 2] = quad.normC;
			normals [i * 4 + 3] = quad.normD;

			i++;
		}

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.uv2 = uv2;
		mesh.normals = normals;
		mesh.colors32 = colors32;
		mesh.triangles = triangles;

		if (infiniteBounds) {
			mesh.bounds = new Bounds(
				new Vector3(0, 0, 0), 
				new Vector3(1000000.0f, 1000000.0f, 1000000.0f)); // NOTE: using infinity here causes an error
		} else {
			mesh.RecalculateBounds ();
		}
	}

	public void Clear() {
		GameObject.Destroy (gameObject);
		gameObject = null;
	}
}
