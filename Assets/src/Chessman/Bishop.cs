using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chessman
{
    internal class Bishop : Chessman
    {
        protected override void RefreshAvailableMoves()
        {
            _availableMoves.Clear();
            //diagonal 
            for (int i = 1; i <= 7; i++)
            {
                if (!AddLegalMoveIfPossible(i, i))
                {
                    break;
                }
            }
            for (int i = 1; i <= 7; i++)
            {
                if (!AddLegalMoveIfPossible(-i, i))
                {
                    break;
                }
            }
            for (int i = 1; i <= 7; i++)
            {
                if (!AddLegalMoveIfPossible(i, -i))
                {
                    break;
                }
            }
            for (int i = 1; i <= 7; i++)
            {
                if (!AddLegalMoveIfPossible(-i, -i))
                {
                    break;
                }
            }
        }
    }
}
