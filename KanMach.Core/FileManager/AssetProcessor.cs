﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KanMach.Core.FileManager
{

    public abstract class AssetProcessor
    {
        public abstract object ProcessObject(Stream stream);
    }

    public abstract class AssetProcessor<T> : AssetProcessor
    {

        public override object ProcessObject(Stream stream) => Process(stream);

        public abstract T Process(Stream stream);

    }
}
