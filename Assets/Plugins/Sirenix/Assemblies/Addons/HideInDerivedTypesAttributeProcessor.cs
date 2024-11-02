#if UNITY_EDITOR && ODIN_INSPECTOR
namespace LizardBrainGames.Editor
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using Sirenix.OdinInspector.Editor;
	using Sirenix.Utilities;

	internal sealed class HideInDerivedTypesAttributeProcessor<T> : OdinAttributeProcessor<T>
	{
		#region Processor Overrides

		public override bool CanProcessSelfAttributes(InspectorProperty property)
		{
			return !property.IsTreeRoot && property.Attributes.HasAttribute<HideInDerivedTypes>();
		}

		public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes)
		{
			InspectorProperty parentValueProperty = property.ParentValueProperty;
			Type baseType = parentValueProperty.ValueEntry.BaseValueType;
			Type valueType = parentValueProperty.ValueEntry.TypeOfValue;

			if (baseType == valueType)
			{
				return;
			}

			if (valueType.InheritsFrom(baseType))
			{
				attributes.Add(new HideInInspector());
			}
		}

		#endregion
	}
}
#endif