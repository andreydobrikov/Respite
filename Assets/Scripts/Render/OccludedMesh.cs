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
[RequireComponent(typeof(BoxCollider))]
public class OccludedMesh : MonoBehaviour 
{
	[SerializeField]
	public LayerMask collisionLayer = 0;
	
	public bool Dynamic				= false;
	public bool ShowCandidateRays 	= false;
	public bool ShowSucceededRays 	= false;
	public bool ShowExtrusionRays 	= false;
	public bool ShowFailedRays		= false;
	public bool DisplayStats		= false;
	
	private float CalculativeOffset	= 0.0f; // Oh dear
	public float m_sphereExpansion	= 1.5f;
	public float m_nudgeMagnitude 	= 0.02f; 
	public float m_centreNudge 		= 0.005f;	// This determines how far vertices are extruded from their mesh centroid when casting rays. Needed to prevent false collisions.
	public float m_vertexCast		= 0.99f;
	private MeshFilter m_filter 	= null;
	private BoxCollider m_viewCollider = null;
	private Dictionary<Collider, ColliderVertices> m_colliderVertices = new Dictionary<Collider, ColliderVertices>();
	
	void OnEnable()
	{
		validVerts.Capacity = 200;
		
		// Bung four values into the extents to set the initial capacity
		extentsVals.Add(Vector3.zero);
		extentsVals.Add(Vector3.zero);
		extentsVals.Add(Vector3.zero);
		extentsVals.Add(Vector3.zero);

		
		for(int occluderID = 0; occluderID < m_maxOccluderVerts; occluderID++)
		{
			vertices[occluderID] = new Vector3();
			uvs[occluderID] 	= new Vector2();
			normals[occluderID] 	= new Vector3(0.0f, -1.0f, 0.0f);
			colors[occluderID]		= new Color(1.0f, 1.0f, 1.0f, 1.0f);
			
			m_occluders[occluderID] = new OccluderVector();		
		}
		 
				
		vertices[0] = new Vector3(0.0f, 0.0f, 0.0f);
		uvs[0] 		= new Vector2(0.5f, 0.5f);
		
		m_occluders[m_maxOccluderVerts] = new OccluderVector();	
	}
	
	void Start () 
	{
		m_filter 		= GetComponent<MeshFilter>();
		m_viewCollider 	= GetComponent<BoxCollider>();
		
		if(m_filter.mesh == null)
		{
			m_filter.mesh = new Mesh();	
		}
		
		CalculativeOffset = -transform.localPosition.y;
		
		enabled = false;	
		
		
	}
	
	void OnBecameVisible()
	{
		enabled = true;	
	}
	
