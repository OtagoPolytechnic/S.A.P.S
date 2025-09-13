using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteAlways]
public class NPCExpressionController : MonoBehaviour
{
    [SerializeField] private CharacterFeaturePackSO featurePack;
    public CharacterFeaturePackSO FeaturePack { get => featurePack; set => featurePack = value; }

    [SerializeField] private float expressionDuration = 3f;
    [SerializeField] private bool lockDeathExpression = true;
    [SerializeField] private CharacterModel model;

    [Header("Offsets (relative to originals)")]
    [SerializeField] private Vector3 eyesOffset = new Vector3(0f, -0.02f, 0.03f);
    [SerializeField] private Vector3 mouthOffset = new Vector3(0f, 0.02f, 0.04f);
    [Tooltip("Rotate offsets by the original local rotation so Z = face normal.")]
    [SerializeField] private bool rotateOffsetsWithOriginals = true;

    [Header("Appearance")]
    [SerializeField] private bool useOriginalRotation = false;
    [SerializeField] private bool mirrorRightEye = true;

    [Header("Type / Drivers")]
    [SerializeField] private bool isGuard = false;                    // true = Guard (Angry), false = NPC (Scared)
    [SerializeField] private bool autoDetectGuardByTag = false;
    [SerializeField] private string guardTag = "Guard";
    [SerializeField, Range(0f,100f)] private float angerLevel = 0f;   // Guards only

    [Header("Debug")]
    [SerializeField] private bool debugLog = false;

    public enum ExpressionType { Neutral, Scared, Angry, Death }

    private Transform eyesParent;
    private Transform mouthParent;

    private readonly List<GameObject> originalEyeObjects = new();
    private readonly List<GameObject> originalMouthObjects = new();

    private Transform leftEyeT;
    private Transform rightEyeT;
    private Vector3 leftPos;
    private Vector3 rightPos;
    private Vector3 pairCenterPos;
    private Quaternion leftRot;
    private Quaternion rightRot;
    private Vector3 eyeChildScale = Vector3.one;

    private Transform mouthT;
    private Vector3 mouthPos;
    private Quaternion mouthRot;
    private Vector3 mouthScale;

    private Transform eyesOrigParent;
    private Transform mouthOrigParent;

    private readonly List<GameObject> exprEyeInstances = new();
    private GameObject exprEyesPaired;
    private GameObject exprMouth;

    private bool originalsHidden;
    private bool isDead;
    private ExpressionType activeExpression = ExpressionType.Neutral;
    private bool isApplying;

    private const string ExprPrefix = "__expr__";

    private void Awake()
    {
        if (autoDetectGuardByTag && !string.IsNullOrEmpty(guardTag) && CompareTag(guardTag))
            isGuard = true;

        ResolveParents();
        CaptureOriginals(true);
    }

    private void OnEnable()
    {
        ResolveParents();
        ClearExpression();
    }

    public void Initialise(CharacterModel createdModel)
    {
        model = createdModel;
        ResolveParents();
        CaptureOriginals(true);
    }

    public void TriggerDeath() => SetExpression(ExpressionType.Death);

    // Civilians use suspicion -> Scared
    public void UpdateSuspicionLevel(float suspicion)
    {
        if (isDead || isGuard) return;
        if (suspicion >= 100f) SetExpression(ExpressionType.Scared);
        else if (suspicion <= 0f && activeExpression == ExpressionType.Scared) SetExpression(ExpressionType.Neutral);
    }

    // Guards use angerLevel -> Angry
    public void UpdateAngerLevel(float value)
    {
        if (isDead || !isGuard) return;
        angerLevel = Mathf.Clamp(value, 0f, 100f);
        if (angerLevel >= 1f) SetExpression(ExpressionType.Angry);
        else if (activeExpression == ExpressionType.Angry) SetExpression(ExpressionType.Neutral);
    }

