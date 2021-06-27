using UnityEngine;
using UnityEditor;

namespace EFA.Dialogue {
    public abstract class EditorNode : EditorWindow {

        protected Rect myRect;
        public Rect initialRect { get; protected set; }

        public abstract void DrawNode(int id);

        public Rect GetRect() {

            return myRect;

        }

        public void SetPosition(Vector2 pos) {

            myRect.x = pos.x;
            myRect.y = pos.y;

        }

        public void SetSize(Vector2 size) {

            myRect.size = size;

        }
    }
}