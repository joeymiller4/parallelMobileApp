using UnityEngine;

namespace Lean.Touch
{
	// This script will place this GameObject along a line of a plane when selected
	public class LeanSelectablePlaceOnLine : LeanSelectableBehaviour
	{
		[Tooltip("A point on the line in world space")]
		public Vector3 LinePoint;

		[Tooltip("The direction of the line in world space")]
		public Vector3 LineDirection = Vector3.up;
		
		protected virtual void Update()
		{
			// Is this GameObject selected?
			if (Selectable.IsSelected == true)
			{
				// Does it have a selected finger?
				var finger = Selectable.SelectingFinger;

				if (finger != null)
				{
					// Find the ray for this finger
					var ray = finger.GetRay();
					
					// Make sure they're not parallel
					if (ray.direction != LineDirection)
					{
						// Find distance along LinePoint & LineDirection that is closes to the ray
						var a = Vector3.Cross(ray.direction, LineDirection);
						var b = Vector3.Cross(LinePoint - ray.origin, ray.direction);
						var d = Vector3.Dot(b, a) / Vector3.Dot(a, a);

						// Place this GameObject there
						transform.position = LinePoint + LineDirection * d;
					}
				}
			}
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.DrawRay(LinePoint,  LineDirection);
			Gizmos.DrawRay(LinePoint, -LineDirection);
		}
#endif
	}
}