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

using UnityEngine;

namespace Demo
{
	/// <summary>
	/// Projectile fired by the player. Calls the HittablesController on collision.
	/// Pooling is advised.
	/// </summary>
	public class PlayerShot : MonoBehaviour
	{
		/// <summary>
		/// Cached reference so we don't search for it with GetComponent which is slow.
		/// </summary>
		[SerializeField]
		private Transform trans;

		/// <summary>
		/// Cached reference so we don't search for it with GetComponent which is slow.
		/// </summary>
		[SerializeField]
		private Rigidbody body;

		/// <summary>
		/// Imprint on environment.
		/// </summary>
		[SerializeField]
		private Material mark;

		/// <summary>
		/// In world units. Use reasonably small values on mobile devices.
		/// </summary>
		[SerializeField]
		[Range(.01f, float.PositiveInfinity)]
		private float markSize = 1f;

		/// <summary>
		/// Previous position - used to determine hit direction.
		/// </summary>
		private Vector3 posPrev;
		
		/// <summary>
		/// Ref to the rigidbody.
		/// </summary>
		public Rigidbody Body { get { return body; } }

		/// <summary>
		/// Direction in which the hit will be applied on collision.
		/// (Velocity is no good - collision handler can be sometimes called after it is afected by impact)
		/// </summary>
		private Vector3 hitDirection;

		/// <summary>
		/// Ignore further processing when colliding with more than one object.
		/// </summary>
		private bool alreadyProcessed;

		private void Start()
		{
			//init
			hitDirection = trans.forward;
			posPrev = trans.position;
		}

		private void FixedUpdate()
		{
			//update hit direction
			hitDirection = trans.position - posPrev;
			posPrev = trans.position;
		}

		private void OnCollisionEnter(Collision collision)
		{
			Debug.Log("HIT PROCESSED!");

			if (alreadyProcessed)
				return;

			//notify the controller
			HittablesController.Instance.OnShotHit(new HitData(collision.contacts[0].point, hitDirection, mark, markSize), Color.red);

			//remove from scene
			gameObject.SetActive(false);

			alreadyProcessed = true;

/*			Debug.Log("HIT PROCESSED!");*/
		}

		private void OnCollisionExit(Collision collision)
		{
			Debug.Log("COLLISION EXIT!");
		}

	}
}