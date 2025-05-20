using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SoftBodyScript : MonoBehaviour
{


    private const float splineOffset = 0.5f;

    [SerializeField]
    public SpriteShapeController spriteShape;
    [SerializeField]
    public Transform[] circlePoints;

    // Start is called before the first frame update
    void Start()
    {
        UpdateVerticies();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateVerticies();
    }

    void UpdateVerticies()
    {
        for (int i = 0; i < circlePoints.Length - 1; i++)
        {
            Vector2 vertex = circlePoints[i].localPosition;

            Vector2 towardsCenter = (Vector2.zero - vertex).normalized;

            float colliderRadius = circlePoints[i].gameObject.GetComponent<CircleCollider2D>().radius;

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

        }
    }
    
}
