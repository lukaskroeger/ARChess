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
        public List<Tuple<int,int>> MovePoints { get; }

    }
}
