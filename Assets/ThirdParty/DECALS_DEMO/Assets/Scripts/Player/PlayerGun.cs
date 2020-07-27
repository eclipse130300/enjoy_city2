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
	/// Responds to input and creates the shot.
	/// </summary>
	public class PlayerGun : MonoBehaviour
	{
		/// <summary>
		/// Fire location.
		/// </summary>
		[SerializeField]
		private Transform shootPoint;

		/// <summary>
		/// Shot to be instantiated.
		/// </summary>
		[SerializeField]
		private PlayerShot shotAsset;

		/// <summary>
		/// Fire force.
		/// </summary>
		[SerializeField]
		[Range(0.01f, float.PositiveInfinity)]
		private float shootForce = 20f;

		/// <summary>
		/// Shooting mode.
		/// </summary>
		[SerializeField]
		private bool singleShot = true;

		private void Update()
		{
			//toggle fire mode if needed
			if(Input.GetKeyDown(KeyCode.F))
				singleShot = !singleShot;

			//fire if input detected
			if(singleShot && Input.GetMouseButtonDown(0) || !singleShot && Input.GetMouseButton(0))
			{
				PlayerShot shot = Instantiate(shotAsset, shootPoint.position, shootPoint.rotation);
				shot.Body.AddForce(shootForce * shootPoint.forward, ForceMode.VelocityChange);
			}
		}
	}
}