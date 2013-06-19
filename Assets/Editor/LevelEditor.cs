using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor 
{

#region Edit Modes
	private enum EditModes
	{
		MoveNodes,
		ConnectNodes,
		
		Max
	}
#endregion

#region Edit Labels
	/// <summary>
	/// Mode labels.
	/// </summary>
	private static string[] labels = 
	{
		"Move Nodes",
		"Connect Nodes"
	};
#endregion
	
#region Methods
	
	public override void OnInspectorGUI() 
	{
		Level editorLevel = (Level)target;
		
		GUILayout.Label("Level");
		GUILayout.BeginScrollView(m_scrollPosition);
		
		GUILayout.EndScrollView();
		
		m_expandNodeSettings = EditorGUILayout.Foldout(m_expandNodeSettings, "Selected Node");
		
		if(editorLevel != null &&  editorLevel.SelectedNode != null && m_expandNodeSettings)
		{
			editorLevel.SelectedNode.OnInspectorGUI();
		}
		
		GUILayout.Label("Level Options");
		
		if(GUILayout.Button("Rebuild Level"))
		{
			LayoutObjectBuilder builder = new LayoutObjectBuilder();
			builder.BuildObjects(editorLevel);
		}
	}
	
	public void OnSceneGUI()
	{
		Level editorLevel = (Level) target;
		Event e = Event.current;
		
		// Delete the current node
		if(e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
		{
			if(editorLevel.SelectedNode != null)
			{
				editorLevel.RemoveNode(editorLevel.SelectedNode);
				e.Use();
				ClearDrag();
			}
		}
		
		if(e.type == EventType.MouseDown && e.button == 1)
		{
			ClearDrag();
		}
		
		UpdateNodes();
		
		if(m_dragStarted)
		{
			Vector3 startPos = new Vector3(m_connectionStart.LocalPosition.x, m_connectionStart.LocalPosition.y, 0.0f);
			
			Plane zeroPlane = new Plane(new Vector3(0.0f, 0.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f));

			Ray ray = HandleUtility.GUIPointToWorldRay(new Vector2(e.mousePosition.x, e.mousePosition.y));

			float hit;
			if(zeroPlane.Raycast(ray, out hit))
			{
				var hitLocation = ray.GetPoint(hit);
				
				Vector3 targetPos = new Vector3(hitLocation.x, hitLocation.y, 0.0f);
				
				Handles.DrawLine(startPos, targetPos);
				HandleUtility.Repaint();
			}
		}
		
		DrawGUILayout();
	}	
	
	private void ClearDrag()
	{
		m_connectionStart = null;
		m_dragStarted = false;
	}
	
	private void DrawGUILayout()
	{
		Handles.BeginGUI();
		
		GUILayout.BeginArea(new Rect(10.0f, 10.0f, 150.0f, 400.0f));
		
		m_mode = (EditModes) GUILayout.SelectionGrid((int)m_mode, labels, 1);
		
		GUILayout.EndArea();
		
		GUILayout.BeginArea(new Rect(10.0f, Screen.height - 100.0f, 150.0f, 90.0f));
		
		if(GUILayout.Button("Add Node"))
		{
			UnityEngine.Camera currentCamera = UnityEngine.Camera.current;
				
			Vector3 placePoint = currentCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 1.0f));
			
			Level editorLevel = (Level) target;
			WallLayoutNode newNode = ScriptableObject.CreateInstance<WallLayoutNode>();
			
			Vector2 newPos;
			newPos.x = placePoint.x - editorLevel.transform.position.x;
			newPos.y = placePoint.y - editorLevel.transform.position.y;
			
			newNode.LocalPosition = newPos;
				
			editorLevel.AddNode(newNode);
		}
		
		GUILayout.EndArea();
		
		Handles.EndGUI();
	}
	
	private void UpdateNodes()
	{
		Level editorLevel = (Level) target;
		
		int idCounter = 1;
		foreach(var node in editorLevel.Nodes)
		{
			Vector3 handlePos = new Vector3(node.LocalPosition.x, node.LocalPosition.y, 0.0f);
			
			if(m_mode == EditModes.MoveNodes)
			{
				node.OnGUIInternal(LayoutNode.EditType.Move, idCounter, node == editorLevel.SelectedNode);
				
				ClearDrag();
			}
			else
			{
				if(node.OnGUIInternal(LayoutNode.EditType.Link, idCounter, node == editorLevel.SelectedNode))
				{
					if(!m_dragStarted)
					{
						m_connectionStart = node;
						m_dragStarted = true;
					
						editorLevel.SelectedNode = node;
					}
					else
					{
						if(node != m_connectionStart)
						{
							node.AddConnection(m_connectionStart);	
							ClearDrag();
						}
					}
				}
			}
			
			// Track whether the slider is selected.
			if(GUIUtility.hotControl == idCounter)
			{
				editorLevel.SelectedNode = node;	
			}
			
			// Draw connections
			foreach(var connection in node.ConnectedNodes)
			{
				if(connection.Target != null && connection.Source != null)
				{
					Vector3 connectionPos = new Vector3(connection.Target.LocalPosition.x, connection.Target.LocalPosition.y, 0.0f);
					
					Handles.color = Color.white;	
					if(connection == editorLevel.SelectedNode.SelectedConnection)
					{
						Handles.color = Color.red;
					}
					
					if(connection.ConnectionType == LayoutConnection.ConnectionTypes.Bezier)
					{
						Vector3 source = connection.Source.LocalPosition;
						Color bezierColor = connection == editorLevel.SelectedNode.SelectedConnection ? Color.red : Color.white;
						Handles.DrawBezier(connection.Source.LocalPosition, connection.Target.LocalPosition, connection.Source.LocalPosition + connection.SourceTangent, connection.Target.LocalPosition + connection.TargetTangent, bezierColor, null, 1.5f);
					}
					else
					{
						Handles.DrawLine(handlePos, connectionPos);			
					}
					
					Handles.color = Color.white;	
				}
			}
			
			idCounter++;
		}
	}
	
#endregion Methods
	
#region Fields
	private LayoutNode m_connectionStart = null;
	private EditModes m_mode 				= EditModes.MoveNodes;
	private bool m_dragStarted 				= false;
	private bool m_expandNodeSettings 		= true;
	private Vector2 m_scrollPosition		= new Vector2();
	
#endregion
}
