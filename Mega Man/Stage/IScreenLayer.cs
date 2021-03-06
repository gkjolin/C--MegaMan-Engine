﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine
{
    public interface IScreenLayer
    {
        float LocationX { get; }
        float LocationY { get; }
        IGameplayContainer Stage { get; }
        MapSquare SquareAt(float px, float py);
    }
}