	void OnBecameInvisible()
	{
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
				
				Vector3 diff = newCollider.transform.position - transform.position;
				
				Vector3 normal = Vector3.Cross(diff, Vector3.up);
				
				Vector3 p0 = (normal.normalized * newCollider.radius);
				Vector3 p1 = -p0;
				
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
				
				newVertices.vertices.Add(new Vector3(-newCollider.size.x / 2.0f, 0.0f, -newCollider.size.z / 2.0f));
				newVertices.vertices.Add(new Vector3(-newCollider.size.x / 2.0f, 0.0f, newCollider.size.z / 2.0f));
				newVertices.vertices.Add(new Vector3( newCollider.size.x / 2.0f, 0.0f, -newCollider.size.z / 2.0f));
				newVertices.vertices.Add(new Vector3( newCollider.size.x / 2.0f, 0.0f, newCollider.size.z / 2.0f));
				
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
		GetOccluders();
		
		if(m_occluderCount > m_maxOccluderVerts)
		{
			Debug.Log("Occluder vert-count exceeds maximum of " + m_maxOccluderVerts + ". light mesh will be malformed");	
		}
		
		int triangleCount = ((m_occluderCount) ) * 3;
		
		// Not much I can do about this allocation
		int[] triangles	= new int[triangleCount];
		
		Quaternion rotationInverse = Quaternion.Inverse(transform.rotation);
		
		float extentsVal = Mathf.Abs(m_viewCollider.size.x * 0.7f * Mathf.Cos(Mathf.PI / 4));
		
		for(int vertID = 0; vertID < m_occluderCount && vertID < m_maxOccluderVerts - 1; vertID++)
		{
			Vector3 localPosition 	= m_occluders[vertID].vec - transform.position;
			
			//Debug.DrawLine(transform.position, transform.position + localPosition, Color.red);
			localPosition.y = 0.0f;
			
			vertices[vertID + 1] 	= rotationInverse *  localPosition;
			uvs[vertID + 1]  = new Vector2((localPosition.x + extentsVal) / (extentsVal * 2.0f), (localPosition.z + extentsVal) / (extentsVal * 2.0f));
		}
		
		// Loop through the points and build them there triangles
		int i = 1;
		for(; i < m_occluderCount && i < m_maxOccluderVerts; i++)
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
		m_filter.sharedMesh.uv1 		= uvs;
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
	private void GetOccluders()
	{
		Vector3 objectPosition = transform.position;
		objectPosition.y += CalculativeOffset;
		
		RaycastHit hitInfo;
		
		// TODO: Don't instantiate these every frame
		validVerts.Clear();
		UpdateExtents();
		
		// Loop through each collider and add its vertices to the list
		foreach(var colliderPair in m_colliderVertices)
		{
			// Process SphereColliders
			if(colliderPair.Key is SphereCollider)
			{
				SphereCollider collider = colliderPair.Key as SphereCollider;
				
				Vector3 diff = colliderPair.Key.transform.position - objectPosition;
				
				Vector3 normal = Vector3.Cross(diff, Vector3.up);
				
				Vector3 p0 = (normal.normalized * collider.radius);
				Vector3 p1 = -p0;
				
				p0 = Quaternion.Inverse(colliderPair.Key.transform.rotation) * (Vector3)p0;
				p1 = Quaternion.Inverse(colliderPair.Key.transform.rotation) * (Vector3)p1;
				
				colliderPair.Value.vertices[0] = p0;
				colliderPair.Value.vertices[1] = p1;
				
				for(int vertID = 0; vertID < colliderPair.Value.vertices.Count; vertID++)
				{
					Vector3 worldPos = colliderPair.Key.transform.TransformPoint(colliderPair.Value.vertices[vertID]);
					Vector3 direction = worldPos - objectPosition;
					
					if(Physics.Raycast(objectPosition, direction.normalized, out hitInfo, direction.magnitude, 1 <<  collisionLayer))
					{
						if(ShowExtrusionRays) { Debug.DrawLine(objectPosition, hitInfo.point, Color.green); }
						
						validVerts.Add(hitInfo.point);
					}
					else
					{
						if(ShowExtrusionRays) { Debug.DrawLine(objectPosition, worldPos, Color.red); }
						
						validVerts.Add(worldPos);	
					}
				}
				
				
				for(int vertID = 0; vertID < colliderPair.Value.vertices.Count; vertID++)
				{
					// Find the transformed position of the vertex scaled by the mysterious expansion factor that
					Vector3 worldPos = colliderPair.Key.transform.TransformPoint(colliderPair.Value.vertices[vertID] * m_sphereExpansion);
					
					
					Vector3 direction = worldPos - objectPosition;
					
				
					
					if(Physics.Raycast(objectPosition, direction.normalized, out hitInfo, m_viewCollider.size.x, 1 <<  collisionLayer))
					{
						if(ShowExtrusionRays) {	Debug.DrawLine(objectPosition, hitInfo.point, Color.green); }
						
						validVerts.Add(hitInfo.point);
					}
					else
					{
						if(ShowExtrusionRays) {	Debug.DrawLine(objectPosition, objectPosition + (direction.normalized * m_viewCollider.size.x), Color.red); }
						
						validVerts.Add(objectPosition + (direction.normalized * m_viewCollider.size.x));	
					}
				}
			}
			else // Process BoxColliders
			{
				for(int vertID = 0; vertID < colliderPair.Value.vertices.Count; vertID++)
				{
					Vector3 worldPos = colliderPair.Key.transform.TransformPoint(colliderPair.Value.vertices[vertID]);
					worldPos.y = objectPosition.y;
					Vector3 direction = worldPos - objectPosition;
					
					
					float val = direction.magnitude -  m_nudgeMagnitude / direction.magnitude;
					
					direction *= m_nudgeMagnitude;
						
					if(Physics.Raycast(objectPosition, direction.normalized, out hitInfo, val, 1 <<  collisionLayer))
					{
						if(ShowFailedRays) { Debug.DrawLine(objectPosition, hitInfo.point, Color.green); }
						
						// Hit something. add it as a vert.
						validVerts.Add(hitInfo.point);
					}
					else
					{
						Vector3 newDirection = (worldPos - objectPosition);
						direction = newDirection * (1.0f + m_nudgeMagnitude);	
										
							
						if(colliderPair.Value.collider != null)
						{
							// If there is going to be an extension ray, nudge the original towards the center of its collider to prevent it
							// being co-linear and confusing the later sort.
							Vector3 offsetDirection = colliderPair.Value.collider.bounds.center - worldPos;
							
							float recipDistance = m_centreNudge / offsetDirection.magnitude;
							
							validVerts.Add(worldPos + (offsetDirection * recipDistance));	
							
							Vector3 vertexRayDirection = (worldPos - (offsetDirection * recipDistance)) - objectPosition;
							
							if(ShowCandidateRays)
							{
								Debug.DrawLine(objectPosition + new Vector3(0.0f, -1.2f, 0.0f), objectPosition + vertexRayDirection + new Vector3(0.0f, -1.2f, 0.0f), Color.yellow);
								Debug.DrawRay(objectPosition + new Vector3(0.0f, -1.2f, 0.0f), (vertexRayDirection.normalized * m_viewCollider.size.x) + new Vector3(0.0f, -1.2f, 0.0f), Color.red);
							}
							
						
							
							if(Physics.Raycast(objectPosition, vertexRayDirection.normalized, out hitInfo, m_viewCollider.size.x, 1 << collisionLayer))
							{
								validVerts.Add(hitInfo.point);
							}
							else
							{
								validVerts.Add(objectPosition + (vertexRayDirection.normalized * m_viewCollider.size.x));	
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
		
		for(int pairID = 0; pairID < extentsVals.Count; pairID++)
		{
			Vector3 source = objectPosition;
			
			Vector3 direction 	= extentsVals[pairID] - objectPosition;
			direction.y = 0.0f;
			
			if(Physics.Raycast(source, direction, out hitInfo, direction.magnitude, 1 << collisionLayer))
			{
				validVerts.Add(hitInfo.point);
			}
			else
			{
				validVerts.Add(objectPosition + direction);	
			}
		}
		
		// Output all results
		for(int vertID = 0; vertID < validVerts.Count && vertID < m_maxOccluderVerts; vertID++)
		{
			Vector3 directionToVert = validVerts[vertID] - objectPosition;
			
			m_occluders[vertID].vec = validVerts[vertID];
			
			Vector3 normalDirection = Vector3.Normalize(directionToVert);
			
			double angle = Math.Atan2((double)normalDirection.x, (double)normalDirection.z);
					
			m_occluders[vertID].angle = angle;
			
			Vector3 source = objectPosition;
		}
		
		m_occluderCount = validVerts.Count;
		
		Array.Sort(m_occluders, 0, m_occluderCount);
		
		float delta = 1.0f / (float)m_occluderCount;
		for(int i = 0; i < m_occluderCount; i++)
		{
			Color current = new Color(delta * i, 0.0f, 0.0f, 1.0f);
			if(ShowSucceededRays)
				Debug.DrawLine(transform.position , m_occluders[i].vec , current);
		}
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
	
	private void UpdateExtents()
	{
		float extentsVal = Mathf.Abs(m_viewCollider.size.x * Mathf.Cos(Mathf.PI / 4));
		
		Vector3 objectPosition = transform.position;
		
		extentsVals[0] = objectPosition + new Vector3(-extentsVal, 0.0f, extentsVal);
		extentsVals[1] = objectPosition + new Vector3(-extentsVal, 0.0f, -extentsVal);
		extentsVals[2] = objectPosition + new Vector3(extentsVal, 0.0f, extentsVal);
		extentsVals[3] = objectPosition + new Vector3(extentsVal, 0.0f, -extentsVal);
	
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
	
	private class OccluderVector : IComparable<OccluderVector>
	{
		public Vector3 vec;
		public double angle;
		
		public int CompareTo(OccluderVector that)
    	{
			
			if(angle == that.angle)
			{
				return 0;
			}
			
			if(angle > that.angle)
			{
				return 1;	
			}
			
			return -1;
		}
	}
	
	private class ColliderVertices
	{
		public Collider collider;
		public List<Vector3> vertices = new List<Vector3>();	
	}
	
	private int m_lastRayCount = 0;
	private int m_sortedEntries = 0;
	
	List<Vector3> extentsVals		= new List<Vector3>();
	List<Vector3> validVerts 		= new List<Vector3>();
	
	private const int m_maxOccluderVerts = 300;
	
	Vector3[] 	vertices 	= new Vector3[m_maxOccluderVerts];
	Vector3[] 	normals 	= new Vector3[m_maxOccluderVerts];
	Color[] 	colors 		= new Color[m_maxOccluderVerts];
	Vector2[] 	uvs 		= new Vector2[m_maxOccluderVerts];
	
	private OccluderVector[] m_occluders = new OccluderVector[m_maxOccluderVerts + 1];
	
	private int m_occluderCount = 0;
	
}
