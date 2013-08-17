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
/// - Heavy on the allocations
/// - Lots of raycasting that can probably be trimmed down.
/// - Remove redundant verts for static light-meshes
/// - Think of a better way to deal with co-linear rays. They cause flickering at the moment
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
	public bool DisplayStats		= false;
	
	public float m_sphereExpansion	= 1.5f;
	public float m_nudgeMagnitude 	= 0.02f; 
	public float m_centreNudge 		= 0.005f;	// This determines how far vertices are extruded from their mesh centroid when casting rays. Needed to prevent false collisions.
	public float m_vertexCast		= 0.99f;
	private MeshFilter m_filter 	= null;
	private SphereCollider m_viewCollider = null;
	private Dictionary<Collider, ColliderVertices> m_colliderVertices = new Dictionary<Collider, ColliderVertices>();
	
	void Start () 
	{
		m_filter 		= GetComponent<MeshFilter>();
		m_viewCollider 	= GetComponent<SphereCollider>();
		
		if(m_filter.mesh == null)
		{
			m_filter.mesh = new Mesh();	
		}
		
		enabled = false;	
	}
	
	void OnBecameVisible()
	{
		Debug.Log(name + " became visible");
		enabled = true;	
	}
	
	void OnBecameInvisible()
	{
		Debug.Log(name + " became invisible");
		enabled = false;	
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("LevelGeo"))
		{
			if(m_colliderVertices.ContainsKey(other))
			{
				return;	
			}
			
			if(other is SphereCollider)
			{
				SphereCollider newCollider = other as SphereCollider;
				ColliderVertices newVertices = new ColliderVertices();
				
				newVertices.collider = newCollider;
				
				Vector2 diff = newCollider.transform.position - transform.position;
				
				Vector3 normal = Vector3.Cross(diff, Vector3.forward);
				
				
				Vector2 p0 = (normal.normalized * newCollider.radius);
				Vector2 p1 = -p0;
				
				newVertices.vertices.Add(p0);
				newVertices.vertices.Add(p1);
				
				m_colliderVertices.Add(other, newVertices);
				RebuildMesh();
			}
			
			
			if(other is BoxCollider)
			{
				BoxCollider newCollider = other as BoxCollider;
				ColliderVertices newVertices = new ColliderVertices();
				
				newVertices.collider = newCollider;
				
				newVertices.vertices.Add(new Vector3(-newCollider.size.x / 2.0f, -newCollider.size.y / 2.0f, 0.0f));
				newVertices.vertices.Add(new Vector3(-newCollider.size.x / 2.0f, newCollider.size.y / 2.0f, 0.0f));
				newVertices.vertices.Add(new Vector3( newCollider.size.x / 2.0f, -newCollider.size.y / 2.0f, 0.0f));
				newVertices.vertices.Add(new Vector3( newCollider.size.x / 2.0f, newCollider.size.y / 2.0f, 0.0f));
				
				m_colliderVertices.Add(other, newVertices);
				RebuildMesh();
			}
		}
	}
	
	void OnRegionTransition()
	{
		m_colliderVertices.Clear();	
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
		Vector2[] 	uvs1 		= new Vector2[occluders.Count + 1];
		int[] 		triangles 	= new int[triangleCount + 3];
		
		// Add the camera point manually
		vertices[0] = new Vector3(0.0f, 0.0f, 0.0f);
		uvs[0] 		= new Vector2(0.5f, 0.5f);
		uvs1[0] 		= new Vector2(0.5f, 0.5f);
		colors[0]	= new Color(1.0f, 1.0f, 1.0f, 1.0f);
		
		int index = 1;
		
		Quaternion rotationInverse = Quaternion.Inverse(transform.rotation);
		
		float extentsVal = Mathf.Abs(m_viewCollider.radius * Mathf.Cos(Mathf.PI / 4));
		
		foreach(OccluderVector vert in occluders)
		{
			Vector3 localPosition 	= vert.vec - transform.position;
			localPosition.z = 0.0f;
			
			vertices[index] 		= rotationInverse *  localPosition;
			uvs[index] 				= new Vector2((localPosition.x + extentsVal) / (extentsVal * 2.0f), (localPosition.y + extentsVal) / (extentsVal * 2.0f));
			uvs1[index]				= uvs[index];
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
		m_filter.sharedMesh.uv1 		= uvs1;
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
		Vector3 objectPosition = transform.position;
		objectPosition.z = 0.0f;
		
		RaycastHit hitInfo;
		
		List<Vector3> validVerts = new List<Vector3>();
		List<Vector3> extentsPairs = GetExtents();
		List<OccluderVector> occluders = new List<OccluderVector>();
		
		// Loop through each collider and add its vertices to the list
		foreach(var colliderPair in m_colliderVertices)
		{
			if(colliderPair.Key is SphereCollider)
			{
				SphereCollider collider = colliderPair.Key as SphereCollider;
				Vector2 diff = colliderPair.Key.transform.position - objectPosition;
				
				Vector3 normal = Vector3.Cross(diff, Vector3.forward);
				
				Vector2 p0 = (normal.normalized * collider.radius);
				Vector2 p1 = -p0;
				
				p0 = Quaternion.Inverse(colliderPair.Key.transform.rotation) * (Vector3)p0;
				p1 = Quaternion.Inverse(colliderPair.Key.transform.rotation) * (Vector3)p1;
				
				colliderPair.Value.vertices[0] = p0;
				colliderPair.Value.vertices[1] = p1;
				
				
				foreach(var vert in colliderPair.Value.vertices)
				{
					Vector3 worldPos = colliderPair.Key.transform.TransformPoint(vert);
					Vector3 direction = worldPos - objectPosition;
					
					if(Physics.Raycast(objectPosition, direction.normalized, out hitInfo, direction.magnitude, 1 <<  collisionLayer))
					{
						if(ShowExtrusionRays)
						Debug.DrawLine(objectPosition, hitInfo.point, Color.green);
						validVerts.Add(hitInfo.point);
					}
					else
					{
						if(ShowExtrusionRays)
						Debug.DrawLine(objectPosition, worldPos, Color.red);
						validVerts.Add(worldPos);	
					}
						
				}
				
				foreach(var vert in colliderPair.Value.vertices)
				{
					Vector3 worldPos = colliderPair.Key.transform.TransformPoint(vert * m_sphereExpansion);
					Vector3 direction = worldPos - objectPosition;
					
					if(Physics.Raycast(objectPosition, direction.normalized, out hitInfo, m_viewCollider.radius, 1 <<  collisionLayer))
					{
						if(ShowExtrusionRays)
						Debug.DrawLine(objectPosition, hitInfo.point, Color.green);
						validVerts.Add(hitInfo.point);
					}
					else
					{
						if(ShowExtrusionRays)
						Debug.DrawLine(objectPosition, objectPosition + (direction.normalized * m_viewCollider.radius), Color.red);
						validVerts.Add(objectPosition + (direction.normalized * m_viewCollider.radius));	
					}
						
				}
				
			}
			else
			{
				foreach(Vector3 vert in colliderPair.Value.vertices)
				{
					Vector3 worldPos = colliderPair.Key.transform.TransformPoint(vert);
					Vector3 direction = worldPos - objectPosition;
					
					direction.z = 0.0f;
					
					// raycast all the vertices and see if any are occluded
					float magnitude = direction.magnitude;
					
					float val = direction.magnitude -  m_nudgeMagnitude / direction.magnitude;
					
					direction *= m_nudgeMagnitude;
					
					
					
						
						
					if(Physics.Raycast(objectPosition, direction.normalized, out hitInfo, val, 1 <<  collisionLayer))
					{
						if(ShowFailedRays)
						Debug.DrawLine(objectPosition, hitInfo.point, Color.green);
						
						// Hit something. add it as a vert.
						validVerts.Add(hitInfo.point);
					}
					else
					{
						Vector3 newDirection = (worldPos - objectPosition);
						newDirection.z = 0.0f;
						direction = newDirection * (1.0f + m_nudgeMagnitude);	
						direction.z = 0.0f;
										
							
						if(colliderPair.Value.collider != null)
						{
							// If there is going to be an extension ray, nudge the original towards the center of its collider to prevent it
							// being co-linear and confusing the later sort.
							Vector3 offsetDirection = colliderPair.Value.collider.bounds.center - worldPos;
							float recipDistance = m_centreNudge / offsetDirection.magnitude;
							
							validVerts.Add(worldPos + (offsetDirection * recipDistance));	
							
							Vector3 vertexRayDirection = (worldPos - (offsetDirection * recipDistance)) - objectPosition;
							vertexRayDirection.z = 0.0f;
							
							if(ShowCandidateRays)
							{
								Debug.DrawLine(objectPosition + new Vector3(0.0f, 0.0f, -1.2f), objectPosition + vertexRayDirection + new Vector3(0.0f, 0.0f, -1.2f), Color.yellow);
								Debug.DrawRay(objectPosition + new Vector3(0.0f, 0.0f, -1.2f), (vertexRayDirection.normalized * m_viewCollider.radius) + new Vector3(0.0f, 0.0f, -1.2f), Color.red);
							}
							
							if(Physics.Raycast(objectPosition, vertexRayDirection.normalized, out hitInfo, m_viewCollider.radius, 1 << collisionLayer))
							{
								validVerts.Add(hitInfo.point);
							}
							else
							{
								validVerts.Add(objectPosition + (vertexRayDirection.normalized * m_viewCollider.radius));	
							}
						}
						else
						{
							validVerts.Add(worldPos);	
						}
					}
				}
			}
		}
		
		foreach(var pair in extentsPairs)
		{
			Vector3 source = objectPosition;
				source.z = 0.0f;
			
			Vector3 direction 	= pair - objectPosition;
			
			Vector3 offset = new Vector3(0.0f, 0.0f, -0.0f);
			
			if(ShowSucceededRays)
				Debug.DrawLine(source + offset, source + direction + offset, Color.red, 1.0f);
			
			if(Physics.Raycast((Vector2)objectPosition, direction, out hitInfo, direction.magnitude, 1 << collisionLayer))
			{
				validVerts.Add(hitInfo.point);
				
				
				
				Vector3 target = hitInfo.point;
				target.z = 0.0f;
				
				if(ShowSucceededRays)
					Debug.DrawLine(source + offset, target + offset, Color.magenta, 1.0f);
					
				
			}
			else
			{
				validVerts.Add(objectPosition + direction);	
			}
		}
		
		// Output all results
		foreach(Vector3 vert in validVerts)
		{
			Vector3 directionToVert = vert - objectPosition;
			
			OccluderVector newOccluder = new OccluderVector();
			newOccluder.vec = vert;
			
			Vector3 normalDirection = Vector3.Normalize(directionToVert);
			
			double angle = Math.Atan2((double)normalDirection.x, (double)normalDirection.y);
					
			newOccluder.angle = angle;
			occluders.Add(newOccluder);
			
			
			Vector3 source = objectPosition;
			source.z = 0.0f;
			
			
			Vector3 offset = new Vector3(0.0f, 0.0f, -2.3f);
			
			if(ShowSucceededRays)
				Debug.DrawLine(source + offset , vert + offset, Color.green);
		}
		m_sortedEntries = occluders.Count;
		occluders.Sort(OccluderComparison);
		
		return occluders;
	}
	
	private void OnGUI()
	{
		if(DisplayStats)
		{
			GUILayout.BeginArea(new Rect(150.0f, 10.0f, 200.0f, 100.0f), (GUIStyle)("Box"));	
			
			GUILayout.Label("Ray Count: \t" + m_lastRayCount);
			GUILayout.Label("Sorted Entries: \t" + m_sortedEntries);
			
			GUILayout.EndArea();
		}
	}
	
	private List<Vector3> GetExtents()
	{
		List<Vector3> verts = new List<Vector3>();
		
		float extentsVal = Mathf.Abs(m_viewCollider.radius * Mathf.Cos(Mathf.PI / 4));
		
		Vector3 objectPosition = transform.position;
		objectPosition.z = 0.0f;
		
		verts.Add(objectPosition + new Vector3(-extentsVal, extentsVal, 0.0f));
		verts.Add(objectPosition + new Vector3(-extentsVal, -extentsVal, 0.0f));
		verts.Add(objectPosition + new Vector3(extentsVal, extentsVal, 0.0f));
		verts.Add(objectPosition + new Vector3(extentsVal, -extentsVal, 0.0f));
	
		return verts;
	}
			
	private static int OccluderComparison(OccluderVector v1, OccluderVector v2)
	{
		if(v1.angle == v2.angle)
		{
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
	
	private class ColliderVertices
	{
		public Collider collider;
		public List<Vector3> vertices = new List<Vector3>();	
	}
	
	private int m_lastRayCount = 0;
	private int m_sortedEntries = 0;
}
