using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Chessman
{
    internal class Chessman
    {
        public ChessmanColor Color { get; set; }
        public GameObject GameObject { get; set; }
        public int CurrentX { get; private set; }
        public int CurrentY { get; private set; }

        protected List<(int x, int y)> _availableMoves;
        public virtual List<(int x, int y)> AvailableMoves 
        {
            get
            {
                RefreshAvailableMoves();
                return _availableMoves;
            }
        }

        protected virtual void RefreshAvailableMoves()
        {
        }

        public Chessman()
        {
            _availableMoves = new List<(int x, int y)>();
        }

        public virtual void InitialPlace(int x, int y)
        {
            CurrentY = y;
            CurrentX = x;
            BoardManager.GamePositions[x, y] = this;
        }

        public void Place((int x, int y) destinationPoint) 
        {
            if (CanMove(destinationPoint))
            {
                BoardManager.GamePositions[destinationPoint.x, destinationPoint.y] = this;
                BoardManager.GamePositions[CurrentX, CurrentY] = null;
                CurrentX = destinationPoint.x;
                CurrentY = destinationPoint.y;
            }
            GameObject.transform.localPosition = new Vector3(CurrentX, 0, CurrentY);
        }
        public void Move(Vector3 destinationPoint)
        {
            GameObject.transform.position = destinationPoint;
        }
        
        public bool CanMove((int x, int y) destinationPoint)
        {
            return AvailableMoves.Contains(destinationPoint);
        }

        internal void CaptureChessman() { }
        internal bool IsCapturePossible() { return false; }

        protected bool AddLegalMoveIfPossible(int x, int y)
        {
            (int x, int y) currentPoint = (CurrentX + x, CurrentY + y);
            if (currentPoint.x >= 0 && currentPoint.x <= 7 && currentPoint.y >= 0 && currentPoint.y <= 7)
            {
                var chessmanAtPosition = BoardManager.GamePositions[currentPoint.x, currentPoint.y];
                if (chessmanAtPosition == null)
                {
                    _availableMoves.Add(currentPoint);
                    return true;
                }
                else if (chessmanAtPosition.Color != Color)
                {
                    _availableMoves.Add(currentPoint);
                    //CaptureMove
                    return false;
                }                
            }
            return false;
        }

    }
}