    public void SetIsGuard(bool value)
    {
        isGuard = value;
        if (isGuard && activeExpression == ExpressionType.Scared) SetExpression(ExpressionType.Neutral);
        if (!isGuard && activeExpression == ExpressionType.Angry) SetExpression(ExpressionType.Neutral);
    }

    public void SetExpression(ExpressionType expression)
    {
        if (isDead && expression != ExpressionType.Death) return;
        if (expression == activeExpression) return;
        if (isApplying) return;

        if (isGuard && expression == ExpressionType.Scared) return;
        if (!isGuard && expression == ExpressionType.Angry) return;

        isApplying = true;

        ResolveParents();
        CaptureOriginals(false);

        if (debugLog) Debug.Log($"[NPCExpression] Request -> {expression} (isGuard={isGuard})", this);

        if (expression == ExpressionType.Scared)
        {
            ApplyExpression(SafeEyes(featurePack?.scaredEyes), SafeMouths(featurePack?.scaredMouths));
        }
        else if (expression == ExpressionType.Angry)
        {
            ApplyExpression(SafeEyes(featurePack?.angryEyes), SafeMouths(featurePack?.angryMouths));
        }
        else if (expression == ExpressionType.Death)
        {
            isDead = true;
            ApplyExpression(SafeEyes(featurePack?.deathEyes), SafeMouths(featurePack?.deathMouths));
        }
        else
        {
            RevertToOriginals();
        }

        StopAllCoroutines();
        if (expression == ExpressionType.Scared && !isGuard) StartCoroutine(ResetWhenCalm());
        if (expression == ExpressionType.Angry && isGuard) StartCoroutine(ResetWhenCalm());
        if (expression == ExpressionType.Death && !lockDeathExpression) StartCoroutine(ResetWhenCalm());

        activeExpression = expression;
        isApplying = false;
    }

    private GameObject[] SafeEyes(GameObject[] exprEyes)
    {
        if (exprEyes != null && exprEyes.Length > 0) return exprEyes;
        return featurePack != null && featurePack.eyes != null && featurePack.eyes.Length > 0 ? featurePack.eyes : null;
    }
    private GameObject[] SafeMouths(GameObject[] exprMouths)
    {
        if (exprMouths != null && exprMouths.Length > 0) return exprMouths;
        return featurePack != null && featurePack.mouths != null && featurePack.mouths.Length > 0 ? featurePack.mouths : null;
    }

    private void ResolveParents()
    {
        eyesParent = eyesOrigParent != null ? eyesOrigParent : GetParent(model != null ? model.eyes : null);
        mouthParent = mouthOrigParent != null ? mouthOrigParent : GetParent(model != null ? model.mouth : null);

        if (eyesParent == null)
        {
            GameObject instEyes = GetInstance(model != null ? model.eyes : null);
            if (instEyes != null) eyesParent = instEyes.transform.parent;
        }
        if (mouthParent == null)
        {
            GameObject instMouth = GetInstance(model != null ? model.mouth : null);
            if (instMouth != null) mouthParent = instMouth.transform.parent;
        }

        if (eyesParent == null) eyesParent = transform;
        if (mouthParent == null) mouthParent = transform;
    }

