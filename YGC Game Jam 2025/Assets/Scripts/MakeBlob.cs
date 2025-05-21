using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MakeBlob : MonoBehaviour
{

    private const float splineOffset = 0.5f;

    [SerializeField]
    public SpriteShapeController spriteShape;
    [SerializeField]
    public List<Transform> circlePointsTransform = new List<Transform>();
    public List<GameObject> circlePoints = new List<GameObject>();
    [SerializeField]
    public GameObject objectToCopy;
    [SerializeField]
    public float radius = 3f;
    [SerializeField]
    public int numberOfObjects = 5;

    // Start is called before the first frame update
    void Start()
    {

        //clear spline shape points
        spriteShape.spline.Clear();

        // create a circle around center point
        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = 360f * i / numberOfObjects;

            float angleRad = angle * Mathf.PI / 180;

            float x = Mathf.Cos(angleRad) * radius;
            float y = Mathf.Sin(angleRad) * radius;

            Vector3 position = new Vector3(x, y, 0);

            // Instantiate(GameObject.CreatePrimitive(PrimitiveType.Circle), transform.position + position, Quaternion.identity);
            GameObject point = Instantiate(objectToCopy, transform.position + position, Quaternion.identity);

            point.transform.SetParent(this.transform);

            circlePointsTransform.Add(point.transform);

            // new GameObject("point", typeof(GameObject));

            spriteShape.spline.InsertPointAt(i, point.transform.position);
            spriteShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);

        }

        circlePointsTransform.Add(this.transform);

        Debug.Log(spriteShape.spline.GetPointCount()); 
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVerticies();
    }
    
    void UpdateVerticies()
    {
        for (int i = 0; i < circlePointsTransform.Count - 1; i++)
        {
            Vector2 vertex = circlePointsTransform[i].localPosition;

            Vector2 towardsCenter = (Vector2.zero - vertex).normalized;

            float colliderRadius = circlePointsTransform[i].gameObject.GetComponent<CircleCollider2D>().radius;

            try
            {
                spriteShape.spline.SetPosition(i, (vertex - towardsCenter * colliderRadius));
            }
            catch
            {
                Debug.Log("Spline points are too close. recalc...");
                spriteShape.spline.SetPosition(i, (vertex - towardsCenter * (colliderRadius + splineOffset)));
            }

            Vector2 leftTangent = spriteShape.spline.GetLeftTangent(i);

            Vector2 newRt = Vector2.Perpendicular(towardsCenter) * leftTangent.magnitude;
            Vector2 newLt = Vector2.zero - (-newRt);

            spriteShape.spline.SetRightTangent(i, -newRt);
            spriteShape.spline.SetLeftTangent(i, newLt);
            // Debug.Log("Update Verticies");
        }
    }
}
