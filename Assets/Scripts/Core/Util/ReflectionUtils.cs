using System;
using System.Collections.Generic;
using System.Reflection;

namespace pdxpartyparrot.Core.Util
{
    public static class ReflectionUtils
    {
        public static void FindImplementorsOfInNamespace<T>(ICollection<Type> typeCollection, string ns)
        {
            Type baseType = typeof(T);
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach(Type t in assembly.GetTypes()) {
                    if(t == baseType || null == t.Namespace) {
                        continue;
                    }

                    if(!baseType.IsAssignableFrom(t)) {
                        continue;
                    }

                    if(!t.Namespace.StartsWith(ns)) {
                        continue;
                    }

                    typeCollection.Add(t);
                }
            }
        }

        public static void FindSubClassesOf<T>(ICollection<Type> typeCollection)
        {
            Type baseType = typeof(T);
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach(Type t in assembly.GetTypes()) {
                    if(!t.IsSubclassOf(baseType)) {
                        continue;
                    }

                    typeCollection.Add(t);
                }
            }
        }

        public static void FindSubClassesOfInNamespace<T>(ICollection<Type> typeCollection, string ns)
        {
            Type baseType = typeof(T);
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach(Type t in assembly.GetTypes()) {
                    if(null == t.Namespace) {
                        continue;
                    }

                    if(!t.IsSubclassOf(baseType)) {
                        continue;
                    }

                    if(!t.Namespace.StartsWith(ns)) {
                        continue;
                    }

                    typeCollection.Add(t);
                }
            }
        }
    }
}
