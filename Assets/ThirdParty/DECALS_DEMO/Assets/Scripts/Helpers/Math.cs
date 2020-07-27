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
	/// Helper math functions.
	/// </summary>
	public static class Math 
	{
		/// <summary>
		/// Return the normal vector to the plane specified by the three vertices. Vertex order matters!
		/// </summary>
		/// <param name="v0"></param>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <returns></returns>
		public static Vector3 Normal(Vector3 v0, Vector3 v1, Vector3 v2)
		{
			return Vector3.Cross(v1 - v0, v2 - v0);
		}

		/// <summary>
		/// Computes the radius of the sphere which inscribes the cube.
		/// </summary>
		/// <param name="length">Cube's edge length</param>
		/// <returns>The radius.</returns>
		public static float GetSphereRadiusInscribingCube(float length)
		{
			//apply pythagoras theorem 2 times
			float edgePow2 = length * length;
			float faceDiag = Mathf.Sqrt(edgePow2 + edgePow2);

			//divide diameter by 2
			return Mathf.Sqrt(edgePow2 + faceDiag * faceDiag) / 2f;
		}
	}
}