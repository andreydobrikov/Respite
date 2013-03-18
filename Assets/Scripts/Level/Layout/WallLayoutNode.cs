using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class WallLayoutNode : LayoutNode 
{
	public override List<GameObject> BuildObject()
	{
		List<GameObject> newObjects = new List<GameObject>();
		
		
		
		
		
		
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
			
			newObject.transform.position = new Vector3(m_position.x, m_position.y, 0.0f);
			
			LayoutNode other = connection.Source == this ? connection.Target : connection.Source;
			
			Vector2 directionToOther = other.m_position - m_position;
			float distance = directionToOther.magnitude;
			float rotation = Mathf.Atan2(directionToOther.x, directionToOther.y);
			
			newObject.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, (-rotation * Mathf.Rad2Deg));
			
			int samplePoints = connection.BezierSections;
			Mesh nodeMesh = new Mesh();
			
			if(connection.ConnectionType == LayoutConnection.ConnectionTypes.Bezier)
			{
				Vector2 normalSource = connection.SourceTangent;
				Vector2 normalTarget = connection.TargetTangent;
				
				Vector2 sourceHandlePos = connection.Source.m_position + normalSource;
				Vector2 targetHandlePos = connection.Target.m_position + normalTarget;
			
				Vector3 sourcePos = new Vector3(connection.Source.m_position.x, connection.Source.m_position.y, 0.0f);
				Vector3 targetPos = new Vector3(connection.Target.m_position.x, connection.Target.m_position.y, 0.0f);
				
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
				Vector3[] 	vertices 	= new Vector3[4];
				Vector2[] 	uvs 		= new Vector2[4];
				int[] 		triangles 	= new int[6];
				
				float wallSize = 0.2f;
				
				Vector3 sourcePos = new Vector3(connection.Source.m_position.x, connection.Source.m_position.y, 0.0f);
				Vector3 targetPos = new Vector3(connection.Target.m_position.x, connection.Target.m_position.y, 0.0f);
				
				Vector3 direction = sourcePos - targetPos;
				direction.Normalize();
				
				float yChangeLeft = 0.0f;
				float yChangeRight = 0.0f;
				
				Vector2 offsets = GetEndPoints(connection);
				Vector2 targetOffsets = other.GetEndPoints(connection);
				
				
				vertices[0] = new Vector3(-wallSize, offsets.y, 0.0f);
				vertices[1] = new Vector3(-wallSize, distance + targetOffsets.y , 0.0f);
				vertices[2] = new Vector3(wallSize,  offsets.x, 0.0f);
				vertices[3] = new Vector3(wallSize, distance + targetOffsets.x, 0.0f);
				
				uvs[0] = new Vector2(0.0f, 0.0f);
				uvs[1] = new Vector2(0.0f, distance);
				uvs[2] = new Vector2(1.0f, 0.0f);
				uvs[3] = new Vector2(1.0f, distance);
				
				
				triangles[0] = 0;
				triangles[1] = 1;
				triangles[2] = 2;
				
				triangles[3] = 2;
				triangles[4] = 1;
				triangles[5] = 3;
				
				
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
	
	public override bool OnGUIInternal(EditType editType, int id, bool selectedNode)
	{
		bool selected = base.OnGUIInternal(editType, id, selectedNode);
		
		if(SelectedConnection != null && SelectedConnection.ConnectionType == LayoutConnection.ConnectionTypes.Bezier && selectedNode)
		{
			Vector2 sourceHandlePos = SelectedConnection.Source.m_position + SelectedConnection.SourceTangent;
			Vector2 targetHandlePos = SelectedConnection.Target.m_position + SelectedConnection.TargetTangent;
			
			Vector3 sourcePos = new Vector3(SelectedConnection.Source.m_position.x, SelectedConnection.Source.m_position.y, 0.0f);
			Vector3 targetPos = new Vector3(SelectedConnection.Target.m_position.x, SelectedConnection.Target.m_position.y, 0.0f);
			
			Vector3 sourceHandle = new Vector3(sourceHandlePos.x, sourceHandlePos.y, 0.0f);
			Vector3 targetHandle = new Vector3(targetHandlePos.x, targetHandlePos.y, 0.0f);
			
			Handles.DrawLine(SelectedConnection.Source.m_position, sourceHandlePos);
			Handles.DrawLine(SelectedConnection.Target.m_position, targetHandlePos);
			
			Vector3 newSourceHandle = Handles.Slider2D(sourceHandle, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 0.4f, Handles.CircleCap, new Vector2(0.5f, 0.5f)) - sourcePos;
			Vector3 newTargetHandle = Handles.Slider2D(targetHandle, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f), 0.4f, Handles.CircleCap, new Vector2(0.5f, 0.5f)) - targetPos;
			
			SelectedConnection.SourceTangent = new Vector2(newSourceHandle.x, newSourceHandle.y);
			SelectedConnection.TargetTangent = new Vector2(newTargetHandle.x, newTargetHandle.y);
		}
		return selected;
	}
}