    private void CaptureOriginals(bool force)
    {
        if (force)
        {
            originalEyeObjects.Clear();
            originalMouthObjects.Clear();
            originalsHidden = false;
            leftEyeT = null;
            rightEyeT = null;
            mouthT = null;
        }

        if (originalEyeObjects.Count == 0)
            CollectByAny(eyesParent != null ? eyesParent : transform, new string[] { "eye", "eyes" }, originalEyeObjects);

        if (originalEyeObjects.Count >= 1)
        {
            originalEyeObjects.Sort((a, b) =>
            {
                float ax = a.transform.localPosition.x;
                float bx = b.transform.localPosition.x;
                return ax.CompareTo(bx);
            });

            leftEyeT = originalEyeObjects[0].transform;
            leftPos = leftEyeT.localPosition;
            leftRot = leftEyeT.localRotation;
            eyeChildScale = leftEyeT.localScale;
            eyesOrigParent = leftEyeT.parent;

            if (originalEyeObjects.Count >= 2)
            {
                rightEyeT = originalEyeObjects[1].transform;
                rightPos = rightEyeT.localPosition;
                rightRot = rightEyeT.localRotation;
                pairCenterPos = (leftPos + rightPos) * 0.5f;
            }
            else
            {
                rightEyeT = null;
                rightPos = leftPos;
                rightRot = leftRot;
                pairCenterPos = leftPos;
            }
        }

        if (originalMouthObjects.Count == 0)
            CollectByAny(mouthParent != null ? mouthParent : transform, new string[] { "mouth", "face" }, originalMouthObjects);

        if (originalMouthObjects.Count > 0)
        {
            mouthT = originalMouthObjects[0].transform;
            mouthPos = mouthT.localPosition;
            mouthRot = mouthT.localRotation;
            mouthScale = mouthT.localScale;
            mouthOrigParent = mouthT.parent;
        }
    }

    private void HideOriginals()
    {
        if (originalsHidden) return;
        foreach (var obj in originalEyeObjects) if (obj != null) obj.SetActive(false);
        foreach (var obj in originalMouthObjects) if (obj != null) obj.SetActive(false);
        originalsHidden = true;
    }

    private void ShowOriginals()
    {
        if (!originalsHidden) return;
        foreach (var obj in originalEyeObjects) if (obj != null) obj.SetActive(true);
        foreach (var obj in originalMouthObjects) if (obj != null) obj.SetActive(true);
        originalsHidden = false;
    }

    private void ApplyExpression(GameObject[] eyesOptions, GameObject[] mouthOptions)
    {
        HideOriginals();
        ClearExpression();

        GameObject nextEyes = PickOne(eyesOptions);
        GameObject nextMouth = PickOne(mouthOptions);

        if (nextEyes != null && eyesParent != null && leftEyeT != null)
        {
            bool paired = nextEyes.name.ToLower().Contains("eyes");

            if (paired)
            {
                exprEyesPaired = Object.Instantiate(nextEyes, eyesParent);
                exprEyesPaired.name = ExprPrefix + nextEyes.name;

                Vector3 off = eyesOffset;
                if (rotateOffsetsWithOriginals) off = leftRot * off;

                exprEyesPaired.transform.localPosition = pairCenterPos + off;
                exprEyesPaired.transform.localRotation = useOriginalRotation ? leftRot : Quaternion.identity;
                exprEyesPaired.transform.localScale = eyeChildScale;
            }
            else
            {
                Vector3 offL = eyesOffset;
                if (rotateOffsetsWithOriginals) offL = leftRot * offL;

                GameObject l = Object.Instantiate(nextEyes, eyesParent);
                l.name = ExprPrefix + nextEyes.name + "_L";
                l.transform.localPosition = leftPos + offL;
                l.transform.localRotation = useOriginalRotation ? leftRot : Quaternion.identity;
                l.transform.localScale = eyeChildScale;
                exprEyeInstances.Add(l);

                if (rightEyeT != null)
                {
                    Vector3 offR = eyesOffset;
                    if (mirrorRightEye) offR.x = -offR.x;
                    if (rotateOffsetsWithOriginals) offR = rightRot * offR;

                    GameObject r = Object.Instantiate(nextEyes, eyesParent);
                    r.name = ExprPrefix + nextEyes.name + "_R";
                    r.transform.localPosition = rightPos + offR;
                    r.transform.localRotation = useOriginalRotation ? rightRot : Quaternion.identity;
                    r.transform.localScale = mirrorRightEye ? new Vector3(-Mathf.Abs(eyeChildScale.x), eyeChildScale.y, eyeChildScale.z) : eyeChildScale;
                    exprEyeInstances.Add(r);
                }
            }
        }

        if (nextMouth != null && mouthParent != null && mouthT != null)
        {
            exprMouth = Object.Instantiate(nextMouth, mouthParent);
            exprMouth.name = ExprPrefix + nextMouth.name;

            Vector3 moff = mouthOffset;
            if (rotateOffsetsWithOriginals) moff = mouthRot * moff;

            exprMouth.transform.localPosition = mouthPos + moff;
            exprMouth.transform.localRotation = useOriginalRotation ? mouthRot : Quaternion.identity;
            exprMouth.transform.localScale = mouthScale;
        }
    }

