using System;
using System.Collections.Generic;


namespace Editor
{
    [Serializable]
    public class FactoryInfo
    {
        public string factoryName;
        public string factoryPath;
        public List<string> prefabPaths = new List<string>();
    }
}
