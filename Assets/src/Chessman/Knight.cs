using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman
{
    internal class Knight : Chessman
    {
        protected override void RefreshAvailableMoves()
        {
            _availableMoves.Clear();
            AddLegalMoveIfPossible(1, 2);
            AddLegalMoveIfPossible(2, 1);

            AddLegalMoveIfPossible(-1, 2);
            AddLegalMoveIfPossible(-2, 1);

            AddLegalMoveIfPossible(1, -2);
            AddLegalMoveIfPossible(2, -1);

            AddLegalMoveIfPossible(-1, -2);
            AddLegalMoveIfPossible(-2, -1);
            
        }
    }
}
