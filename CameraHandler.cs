using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    
    public class CameraHandler
    {

        public static Bounds CalculateTargetsBoundingBox(List<Transform> targets, float boundingBoxPadding = 0f)
     {
         float minX = Mathf.Infinity;
         float maxX = Mathf.NegativeInfinity;
         float minY = Mathf.Infinity;
         float maxY = Mathf.NegativeInfinity;
 
         foreach (Transform target in targets) {
             Vector3 position = target.position;
             Bounds bounds = target.gameObject.GetComponent<SpriteRenderer>().bounds;
 
             minX = Mathf.Min(minX, position.x - (bounds.size.x / 2));
             minY = Mathf.Min(minY, position.y - (bounds.size.x/2));
             maxX = Mathf.Max(maxX, position.x + (bounds.size.x /2));
             maxY = Mathf.Max(maxY, position.y + (bounds.size.x/2));
         }
          Bounds b = new Bounds();
          b.center = new Vector3(((maxX - minX)/2) + minX, ((maxY - minY)/2) + minY, 1f);
          b.SetMinMax(new Vector3(minX, minY, 1f), new Vector3(maxX, maxY, 1f));
          return b;
     }

        /*
            List of Objects to follow given their center position
            Set this field
        */
        private List<GameObject> _targetObjects;
        public List<GameObject> TargetObjects
        {
            get
            {
                return _targetObjects;
            }
            set
            {
                _targetObjects = value;
                List<Transform> list = new List<Transform>();
                if (_targetObjects.Count == 0) return;
                foreach (GameObject go in _targetObjects)
                {
                    list.Add(go.transform);
                }
                _bounds = CalculateTargetsBoundingBox(list);
            }
        }

        /*
            TargetObject to follow (set this field)
        */
        public GameObject TargetObject;

        public Vector2 TargetPos = new Vector2(-1f, -1f);

        public bool IgnoreY = true;

        public bool DisableAfterFollow = false; 

        /*
            Bounds for TargetBounds calculated on set
        */
        private Bounds _bounds;

        /*
            Speed to follow the object.
        */

        public float FollowSpeed = 10.0f;

        /*
            True if following 'TargetObject' it is not null
        */
        [SerializeField]
        private bool _following;
        public bool Following
        {
            get => _following;
            set
            {
                _following = value;
            }
        }

        public void FixedUpdate()
        {
            if (Following && (TargetObject != null || (TargetPos.x != -1 && TargetPos.y != -1)))
            {
                float interpolation = FollowSpeed * Time.deltaTime;

                Vector3 position = this.transform.position;
                Vector2 newPos = TargetObject == null ? TargetPos : new Vector2(TargetObject.transform.position.x, TargetObject.transform.position.y);
                if (!IgnoreY) position.y = Mathf.Lerp(this.transform.position.y, newPos.y, interpolation);
                position.x = Mathf.Lerp(this.transform.position.x, newPos.x, interpolation);

                if (DisableAfterFollow) {
                    float diff = Mathf.Abs(this.transform.position.x - newPos.x);
                    if (diff < 0.18f)
                    {
                        DisableAfterFollow = false;
                        Following = false;
                        this.transform.position = new Vector3(newPos.x, this.transform.position.y, this.transform.position.z);
                    }
                }

                this.transform.position = new Vector3(position.x, position.y, this.transform.position.z);
            }
        }
    }
