using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities.Editor;
#endif

using Debug = UnityEngine.Debug;

namespace Sirenix.OdinInspector
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	[DontApplyToListElements]
	public class LabelFoldoutAttribute : Attribute 
	{
	}
	
	#if UNITY_EDITOR
	public class LabelFoldoutAttributeDrawer : OdinAttributeDrawer<LabelFoldoutAttribute> 
	{
		protected override void DrawPropertyLayout(GUIContent _label)
		{
			Property.ValueEntry.WeakSmartValue = SirenixEditorGUI.Foldout((bool)Property.ValueEntry.WeakSmartValue, _label);
		}
	}
	#endif
}
