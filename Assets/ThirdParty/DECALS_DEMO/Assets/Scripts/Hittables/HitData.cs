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
	/// Stores information about a hit.
	/// </summary>
	public struct HitData
	{
		public Vector3 Position { get; private set; }
		/// <summary>
		/// Needed to determine the projection direction.
		/// </summary>
		public Vector3 Direction { get; private set; }
		/// <summary>
		/// Used to render the mark.
		/// </summary>
		public Material Mark { get; private set; }
		/// <summary>
		/// In world units.
		/// </summary>
		public float MarkSize { get; private set; }


		public HitData(Vector3 position, Vector3 direction, Material mark, float markSize)
		{
			Position = position;
			Direction = direction;
			Mark = mark;
			MarkSize = markSize;

		}
	}
}
