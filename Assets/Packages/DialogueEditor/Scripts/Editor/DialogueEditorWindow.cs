using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace EFA.Dialogue {

    public class DialogueEditorWindow : EditorWindow {

        private static DialogueEditorWindow thisWindow;

        public static List<EditorNode> nodes = new List<EditorNode>();
        public static Event currentEvent;
        public static Vector2 localMousePos;
        
        private Vector2 initialCameraDeltaPos = Vector2.zero;
        private Vector2 pivotTargetPos;
        private Vector2 previousLocalMousePos;
        private int curMouseDeltaFrame = 0;

        //---- Camera Scale Settings ----
        private Vector2 cameraCurZoomScale = Vector2.one;
        private float cameraMoveSpeed = .5f;
        private float cameraZoomSpeed = .5f;
        private float cameraMinScale = .5f;

        //--- GUI ----
        private static Texture2D backgroundGridTexture;

        [MenuItem ("Window/Dialogue Editor")]
        public static void ShowWindow() {

            thisWindow = (DialogueEditorWindow) EditorWindow.GetWindow(typeof(DialogueEditorWindow), false, "Dialogue Editor");
            LoadCustomResources();

        }

        private static void LoadCustomResources() {

            backgroundGridTexture = (Texture2D) Resources.Load("seamless_grid", typeof(Texture2D));

        }

        private void Update() {

            Repaint();

        }

        private void OnGUI() {

            currentEvent = Event.current;
            GUIUtility.ScaleAroundPivot(cameraCurZoomScale, pivotTargetPos);

            DrawBackground();
            CalculateMousePosition();
            UpdateNodes();
            RightclickMenu();
            CameraMovement();
            CameraFovScaler();
        }

        private void DrawBackground() {
            
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor) {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            Vector3 newOffset = initialCameraDeltaPos;

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void CameraFovScaler() {

            if(currentEvent.type != EventType.ScrollWheel)
                return;

            Vector2 scrollVal = currentEvent.delta;
            bool zoomIn = scrollVal.y < 0.0f;
            Vector2 zoomScale = Vector2.one * Time.deltaTime * cameraZoomSpeed;
            pivotTargetPos = localMousePos;

            if(zoomIn)
                cameraCurZoomScale += zoomScale;
            else
                cameraCurZoomScale -= zoomScale;

            if(cameraCurZoomScale.y < cameraMinScale) {
                cameraCurZoomScale.x = cameraMinScale;
                cameraCurZoomScale.y = cameraMinScale;
            }
        }

        private void CameraMovement() {

            if(currentEvent.type != EventType.MouseDrag)
                return;

            Vector2 deltaPos = localMousePos - previousLocalMousePos;
            deltaPos *= cameraMoveSpeed;
            initialCameraDeltaPos += deltaPos;

            for(int i = 0; i < nodes.Count; i++) {
                Vector2 nodePos = nodes[i].GetRect().position + deltaPos;
                nodes[i].SetPosition(nodePos);
            }   
        }

        private void UpdateNodes() {

            BeginWindows();

                for(int i = 0; i < nodes.Count; i++)
                    nodes[i].DrawNode(i);

            EndWindows();
        }

        private void RightclickMenu() {

            Rect allowedArea = EditorGUILayout.GetControlRect();
            
            if(IsMouseOverWindow(this) && currentEvent.type == EventType.ContextClick) {
                GenericMenu menu = new GenericMenu();
            
                menu.AddItem(new GUIContent("Add Text Node"), false, AddNode);
                menu.AddItem(new GUIContent("Remove All Nodes"), false, RemoveNodes);
                menu.ShowAsContext();
            
                currentEvent.Use(); 
            }

        }

        private void AddNode() {
        
            nodes.Add(new TextNode(new Rect(localMousePos.x, localMousePos.y, 100, 100)));
        
        }

        private void RemoveNodes() {

            nodes.Clear();

        }

        public static bool IsMouseOverWindow(EditorWindow window) {

            return mouseOverWindow && window.GetType().FullName == mouseOverWindow.ToString().Split('(', ')')[1];
        
        }

        private void CalculateMousePosition() {
            
            //How ofter we calculate mouse delta (16 means once per 16 frames).
            curMouseDeltaFrame++;
            if(curMouseDeltaFrame == 16){
                previousLocalMousePos = localMousePos;
                curMouseDeltaFrame = 0;
            }

            Vector2 windowPos = new Vector2(thisWindow.position.x, thisWindow.position.y);
            localMousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition) - windowPos;

        }
    }
}