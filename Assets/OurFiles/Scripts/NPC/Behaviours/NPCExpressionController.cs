using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Runtime controller that swaps NPC facial parts with prefabs (eyes/mouth)
/// and exposes the current <see cref="ExpressionType"/>. Guards show Angry,
/// civilians show Scared, and Death overrides all.
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public class NPCExpressionController : MonoBehaviour
{
    /// <summary>Feature pack that contains prefab arrays for eyes and mouths.</summary>
    [SerializeField] private CharacterFeaturePackSO featurePack;
    /// <summary>Assigned feature pack.</summary>
    public CharacterFeaturePackSO FeaturePack { get => featurePack; set => featurePack = value; }

    /// <summary>Character model reference used to locate anchors.</summary>
    [SerializeField] private CharacterModel model;

    /// <summary>Offset from original eye anchor(s), in local space.</summary>
    [SerializeField] private Vector3 eyesOffset = new Vector3(0f, -0.02f, 0.03f);
    /// <summary>Offset from original mouth anchor, in local space.</summary>
    [SerializeField] private Vector3 mouthOffset = new Vector3(0f, 0.02f, 0.04f);
    /// <summary>If true, applies offsets in the original local rotation space.</summary>
    [SerializeField] private bool rotateOffsetsWithOriginals = true;

    /// <summary>If true, uses the originals' local rotation for spawned parts.</summary>
    [SerializeField] private bool useOriginalRotation = false;
    /// <summary>If true, mirrors the right-eye instance on X.</summary>
    [SerializeField] private bool mirrorRightEye = true;

    /// <summary>Marks this NPC as a guard (Angry instead of Scared).</summary>
    [SerializeField] private bool isGuard = false;
    /// <summary>Guard aggression driver (≥1 considered engaged).</summary>
    [SerializeField, Range(0f, 100f)] private float angerLevel = 0f;

    /// <summary>Enable debug logs.</summary>
    [SerializeField] private bool debugLog = false;

    /// <summary>All supported expression states.</summary>
    public enum ExpressionType { Neutral, Scared, Angry, Death }

    /// <summary>Current expression after the most recent application.</summary>
    public ExpressionType ActiveExpression => activeExpression;
    /// <summary>True if this NPC is a guard.</summary>
    public bool IsGuard => isGuard;
    /// <summary>Raised after <see cref="SetExpression(ExpressionType)"/> completes.</summary>
    public event System.Action<ExpressionType> ExpressionChanged;

    // captured anchors / parents
    private Transform eyesParent, mouthParent;
    private readonly List<GameObject> originalEyeObjects = new List<GameObject>();
    private readonly List<GameObject> originalMouthObjects = new List<GameObject>();

    private Transform leftEyeT, rightEyeT, mouthT, eyesOrigParent, mouthOrigParent;
    private Vector3 leftPos, rightPos, pairCenterPos, mouthPos, mouthScale, eyeChildScale = Vector3.one;
    private Quaternion leftRot, rightRot, mouthRot;

    // spawned instances
    private readonly List<GameObject> exprEyeInstances = new List<GameObject>();
    private GameObject exprEyesPaired, exprMouth;

    // state
    private bool originalsHidden = false;
    private bool isDead, isApplying;
    private ExpressionType activeExpression = ExpressionType.Neutral;

    private const string ExprPrefix = "__expr__";

    // UnityEvent listener cache (so RemoveListener uses the same delegate)
    private readonly Dictionary<System.Delegate, UnityAction<GameObject>> uaCache =
        new Dictionary<System.Delegate, UnityAction<GameObject>>();

    /// <summary>Initial setup: resolve parents and capture originals.</summary>
    private void Awake()
    {
        ResolveParents();
        CaptureOriginals(true);
    }

    /// <summary>Refreshes and subscribes to global NPC events.</summary>
    private void OnEnable()
    {
        ResolveParents();
        ClearExpression();
        HookEvents(true);
    }

    private void OnDisable() { HookEvents(false); }
    private void OnDestroy() { HookEvents(false); }

    /// <summary>Injects a runtime-created model and refreshes anchors.</summary>
    public void Initialise(CharacterModel createdModel)
    {
        model = createdModel;
        ResolveParents();
        CaptureOriginals(true);
    }

    /// <summary>Forces Death expression.</summary>
    public void TriggerDeath() => SetExpression(ExpressionType.Death);
    /// <summary>For civilians: forces Scared expression.</summary>
    public void TriggerPanic() { if (!isGuard && !isDead) SetExpression(ExpressionType.Scared); }
    /// <summary>For guards: forces Angry expression.</summary>
    public void TriggerChase() { if (isGuard && !isDead) SetExpression(ExpressionType.Angry); }
    /// <summary>Returns to Neutral expression.</summary>
    public void TriggerCalm() { if (!isDead) SetExpression(ExpressionType.Neutral); }

    /// <summary>Driver for civilians: suspicion ≥100 => Scared, ≤0 => Neutral.</summary>
    public void UpdateSuspicionLevel(float suspicion)
    {
        if (isDead || isGuard) return;
        if (suspicion >= 100f) SetExpression(ExpressionType.Scared);
        else if (suspicion <= 0f && activeExpression == ExpressionType.Scared) SetExpression(ExpressionType.Neutral);
    }

    /// <summary>Driver for guards: anger ≥1 => Angry, 0 => Neutral.</summary>
    public void UpdateAngerLevel(float value)
    {
        if (isDead || !isGuard) return;
        angerLevel = Mathf.Clamp(value, 0f, 100f);
        if (angerLevel >= 1f) SetExpression(ExpressionType.Angry);
        else if (activeExpression == ExpressionType.Angry) SetExpression(ExpressionType.Neutral);
    }

    /// <summary>Switches guard mode and resolves incompatible current states.</summary>
    public void SetIsGuard(bool value)
    {
        isGuard = value;
        if (isGuard && activeExpression == ExpressionType.Scared) SetExpression(ExpressionType.Neutral);
        if (!isGuard && activeExpression == ExpressionType.Angry) SetExpression(ExpressionType.Neutral);
    }

    /// <summary>Applies the requested expression immediately.</summary>
    public void SetExpression(ExpressionType expression)
    {
        if (isDead && expression != ExpressionType.Death) return;
        if (expression == activeExpression || isApplying) return;
        if (isGuard && expression == ExpressionType.Scared) return;
        if (!isGuard && expression == ExpressionType.Angry) return;

        isApplying = true;
        ResolveParents();
        CaptureOriginals(false);

        if (debugLog) Debug.Log("[NPCExpression] Request -> " + expression + " (isGuard=" + isGuard + ")", this);

        if (expression == ExpressionType.Scared)
        {
            ApplyExpression(SafeEyes(featurePack != null ? featurePack.scaredEyes : null),
                            SafeMouths(featurePack != null ? featurePack.scaredMouths : null));
        }
        else if (expression == ExpressionType.Angry)
        {
            ApplyExpression(SafeEyes(featurePack != null ? featurePack.angryEyes : null),
                            SafeMouths(featurePack != null ? featurePack.angryMouths : null));
        }
        else if (expression == ExpressionType.Death)
        {
            isDead = true;
            ApplyExpression(SafeEyes(featurePack != null ? featurePack.deathEyes : null),
                            SafeMouths(featurePack != null ? featurePack.deathMouths : null));
        }
        else
        {
            RevertToOriginals();
        }

        activeExpression = expression;
        isApplying = false;
        ExpressionChanged?.Invoke(activeExpression);
    }

    /// <summary>Returns expression-specific eyes or defaults from the pack.</summary>
    private GameObject[] SafeEyes(GameObject[] exprEyes)
    {
        if (exprEyes != null && exprEyes.Length > 0) return exprEyes;
        return featurePack != null && featurePack.eyes != null && featurePack.eyes.Length > 0 ? featurePack.eyes : null;
    }

    /// <summary>Returns expression-specific mouths or defaults from the pack.</summary>
    private GameObject[] SafeMouths(GameObject[] exprMouths)
    {
        if (exprMouths != null && exprMouths.Length > 0) return exprMouths;
        return featurePack != null && featurePack.mouths != null && featurePack.mouths.Length > 0 ? featurePack.mouths : null;
    }

    /// <summary>Finds parents for eyes/mouth from the model or falls back to this transform.</summary>
    private void ResolveParents()
    {
        eyesParent  = eyesOrigParent  != null ? eyesOrigParent  : GetParent(model != null ? model.eyes  : null);
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

    /// <summary>Captures original feature transforms and defines left/right/mid anchors.</summary>
    private void CaptureOriginals(bool force)
    {
        if (force)
        {
            originalEyeObjects.Clear();
            originalMouthObjects.Clear();
            originalsHidden = false;
            leftEyeT = rightEyeT = mouthT = null;
        }

        if (originalEyeObjects.Count == 0)
            CollectByAny(eyesParent != null ? eyesParent : transform, new string[] { "eye", "eyes" }, originalEyeObjects);

        if (originalEyeObjects.Count >= 1)
        {
            originalEyeObjects.Sort((GameObject a, GameObject b) => a.transform.localPosition.x.CompareTo(b.transform.localPosition.x));

            leftEyeT       = originalEyeObjects[0].transform;
            leftPos        = leftEyeT.localPosition;
            leftRot        = leftEyeT.localRotation;
            eyeChildScale  = leftEyeT.localScale;
            eyesOrigParent = leftEyeT.parent;

            if (originalEyeObjects.Count >= 2)
            {
                rightEyeT = originalEyeObjects[1].transform;
                rightPos  = rightEyeT.localPosition;
                rightRot  = rightEyeT.localRotation;
                pairCenterPos = (leftPos + rightPos) * 0.5f;
            }
            else
            {
                rightEyeT = null;
                pairCenterPos = leftPos;
            }
        }

        if (originalMouthObjects.Count == 0)
            CollectByAny(mouthParent != null ? mouthParent : transform, new string[] { "mouth", "face" }, originalMouthObjects);

        if (originalMouthObjects.Count > 0)
        {
            mouthT          = originalMouthObjects[0].transform;
            mouthPos        = mouthT.localPosition;
            mouthRot        = mouthT.localRotation;
            mouthScale      = mouthT.localScale;
            mouthOrigParent = mouthT.parent;
        }
    }

    /// <summary>Spawns eyes/mouth prefabs at computed anchors and hides originals.</summary>
    private void ApplyExpression(GameObject[] eyesOptions, GameObject[] mouthOptions)
    {
        HideOriginals();
        ClearExpression();

        GameObject nextEyes  = PickOne(eyesOptions);
        GameObject nextMouth = PickOne(mouthOptions);

        if (nextEyes != null && eyesParent != null && leftEyeT != null)
        {
            bool paired = nextEyes.name.ToLower().Contains("eyes");
            if (paired)
            {
                exprEyesPaired = Object.Instantiate(nextEyes, eyesParent);
                exprEyesPaired.name = ExprPrefix + nextEyes.name;

                Vector3 off = rotateOffsetsWithOriginals ? (leftRot * eyesOffset) : eyesOffset;
                exprEyesPaired.transform.localPosition = pairCenterPos + off;
                exprEyesPaired.transform.localRotation = useOriginalRotation ? leftRot : Quaternion.identity;
                exprEyesPaired.transform.localScale    = eyeChildScale;
            }
            else
            {
                Vector3 offL = rotateOffsetsWithOriginals ? (leftRot * eyesOffset) : eyesOffset;

                GameObject l = Object.Instantiate(nextEyes, eyesParent);
                l.name = ExprPrefix + nextEyes.name + "_L";
                l.transform.localPosition = leftPos + offL;
                l.transform.localRotation = useOriginalRotation ? leftRot : Quaternion.identity;
                l.transform.localScale    = eyeChildScale;
                exprEyeInstances.Add(l);

                if (rightEyeT != null)
                {
                    Vector3 offR = rotateOffsetsWithOriginals ? (rightRot * eyesOffset) : eyesOffset;
                    if (mirrorRightEye) offR.x = -offR.x;

                    GameObject r = Object.Instantiate(nextEyes, eyesParent);
                    r.name = ExprPrefix + nextEyes.name + "_R";
                    r.transform.localPosition = rightPos + offR;
                    r.transform.localRotation = useOriginalRotation ? rightRot : Quaternion.identity;
                    r.transform.localScale    = mirrorRightEye
                        ? new Vector3(-Mathf.Abs(eyeChildScale.x), eyeChildScale.y, eyeChildScale.z)
                        : eyeChildScale;
                    exprEyeInstances.Add(r);
                }
            }
        }

        if (nextMouth != null && mouthParent != null && mouthT != null)
        {
            exprMouth = Object.Instantiate(nextMouth, mouthParent);
            exprMouth.name = ExprPrefix + nextMouth.name;

            Vector3 moff = rotateOffsetsWithOriginals ? (mouthRot * mouthOffset) : mouthOffset;
            exprMouth.transform.localPosition = mouthPos + moff;
            exprMouth.transform.localRotation = useOriginalRotation ? mouthRot : Quaternion.identity;
            exprMouth.transform.localScale    = mouthScale;
        }
    }

    /// <summary>Clears spawned instances and restores originals; sets Neutral.</summary>
    private void RevertToOriginals()
    {
        if (isDead) return;
        ClearExpression();
        ShowOriginals();
        activeExpression = ExpressionType.Neutral;
        ExpressionChanged?.Invoke(activeExpression);
    }

    /// <summary>Destroys spawned instances and removes stray prefixed children.</summary>
    private void ClearExpression()
    {
        foreach (GameObject g in exprEyeInstances)
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

    /// <summary>Recursively removes children whose names start with the expression prefix.</summary>
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

    /// <summary>True while guard anger (≥1) or civilian suspicion (≥100) is engaged.</summary>
    private bool IsEngagedForType()
    {
        if (isGuard) return angerLevel >= 1f;
        VisionBehaviour vb = GetComponentInChildren<VisionBehaviour>(true);
        return vb != null && vb.Suspicion >= 100f;
    }

    /// <summary>DFS collect by name fragments.</summary>
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

    /// <summary>Randomly selects an element from an array.</summary>
    private static GameObject PickOne(GameObject[] options)
    {
        if (options == null || options.Length == 0) return null;
        int i = Random.Range(0, options.Length);
        return options[i];
    }

    /// <summary>Reads a Transform "Parent" from a feature object via reflection.</summary>
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

    /// <summary>Reads a GameObject "Instance" from a feature object via reflection.</summary>
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

    /// <summary>Disables original eyes/mouth once per activation.</summary>
    private void HideOriginals()
    {
        if (originalsHidden) return;
        foreach (GameObject obj in originalEyeObjects) if (obj != null) obj.SetActive(false);
        foreach (GameObject obj in originalMouthObjects) if (obj != null) obj.SetActive(false);
        originalsHidden = true;
    }

    /// <summary>Re-enables original eyes/mouth.</summary>
    private void ShowOriginals()
    {
        if (!originalsHidden) return;
        foreach (GameObject obj in originalEyeObjects) if (obj != null) obj.SetActive(true);
        foreach (GameObject obj in originalMouthObjects) if (obj != null) obj.SetActive(true);
        originalsHidden = false;
    }

    /// <summary>Subscribes/unsubscribes to global NPC UnityEvents if present.</summary>
    private void HookEvents(bool subscribe)
    {
        NPCEventManager mgr = NPCEventManager.Instance;
        if (mgr == null) return;

        TryHook(mgr, subscribe, new string[] { "onPanic", "Panic", "OnPanic" }, OnPanic);
        TryHook(mgr, subscribe, new string[] { "onChase", "Chase", "OnChase" }, OnChase);
        TryHook(mgr, subscribe, new string[] { "onCalm", "Calm", "OnCalm" }, OnCalm);
        TryHook(mgr, subscribe, new string[] { "onDeath", "OnDeath", "onPlayerArrested", "PlayerArrested" }, OnDeath);
    }

    /// <summary>Gets a cached UnityAction wrapper for a callback.</summary>
    private UnityAction<GameObject> GetUnityAction(System.Action<GameObject> cb)
    {
        UnityAction<GameObject> ua;
        bool found = uaCache.TryGetValue(cb, out ua);
        if (!found)
        {
            ua = delegate (GameObject go) { cb(go); };
            uaCache[cb] = ua;
        }
        return ua;
    }

    /// <summary>Adds/removes a listener from a UnityEvent&lt;GameObject&gt; found by name.</summary>
    private void TryHook(object target, bool sub, string[] candidateNames, System.Action<GameObject> cb)
    {
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        UnityAction<GameObject> ua = GetUnityAction(cb);

        foreach (string name in candidateNames)
        {
            FieldInfo f = target.GetType().GetField(name, flags);
            if (f != null && f.FieldType.IsGenericType &&
                f.FieldType.GetGenericTypeDefinition() == typeof(UnityEvent<>))
            {
                UnityEvent<GameObject> evField = f.GetValue(target) as UnityEvent<GameObject>;
                if (evField != null)
                {
                    if (sub) evField.AddListener(ua); else evField.RemoveListener(ua);
                }
                continue;
            }

            PropertyInfo p = target.GetType().GetProperty(name, flags);
            if (p != null && p.PropertyType.IsGenericType &&
                p.PropertyType.GetGenericTypeDefinition() == typeof(UnityEvent<>))
            {
                UnityEvent<GameObject> evProp = p.GetValue(target, null) as UnityEvent<GameObject>;
                if (evProp != null)
                {
                    if (sub) evProp.AddListener(ua); else evProp.RemoveListener(ua);
                }
            }
        }
    }

    /// <summary>Bridges global Panic event.</summary>
    private void OnPanic(GameObject who) { if (who == gameObject) TriggerPanic(); }
    /// <summary>Bridges global Chase event.</summary>
    private void OnChase(GameObject who) { if (who == gameObject) TriggerChase(); }
    /// <summary>Bridges global Calm event.</summary>
    private void OnCalm(GameObject who) { if (who == gameObject) TriggerCalm(); }
    /// <summary>Bridges global Death event.</summary>
    private void OnDeath(GameObject who) { if (who == gameObject) TriggerDeath(); }
}
