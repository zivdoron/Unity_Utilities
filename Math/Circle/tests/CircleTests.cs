using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForThisCity.Utils
{
    public class CircleTests : MonoBehaviour
    {

        public Transform pointA;
        public Transform pointB;

        public float angle;
        public float radius;

        [Header("two angles check")]

        public UnityEngine.UI.InputField firstAngle;
        public UnityEngine.UI.InputField secondAngle;
        public void PrintAngle()
        {
            print(Circle.PointToAngle(new Vector2(pointB.position.x - pointA.position.x, pointB.position.z - pointA.position.z)));
        }
        public void PrintOffsetPointFromAngle()
        {
            print(Circle.AngleToPoint(angle));
        }

        public void PrintAngleDifference()
        {
            print(Circle.AngleDifference(float.Parse(firstAngle.text), float.Parse(secondAngle.text)));
        }

        public void PrintClosestPointInCircle()
        {
            print(Circle.GetClosestCirclePoint(pointB, pointA, radius));
        }
    }
}