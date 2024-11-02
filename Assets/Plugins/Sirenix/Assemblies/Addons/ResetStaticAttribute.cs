#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using Sirenix.OdinInspector;


[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ResetStaticAttribute : Attribute
{
    public object DefaultValue { get; private set; }

    public ResetStaticAttribute(object defaultValue = null)
    {
        DefaultValue = defaultValue;
    }
}


[InitializeOnLoad]
public static class ResetStaticOnPlay
{
    static ResetStaticOnPlay()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChange;
    }

    private static void OnPlayModeStateChange(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            ResetAllTaggedStaticFields();
        }
    }

    private static void ResetAllTaggedStaticFields()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var field in fields)
                {
                    var attribute = (ResetStaticAttribute)Attribute.GetCustomAttribute(field, typeof(ResetStaticAttribute));
                    if (attribute != null)
                    {
                        object defaultValue = attribute.DefaultValue ?? GetDefault(field.FieldType);
                        field.SetValue(null, defaultValue);
                    }
                }
            }
        }
    }

    private static object GetDefault(Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        return null;
    }
}
#endif
