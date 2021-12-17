using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman
{
    internal class Pawn : Chessman
    {
        protected override void RefreshAvailableMoves()
        {
            _availableMoves.Clear();
            AddLegalMoveIfPossible(0, 1);
            if(CurrentY == 1)
            {
                AddLegalMoveIfPossible(0, 2);
            }
            // TODO Add En Pasent
        }
    }
}
