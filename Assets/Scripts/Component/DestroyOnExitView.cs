using UnityEngine;

public class DestroyOnExitView : MonoBehaviour
{
	private SpriteRenderer rend;

	private void Awake()
	{
		rend = GetComponent<SpriteRenderer>();
	}

	private void LateUpdate()
	{
		if (Time.time == 0)
			return;

		var planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
		bool isOnView = GeometryUtility.TestPlanesAABB(planes, rend.bounds);

        if (!isOnView)
            Pool.instances.DestroyObject(this.gameObject);
	}
}
