﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Interpreter
{
    public class Variable : INamable
    {
        public Value value;

        public string Name { get; set; }
    }
}