    private void RevertToOriginals()
    {
        if (isDead) return;
        ClearExpression();
        ShowOriginals();
        activeExpression = ExpressionType.Neutral;
    }

    private void ClearExpression()
    {
        foreach (var g in exprEyeInstances)
        {
            if (g == null) continue;
#if UNITY_EDITOR
            if (!Application.isPlaying) Object.DestroyImmediate(g);
            else Object.Destroy(g);
#else
            Object.Destroy(g);
#endif
        }
        exprEyeInstances.Clear();

        if (exprEyesPaired != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) Object.DestroyImmediate(exprEyesPaired);
            else Object.Destroy(exprEyesPaired);
#else
            Object.Destroy(exprEyesPaired);
#endif
            exprEyesPaired = null;
        }

        if (exprMouth != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) Object.DestroyImmediate(exprMouth);
            else Object.Destroy(exprMouth);
#else
            Object.Destroy(exprMouth);
#endif
            exprMouth = null;
        }

        RemoveExprUnder(transform);
    }

    private void RemoveExprUnder(Transform root)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
        {
            Transform c = root.GetChild(i);
            if (c != null && c.name.StartsWith(ExprPrefix))
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) Object.DestroyImmediate(c.gameObject);
                else Object.Destroy(c.gameObject);
#else
                Object.Destroy(c.gameObject);
#endif
            }
            else
            {
                RemoveExprUnder(c);
            }
        }
    }

    private IEnumerator ResetWhenCalm()
    {
        if (isDead) yield break;

        float waited = 0f;
        while (IsEngagedForType())
        {
            yield return null;
            waited += Time.deltaTime;
        }
        if (waited < expressionDuration)
            yield return new WaitForSeconds(expressionDuration - waited);

        if (!IsEngagedForType())
            SetExpression(ExpressionType.Neutral);
    }

    private bool IsEngagedForType()
    {
        if (isGuard) return angerLevel >= 1f;
        VisionBehaviour vb = GetComponentInChildren<VisionBehaviour>(true);
        return vb != null && vb.Suspicion >= 100f;
    }

    private static void CollectByAny(Transform root, string[] keys, List<GameObject> results)
    {
        if (root == null) return;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform c = root.GetChild(i);
            string n = c.name.ToLower();
            bool match = false;
            for (int k = 0; k < keys.Length; k++)
            {
                if (n.Contains(keys[k].ToLower())) { match = true; break; }
            }
            if (match) results.Add(c.gameObject);
            CollectByAny(c, keys, results);
        }
    }

    private static GameObject PickOne(GameObject[] options)
    {
        if (options == null || options.Length == 0) return null;
        int i = Random.Range(0, options.Length);
        return options[i];
    }

    private static Transform GetParent(object feature)
    {
        if (feature == null) return null;
        System.Type t = feature.GetType();
        FieldInfo f = t.GetField("Parent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (f != null) return f.GetValue(feature) as Transform;
        PropertyInfo p = t.GetProperty("Parent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (p != null) return p.GetValue(feature, null) as Transform;
        return null;
    }

    private static GameObject GetInstance(object feature)
    {
        if (feature == null) return null;
        System.Type t = feature.GetType();
        FieldInfo f = t.GetField("Instance", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (f != null) return f.GetValue(feature) as GameObject;
        PropertyInfo p = t.GetProperty("Instance", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (p != null) return p.GetValue(feature, null) as GameObject;
        return null;
    }
}
