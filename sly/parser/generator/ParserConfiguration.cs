﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace sly.parser.generator
{
    public class ParserConfiguration<T>
    {
        public Dictionary<string, MethodInfo> Functions { get; set; }
        public Dictionary<string, NonTerminal<T>> NonTerminals { get; set; }
    }
}
