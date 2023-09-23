using System;
using System.Reflection;

namespace Assets.Core
{
    /// <summary>
    /// Кэширует список типов в выполняемой сборке
    /// </summary>
    public static class ReflectionCache
    {
        public readonly static Type[] AllTypes;
        static ReflectionCache()
        {
            AllTypes = Assembly.GetExecutingAssembly().GetTypes();
        }
    }
}
