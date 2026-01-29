using UnityEngine;

public static class Vector3Extension
{
	public static Vector3 RandomRange(this Vector3 v1, Vector3 v2)
		=> new Vector3(Random.Range(v1.x, v2.x), Random.Range(v1.y, v2.y), Random.Range(v1.z, v2.z));
}