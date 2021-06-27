using UnityEngine;

namespace EFA.Dialogue {
    
    public class TextNode : EditorNode {

        public TextNode(Rect r) {
            this.myRect = r;
            this.initialRect = new Rect(r);
        }

        public override void DrawNode(int id) {

            myRect = GUI.Window(id, myRect, DrawWindow, "test");

        }

        private void DrawWindow(int windowID) {

            GUI.DragWindow(new Rect(0, 0, myRect.width, myRect.height));

        }
    }
}