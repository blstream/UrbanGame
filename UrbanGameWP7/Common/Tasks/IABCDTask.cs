﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public interface IABCDTask : IBaseTask
    {
        string ABCDPossibleAnswers { get; set; }

        byte? ABCDCorrectAnswer { get; set; }
    }
}
