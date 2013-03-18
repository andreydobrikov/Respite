using UnityEngine;
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
public class AgentCamera : WorldViewObject 
{
	public LayerMask collisionLayer = 0;
	
	private float m_nudgeMagnitude 	= 0.01f;							// This determines how far vertices are extruded from their mesh centroid when casting rays. Needed to prevent false collisions.
	private MeshFilter m_filter 	= null;
	private Camera m_camera 		= null;
	private List<Collider> m_collidersInView = new List<Collider>();
	private Mesh m_cameraMesh;
	
	public Camera GetCamera()
	{
		return m_camera;	
	}
	
	void Start () 
	{
		m_camera = m_worldObject as Camera;
		m_filter = GetComponent<MeshFilter>();
		
		// Build an un-occluded view wedge
		BuildColliderMesh();
		
		// Use it for the collider mesh
		MeshCollider collider = GetComponent<MeshCollider>();
		collider.sharedMesh = null;
		collider.sharedMesh = m_filter.mesh;
	}
	
	public void SensorChanged(bool spotted)
	{
		if(m_camera != null)
		{
			if(spotted)
			{
				m_camera.ChangeState(Camera.TargetState.Spotted);
			}
			else
			{
				m_camera.ChangeState(Camera.TargetState.Searching);	
			}
		}
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Geometry")
		{
			m_collidersInView.Add(other);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		m_collidersInView.Remove(other);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		RebuildMesh();
	}
	
	private class VectorOffsetPair
	{
		public Vector3 vec;
		public Vector3 offsetVec;
	}
	
	private class OccluderVector
	{
		public Vector3 vec;
		public float angle;
	}
	
	/// <summary>
	/// Builds the collider mesh. This is used for identifying items within the camera's vision and is therefore un-culled.
	/// This should only be called once on Start().
	/// </summary>
	private void BuildColliderMesh()
	{
		m_cameraMesh = new Mesh();
		
		Vector3[] 	vertices 	= new Vector3[5];
		Vector2[] 	uvs 		= new Vector2[5];
		int[] 		triangles 	= new int[6 * 3];
		
		float halfWidth = m_camera.range * Mathf.Tan(((m_camera.fov_degrees * 1.1f) / 2.0f) * Mathf.Deg2Rad);
		
		vertices[0] = new Vector3(0.0f, 0.0f, 0.0f);
		vertices[1] = new Vector3(-halfWidth, m_camera.range, 0.5f);
		vertices[2] = new Vector3(halfWidth, m_camera.range, 0.5f);
		vertices[3] = new Vector3(-halfWidth, m_camera.range, -0.5f);
		vertices[4] = new Vector3(halfWidth, m_camera.range, -0.5f);
		
		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 2;
		
		triangles[3] = 0;
		triangles[4] = 4;
		triangles[5] = 3;
		
		triangles[6] = 0;
		triangles[7] = 3;
		triangles[8] = 1;
		
		triangles[9] = 0;
		triangles[10] = 2;
		triangles[11] = 4;
		
		triangles[12] = 2;
		triangles[13] = 1;
		triangles[14] = 4;
		
		triangles[15] = 1;
		triangles[16] = 3;
		triangles[17] = 4;
		
		m_cameraMesh.vertices 	= vertices;
		m_cameraMesh.uv 			= uvs;
		m_cameraMesh.triangles 	= triangles;
		
		m_filter.sharedMesh = m_cameraMesh;
	}
	
	private void RebuildMesh()
	{
		List<OccluderVector> occluders = GetOccluders();
		
		int triangleCount = ((occluders.Count) - 1) * 3;
		
		Vector3[] 	vertices 	= new Vector3[occluders.Count + 1];
		Vector3[] 	normals 	= new Vector3[occluders.Count + 1];
		Vector2[] 	uvs 		= new Vector2[occluders.Count + 1];
		int[] 		triangles 	= new int[triangleCount];
		
		// Add the camera point manually
		vertices[0] = new Vector3(0.0f, 0.0f, 0.0f);
		
		int index = 1;
		
		Quaternion rotationInverse = Quaternion.Inverse(transform.rotation);
		
		foreach(OccluderVector vert in occluders)
		{
			Vector3 worldPosition = vert.vec - transform.position;
			vertices[index] = rotationInverse *  worldPosition;
			uvs[index] 		= new Vector2(worldPosition.x, worldPosition.y);
			normals[index] = new Vector3(0.0f, 0.0f -1.0f);
			
			index++;
		}
		
		// Loop through the points and build them there triangles
		for(int i = 1; i < occluders.Count; i++)
		{
			triangles[(i - 1) * 3] = 0;
			triangles[(i - 1) * 3 + 1] = i;
			triangles[(i - 1) * 3 + 2] = i + 1;
		}
		
		m_filter.sharedMesh.Clear();
		m_filter.sharedMesh.vertices 	= vertices;
		m_filter.sharedMesh.uv 			= uvs;
		m_filter.sharedMesh.normals = normals;
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
		Vector3 cameraDirection = m_camera.transform.rotation * Vector3.up;
		
		List<VectorOffsetPair> verts = new List<VectorOffsetPair>();
		
		// Loop through each collider and add its vertices to the list
		foreach(Collider viewCollider in m_collidersInView)
		{
			MeshFilter meshFilter = viewCollider.GetComponent<MeshFilter>();
			
			foreach(Vector3 vert in meshFilter.mesh.vertices)
			{
				Vector3 vertexWorldPos = viewCollider.transform.TransformPoint(vert); // This can be replaced with an addition of the transform position if everything is grid-aligned.
				Vector3 cameraToVertex = vertexWorldPos - transform.position;
				
				Debug.DrawRay (this.transform.position  + new Vector3(0.0f, 0.0f, -4.0f), cameraToVertex , Color.magenta);
				
				// This works out the magnitude of the vector if it were to reach the max range.
				// TODO: This could be removed if the camera area was a correct sweep rather than a triangle. The magnitude of the vector would just be
				//		 tested against the range. The difficulty is the lateral sweep becomes harder over a curve.
				float cameraDirVertexDot = Vector3.Dot(cameraDirection, Vector3.Normalize(cameraToVertex));
				float angle = Mathf.Acos(cameraDirVertexDot);
				float mag = (cameraToVertex).magnitude;
				
				float thing = mag * cameraDirVertexDot;
				
				if(thing < m_camera.range && angle <= Mathf.Deg2Rad * (m_camera.fov_degrees ) / 2.0f)
				{
					VectorOffsetPair newPair = new VectorOffsetPair();
					newPair.vec = 	vertexWorldPos;
				
					Vector3 offsetDirection = newPair.vec - (meshFilter.mesh.bounds.center + meshFilter.gameObject.transform.position);
					newPair.offsetVec = newPair.vec + (offsetDirection * m_nudgeMagnitude);
					newPair.offsetVec.z = newPair.vec.z;
				
					verts.Add(newPair);	
				}
			}
		}
		
	
		List<Vector3> validVerts = new List<Vector3>();
		validVerts.Clear();
		
		RaycastHit hitInfo;
		
		
		// Manually cast the edges
		Vector3 right = Quaternion.Euler(0.0f, 0.0f, -((m_camera.fov_degrees / 2.0f))) * cameraDirection;
		Vector3 left = Quaternion.Euler(0.0f, 0.0f, ((m_camera.fov_degrees / 2.0f))) * cameraDirection;
		
		Vector3 leftDirection = left;
		
		float edgeCosTheta = Vector3.Dot(cameraDirection, Vector3.Normalize(right));
		float edgeMaxMagnitude = m_camera.range / edgeCosTheta;
		
		right = Vector3.Normalize(right) * edgeMaxMagnitude;
		left = Vector3.Normalize(left) * edgeMaxMagnitude;
		
		List<OccluderVector> occluders = new List<OccluderVector>();
		
		
		if(!Physics.Raycast(this.transform.position, right, out hitInfo, edgeMaxMagnitude, collisionLayer))
		{
			validVerts.Add(transform.position + right);
		}
		else
		{
			validVerts.Add(hitInfo.point);	
		}
		
		if(!Physics.Raycast(this.transform.position, left, out hitInfo, edgeMaxMagnitude, collisionLayer))
		{
			validVerts.Add(transform.position + left);
		}
		else
		{
			validVerts.Add(hitInfo.point);	
		}
		
		right += transform.position;
		left += transform.position;
		
		// Iterate through the initial verts and add both valid verts and projected offset vert intersections
		foreach(VectorOffsetPair vert in verts)
		{
			Vector3 directionToVert = vert.vec - this.transform.position;
			
			directionToVert = vert.offsetVec - this.transform.position;
			float magnitude = directionToVert.magnitude * 0.98f;
			
			// Check to see if the original vertex is occluded
			if (!Physics.Raycast (this.transform.position, directionToVert, out hitInfo, magnitude, collisionLayer)) 
			{
				// Not occluded. The vert itself is fine. Next we check the projected ray...
				validVerts.Add(vert.vec);
				
				float cosTheta = Vector3.Dot(cameraDirection, Vector3.Normalize(directionToVert));
				float maxMagnitude = m_camera.range / cosTheta;
				
				Vector3 maxPosition = transform.position + (Vector3.Normalize(directionToVert) * maxMagnitude);
				Vector3 newDirection = Vector3.Normalize(directionToVert) * maxMagnitude ;
				
				if(!Physics.Raycast(this.transform.position, newDirection, out hitInfo, newDirection.magnitude, collisionLayer))
				{
					validVerts.Add(maxPosition);
				}
				else
				{
					validVerts.Add (hitInfo.point);	
				}
			}
		}
		
		// Ray-cast along the extent of the wedge
		RaycastHit[] hits;
		RaycastHit[] hitsReverse;
		Vector3 direction = right - left;
		
		hits 		= Physics.RaycastAll(left, direction, direction.magnitude, collisionLayer);
		hitsReverse = Physics.RaycastAll(right, -direction, direction.magnitude, collisionLayer);
		
		RaycastHit[] allHits = new RaycastHit[hits.Length + hitsReverse.Length];
		hits.CopyTo(allHits, 0);
		hitsReverse.CopyTo(allHits, hits.Length);
		
		foreach(RaycastHit hit in allHits)
		{
			Vector3 directionToVert = hit.point - this.transform.position;
			if (!Physics.Raycast (this.transform.position, directionToVert, out hitInfo, directionToVert.magnitude * 0.99f, collisionLayer)) 
			{
				validVerts.Add(hit.point);
			}
		}
		
		// Output all results
		foreach(Vector3 vert in validVerts)
		{
			Vector3 directionToVert = vert - this.transform.position;
			
			OccluderVector newOccluder = new OccluderVector();
			newOccluder.vec = vert;
			Vector3 normalDirection = Vector3.Normalize(directionToVert);
			
			float angle = Mathf.Acos(Vector3.Dot(Vector3.Normalize(leftDirection), normalDirection));
			
			if(Vector3.Normalize(leftDirection) == normalDirection)
			{
				angle = 0.0f;	
			}
			
			newOccluder.angle = angle;
			occluders.Add(newOccluder);
		}
	
		occluders.Sort(OccluderComparison);
		
		foreach(OccluderVector vert in occluders)
		{
			Debug.DrawRay (this.transform.position  + new Vector3(0.0f, 0.0f, -5.0f), vert.vec - this.transform.position , Color.green);
		}
		
		return occluders;
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
		
}
