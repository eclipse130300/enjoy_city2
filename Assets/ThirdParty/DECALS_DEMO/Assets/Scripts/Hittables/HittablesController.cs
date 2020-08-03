// Copyright (c) 2018 stefan.v
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// // copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections.Generic;
using UnityEngine;

namespace Demo
{
	/// <summary>
	/// Handles collision of shots with the hittable environment.
	/// </summary>
	public class HittablesController : MonoBehaviour
	{
		/// <summary>
		/// How many may be visible at one time.
		/// After the limit is reached, old marks will be replaced by the new ones.
		/// USE small values (~100) for mobile devices.
		/// </summary>
		public const uint MAX_MARKS = 400;


		/// <summary>
		/// Cache for easy access.
		/// </summary>
		public List<Hittable> hittables;

		/// <summary>
		/// Cache for hits.
		/// </summary>
		private Vector3[] hits = new Vector3[MAX_MARKS];

		/// <summary>
		/// Index to store next hit.
		/// </summary>
		/// 
		private int hitIndex;

		/// <summary>
		/// Used to overwrite old hits.
		/// </summary>
		private bool hitLimitReached;

		/// <summary>
		/// Singleton-ish style.
		/// </summary>
		public static HittablesController Instance { get; private set; }

		private void Awake()
		{
			//there can be only one!
			if(Instance != null)
				Destroy(this);

			Instance = this;
		}

		/// <summary>
		/// Called when a shot has a collision.
		/// Checks all registered hittables with the provided shot data to see which one was hit.
		/// </summary>
		/// <param name="shot"></param>
		public void OnShotHit(HitData hitData, Color color)
		{
			//combine removing with adding
			Vector3? hitToOverwrite = (!hitLimitReached)? (Vector3?)null : hits[hitIndex];

			//global resultant - sum of all normalized resultants per hittable
			//resultant per hittable = sum of all triangles' (intersecting and not culled) normals
			Vector3 hitResultant;

			//if we have hits
			if(CheckAndStartSetup(hitData, out hitResultant, hitToOverwrite))
			{
/*				//get a random color
				Color color = new Color(Random.Range(0f, .9f), Random.Range(0f, .9f), Random.Range(0f, .9f), 1f);*/


				//get transform matrix
				Matrix4x4 shotSpaceTRS = GetShotSpaceTRSMatrix(hitData, -hitResultant);

				foreach(Hittable hittable in hittables)
				{
					//set rest of data
					hittable.EndHitSetup(shotSpaceTRS, color);

					//check again for removal - non-hit ones were not checked
					if(hitLimitReached)
						hittable.Clear(hitToOverwrite.Value);
				}

				//store current hit
				hits[hitIndex++] = hitData.Position;

				//check if limit reached
				if(hitIndex == MAX_MARKS)
				{
					hitIndex = 0;
					hitLimitReached = true;
				}
			}
		}

		/// <summary>
		/// Checks the list of hittables and combines the hit triangles' own resultant into a global one.
		/// Start the setup for a new hit on valid ones.
		/// </summary>
		/// <param name="hitData">Current shot data.</param>
		/// <param name="resultant">Sum of all hit hittables.</param>
		/// <param name="hitToOverwrite">Hit location which will be overwritten by the new hit.</param>
		/// <returns>True if at least one hittables was hit, otherwise false.</returns>
		private bool CheckAndStartSetup(HitData hitData, out Vector3 resultant, Vector3? hitToOverwrite)
		{
			//bounds to do a fast gross intersection test
			Bounds checkBox = new Bounds(hitData.Position, Vector3.one * hitData.MarkSize);

			//radius of the sphere used to check for intersecting triangles.
			float radius = Math.GetSphereRadiusInscribingCube(hitData.MarkSize);

			//true if at least one hit checks out
			bool valid = false;

			//global resultant
			resultant = Vector3.zero;

			//start per hittable check
			foreach(Hittable hittable in hittables)
			{
				//check bounds first - fast test
				if(checkBox.Intersects(hittable.Bounds))
				{
					//check for hit
					if(hittable.Check(hitData.Position, radius))
					{
						//update overall check result
						valid = true;

						//start hittable setup and add to global resultant
						resultant += hittable.StartHitSetup(hitData, hitToOverwrite).normalized;
					}
				}
			}

			//return check status
			return valid;
		}

		/// <summary>
		/// Construct a TRS matrix to use for shifting model's vertices to shot space.
		/// </summary>
		/// <param name="hitData">Current shot data.</param>
		/// <param name="forward">Direction used to determine TRS rotation.</param>
		/// <returns>TRS matrix for shot space.</returns>
		private Matrix4x4 GetShotSpaceTRSMatrix(HitData hitData, Vector3 forward)
		{
			//determine rotation, applying a random spin
			Quaternion rotation = Quaternion.AngleAxis(Random.Range(0f, 360f), forward) * Quaternion.LookRotation(forward);

			//shift start position so that hit point will in center (using Vector3.up because we are facing forward)
			Vector3 position = hitData.Position - rotation * (.5f * hitData.MarkSize * (Vector3.right + Vector3.up));

			//scale - x and y valuse matter (different values mean rectangular mark)
			Vector3 scale = hitData.MarkSize * Vector3.one;

			return Matrix4x4.TRS(position, rotation, scale).inverse;
		}
	}
}