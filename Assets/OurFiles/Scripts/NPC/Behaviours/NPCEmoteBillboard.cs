using UnityEngine;

/// <summary>
/// Emote icon above a simple "bean" model. No anchors needed.
/// - Picks sprites from an EmoteIconSetSO (lists per emotion)
/// - Auto-places above the bean using Renderer bounds
/// - Billboards toward camera with small random tilt
/// Designed to live on the same GameObject as NPCExpressionController.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(NPCExpressionController))]
[AddComponentMenu("NPC/Emote Billboard (Bean)")]
public class NPCEmoteBillboard_Bean : MonoBehaviour
{
    public enum PickMode { First, Index, Random }

    [Header("Icons (ScriptableObject)")]
    [SerializeField] private EmoteIconSetSO iconSet;

    [Header("Neutral Pick")]
    [SerializeField] private PickMode neutralPickMode = PickMode.First;
    [SerializeField, Min(0)] private int neutralIndex = 0;

    [Header("Scared Pick")]
    [SerializeField] private PickMode scaredPickMode = PickMode.Random;
    [SerializeField, Min(0)] private int scaredIndex = 0;

    [Header("Angry Pick")]
    [SerializeField] private PickMode angryPickMode = PickMode.Random;
    [SerializeField, Min(0)] private int angryIndex = 0;

    [Header("Death Pick")]
    [SerializeField] private PickMode deathPickMode = PickMode.First;
    [SerializeField, Min(0)] private int deathIndex = 0;

    [Header("Placement (Bean)")]
    [Tooltip("Extra height above the top of the bean’s render bounds.")]
    [SerializeField, Min(0f)] private float heightOffset = 0.25f;
    [SerializeField, Min(0f)] private float iconScale = 0.12f;

    [Header("Billboarding")]
    [SerializeField] private bool faceCamera = true;
    [SerializeField, Range(0f, 1f)] private float faceDamping = 0.25f; 
    [SerializeField] private Vector2 randomTiltDegrees = new Vector2(7f, 7f); 

    [Header("Visibility")]
    [SerializeField] private bool hideWhenNeutral = true;

    private NPCExpressionController source;
    private SpriteRenderer sr;
    private Transform camT;
    private float yawJitter;
    private float rollJitter;
    private NPCExpressionController.ExpressionType lastExpr = NPCExpressionController.ExpressionType.Neutral;

    private void Reset()
    {
        source = GetComponent<NPCExpressionController>();
    }

    private void OnValidate()
    {
        if (iconScale < 0f) iconScale = 0f;
        if (heightOffset < 0f) heightOffset = 0f;
    }

    private void Awake()
    {
        source = GetComponent<NPCExpressionController>();

        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
        {
            GameObject g = new GameObject("_EmoteIcon");
            g.transform.SetParent(transform, false);
            sr = g.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 50;
        }

        float yaw = Random.Range(-randomTiltDegrees.x, randomTiltDegrees.x);
        float roll = Random.Range(-randomTiltDegrees.y, randomTiltDegrees.y);
        yawJitter = yaw;
        rollJitter = roll;

        camT = Camera.main != null ? Camera.main.transform : null;

        ApplyScale();
        ForceSpriteRefresh();
        RepositionAboveBean();
        Billboard();
        UpdateVisibility();
    }

    private void LateUpdate()
    {
        RepositionAboveBean();
        UpdateSpriteIfChanged();
        UpdateVisibility();
        Billboard();
    }

    private void RepositionAboveBean()
    {
        Bounds b = CalculateRenderBounds(transform);
        Vector3 top = b.center + new Vector3(0f, b.extents.y, 0f);
        Vector3 pos = top + new Vector3(0f, heightOffset, 0f);
        transform.position = pos;
    }

    private static Bounds CalculateRenderBounds(Transform root)
    {
        Renderer[] rends = root.GetComponentsInChildren<Renderer>(true);

        if (rends == null || rends.Length == 0)
            return new Bounds(root.position, new Vector3(0.1f, 0.2f, 0.1f));

        Bounds b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++)
        {
            if (rends[i] == null) continue;
            b.Encapsulate(rends[i].bounds);
        }
        return b;
    }
    private void Billboard()
    {
        if (!faceCamera || sr == null) return;

        if (camT == null)
        {
            Camera cam = Camera.main;
            if (cam != null) camT = cam.transform;
            if (camT == null) return;
        }

        Vector3 toCam = camT.position - transform.position;
        if (toCam.sqrMagnitude < 0.0001f) return;

        Quaternion face = Quaternion.LookRotation(toCam.normalized, Vector3.up);
        Quaternion tilt = Quaternion.Euler(0f, yawJitter, rollJitter);
        Quaternion target = face * tilt;

        float t = faceDamping <= 0f ? 1f : Mathf.Clamp01(Time.deltaTime / Mathf.Max(0.0001f, faceDamping));
        transform.rotation = Quaternion.Slerp(transform.rotation, target, t);
    }

    private void UpdateVisibility()
    {
        if (sr == null || source == null) return;

        bool show = sr.sprite != null;
        if (hideWhenNeutral && source.ActiveExpression == NPCExpressionController.ExpressionType.Neutral)
            show = false;

        if (sr.enabled != show) sr.enabled = show;
    }

    private void UpdateSpriteIfChanged()
    {
        if (source == null || sr == null) return;

        NPCExpressionController.ExpressionType expr = source.ActiveExpression;
        if (expr != lastExpr || sr.sprite == null)
        {
            sr.sprite = PickSpriteFor(expr);
            lastExpr = expr;
        }
    }

    private void ForceSpriteRefresh()
    {
        if (source == null || sr == null) return;
        lastExpr = (NPCExpressionController.ExpressionType)(-1);
        UpdateSpriteIfChanged();
    }

    private Sprite PickSpriteFor(NPCExpressionController.ExpressionType expr)
    {
        if (iconSet == null) return null;

        Sprite[] pool = SelectPool(expr);
        if (pool == null || pool.Length == 0) return null;

        PickMode mode = SelectMode(expr);
        int index = SelectIndex(expr);

        if (mode == PickMode.First) return pool[0];

        if (mode == PickMode.Index)
        {
            if (index < 0) index = 0;
            if (index >= pool.Length) index = pool.Length - 1;
            return pool[index];
        }

        int i = Random.Range(0, pool.Length);
        return pool[i];
    }

    private Sprite[] SelectPool(NPCExpressionController.ExpressionType expr)
    {
        if (expr == NPCExpressionController.ExpressionType.Scared) return iconSet.scared;
        if (expr == NPCExpressionController.ExpressionType.Angry)  return iconSet.angry;
        if (expr == NPCExpressionController.ExpressionType.Death)  return iconSet.death;
        return iconSet.neutral;
    }

    private PickMode SelectMode(NPCExpressionController.ExpressionType expr)
    {
        if (expr == NPCExpressionController.ExpressionType.Scared) return scaredPickMode;
        if (expr == NPCExpressionController.ExpressionType.Angry)  return angryPickMode;
        if (expr == NPCExpressionController.ExpressionType.Death)  return deathPickMode;
        return neutralPickMode;
    }

    private int SelectIndex(NPCExpressionController.ExpressionType expr)
    {
        if (expr == NPCExpressionController.ExpressionType.Scared) return scaredIndex;
        if (expr == NPCExpressionController.ExpressionType.Angry)  return angryIndex;
        if (expr == NPCExpressionController.ExpressionType.Death)  return deathIndex;
        return neutralIndex;
    }

    private void ApplyScale()
    {
        transform.localScale = Vector3.one * iconScale;
    }
}
