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
using Sirenix.OdinInspector.Editor.ValueResolvers;
#endif
using Debug = UnityEngine.Debug;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	[DontApplyToListElements]
	public class ListPageAttribute : Attribute 
	{
		public string StringAction;

		public ListPageAttribute(string action = null)
		{
			StringAction = action;
		}
	}

	#if UNITY_EDITOR
	public class ListPageAttributeDrawer : OdinAttributeDrawer<ListPageAttribute> 
	{
		private const BindingFlags PrivateInstance = BindingFlags.NonPublic | BindingFlags.Instance;
		
		protected override void DrawPropertyLayout(GUIContent label)
		{

				UpdatePageIndex(Property, ValueResolver.Get<int>(Property, Attribute.StringAction, 1).GetValue());
			CallNextDrawer(label);
		}

		private int lastVisibleIndex;

		private void UpdatePageIndex(InspectorProperty _property, int visibleIndex)
		{
			// if (lastVisibleIndex == visibleIndex)
			// {
			// 	SetPageIndex(_property, visibleIndex);
			// 	return;
			// }

			int _setToindex = visibleIndex - 1;
			OdinDrawer _collectionDrawer =
				_property.GetActiveDrawerChain().BakedDrawerArray.SingleOrDefault(_drawer =>
					_drawer.GetType().Name.Contains("CollectionDrawer"));

			if (_collectionDrawer == null)
				return;

			var _infoField = _collectionDrawer.GetType().GetField("info", PrivateInstance);
			var _infoValue = _infoField.GetValue(_collectionDrawer);

			_infoField.FieldType.GetField("StartIndex").SetValue(_infoValue, _setToindex);
			_infoField.FieldType.GetField("EndIndex").SetValue(_infoValue, _setToindex);
			lastVisibleIndex = visibleIndex;
		}

		private void SetPageIndex(InspectorProperty _property, int visibleIndex)
		{
			int _setToindex = visibleIndex - 1;
			OdinDrawer _collectionDrawer =
				_property.GetActiveDrawerChain().BakedDrawerArray.SingleOrDefault(_drawer =>
					_drawer.GetType().Name.Contains("CollectionDrawer"));

			if (_collectionDrawer == null)
				return;
			var _infoField = _collectionDrawer.GetType().GetField("info", PrivateInstance);
			var _infoValue = _infoField.GetValue(_collectionDrawer);

			visibleIndex = (int)_infoField.FieldType.GetField("StartIndex").GetValue(_infoValue) + 1;
		}
	}
	#endif
}
