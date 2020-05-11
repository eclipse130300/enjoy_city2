using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utils.Editor
{
	public static class MenuTest
	{
		[MenuItem("Test/TimeScale/0.1")]
		public static void TimeScale01()
		{
			Time.timeScale = 0.1f;
		}

		[MenuItem("Test/TimeScale/0.25")]
		public static void TimeScale025()
		{
			Time.timeScale = 0.25f;
		}

		[MenuItem("Test/TimeScale/0.5")]
		public static void TimeScale05()
		{
			Time.timeScale = 0.5f;
		}

		[MenuItem("Test/TimeScale/1.0")]
		public static void TimeScale10()
		{
			Time.timeScale = 1f;
		}

		[MenuItem("Test/TimeScale/Pause")]
		public static void TimeScalePause()
		{
			Time.timeScale = 0;
		}

	    [MenuItem("Test/Print Stats")]
        public static void PrintStats()
	    {
	        var result = StatsHelper.Instance.GetResult();
            Debug.Log($"Min: {result.Min}, Max: {result.Max}, Average: {result.Average}, Count: {result.Count}");
	    }
	}
}
