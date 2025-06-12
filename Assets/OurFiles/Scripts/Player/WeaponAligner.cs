using UnityEngine;

//written by Rohan Anakin
/// <summary>
/// Simple script that checks when the weapon is out and makes sure it stays in the correct local direction to the controller
/// </summary>
public class WeaponAligner : MonoBehaviour
{
    readonly Vector3 POS = new(0f, 0.08427072f, 0.05924988f);
    readonly Quaternion ROT = new(0.435129315f, 0, 0, 0.900367975f); //copied from the transform on the weapon in editor

    void Update()
    {
        if (gameObject.activeInHierarchy) //simple way of checking if the weapon is out
        {
            gameObject.transform.SetLocalPositionAndRotation(POS, ROT);
        }
    }
}
