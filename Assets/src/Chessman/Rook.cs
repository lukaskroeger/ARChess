using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman
{
    internal class Rook : Chessman
    {
        protected override void RefreshAvailableMoves()
        {
            _availableMoves.Clear();
            //horizontal right
            for (int i = 1; i <= 7; i++)
            {
                if (!AddLegalMoveIfPossible(0, i))
                {
                    break;
                }
            }
            //horizontal left
            for (int i = 1; i <= 7; i++)
            {
                if (!AddLegalMoveIfPossible(0, -i))
                {
                    break;
                }
            }
            //vertical top
            for (int i = 1; i <= 7; i++)
            {
                if (!AddLegalMoveIfPossible(i, 0))
                {
                    break;
                }
            }
            //vertical bottom
            for (int i = 1; i <= 7; i++)
            {
                if (!AddLegalMoveIfPossible(-i, 0))
                {
                    break;
                }
            }
        }
    }
}
