/// <summary>
/// Wall layout node.
/// 
/// A layout-node defining a wall point
/// 
/// </summary>

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

public class WallLayoutNode : LayoutNode 
{
	public override List<GameObject> BuildObject()
	{
		
		m_connections.Sort();
		List<GameObject> newObjects = new List<GameObject>();
		#if UNITY_EDITOR
		foreach(var connection in m_connections)
		{
			if(connection.Built)
				continue; 
			
			GameObject newObject = new GameObject();
			newObject.tag = "Geometry";
			newObject.layer = LayerMask.NameToLayer("WorldCollision");
			
			newObject.AddComponent<MeshFilter>();
			newObject.AddComponent<MeshRenderer>();
			
			Material wallMaterial = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Wall.mat", typeof(Material));
			newObject.GetComponent<MeshRenderer>().sharedMaterial = wallMaterial;
			
			newObject.transform.position = new Vector3(LocalPosition.x, LocalPosition.y, 0.0f);
			
			LayoutNode other = connection.Source == this ? connection.Target : connection.Source;
			
			Vector2 directionToOther = other.LocalPosition - LocalPosition;
			float distance = directionToOther.magnitude;
			float rotation = Mathf.Atan2(directionToOther.x, directionToOther.y);
			
			newObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, (-rotation * Mathf.Rad2Deg));
			
			int samplePoints = connection.BezierSections;
			Mesh nodeMesh = new Mesh();
			
			if(connection.ConnectionType == LayoutConnection.ConnectionTypes.Bezier)
			{
				Vector2 normalSource = connection.SourceTangent;
				Vector2 normalTarget = connection.TargetTangent;
				
				Vector2 sourceHandlePos = connection.Source.LocalPosition + normalSource;
				Vector2 targetHandlePos = connection.Target.LocalPosition + normalTarget;
			
				Vector3 sourcePos = new Vector3(connection.Source.LocalPosition.x, connection.Source.LocalPosition.y, 0.0f);
				Vector3 targetPos = new Vector3(connection.Target.LocalPosition.x, connection.Target.LocalPosition.y, 0.0f);
				
				Vector2 v0 = new Vector2(sourcePos.x, sourcePos.y);
				Vector2 v1 = new Vector2(targetPos.x, targetPos.y);
				Vector2 t0 = sourceHandlePos;
				Vector2 t1 = targetHandlePos;
				
				Mesh newMesh = Bezier.GetBezierMesh(v0, t0, v1, t1, 10, false);
				
				newObject.transform.localRotation = Quaternion.identity;
				newObject.transform.position = Vector3.zero;
			}
			else
			{
				Vector3[] 	vertices 	= new Vector3[6];
				Vector2[] 	uvs 		= new Vector2[6];
				int[] 		triangles 	= new int[6 * 4];
				
				Vector3 sourcePos = new Vector3(connection.Source.LocalPosition.x, connection.Source.LocalPosition.y, 0.0f);
				Vector3 targetPos = new Vector3(connection.Target.LocalPosition.x, connection.Target.LocalPosition.y, 0.0f);
				
				Vector3 direction = sourcePos - targetPos;
				direction.Normalize();
				
				Vector2 offsets = GetEndPoints(connection);
				Vector2 targetOffsets = other.GetEndPoints(connection);
				
				
				vertices[0] = new Vector3(-m_wallWidth, offsets.y, 0.0f);
				vertices[1] = new Vector3(-m_wallWidth, distance - targetOffsets.x , 0.0f);
				vertices[2] = new Vector3(m_wallWidth,  offsets.x, 0.0f);
				vertices[3] = new Vector3(m_wallWidth, distance - targetOffsets.y, 0.0f);
				vertices[4] = new Vector3(0.0f, 0.0f, 0.0f);
				vertices[5] = new Vector3(0.0f, distance, 0.0f);
				
				uvs[0] = new Vector2(0.0f, offsets.y);
				uvs[1] = new Vector2(0.0f, (distance - targetOffsets.y) / (2.0f * m_wallWidth));
				uvs[2] = new Vector2(1.0f, offsets.y);
				uvs[3] = new Vector2(1.0f, (distance - targetOffsets.y) / (2.0f * m_wallWidth));
				uvs[4] = new Vector2(0.5f, offsets.y);
				uvs[5] = new Vector2(0.5f, (distance - targetOffsets.y) / (2.0f * m_wallWidth));
				
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;
				
				triangles[6] = 1;
				triangles[7] = 5;
				triangles[8] = 3;
				
				triangles[9] = 0;
				triangles[10] = 2;
				triangles[11] = 4;
				
				nodeMesh.vertices 	= vertices;
				nodeMesh.uv 			= uvs;
				nodeMesh.triangles 	= triangles;
			}
							
			nodeMesh.RecalculateNormals();
			nodeMesh.Optimize();
			newObject.GetComponent<MeshFilter>().sharedMesh = nodeMesh;
			
		
			if(connection.ConnectionType == LayoutConnection.ConnectionTypes.Bezier)
			{
				BuildCollision(newObject, newObject.GetComponent<MeshFilter>().sharedMesh);
			}
			else
			{
				BuildCollision(newObject, null);
			}
			
			newObjects.Add(newObject);
			
			connection.Built = true;
			
		}
#endif
		return newObjects;
	}
	
	private void BuildCollision(GameObject newObject, Mesh collisionMesh)
	{
		newObject.AddComponent<Rigidbody>();
			
		Rigidbody rigidBody = newObject.GetComponent<Rigidbody>();
		rigidBody.useGravity = false;
		rigidBody.freezeRotation = true;
		
		rigidBody.constraints = RigidbodyConstraints.FreezeAll;
		
		if(collisionMesh != null)
		{
			MeshCollider newCollider = newObject.AddComponent<MeshCollider>();
			newCollider.sharedMesh = collisionMesh;
		}
		else
		{
			newObject.AddComponent<BoxCollider>();
		}
	}
	#if UNITY_EDITOR
	public override bool OnGUIInternal(EditType editType, int id, bool selectedNode)
	{
		bool selected = base.OnGUIInternal(editType, id, selectedNode);
		
		if(SelectedConnection != null && SelectedConnection.ConnectionType == LayoutConnection.ConnectionTypes.Bezier && selectedNode)
		{
			Vector2 sourceHandlePos = SelectedConnection.Source.m_worldPosition + SelectedConnection.SourceTangent;
			Vector2 targetHandlePos = SelectedConnection.Target.m_worldPosition + SelectedConnection.TargetTangent;
			
			Vector3 sourcePos = new Vector3(SelectedConnection.Source.m_worldPosition.x, SelectedConnection.Source.m_worldPosition.y, 0.0f);
			Vector3 targetPos = new Vector3(SelectedConnection.Target.m_worldPosition.x, SelectedConnection.Target.m_worldPosition.y, 0.0f);
			
			Vector3 sourceHandle = new Vector3(sourceHandlePos.x, sourceHandlePos.y, 0.0f);
			Vector3 targetHandle = new Vector3(targetHandlePos.x, targetHandlePos.y, 0.0f);
			
			Handles.DrawLine(SelectedConnection.Source.m_worldPosition, sourceHandlePos);
			Handles.DrawLine(SelectedConnection.Target.m_worldPosition, targetHandlePos);
			
			Vector3 newSourceHandle = Handles.Slider2D(sourceHandle, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 0.4f, Handles.CircleCap, new Vector2(0.5f, 0.5f)) - sourcePos;
			Vector3 newTargetHandle = Handles.Slider2D(targetHandle, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 0.4f, Handles.CircleCap, new Vector2(0.5f, 0.5f)) - targetPos;
			
			SelectedConnection.SourceTangent = new Vector2(newSourceHandle.x, newSourceHandle.y);
			SelectedConnection.TargetTangent = new Vector2(newTargetHandle.x, newTargetHandle.y);
		}
		return selected;
	}
#endif
}
