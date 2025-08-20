using System.Collections;
using UnityEngine;

public class NPCExpressionController : MonoBehaviour
{
	// Inspector-assignable, but still encapsulated for code access
	[SerializeField] private CharacterFeaturePackSO featurePack;
	public CharacterFeaturePackSO FeaturePack
	{
		get => featurePack;
		set => featurePack = value;
	}

	[SerializeField] private float expressionDuration = 3f;
	[SerializeField] private bool lockDeathExpression = true; // don't auto-revert when dead

	// Provided by CharacterCreator.Initialise(model)
	[SerializeField] private CharacterModel model;

	// Cache defaults so we can revert
	private GameObject defaultMouthPrefab;
	private GameObject defaultEyesPrefab;

	public enum ExpressionType { Neutral, Scared, Death }

	private void Awake()
	{
		// If CharacterCreator called Initialise already, these will be set there.
		// If not, try to cache defaults if a model reference was serialized.
		if (model != null)
		{
			defaultMouthPrefab = model.mouth?.FeaturePrefab;
			defaultEyesPrefab  = model.eyes?.FeaturePrefab;
		}
	}

	/// <summary>Call this from CharacterCreator once NPC is spawned so defaults are stored.</summary>
	public void Initialise(CharacterModel createdModel)
	{
		model = createdModel;
		defaultMouthPrefab = model.mouth?.FeaturePrefab;
		defaultEyesPrefab  = model.eyes?.FeaturePrefab;
	}

	/// <summary>Change NPC face to match an expression.</summary>
	public void SetExpression(ExpressionType expression)
	{
		if (model == null || featurePack == null) return;

		switch (expression)
		{
			case ExpressionType.Scared:
				SetMouth(featurePack.scaredMouths);
				SetEyes(featurePack.scaredEyes);
				break;

			case ExpressionType.Death:
				SetMouth(featurePack.deathMouths);
				SetEyes(featurePack.deathEyes);
				break;

			case ExpressionType.Neutral:
			default:
				SetMouth(new[] { defaultMouthPrefab });
				SetEyes(new[] { defaultEyesPrefab });
				break;
		}

		StopAllCoroutines();

		// Only auto-revert for temporary expressions
		if (expression == ExpressionType.Scared)
		{
			StartCoroutine(ResetAfterDelay());
		}
		// Death is permanent unless you disable lockDeathExpression
		else if (expression == ExpressionType.Death && !lockDeathExpression)
		{
			StartCoroutine(ResetAfterDelay());
		}
	}

	private void SetMouth(GameObject[] options)
	{
		if (options != null && options.Length > 0)
		{
			int i = Random.Range(0, options.Length);
			model.mouth.FeaturePrefab = options[i];
		}
	}

	private void SetEyes(GameObject[] options)
	{
		if (options != null && options.Length > 0)
		{
			int i = Random.Range(0, options.Length);
			model.eyes.FeaturePrefab = options[i];
		}
	}

	private IEnumerator ResetAfterDelay()
	{
		yield return new WaitForSeconds(expressionDuration);
		SetExpression(ExpressionType.Neutral);
	}
}
