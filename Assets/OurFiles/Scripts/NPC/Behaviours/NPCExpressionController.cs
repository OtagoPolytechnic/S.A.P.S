using System.Collections;
using UnityEngine;

public class NPCExpressionController : MonoBehaviour
{
	[Header("Required References")]
	[SerializeField] private CharacterModel model;
	[SerializeField] private CharacterFeaturePackSO featurePack;

	[Header("Settings")]
	[SerializeField] private float expressionDuration = 3f;

	// Store original features so we can revert
	private GameObject defaultMouthPrefab;
	private GameObject defaultEyesPrefab;

	public enum ExpressionType { Neutral, Scared, Death }

	private void Awake()
	{
		// If model is already created by CharacterCreator, grab defaults
		if (model != null)
		{
			defaultMouthPrefab = model.mouth?.FeaturePrefab;
			defaultEyesPrefab = model.eyes?.FeaturePrefab;
		}
	}

	/// <summary>
	/// Changes NPC face to match an expression.
	/// </summary>
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
		if (expression != ExpressionType.Neutral)
		{
			StartCoroutine(ResetAfterDelay());
		}
	}

	private void SetMouth(GameObject[] mouthOptions)
	{
		if (mouthOptions != null && mouthOptions.Length > 0)
			model.mouth.FeaturePrefab = mouthOptions[Random.Range(0, mouthOptions.Length)];
	}

	private void SetEyes(GameObject[] eyeOptions)
	{
		if (eyeOptions != null && eyeOptions.Length > 0)
			model.eyes.FeaturePrefab = eyeOptions[Random.Range(0, eyeOptions.Length)];
	}

	private IEnumerator ResetAfterDelay()
	{
		yield return new WaitForSeconds(expressionDuration);
		SetExpression(ExpressionType.Neutral);
	}

	/// <summary>
	/// Call this from CharacterCreator once NPC is spawned so defaults are stored.
	/// </summary>
	public void Initialise(CharacterModel createdModel)
	{
		model = createdModel;
		defaultMouthPrefab = model.mouth?.FeaturePrefab;
		defaultEyesPrefab = model.eyes?.FeaturePrefab;
	}
}
