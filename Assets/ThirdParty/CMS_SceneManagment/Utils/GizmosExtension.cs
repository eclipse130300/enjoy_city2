using UnityEngine;

namespace Utils
{
	public class GizmosExtension
	{
		public static void DrawCircle(Vector3 center, float radius, int parts = 360)
		{
			float delta = 2 * Mathf.PI / parts;
			//Vector3 prev = center;
			for (int i = 0; i < parts - 1; i++)
			{
				var angle1 = delta * i;
				var pos1 = center + new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * radius;
				var angle2 = delta * ((i + 1) % parts);
				var pos2 = center + new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * radius;
				Gizmos.DrawLine(pos1, pos2);
				//prev = pos;
			}
		}

		public static void DrawTape(Vector3 center, float innerRadius, float outerRadius, int parts = 360)
		{
			float delta = 2 * Mathf.PI / parts;
			//Vector3 prev = center;
			for (int i = 0; i < parts - 1; i++)
			{
				var radius1 = i % 2 == 0 ? innerRadius : outerRadius;
				var radius2 = i % 2 != 0 ? innerRadius : outerRadius;

				var angle1 = delta * i;
				var pos1 = center + new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * radius1;
				var angle2 = delta * ((i + 1) % parts);
				var pos2 = center + new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * radius2;
				Gizmos.DrawLine(pos1, pos2);
				//prev = pos;
			}
		}
	}
}
