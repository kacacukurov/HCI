using System;
using System.Collections.Generic;

namespace HCI_Projekat
{
    public class UndoRedoStack
    {
        private Stack<string> _Undo;
        private Stack<string> _Redo;

        public Stack<string> GetUndo()
        {
            return _Undo;
        }

        public Stack<string> GetRedo()
        {
            return _Redo;
        }

        public int UndoCount
        {
            get{  return _Undo.Count; }
        }
        public int RedoCount
        {
            get { return _Redo.Count; }
        }

        public UndoRedoStack()
        {
            Reset();
        }

        public void Reset()
        {
            _Undo = new Stack<string>();
            _Redo = new Stack<string>();
        }

        public void Do(string input)
        {
            _Undo.Push(input);
            _Redo.Clear();
        }

        public string Undo()
        {
            if (_Undo.Count > 0)
            {
                string rc = _Undo.Pop();
                _Redo.Push(rc);
                return rc;
            }
            else
            {
                return "";
            }
        }

        public string Redo()
        {
            if (_Redo.Count > 0)
            {
                string rc = _Redo.Pop();
                _Undo.Push(rc);
                return rc;
            }
            else
            {
                return "";
            }
        }


    }
}
