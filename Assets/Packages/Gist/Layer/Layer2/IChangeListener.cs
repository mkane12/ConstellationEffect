﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nobnak.Gist.Layer2 {

    public interface IChangeListener<Target> {

        void TargetOnChange(Target target);
    }
}
