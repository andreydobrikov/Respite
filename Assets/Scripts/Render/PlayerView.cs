using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// DON'T LOOK AT ME, I'M HIDEOUS.
/// 
/// This thing is Dr Wright's fetish material. 
/// 
/// The following list details all the idiotic quirks of this class.
/// 
/// 
/// 
/// </summary>

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(SphereCollider))]
public class PlayerView : MonoBehaviour 
{
	[SerializeField]
	public LayerMask collisionLayer = 0;
	
	public bool Dynamic				= false;
	public bool ShowCandidateRays 	= false;
	public bool ShowSucceededRays 	= true;
	public bool ShowExtrusionRays 	= false;
	public bool ShowFailedRays		= false;
	
	public float m_nudgeMagnitude 	= 0.02f; // This determines how far vertices are extruded from their mesh centroid when casting rays. Needed to prevent false collisions.
	public float m_centreNudge 		= 0.02f;
	private MeshFilter m_filter 	= null;
	private Mesh m_cameraMesh;
	private SphereCollider m_viewCollider = null;
	private Dictionary<Collider, BoxColliderVertices> m_colliderVertices = new Dictionary<Collider, BoxColliderVertices>();
	
	void Start () 
	{
		m_filter 		= GetComponent<MeshFilter>();
		m_viewCollider 	= GetComponent<SphereCollider>();
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("LevelGeo") && other is BoxCollider)
		{
			BoxCollider newCollider = other as BoxCollider;
			BoxColliderVertices newVertices = new BoxColliderVertices();
			
			newVertices.collider = newCollider;
			
			newVertices.vertices.Add(new Vector3(-newCollider.size.x / 2.0f, -newCollider.size.y / 2.0f, 0.0f));
			newVertices.vertices.Add(new Vector3(-newCollider.size.x / 2.0f, newCollider.size.y / 2.0f, 0.0f));
			newVertices.vertices.Add(new Vector3( newCollider.size.x / 2.0f, -newCollider.size.y / 2.0f, 0.0f));
			newVertices.vertices.Add(new Vector3( newCollider.size.x / 2.0f, newCollider.size.y / 2.0f, 0.0f));
			
			m_colliderVertices.Add(other, newVertices);
			Debug.Log ("Added Collider");
			RebuildMesh();
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		m_colliderVertices.Remove(other);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Dynamic)
		{
			RebuildMesh();
		}
	}
	
	/// <summary>
	/// Rebuilds the view-mesh from a sorted list of occluders.
	/// </summary>
	private void RebuildMesh()
	{
		List<OccluderVector> occluders = GetOccluders();
		
		if(occluders.Count == 0)
		{
			m_filter.sharedMesh.Clear();
			return;	
		}
		
		int triangleCount = ((occluders.Count) - 1) * 3;
		
		Vector3[] 	vertices 	= new Vector3[occluders.Count + 1];
		Vector3[] 	normals 	= new Vector3[occluders.Count + 1];
		Color[] 	colors 		= new Color[occluders.Count + 1];
		Vector2[] 	uvs 		= new Vector2[occluders.Count + 1];
		int[] 		triangles 	= new int[triangleCount + 3];
		
		// Add the camera point manually
		vertices[0] = new Vector3(0.0f, 0.0f, 0.0f);
		uvs[0] 		= new Vector2(0.5f, 0.5f);
		colors[0]	= new Color(1.0f, 1.0f, 1.0f, 1.0f);
		
		int index = 1;
		
		Quaternion rotationInverse = Quaternion.Inverse(transform.rotation);
		
		float extentsVal = Mathf.Abs(m_viewCollider.radius * Mathf.Cos(Mathf.PI / 4));
		
		foreach(OccluderVector vert in occluders)
		{
			Vector3 localPosition 	= vert.vec - transform.position;
			vertices[index] 		= rotationInverse *  localPosition;
			uvs[index] 				= new Vector2((localPosition.x + extentsVal) / (extentsVal * 2.0f), (localPosition.y + extentsVal) / (extentsVal * 2.0f));
			normals[index] 			= new Vector3(0.0f, 0.0f -1.0f);
			colors[index]			= new Color(1.0f, 1.0f, 1.0f, 1.0f);
			
			index++;
		}
		
		// Loop through the points and build them there triangles
		int i = 1;
		for(; i < occluders.Count; i++)
		{
			triangles[(i - 1) * 3] 		= 0;
			triangles[(i - 1) * 3 + 1] 	= i;
			triangles[(i - 1) * 3 + 2] 	= i + 1;
		}
		
		triangles[(i - 1) * 3] 		= 0;
		triangles[(i - 1) * 3 + 1] 	= i;
		triangles[(i - 1) * 3 + 2] 	= 1;
		
		m_filter.sharedMesh.Clear();
		m_filter.sharedMesh.vertices 	= vertices;
		m_filter.sharedMesh.uv 			= uvs;
		m_filter.sharedMesh.normals 	= normals;
		m_filter.sharedMesh.colors		= colors;
		m_filter.sharedMesh.triangles 	= triangles;
	}
	
	/// <summary>
	/// Gets a list of all vertices that occlude the view area.
	/// The OccluderVector structure contains the hit-points of each collision, as well as the angle of the hit-point from the left extent.
	/// </summary>
	/// <returns>
	/// The occluder list, sorted around increasing angle from the left extent.
	/// </returns>
	private List<OccluderVector> GetOccluders()
	{
		RaycastHit hitInfo;
		
		List<Vector3> validVerts = new List<Vector3>();
		List<Vector3> extentsPairs = GetExtents();
		List<OccluderVector> occluders = new List<OccluderVector>();
		
		// Loop through each collider and add its vertices to the list
		foreach(var colliderPair in m_colliderVertices)
		{
			foreach(Vector3 vert in colliderPair.Value.vertices)
			{
				Vector3 worldPos = colliderPair.Key.transform.TransformPoint(vert);
				Vector3 direction = worldPos - transform.position;
				
				direction.z = 0.0f;
				direction *= 0.97f;
				
				// raycast all the vertices and see if any are occluded
				float magnitude = direction.magnitude;
				if(Physics.Raycast(transform.position, direction.normalized, out hitInfo, magnitude,  ~collisionLayer.value))
				{
					if(ShowCandidateRays)
					{
						Debug.DrawRay(transform.position, direction, Color.magenta); 
					}
					// Hit something. add it as a vert.
					validVerts.Add(hitInfo.point);
				}
				else
				{
					
					Vector3 newDirection = (worldPos - transform.position);
					newDirection.z = 0.0f;
					direction = newDirection * (1.0f + m_nudgeMagnitude);	
					direction.z = 0.0f;
					
					// Now check past the vertex
					if(direction.magnitude < m_viewCollider.radius)
					{
						
						
						
						// Look slightly past the vertex. If the new point is in the bounds of the collider, the ray can't pass.
						// Otherwise, bung a ray past the vertex and see what hits next.
						bool interior = false;
						foreach(var collider in m_colliderVertices)
						{
							if(collider.Key.bounds.Contains(transform.position + direction))
							{
								interior = true;
								break;
							}
						}
						
						if(colliderPair.Value.collider == null || !interior)
						{
							if(colliderPair.Value.collider != null)
							{
								// If there is going to be an extension ray, nudge the original towards the center of its collider to prevent it
								// being co-linear and confusing the later sort.
								Vector3 offsetDirection = colliderPair.Value.collider.bounds.center - worldPos;
								float recipDistance = m_centreNudge / offsetDirection.magnitude;
								
								Debug.DrawRay(worldPos + new Vector3(0.0f, 0.0f, -3.0f), offsetDirection, Color.red); 
								Debug.DrawRay(worldPos + new Vector3(0.0f, 0.0f, -3.2f), offsetDirection * m_centreNudge, Color.green); 
								validVerts.Add(worldPos + (offsetDirection * recipDistance));	
							}
							else
							{
								validVerts.Add(worldPos);	
							}
							
							
							Debug.DrawRay(transform.position, direction, Color.yellow); 
							Vector3 rayOrigin 			= transform.position + direction;
							magnitude					=  m_viewCollider.radius -  direction.magnitude;
							direction.Normalize();
							
							Debug.DrawRay(rayOrigin + new Vector3(0.0f, 0.0f, -1.0f), (direction * magnitude) + new Vector3(0.0f, 0.0f, -1.0f), Color.magenta);
							if(Physics.Raycast(rayOrigin, direction, out hitInfo, magnitude, ~collisionLayer.value))
							{
								// Hit
								validVerts.Add(hitInfo.point);
							}
							else
							{
								validVerts.Add((rayOrigin + (direction * magnitude)) + new Vector3(0.01f, 0.0f, 0.0f));	
							}
						}
						else
						{
							validVerts.Add(worldPos);	
						}
					}
					else
					{
						validVerts.Add(worldPos);		
					}		
				}
			}
		}
		
		foreach(var pair in extentsPairs)
		{
			Vector3 direction 	= pair - transform.position;
			
			if(Physics.Raycast(transform.position, direction, out hitInfo, direction.magnitude, ~collisionLayer.value))
			{
				validVerts.Add(hitInfo.point);
			}
			else
			{
				validVerts.Add(transform.position + direction);	
			}
		}
		
		// Output all results
		foreach(Vector3 vert in validVerts)
		{
			Vector3 directionToVert = vert - this.transform.position;
			
			OccluderVector newOccluder = new OccluderVector();
			newOccluder.vec = vert;
			newOccluder.vec.z = transform.position.z;
			Vector3 normalDirection = Vector3.Normalize(directionToVert);
			
			float dot = Vector3.Dot(Vector3.up, normalDirection);
			
			double angle = Math.Atan2((double)normalDirection.x, (double)normalDirection.y);
			if(double.IsNaN(angle))
			{
				Debug.LogWarning("NaN found in PlayerView: " + dot);	
			}
					
			newOccluder.angle = angle;
			occluders.Add(newOccluder);
		}
	
		occluders.Sort(OccluderComparison);
		
		if(ShowSucceededRays)
		{
			Color color1 = new Color(1.0f, 0.5f, 0.0f, 1.0f);
			Color color2 = new Color(0.5f, 1.0f, 0.0f, 1.0f);
			bool useColor1 = false;
			
			foreach(OccluderVector vert in occluders)
			{
				useColor1 = !useColor1;
				Debug.DrawRay (this.transform.position  + new Vector3(0.0f, 0.0f, -1.0f), vert.vec - this.transform.position , useColor1 ? color1 : color2);
			}
		}
		
		return occluders;
	}
	
	private List<Vector3> GetExtents()
	{
		List<Vector3> verts = new List<Vector3>();
		
		float extentsVal = Mathf.Abs(m_viewCollider.radius * Mathf.Cos(Mathf.PI / 4));
		
		verts.Add(transform.position + new Vector3(-extentsVal, extentsVal, 0.0f));
		verts.Add(transform.position + new Vector3(-extentsVal, -extentsVal, 0.0f));
		verts.Add(transform.position + new Vector3(extentsVal, extentsVal, 0.0f));
		verts.Add(transform.position + new Vector3(extentsVal, -extentsVal, 0.0f));
	
		return verts;
	}
			
	private static int OccluderComparison(OccluderVector v1, OccluderVector v2)
	{
		if(v1.angle == v2.angle)
		{
			// If the angles are equivalent, sort on their distance from the camera
			if( v1.vec.sqrMagnitude > v2.vec.sqrMagnitude)
			{
				return 1;	
			}
			else if( v2.vec.sqrMagnitude < v1.vec.sqrMagnitude)
			{
				return -1;	
			}
			return 0;
		}
		
		if(v1.angle > v2.angle)
		{
			return 1;	
		}
		
		return -1;
	}
	
	private class OccluderVector
	{
		public Vector3 vec;
		public double angle;
	}
	
	private class BoxColliderVertices
	{
		public BoxCollider collider;
		public List<Vector3> vertices = new List<Vector3>();	
	}
}
