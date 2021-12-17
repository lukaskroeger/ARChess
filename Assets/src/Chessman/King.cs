using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace Chessman
{
    internal class King : Chessman
    {        
        protected override void RefreshAvailableMoves()
        { 
            _availableMoves.Clear();
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;
                    AddLegalMoveIfPossible(i, j);
                }
            }            
        }
    }
}
