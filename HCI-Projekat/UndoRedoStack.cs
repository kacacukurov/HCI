using System;
using System.Collections.Generic;

namespace HCI_Projekat
{
    public class UndoRedoStack
    {
        private Stack<string> Undo;
        private Stack<string> Redo;

        public UndoRedoStack()
        {
            Undo = new Stack<string>();
            Redo = new Stack<string>();
        }

        public Stack<string> GetUndo()
        {
            return Undo;
        }

        public Stack<string> GetRedo()
        {
            return Redo;
        }

        public int UndoCount
        {
            get{  return Undo.Count; }
        }
        public int RedoCount
        {
            get { return Redo.Count; }
        }

        public string Undo_Pop()
        {
            if (Undo.Count > 0)
            {
                string rc = Undo.Pop();
                return rc;
            }
            else
            {
                return "";
            }
        }

        public string Redo_Pop()
        {
            if (Redo.Count > 0)
            {
                string rc = Redo.Pop();
                return rc;
            }
            else
            {
                return "";
            }
        }
    }
}
