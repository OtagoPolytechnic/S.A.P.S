using System;
using System.Collections.Generic;
using UnityEngine;

// base written by joshii

/// <summary>
/// The visual representation of an NPC
/// </summary>
public class CharacterModel
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
    private float radius = 0.5f;
    private float height = 2;
    public readonly BodyMargins bodyMargins;
    public GameObject body;
    public Feature eyes;
    public Feature snoz;
    public Feature mouth;
    public List<Feature> features = new();

    public CharacterModel(BodyMargins bodyMargins)
    {
        this.bodyMargins = bodyMargins;
    }

    /// <summary>
    /// Instantiates the body GameObject as a child of the parent Transform
    /// </summary>
    public GameObject SpawnBody(GameObject bodyObj, Transform parent)
    {
        body = GameObject.Instantiate(bodyObj, parent);
        body.transform.localPosition -= Vector3.up;
        return body;
    }

    /// <summary>
    /// The radius of the character body
    /// </summary>
    public float Radius
    {
        get => radius;
        set
        {
            radius = value;
            float scale = value / bodyMargins.radius;
            body.transform.localScale = new
            (
                scale, body.transform.localScale.y, scale
            );
        }
    }

    /// <summary>
    /// The height of the character body
    /// </summary>
    public float Height
    {
        get => height;
        set
        {
            height = value;
            float scale = value / bodyMargins.height;
            body.transform.localScale = new
            (
                body.transform.localScale.x, scale, body.transform.localScale.z
            );
        }
    }

    public Feature AddFeature(GameObject featurePrefab, Feature.PlacementSetting placement)
    {
        Feature feature = new(this, featurePrefab, placement);
        features.Add(feature);
        return feature;
    }

    public void RemoveFeature(Feature feature)
    {
        features.Remove(feature);
        feature.DestroyFeatureObject();
    }
    #endregion

    #region Feature
    /// <summary>
    /// An object that is on (or worn by) a character body
    /// </summary>
    [Serializable]
    public class Feature
    {
        private GameObject featurePrefab;
        private GameObject featureObject;
        readonly CharacterModel model;

        public GameObject FeaturePrefab
        {
            get => featurePrefab; set
            {
                if (featurePrefab == value)
                {
                    return;
                }
                if (featureObject != null)
                {
                    DestroyFeatureObject();
                }
                featurePrefab = value;
                featureObject = GameObject.Instantiate(featurePrefab, model.body.transform);
                SetPositionFromPlacement();
            }
        }

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
                        GameObject.DestroyImmediate(mirroredObj);
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
        public Feature(CharacterModel model, GameObject featurePrefab, PlacementSetting placement)
        {
            this.model = model;
            FeaturePrefab = featurePrefab;
            Placement = placement;
        }

        public void DestroyFeatureObject()
        {
            if (featureObject == null)
            {
                return;
            }
            GameObject.DestroyImmediate(featureObject);
            if (placement.mirroring)
            {
                MirroredObj = null;
            }
        }

        /// <summary>
        /// Converts angle and height from Placement to position and rotation, relative to capsule body
        /// </summary>
        public void SetPositionFromPlacement()
        {
            if (model == null)
            {
                Debug.LogWarning($"Tried setting position of {featureObject.name} but couldn't find the body.");
                return;
            }

            if (featureObject == null)
            {
                return;
            }

            BodyMargins margins = model.bodyMargins;

            // get the midpoint of the body, but not above or below the cylindrical area
            // these two magic numbers are fractionally where capsule caps begin; i.e. where cylinder becomes a semisphere
            Vector3 originInCylinder = new() { y = Mathf.Clamp(placement.height * margins.height, margins.cylinderBottom, margins.cylinderTop) };

            // account for rounding of the top and bottom of capsule
            Vector2 distanceFromOrigin = new()
            {
                y = Mathf.Abs(placement.height * margins.height - originInCylinder.y)
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
            featureObject.transform.localPosition = new(
                margins.radius * radiusScale * Mathf.Sin(placement.angle),
                margins.height * placement.height,
                margins.radius * radiusScale * Mathf.Cos(placement.angle)
            );

            if (placement.protruding)
            {
                // stand up along the angle from origin
                Vector3 positionOnMesh = featureObject.transform.localPosition;
                positionOnMesh.y = placement.height * margins.height;
                featureObject.transform.up = (positionOnMesh - originInCylinder).normalized;
            }
            else
            {
                // face horizontally away from the midpoint (0, 0)
                Vector3 direction = new Vector3()
                {
                    x = featureObject.transform.localPosition.x,
                    z = featureObject.transform.localPosition.z
                }.normalized;

                if (direction.magnitude == 0) return;

                featureObject.transform.forward = direction;
            }

            if (placement.mirroring)
            {
                if (MirroredObj == null)
                    MirroredObj = GameObject.Instantiate(featureObject, featureObject.transform.parent);

                MirroredObj.transform.localScale = MirrorX(featureObject.transform.localScale);
                MirroredObj.transform.localPosition = MirrorX(featureObject.transform.localPosition);
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

    #region FeatureSO
    [CreateAssetMenu(fileName = "CharacterCreatorSO", menuName = "Scriptable Objects/CharacterCreatorSO")]
    public class FeatureSO : ScriptableObject
    {
        public GameObject featureObject;
        public Feature feature;
        public Feature.PlacementRange placementRange;
    }
    #endregion
}
