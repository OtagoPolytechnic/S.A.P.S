using System;
using System.Collections.Generic;
using UnityEngine;

// base written by joshii

/// <summary>
/// Lightweight visual shell for an NPC: spawns/scales a capsule body, attaches
/// facial/features, applies skin material, and positions features via simple
/// parametric placement. Uses DestroyImmediate so it works in-editor.
/// </summary>
public class CharacterModel
{

    /// <summary>Capsule measurements for the body mesh (in meters).</summary>
    [Serializable]
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
    private Material skinColor;

     /// <summary>Static body geometry/limits used for scaling/placement.</summary>
    public readonly BodyMargins bodyMargins;
    public GameObject body;
    public CharacterVoicePackSO voice;
    public Feature eyes;
    public Feature snoz;
    public Feature mouth;

    /// <summary>All attached features (includes eyes/mouth/snoz + accessories).</summary>
    public List<Feature> features = new();

    /// <param name="bodyMargins">Required for character model to have the correct measurements of the body mesh</param>
    public CharacterModel(BodyMargins bodyMargins)
    {
        this.bodyMargins = bodyMargins;
    }

     /// <summary>Instantiates the body under <paramref name="parent"/> and offsets it down 1m.</summary>
    public GameObject SpawnBody(GameObject bodyObj, Transform parent)
    {
        body = GameObject.Instantiate(bodyObj, parent);
        body.transform.localPosition -= Vector3.up;
        return body;
    }

    /// <summary>
    /// Setting this changes the X/Z scale of the character's body to match the radius in meters
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
    /// Setting this changes the Y scale of the character's body to match the height in meters
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

    /// <summary>
    /// Applies the skin material to the LOD body renderers (first renderer per LOD)
    /// and any features that should match skin (currently the snoz).
    /// </summary>
    public Material SkinColor
    {
        get => skinColor;
        set
        {
            skinColor = value;
            LOD[] lods = body.GetComponent<LODGroup>().GetLODs(); //GetComponentsInChildren<MeshRenderer>();

            foreach (LOD lod in lods)
            {
                lod.renderers[0].material = skinColor;
            }

            // Add all features that should use skin colour here. 
            snoz.featureObject.GetComponentInChildren<MeshRenderer>().material = skinColor;
        }
    }

    /// <summary>
    /// Adds and instantiates a new feature
    /// </summary>
    /// <returns>The generated feature as a <c>Feature</c></returns>
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

    /// <summary>Attachable item on the body (e.g., eyes, mouth, hat, accessory).</summary>
    [Serializable]
    public class Feature
    {
        private GameObject featurePrefab;
        public GameObject featureObject { get; private set; }
        readonly CharacterModel model;

        /// <summary>
        /// Swapping this re-instantiates the feature prefab under the body and
        /// reapplies placement.
        /// </summary>
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
        /// Placement parameters:
        /// angle (rad, clockwise around Y), height (0–1 along body height),
        /// mirroring (clone at -X), protruding (local up points away from surface),
        /// fixedPosition (ignore placement; keep at origin for clothes/rig-bound items).
        /// </summary>
        [Serializable]
        public struct PlacementSetting
        {
            public float angle;
            public float height;
            public bool mirroring;
            public bool protruding;
            public bool fixedPosition;
        }

        /// <summary>Allowed/typical ranges for randomization.</summary>
        [Serializable]
        public struct PlacementRange
        {
            public float angleMin;
            public float angleMax;
            public float heightMin;
            public float heightMax;
        }

        private GameObject mirroredObj;
        
         /// <summary>Generated duplicate when mirroring is enabled.</summary>
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
        /// Updates transform(s) to match <see cref="placement"/>; destroys mirror if disabled.
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

        /// <summary>
        /// Destroys the GameObject attached to this feature
        /// </summary>
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
        /// Converts <see cref="Placement"/> (angle/height) into local position/rotation on the capsule,
        /// accounting for rounded caps and optional protrusion/mirroring.
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

            if (placement.fixedPosition)
            {
                featureObject.transform.localPosition = Vector3.zero;
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
                // accounts for the radius of the horizontal cross section getting smaller as you approach the ends of the capsule
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
}
