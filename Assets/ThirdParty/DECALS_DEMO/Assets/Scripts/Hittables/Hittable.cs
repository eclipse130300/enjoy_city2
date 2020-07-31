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
using Utils;

namespace Demo
{
	/// <summary>
	/// Describes a hittable envritonment object.
	/// </summary>
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class Hittable : MonoBehaviour
	{
		/// <summary>
		/// Maximum triangle hits to store.
		/// </summary>
		private const int TRI_HITS_MAX = 2048;
		/// <summary>
		/// Contains first indices of intersecting triangles with the shot sphere test.
		/// Keep it static so we don't always allocate for this.
		/// </summary>
		private readonly static int[] triHits = new int[TRI_HITS_MAX];
		/// <summary>
		/// Current number of intersecting triangles.
		/// </summary>
		private static int triHitsCount;

		/// <summary>
		/// Shader shot TRS matrix transform property name.
		/// </summary>
		private readonly static int projID = Shader.PropertyToID("_ShotSpace");
		private readonly static int colorID = Shader.PropertyToID("_TintColor");

		[SerializeField]
		private Transform trans;
		[SerializeField]
		private MeshFilter filter;
		[SerializeField]
		private new MeshRenderer renderer;

		/// <summary>
		/// Angle between hit direction and face normal.
		/// Bigger values are culled.
		/// </summary>
		[Tooltip("Angle between hit direction and face normal. Bigger values are culled.")]
		[Range(1f, 180f)]
		[SerializeField]
		private float cullAngle = 90;

		private Mesh mesh;
		/// <summary>
		/// Use global values when testing for sphere/triangle intersection so that scale is taken into account.
		/// </summary>
		private Vector3[] vertsGlobal;
		private int[] tris;

		/// <summary>
		/// Renderer's materials.
		/// </summary>
		private List<Material> materials;

		/// <summary>
		/// In case of hit, stores current submeshes' material.
		/// </summary>
		private Material material;

		/// <summary>
		/// Helps with adding and removing hits, skipping initial submeshes and materials.
		/// </summary>
		private int subMeshCountInitial;

		/// <summary>
		/// Current hits on this object. Needed when clearing hits.
		/// </summary>
		private List<Vector3?> hits;

		/// <summary>
		/// If we detected and replaced the hit which is to be cleared.
		/// </summary>
		private bool hitOverwritten;

		/// <summary>
		/// Used to bypass searching for nothing.
		/// </summary>
		private int emptySlots;

		/// <summary>
		/// The mesh renderer's bounds.
		/// </summary>
		public Bounds Bounds { get { return renderer.bounds; } }

		#region Called from the custom editor.
			#if UNITY_EDITOR
				public Transform Trans { set { trans = value; } }
				public MeshFilter Filter { set { filter = value; } }
				public MeshRenderer Renderer { set { renderer = value; } }
			#endif
		#endregion

		private void Start()
		{
			//asign once
			mesh = filter.mesh;

			vertsGlobal = filter.mesh.vertices; //local
			//turn global
			for(int i = 0; i < vertsGlobal.Length; i++)
				vertsGlobal[i] = trans.TransformPoint(vertsGlobal[i]);

			tris = filter.mesh.triangles;
			
			//init
			materials = new List<Material>(renderer.materials);
			subMeshCountInitial = mesh.subMeshCount;

			hits = new List<Vector3?>();
		}

		/// <summary>
		/// Checks for intersecting triangles with the sphere representing the colliding shot and stores them.
		/// </summary>
		/// <param name="center">Sphere center.</param>
		/// <param name="radius">Sphere radius.</param>
		/// <returns>True, if any intersecting triangles, else false.</returns>
		public bool Check(Vector3 center, float radius)
		{
			//check and store hit triangles
			triHitsCount = Intersections.SphereTrianglesIntersection(
				center,
				radius, vertsGlobal, tris, tris.Length,
				triHits, TRI_HITS_MAX
			);

			return triHitsCount != 0;
		}

		/// <summary>
		/// Starts the setup for a new hit:
		/// - culls the detected triangles according to the current cull angle.
		/// - if any left, ads a new submesh and assigns culled triangles to it (or replaces an old one in case limit of hits was reached).
		/// - creates the hit material which will be have the data set in EndHitSetup.
		/// </summary>
		/// <param name="hitData">Current shot data.</param>
		/// <param name="hitToReplace">Hit location which will be overwritten by the new hit.</param>
		/// <returns>Resultant vector from non-culled triangles.</returns>
		public Vector3 StartHitSetup(HitData hitData, Vector3? hitToReplace)
		{
			//submesh triangles
			int[] submeshTris;

			//cull triangles based on hit direction and get resultant of rest of triangles' normal
			Vector3 resultant = FilterTris(hitData.Direction, out submeshTris);;

			//everything got culled			
			if(submeshTris.Length == 0)
				return resultant; //Vector3.zero

			//check if we 'have' the hit to replace
			int subMeshIndex = (hitToReplace.HasValue)? GetHitSubMeshIndex(hitToReplace.Value) : -1;

			//search for free slot
			if(subMeshIndex == -1 && emptySlots > 0)
			{
				subMeshIndex = materials.IndexOf(null);

				material = new Material(hitData.Mark);
				materials[subMeshIndex] = material;

				hits[subMeshIndex - subMeshCountInitial] = hitData.Position;

				emptySlots--;
			}
			else
			{
				//if we have a hit to replace
				if(subMeshIndex != -1)
				{
					material = materials[subMeshIndex];
					hits[subMeshIndex - subMeshCountInitial] = hitData.Position;

					hitOverwritten = true;
				}
				else //no hit to replace, create new
				{
					//save index
					subMeshIndex = mesh.subMeshCount;

					//make room fornew submesh
					mesh.subMeshCount++;

					//create mark material
					material = new Material(hitData.Mark);
					materials.Add(material);

					//store hit
					hits.Add(hitData.Position);
				}
			}

			//set the triangles of the new mark's submesh
			mesh.SetTriangles(submeshTris, subMeshIndex, false);

			return resultant;
		}

		/// <summary>
		/// Filters (culls) current hit triangles based on the hit direction.
		/// </summary>
		/// <param name="hitDir">Hit direction to test with.</param>
		/// <param name="submeshTris">Array with remaining triangles after filtering.</param>
		/// <returns>Resultant vector from non-culled triangles.</returns>
		private Vector3 FilterTris(Vector3 hitDir, out int[] submeshTris)
		{
			int exclusions = 0, triIndex, i;
			Vector3 resultant = Vector3.zero;

			//check away facing triangles and exclude them
			for(i = 0; i < triHitsCount; i++)
			{
				//compute triangle's normal 
				triIndex = triHits[i];
				Vector3 normal = Math.Normal(vertsGlobal[tris[triIndex]], vertsGlobal[tris[triIndex + 1]], vertsGlobal[tris[triIndex + 2]]).normalized;

				//add to resultant if direction is in range
				if(Vector3.Angle(normal, -hitDir) <= cullAngle)
					resultant += normal;
				else //exclude it all together
				{
					triHits[i] = -1; //mark it
					exclusions++; //increase 
				}
			}

			//construct triHits 'trimmed'
			submeshTris = new int[(triHitsCount - exclusions) * 3];
			int addIndex = 0;

			for(i = 0; i < triHitsCount; i++)
			{
				triIndex = triHits[i];

				//it was excluded
				if(triIndex == -1)
					continue;
				
				int index = addIndex * 3;

				submeshTris[index] = tris[triIndex];
				submeshTris[index + 1] = tris[triIndex + 1];
				submeshTris[index + 2] = tris[triIndex + 2];

				addIndex++;
			}

			return resultant;
		}

		/// <summary>
		/// Assigns shot space TRS matrix to current material, if it was hit, and sets the renderer's materials array.
		/// </summary>
		/// <param name="shotSpaceTRS">TRS matrix to use for shifting model's vertices to shot space.</param>
		public void EndHitSetup(Matrix4x4 shotSpaceTRS, Color color)
		{
			//no hit at the moment - exit
			if(material == null)
				return;

			//set color
			material.SetColor(colorID, color);

			//set matrix, appending with object space to world space one
			material.SetMatrix(projID, shotSpaceTRS * trans.localToWorldMatrix);

			//reassign materials array to renderer
			renderer.materials = materials.ToArray();

			//clear current
			material = null;
			
		}

		/// <summary>
		/// Cleares the specified hit from cache if it was not already overwritten.
		/// </summary>
		/// <param name="hit">Hit position.</param>
		public void Clear(Vector3 hit)
		{
			if(hitOverwritten)
			{
				hitOverwritten = false;
				return;
			}

			int subMeshIndex = GetHitSubMeshIndex(hit);

			if(subMeshIndex == -1)
				return;

			//clear hit data
			mesh.SetTriangles(default(int[]), subMeshIndex, false);
			materials[subMeshIndex] = null;
			renderer.materials = materials.ToArray();
			hits[subMeshIndex - subMeshCountInitial] = null;

			emptySlots++;
		}

		/// <summary>
		/// Return the index of the specified hit.
		/// </summary>
		/// <param name="hit">Hit position.</param>
		/// <returns>The index if it is in cache, else -1.</returns>
		private int GetHitSubMeshIndex(Vector3 hit)
		{
			//check if we have any hits - if these match => no hits, so exit
			if(mesh.subMeshCount == subMeshCountInitial)
				return -1;

			//skip initial submeshes and materials
			for(int i = subMeshCountInitial; i < mesh.subMeshCount; i++)
				//check the hit
				if(hits[i - subMeshCountInitial].HasValue && hits[i - subMeshCountInitial].Value == hit)
					return i;

			//still nothing
			return -1;
		}
	}
}