using System;
using System.Collections.Generic;
using UnityEngine;

// base written by joshii

/// <summary>
/// The visual representation of an NPC
/// </summary>
public class CharacterModel : MonoBehaviour
{
    [Serializable]
    // Values to correct for capsule mesh being 0.5 x 2 x 0.5 when scale is (1, 1, 1)
    public struct BodyMargins
    {
        public float height;
        public float radius;
        // The Y coordinates of the 2m capsule mesh where the caps (transition from cylinder => hemisphere) are
        public float cylinderBottom;
        public float cylinderTop;
    }
    #region Model
    public readonly BodyMargins bodyMargins;
    public GameObject body;
    public Feature eyes;
    public Feature snoz;
    public Feature mouth;
    public List<Feature> features = new();


    /// <summary>
    /// Instantiates the body GameObject as a child of the model GameObject
    /// </summary>
    /// <param name="bodyObj">The object to instantiate</param>
    public void SpawnBody(GameObject bodyObj)
    {
        body = Instantiate(bodyObj, transform);
    }

    /// <summary>
    /// The radius of the character body
    /// </summary>
    public float Radius
    {
        get => body.transform.localScale.x * bodyMargins.radius;
        set
        {
            float scale = value / bodyMargins.radius;
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
        get => body.transform.localScale.y * bodyMargins.height;
        set
        {
            float scale = value / bodyMargins.height;
            body.transform.localScale = new
            (
                body.transform.localScale.x, scale, body.transform.localScale.z
            );
            RescaleFeatures();
        }
    }

    public void AddFeature(GameObject featureObj, Feature.PlacementSetting placement, string name = "feature")
    {
        features.Add(new Feature(this, featureObj, placement, name));
    }

    public void RemoveFeature(Feature feature)
    {
        features.Remove(feature);
        DestroyImmediate(feature.gameObject);
    }

    void RescaleFeatures()
    {
        foreach (Feature feature in features)
        {
            feature.SetPositionFromPlacement();
        }
    }

    #region Feature
    /// <summary>
    /// An object that is on (or worn by) a character body
    /// </summary>
    [Serializable]
    public class Feature
    {
        public GameObject gameObject;
        public string name;
        private CharacterModel model;

        /// <summary>
        /// <para>angle: in radians, clockwise, around the body.</para>
        /// <para>height: fraction (from 0 to 1) between base of the body and the top</para>
        /// <para>mirroring: when mirroring, a clone of the feature is placed at the inverted angle</para>
        /// <para>protruding: when protruding, the local Y direction will point directly 
        ///                  away from the surface of the body (works great for hats)</para>
        /// </summary>
        [Serializable]
        public struct PlacementSetting
        {
            public float angle;
            public float height;
            public bool mirroring;
            public bool protruding;
        }

        /// <summary>
        /// Sets the boundaries within which a feature's PlacementSetting is allowed to exist.
        /// Also includes default values.
        /// </summary>
        [Serializable]
        public struct PlacementRange
        {
            public float angleMin;
            public float angleMax;
            public float heightMin;
            public float heightMax;
            public PlacementSetting defaultSetting;
        }

        private GameObject mirroredObj;
        /// <summary>
        /// A generated duplicate of the feature when mirroring is enabled
        /// </summary>
        private GameObject MirroredObj
        {
            get => mirroredObj; set
            {
                if (value == null)
                {
                    if (mirroredObj != null)
                        DestroyImmediate(mirroredObj);
                    return;
                }
                mirroredObj = value;
            }
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
                if (!placement.mirroring)
                {
                    MirroredObj = null;
                }
                SetPositionFromPlacement();
            }
        }

        /// <summary>
        /// Constructs a new feature on a given character model, based on angle and height
        /// </summary>
        /// <param name="model"></param>
        public Feature(CharacterModel model, GameObject gameObject, PlacementSetting placement, string name)
        {
            this.model = model;
            this.gameObject = Instantiate(gameObject, model.transform);
            this.name = name;
            Placement = placement;
        }

        /// <summary>
        /// Converts angle and height from Placement to position and rotation, relative to capsule body
        /// </summary>
        public void SetPositionFromPlacement()
        {
            // CharacterModel model = gameObject.GetComponentInParent<CharacterModel>();
            if (model == null)
            {
                Debug.LogWarning($"Tried setting position of {featureObject.name} but couldn't find the body.");
                return;
            }

            BodyMargins margins = model.bodyMargins;

            // get the midpoint of the body, but not above or below the cylindrical area
            // these two magic numbers are fractionally where capsule caps begin; i.e. where cylinder becomes a semisphere
            Vector3 originInMesh = new() { y = Mathf.Clamp(placement.height * margins.height, margins.cylinderBottom, margins.cylinderTop) };

            // account for rounding of the top and bottom of capsule
            Vector2 distanceFromOrigin = new()
            {
                y = Mathf.Abs(placement.height * margins.height - originInMesh.y)
            };
            float radiusScale = 1;
            if (placement.protruding && distanceFromOrigin.y > 0)
            {
                // scales horizontal coords to stick feature to body when height is above or below the capsule crossover point
                // using trig: hypotenuse = margins.radius, side B = originDistance, looking for length of side A
                distanceFromOrigin.x = Mathf.Sqrt(Mathf.Pow(margins.radius, 2) - Mathf.Pow(distanceFromOrigin.y, 2));
                distanceFromOrigin.x = Mathf.Clamp(Mathf.Abs(distanceFromOrigin.x), 0, margins.radius);
                radiusScale = Mathf.InverseLerp(0, margins.radius, distanceFromOrigin.x);
            }

            // roughly get the point on the surface of the body
            featureObject.transform.position = new(
                model.Radius * radiusScale * Mathf.Sin(placement.angle),
                model.Height * placement.height,
                model.Radius * radiusScale * Mathf.Cos(placement.angle)
            );

            if (placement.protruding)
            {
                // stand up along the angle from origin
                Vector3 positionOnMesh = featureObject.transform.position;
                positionOnMesh.y = placement.height * margins.height;
                featureObject.transform.up = (positionOnMesh - originInMesh).normalized;
            }
            else
            {
                // face horizontally away from the midpoint (0, 0)
                Vector3 direction = new Vector3()
                {
                    x = featureObject.transform.position.x,
                    z = featureObject.transform.position.z
                }.normalized;

                if (direction.magnitude == 0) return;

                featureObject.transform.forward = direction;
            }

            if (placement.mirroring)
            {
                if (MirroredObj == null)
                    MirroredObj = GameObject.Instantiate(featureObject, featureObject.transform.parent);

                MirroredObj.transform.localScale = MirrorX(featureObject.transform.localScale);
                MirroredObj.transform.position = MirrorX(featureObject.transform.position);
                MirroredObj.transform.forward = MirrorX(featureObject.transform.forward);
            }
        }

        Vector3 MirrorX(Vector3 vector)
        {
            return new Vector3
            (
                -vector.x, vector.y, vector.z
            );
        }
    }
    #endregion
}