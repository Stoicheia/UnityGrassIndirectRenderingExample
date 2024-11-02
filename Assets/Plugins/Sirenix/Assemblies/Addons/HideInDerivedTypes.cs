namespace LizardBrainGames
{
	using System;
	using System.Diagnostics;
	using Sirenix.OdinInspector;

	[Conditional("UNITY_EDITOR"), DontApplyToListElements]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class HideInDerivedTypes : Attribute { }
}