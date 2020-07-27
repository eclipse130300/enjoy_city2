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
using System.Runtime.InteropServices;

namespace Demo
{
	/// <summary>
	/// Helper intersection functions.
	/// </summary>
	public static class Intersections
	{
		/// <summary>
		/// Performs sphere/triangle intersection with the provided data.
		/// Ref: https://github.com/sharpdx/SharpDX/blob/master/Source/SharpDX.Mathematics/Collision.cs
		/// </summary>
		/// <param name="center">Sphere local position.</param>
		/// <param name="radius">Sphere radius.</param>
		/// <param name="verts">Vertices array.</param>
		/// <param name="tris">Triangles array.</param>
		/// <param name="trisCount">Length of triangles array.</param>
		/// <param name="intersections">Out: First indices of triangles that are intersectingwith the sphere.</param>
		/// <param name="maxIntersections">Maximum of allowed intersections.</param>
		/// <returns>No. of intersecting triangles.</returns>
		[DllImport("IntersectionTest")]
		public static extern int SphereTrianglesIntersection(
			Vector3 center, float radius,
			Vector3[] verts, int[] tris, int trisCount,
			int[] intersections, int maxIntersections
		);
	}
}