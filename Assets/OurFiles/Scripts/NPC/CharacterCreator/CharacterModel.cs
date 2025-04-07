using System;
using System.Collections.Generic;
using UnityEngine;

// base written by joshii

/// <summary>
/// The visual representation of an NPC
/// </summary>
public class CharacterModel : MonoBehaviour
{
    // The Y coordinates of the 2m capsule mesh where the caps (transition from cylinder => hemisphere) are
    const float CAPSULE_BASE = 0.5f;
    const float CAPSULE_TOP = 1.5f;

    // fields are serialized so we can easily make a default character prefab
    [SerializeField] private GameObject body;
    [SerializeField] private List<Feature> features;
    public List<Feature> Features { get => features; set => features = value; }

    /// <summary>
    /// The radius of the character body
    /// </summary>
    public float Radius
    {
        // The capsule mesh has a radius of 0.5 when scale is 1
        get => body.transform.localScale.x / 2f;
        set
        {
            float scale = value * 2;
            body.transform.localScale = new
            (
                scale, body.transform.localScale.y, scale
            );
            RescaleFeatures();
        }
    }

    /// <summary>
    /// The height of the character body
    /// </summary>
    public float Height
    {
        // The capsule mesh has a height of 2 when scale is 1
        get => body.transform.localScale.y * 2;
        set
        {
            float scale = value / 2f;
            body.transform.localScale = new
            (
                body.transform.localScale.x, scale, body.transform.localScale.z
            );
            RescaleFeatures();
        }
    }

    void RescaleFeatures()
    {
        foreach (Feature feature in features)
        {
            feature.SetPositionFromPlacement();
        }
    }

    /// <summary>
    /// An object that is on (or worn by) a character body
    /// </summary>
    [Serializable]
    public class Feature
    {
        public GameObject gameObject;

        /// <summary>
        /// <para>angle: in radians, clockwise, around the body.</para>
        /// <para>height: fraction (from 0 to 1) between base of the body and the top</para>
        /// <para>protrudes: when protruding, the local Y direction will point directly 
        ///                  away from the surface of the body (works great for hats)</para>
        /// </summary>
        [Serializable]
        public struct PlacementSetting
        {
            public float angle;
            public float height;
            public bool protrudes;
        }

        [SerializeField] private PlacementSetting placement;
        /// <summary>
        /// Sets the position and rotation of feature based on angle and height.
        /// </summary>
        public PlacementSetting Placement
        {
            get => placement; set
            {
                placement = value;
                SetPositionFromPlacement();
            }
        }

        public void SetPositionFromPlacement()
        {
            CharacterModel model = gameObject.GetComponentInParent<CharacterModel>();
            if (model == null)
            {
                Debug.LogWarning($"Tried setting position of {gameObject.name} but couldn't find the body.");
                return;
            }

            // get the midpoint of the body, but not above or below the cylindrical area
            // these two magic numbers are fractionally where capsule caps begin; i.e. where cylinder becomes a semisphere
            Vector3 origin = new() { y = Mathf.Clamp(placement.height * 2, CAPSULE_BASE, CAPSULE_TOP) };

            // account for rounding of the top and bottom of capsule
            float originDistance = Mathf.Abs(placement.height * 2 - origin.y);
            // origin distance should only ever be up to 0.5
            float radiusHeightModifier = Mathf.Cos(originDistance * Mathf.PI);

            // roughly get the point on the surface of the body
            gameObject.transform.position = new(
                model.Radius * radiusHeightModifier * Mathf.Sin(placement.angle),
                model.Height * placement.height,
                model.Radius * radiusHeightModifier * Mathf.Cos(placement.angle)
            );

            if (placement.protrudes)
            {
                // stand up along the angle from origin
                gameObject.transform.up = (gameObject.transform.position - origin).normalized;
            }
            else
            {
                // face horizontally away from the midpoint (0, 0)
                Vector3 direction = new Vector3()
                {
                    x = gameObject.transform.position.x,
                    z = gameObject.transform.position.z
                }.normalized;

                if (direction.magnitude == 0) return;

                gameObject.transform.forward = direction;
            }
        }
    }
}