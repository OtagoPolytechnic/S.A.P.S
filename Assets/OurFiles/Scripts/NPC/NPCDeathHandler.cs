using System.Collections;
using UnityEngine;

public class NPCDeathHandler : MonoBehaviour
{
    [SerializeField] private float ragdollTimer = 10f;
    [SerializeField] private Hurtbox hurtbox;

    private void Start()
    {
        hurtbox.onDie.AddListener(OnDie);
    }

    private void OnDie(GameObject npc)
    {
        DespawnCooldown(ragdollTimer);
    }

    private IEnumerator DespawnCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        Despawn();
    }

    private void Despawn()
    {
        GetComponent<NPCPather>().DestroySelf();
    }
}
