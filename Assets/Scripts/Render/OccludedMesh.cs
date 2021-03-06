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
/// - Store static-colliders separately to dynamic and cache their transformed vert
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
		m_viewCollider 	= GetComponent<BoxCollider>();
		
		//validVerts.Capacity = m_maxOccluderVerts;
		extentsVals.Clear();

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
		
		float extentsVal = Mathf.Abs(m_viewCollider.size.x * Mathf.Cos(Mathf.PI / 4));
		
		m_fixedExtents[0] = new Vector3(-extentsVal, 0.0f, extentsVal);
		m_fixedExtents[1] = new Vector3(-extentsVal, 0.0f, -extentsVal);
		m_fixedExtents[2] = new Vector3(extentsVal, 0.0f, extentsVal);
		m_fixedExtents[3] = new Vector3(extentsVal, 0.0f, -extentsVal);
	}
	
	void Start () 
	{
		m_filter 		= GetComponent<MeshFilter>();
		
		if(m_filter.mesh == null)
		{
			m_filter.mesh = new Mesh();	
		}

		// TODO: For fuck's sake. I've even forgotten what this is supposed to do.
		// I suppose it's to ensure that all calculations take place on the y-zero plane, but that should surely the be
		// position, rather than localposition, no?
		CalculativeOffset = -transform.localPosition.y;
		
		enabled = false;	
	}
	
	void OnBecameVisible()
	{
		enabled = true;
		if(Dynamic)
		{
			m_activeMeshes++;
			m_meshes.Add(this);
		}
	}
	
	void OnBecameInvisible()
	{
		enabled = false;
		if(Dynamic)
		{
			m_activeMeshes--;
			m_meshes.Remove(this);
		}
	}
	
	public void OnTriggerEnter(Collider other)
	{
		
		if(other.gameObject.layer == LayerMask.NameToLayer("LevelGeo"))
		{
			if(m_colliderVertices.ContainsKey(other))
			{
				return;	
			}
			
			if(other is CapsuleCollider)
			{
				CapsuleCollider newCollider = other as CapsuleCollider;
				ColliderVertices newVertices = new ColliderVertices();
				
				newVertices.collider = newCollider;
				
				Vector3 diff = newCollider.transform.position - transform.position;
				
				Vector3 normal = Vector3.Cross(diff, Vector3.up);
				
				Vector3 p0 = (normal.normalized * newCollider.radius);
				Vector3 p1 = -p0;
				
				newVertices.vertices.Add(p0);
				newVertices.vertices.Add(p1);
				
				m_colliderVertices.Add(other, newVertices);

				if (GameObjectHelper.SearchParentsForComponent<Rigidbody>(other.gameObject) != null && GameObjectHelper.SearchParentsForComponent<OcclusionMeshStatic>(other.gameObject) == null)
				{
					DynamicColliderPair newPair = new DynamicColliderPair();

					newPair.collider = other;
					newPair.lastPosition = other.transform.position;
					newPair.lastRotation = other.transform.rotation;
					newPair.lastScale = other.transform.lossyScale;

					try
					{
						m_dynamicCollidersInView.Add(newPair);
						m_dynamicPairMap.Add(other, newPair);
					}
					catch (Exception e)
					{
						Debug.LogWarning("Failed added collider: " + other.name);
					}
					
				}
				
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

				if (GameObjectHelper.SearchParentsForComponent<Rigidbody>(other.gameObject) != null && GameObjectHelper.SearchParentsForComponent<OcclusionMeshStatic>(other.gameObject) == null)
				{
					DynamicColliderPair newPair = new DynamicColliderPair();

					newPair.collider = other;
					newPair.lastPosition = other.transform.position;
					newPair.lastRotation = other.transform.rotation;
					newPair.lastScale = other.transform.lossyScale;

					try
					{
						m_dynamicCollidersInView.Add(newPair);
						m_dynamicPairMap.Add(other, newPair);
					}
					catch (Exception e)
					{
						Debug.LogWarning("Failed added collider: " + other.name);
					}
				}
			}
			
			// If this is a static light, rebuild meshes immediately as there will be no Update
			if(!Dynamic)
			{
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

		DynamicColliderPair targetPair = null;
		if(m_dynamicPairMap.TryGetValue(other, out targetPair))
		{
			m_dynamicCollidersInView.RemoveAll(x => x.collider == other);
			m_dynamicPairMap.Remove(other);
		}

		
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_activeMeshes = 0;
		if(Dynamic)
		{
			// Check dynamic colliders to see if they've changed before we recalculate everything.
			bool changed = false;

			Vector3 newPosition = Vector3.zero;
			Quaternion newRotation = Quaternion.identity;
			Vector3 newScale = Vector3.one;

			foreach (var collider in m_dynamicCollidersInView)
			{
				newPosition = collider.collider.transform.position;
				newRotation = collider.collider.transform.rotation;
				newScale = collider.collider.transform.lossyScale;

				if (collider.lastPosition != newPosition ||
					collider.lastRotation != newRotation ||
					collider.lastScale != newScale)
				{
					collider.lastPosition = newPosition;
					collider.lastRotation = newRotation;
					collider.lastScale = newScale;
					//Debug.Log("Dynamic occluder changed!");
					changed = true;
				}
			}

			// See if the mesh itself has moved
			if (transform.position != m_lastPosition || transform.rotation != m_lastRotation)
			{
				m_lastPosition = transform.position;
				m_lastRotation = transform.rotation;
				changed = true;
			}

			if (changed)
			{
				RebuildMesh();
			}
			
		}

		if(m_outputColliders)
		{
			m_outputColliders = false;
			Debug.Log("Outputting colliders...\n-------------------------");
			foreach(var collider in m_colliderVertices)
			{
				Debug.Log(collider.Key.name);
			}
		}
	}

	void LateUpdate()
	{
		m_activeMeshes++;
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
		Vector3 position = transform.position;
		
		for(int vertID = 0; vertID < m_occluderCount && vertID < m_maxOccluderVerts - 1; vertID++)
		{
			Vector3 localPosition 	= m_occluders[vertID].vec - position;
			
			//Debug.DrawLine(transform.position, transform.position + localPosition, Color.red);
			localPosition.y = 0.0f;
			
			vertices[vertID + 1] 	= rotationInverse *  localPosition;
			uvs[vertID + 1].x = (localPosition.x + extentsVal) / (extentsVal * 2.0f);
			uvs[vertID + 1].y = (localPosition.z + extentsVal) / (extentsVal * 2.0f);
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
		RaycastHit hitInfo;

		m_cachedPosition = transform.position;
		
		Vector3 objectPosition = m_cachedPosition;
		objectPosition.y += CalculativeOffset;
		
		validVerts.Clear();

		// Update the four extents values now that m_cachedPosition is updated
		UpdateExtents();
		
		// Loop through each collider and add its vertices to the list
		foreach(var colliderPair in m_colliderVertices)
		{
			// TODO: given that all the maths here concerns circles, perhaps just ignore rotations, yeah?
			// Process SphereColliders
			if(colliderPair.Key is CapsuleCollider)
			{
				CapsuleCollider collider = colliderPair.Key as CapsuleCollider;


				Vector3 diff = colliderPair.Key.transform.position - objectPosition;
				
				Vector3 normal = Vector3.Cross(diff, Vector3.up);
				float normalMagnitude = normal.magnitude;
				
				Vector3 p0 = (normal.normalized * collider.radius * 0.99f);
				Vector3 p1 = -p0;

								
				p0 = Quaternion.Inverse(colliderPair.Key.transform.rotation) * (Vector3)p0;
				p1 = Quaternion.Inverse(colliderPair.Key.transform.rotation) * (Vector3)p1;
				
				colliderPair.Value.vertices[0] = p0;
				colliderPair.Value.vertices[1] = p1;

				// Check the edges of the capsule.

				for(int vertID = 0; vertID < colliderPair.Value.vertices.Count; vertID++)
				{
					Vector3 worldPos = colliderPair.Key.transform.TransformPoint(colliderPair.Value.vertices[vertID]);
					worldPos.y = objectPosition.y;
					Vector3 direction = worldPos - objectPosition;
					direction.y = 0.0f;

				//	Debug.DrawLine(objectPosition, objectPosition + direction, Color.blue);
					
					if(Physics.Raycast(objectPosition, direction.normalized, out hitInfo, direction.magnitude, 1 <<  collisionLayer))
					{
						if(ShowExtrusionRays) { Debug.DrawLine(objectPosition, hitInfo.point, Color.green); }
						
						validVerts.Add(hitInfo.point);
					} 
					else
					{
						if(ShowExtrusionRays) { Debug.DrawLine(objectPosition, worldPos, Color.magenta); }
						
						validVerts.Add(worldPos);	
					}
				}

				for(int vertID = 0; vertID < colliderPair.Value.vertices.Count; vertID++)
				{
					// TODO: The 0.3f here is to expand the side rays away from the collider when the player gets close to the occluder.
					// The value is arbitrary and needs to be fed with something more sensible.
					Vector3 worldPos = colliderPair.Key.transform.TransformPoint(colliderPair.Value.vertices[vertID] * (m_sphereExpansion + 0.3f / normalMagnitude));
					worldPos.y = objectPosition.y;
					
					Vector3 direction = worldPos - objectPosition;
					direction.y = 0.0f;

					//Debug.DrawLine(objectPosition, objectPosition + direction, Color.red);
					
					if(Physics.Raycast(objectPosition, direction.normalized, out hitInfo, m_viewCollider.size.x, 1 <<  collisionLayer))
					{
						if(ShowExtrusionRays) {	Debug.DrawLine(objectPosition, hitInfo.point, Color.green); }
						
						validVerts.Add(hitInfo.point);

					}
					else
					{
						if(ShowExtrusionRays) {	Debug.DrawLine(objectPosition, objectPosition + (direction.normalized * m_viewCollider.size.x), Color.magenta); }
						
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
					
					float magnitude = direction.magnitude;
					float val = magnitude -  m_nudgeMagnitude / magnitude;
					
					direction *= m_nudgeMagnitude;

#if OCCLUDED_MESH_EXCEPTIONS
					try
					{
#endif
					if(Physics.Raycast(objectPosition, direction / magnitude, out hitInfo, val, 1 <<  collisionLayer))
					{
						if(ShowFailedRays) { Debug.DrawLine(objectPosition, direction, Color.green); }
						
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
								Debug.DrawRay(objectPosition + new Vector3(0.0f, -1.2f, 0.0f), (vertexRayDirection.normalized * m_viewCollider.size.x) + new Vector3(0.0f, -1.2f, 0.0f), Color.green);
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
#if OCCLUDED_MESH_EXCEPTIONS
					}
					catch(Exception e)
					{
						Debug.LogError(GameObjectHelper.LogHierarchy(gameObject));
					}
#endif
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

		m_occluderHeap.Reset();

		
		// Output all results
		for(int vertID = 0; vertID < validVerts.Count && vertID < m_maxOccluderVerts; vertID++)
		{
			Vector3 directionToVert = validVerts[vertID] - objectPosition;
			
			m_occluders[vertID].vec = validVerts[vertID];
			
			Vector3 normalDirection = Vector3.Normalize(directionToVert);
			
			double angle = Math.Atan2((double)normalDirection.x, (double)normalDirection.z);
					
			m_occluders[vertID].angle = angle;

			m_occluderHeap.Insert(validVerts[vertID], (float)angle);
		}
		
		m_occluderCount = validVerts.Count;
		m_lastRayCount = m_occluderCount;
		
#if UNITY_EDITOR
		if(m_occluderCount >= m_maxOccluderVerts)
		{
			Debug.LogError("Number of occluder-verts has exceeded pre-allocated buffers. Mesh will be malformed");	
		}
#endif
		//Debug.Log("LOGGING\n==============");
		//m_occluderHeap.OutputGraph(false);
	//Array.Sort(m_occluders, 0, occluderCount);
	

		for(int i = 0; i < m_occluderCount; i++)
		{
		//	float metric = m_occluderHeap.GetTopMetric();

			m_occluders[i].vec = m_occluderHeap.RemoveTop();

		//	Debug.Log(metric);
		}


		if(ShowSucceededRays)
		{
			for(int i = 0; i < m_occluderCount; i++)
			{
				Debug.DrawLine(transform.position , m_occluders[i].vec , Color.red);
			}
		}
	}
	
	private void OnGUI()
	{
		if(DisplayStats)
		{
			GUILayout.BeginArea(new Rect(150.0f, Screen.height - 200.0f, 200.0f, 100.0f), (GUIStyle)("Box"));	
			
			GUILayout.Label("Ray Count: \t" + m_lastRayCount);
			GUILayout.Label("Sorted Entries: \t" + m_sortedEntries);
			
			GUILayout.EndArea();
		}
	}
	
	private void UpdateExtents()
	{
		extentsVals[0] = m_cachedPosition + m_fixedExtents[0];
		extentsVals[1] = m_cachedPosition + m_fixedExtents[1];
		extentsVals[2] = m_cachedPosition + m_fixedExtents[2];
		extentsVals[3] = m_cachedPosition + m_fixedExtents[3];
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

	private class DynamicColliderPair
	{
		public Collider collider;
		public Vector3 lastPosition;
		public Quaternion lastRotation;
		public Vector3 lastScale;
	}
	
	private int m_lastRayCount = 0;
	private int m_sortedEntries = 0;
	
	Vector3[] m_fixedExtents		= new Vector3[4];
	List<Vector3> extentsVals		= new List<Vector3>();
	List<Vector3> validVerts 		= new List<Vector3>();
	
	private const int m_maxOccluderVerts = 300;
	
	Vector3[] 	vertices 	= new Vector3[m_maxOccluderVerts];
	Vector3[] 	normals 	= new Vector3[m_maxOccluderVerts];
	Color[] 	colors 		= new Color[m_maxOccluderVerts];
	Vector2[] 	uvs 		= new Vector2[m_maxOccluderVerts];

	private BinaryHeap<Vector3> m_occluderHeap = new BinaryHeap<Vector3>(m_maxOccluderVerts);
	private OccluderVector[] m_occluders = new OccluderVector[m_maxOccluderVerts + 1];
	private List<DynamicColliderPair> m_dynamicCollidersInView = new List<DynamicColliderPair>();
	private Dictionary<Collider, DynamicColliderPair> m_dynamicPairMap = new Dictionary<Collider, DynamicColliderPair>();
	
	private int m_occluderCount = 0;
	
	private Vector3 m_cachedPosition = Vector3.zero;
	public static int m_activeMeshes = 0;
	public static List<OccludedMesh> m_meshes = new List<OccludedMesh>();
	public bool m_outputColliders = false;

	private Vector3 m_lastPosition = Vector3.zero;
	private Quaternion m_lastRotation = Quaternion.identity;
}
