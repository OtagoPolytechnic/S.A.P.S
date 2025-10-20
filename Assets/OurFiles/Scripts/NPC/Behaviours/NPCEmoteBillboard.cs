using UnityEngine;

/// <summary>
/// Displays an emoji sprite above an NPC based on <see cref="NPCExpressionController"/>.
/// </summary>
[DisallowMultipleComponent]
public class NPCEmoteBillboard : MonoBehaviour
{
    [SerializeField] private NPCExpressionController source;
    [SerializeField] private Sprite[] scaredIcons;
    [SerializeField] private Sprite[] angryIcons;
    [SerializeField] private Sprite[] deathIcons;
    [SerializeField] private Sprite[] neutralIcons;
    [SerializeField] private float yOffset = 1.6f;
    [SerializeField] private float iconScale = 0.12f;

    private SpriteRenderer sr;

    /// <summary>Initialises the emoji renderer.</summary>
    private void Awake()
    {
        if (source == null) source = GetComponent<NPCExpressionController>();

        Transform child = transform.Find("_EmoteIcon");
        if (child == null)
        {
            GameObject g = new GameObject("_EmoteIcon");
            g.transform.SetParent(transform, false);
            child = g.transform;
        }

        sr = child.GetComponent<SpriteRenderer>();
        if (sr == null) sr = child.gameObject.AddComponent<SpriteRenderer>();

        child.localPosition = new Vector3(0f, yOffset, 0f);
        child.localRotation = Quaternion.identity;
        child.localScale = Vector3.one * iconScale;
    }

    /// <summary>Subscribes to <see cref="NPCExpressionController.ExpressionChanged"/>.</summary>
    private void OnEnable()
    {
        if (source == null) source = GetComponent<NPCExpressionController>();
        if (source != null) source.ExpressionChanged += OnExpressionChanged;

        if (source != null) OnExpressionChanged(source.ActiveExpression);
        else OnExpressionChanged(NPCExpressionController.ExpressionType.Neutral);
    }

    /// <summary>Unsubscribes from <see cref="NPCExpressionController.ExpressionChanged"/>.</summary>
    private void OnDisable()
    {
        if (source != null) source.ExpressionChanged -= OnExpressionChanged;
    }

    /// <summary>Updates the sprite when the NPC’s expression changes.</summary>
    private void OnExpressionChanged(NPCExpressionController.ExpressionType expr)
    {
        if (sr == null) return;

        Sprite pick = null;
        if (expr == NPCExpressionController.ExpressionType.Scared) pick = Pick(scaredIcons);
        else if (expr == NPCExpressionController.ExpressionType.Angry) pick = Pick(angryIcons);
        else if (expr == NPCExpressionController.ExpressionType.Death) pick = Pick(deathIcons);
        else pick = Pick(neutralIcons);

        sr.sprite = pick;
        sr.enabled = pick != null;
    }

    /// <summary>Randomly selects a sprite from the array.</summary>
    private static Sprite Pick(Sprite[] arr)
    {
        if (arr == null || arr.Length == 0) return null;
        int i = Random.Range(0, arr.Length);
        return arr[i];
    }
}
