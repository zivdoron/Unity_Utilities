using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForThisCity.Utils
{
    public static class Circle
    {
        public static Vector2 MiddlePointInRange(Vector2 range)
        {
            float middleAngle = (range.x + range.y) * 0.5f;
            Vector2 point = AngleToPoint(middleAngle);
            return new Vector2(point.x, point.y);
        }
        public static Vector3 MiddlePointInRangeFixedPosition(Vector2 range, Vector3 circleMiddlePoint, float radius)
        {
            Vector2 middle = MiddlePointInRange(range);
            middle *= radius;
            return new Vector3(circleMiddlePoint.x + middle.x, circleMiddlePoint.y, circleMiddlePoint.z + middle.y);

        }
        public static Vector2 AngleToPoint(float angle)
        {
            angle *= Mathf.Deg2Rad;
            if(angle != 0 || angle != 90 || angle != 180 || angle != 270)
                return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            else
            {
                if(angle == 0)
                {
                    return new Vector2(0, 1);
                }
                if (angle == 90)
                {
                    return new Vector2(1, 0);
                }
                if (angle == 180)
                {
                    return new Vector2(0, -1);
                }
                if (angle == 270)
                {
                    return new Vector2(-1, 0);
                }
            }
            Debug.Log("could not find the offset point from angle");
            return new Vector2();
        }
        public static float PointToAngle(Vector2 point)
        {
            return Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg;
        }
        public static float PointToAngle(Vector3 point)
        {
            return PointToAngle(new Vector2(point.x, point.z));
        }
        public static float AngleDifference(float firstAngle, float secondAngle)
        {
            //if (firstAngle < -90 && secondAngle > 0) { firstAngle = -180 - firstAngle; }
            //if (secondAngle < -90 && firstAngle > 0) { secondAngle = -180 - secondAngle; }
            float result = Mathf.Abs( secondAngle - firstAngle);

            if(result > 180) { return 360 - result; }
            return result;
        }

        public static Vector3 GetClosestCirclePoint(Transform goingToCircleTransform, Transform middleTransform, float circleRadius)
        {
            float angle = Circle.PointToAngle(new Vector2(goingToCircleTransform.position.x - middleTransform.position.x, goingToCircleTransform.position.z - middleTransform.position.z));
            Vector2 finalPoint = Circle.AngleToPoint(angle) * circleRadius;
            return new Vector3(finalPoint.x, middleTransform.position.y, finalPoint.y);
        }
        public static Vector3 GetClosestCirclePointFixedPosition(Transform goingToCircleTransform, Transform middleTransform, float circleRadius)
        {
            return middleTransform.position + GetClosestCirclePoint(goingToCircleTransform, middleTransform, circleRadius);
        }

        public static List<Vector2> GetOptimalRanges(List<Vector2> ranges, int desiredRangesCount)
        {
            if (ranges.Count > 0 && ranges != null)
            {
                ranges = new List<Vector2>(ranges);
                List<Vector2> optimalRanges = new List<Vector2>();
                for (int i = 0; i < desiredRangesCount; i++)
                {
                    Vector2 currentOptimal = ranges[i];
                    for (int j = 0; j < ranges.Count - 1; j++)
                    {
                        if (ranges[j + 1].y - ranges[j + 1].x > ranges[j].y - ranges[j].x)
                        {
                            currentOptimal = ranges[j + 1];
                        }
                    }
                    optimalRanges.Add(currentOptimal);
                    ranges.Remove(currentOptimal);
                }
            return optimalRanges;
            }
            else
            {
                return new List<Vector2>();
            }
        }
        public static List<Vector2> GetOptimalRanges(List<Vector2> ranges, int desiredRangesCount, Transform self, Transform middle)
        {
            List<Vector2> optimalRanges = GetOptimalRanges(ranges, desiredRangesCount);
            if (optimalRanges.Count > 0)
            {
                for (int i = 0; i < optimalRanges.Count; i++)
                {
                    float currPoint = PointToAngle(new Vector2(self.position.x - middle.position.x, self.position.z - middle.position.z));
                    if (currPoint > optimalRanges[i].x && currPoint < optimalRanges[i].y)
                    {
                        Vector2 optimalByPos = optimalRanges[i];
                        optimalRanges.Add(new Vector2());
                        for (int j = 1; j < optimalRanges.Count; j++)
                        {
                            optimalRanges[j] = optimalRanges[j - 1];
                        }
                        optimalRanges[0] = optimalByPos;
                        break;
                    }
                }
                return optimalRanges;
            }
            else
            {
                return new List<Vector2>();
            }
        }
        public static List<Vector2> GetCircularRanges(Transform enemyTransform, Transform middle, List<Vector3> RegisteredEnemies)
        {
            #region sort by angle
            List<Vector3> registeredEnemies = new List<Vector3>();
            registeredEnemies.AddRange(RegisteredEnemies);
            List<Vector3> targets = new List<Vector3>();
            if (registeredEnemies.Count == 1)
            {
                return new List<Vector2>() { new Vector2(179, -179) };

            }
            if (registeredEnemies.Count == 2)
            {
                float angle = PointToAngle(registeredEnemies[0] - middle.position);

                float opposite;
                if (angle > 180) { opposite = angle - 180; } else { opposite = angle + 180; }
                Debug.Log(angle);
                return new List<Vector2>() { new Vector2(opposite, opposite - 1) };
            }
            while (registeredEnemies.Count > 0)
            {
                Vector3 currTransform = registeredEnemies[0];
                for (int j = 0; j < registeredEnemies.Count; j++)
                {
                    if (PointToAngle(registeredEnemies[j] - middle.transform.position)
                        < PointToAngle(currTransform - middle.transform.position))
                    {
                        currTransform = registeredEnemies[j];
                    }
                }
                targets.Add(currTransform);
                registeredEnemies.Remove(registeredEnemies.Find(Enemy => Enemy == currTransform));
            }
            #endregion

            List<Vector2> ranges = new List<Vector2>();
            for (int i = 0; i < targets.Count - 1; i++)
            {
                Vector2 currRange = new Vector2(Mathf.Atan2(targets[i].y - middle.transform.position.y, targets[i].x - middle.transform.position.x), Mathf.Atan2(targets[i + 1].y - middle.transform.position.y, targets[i + 1].x - middle.transform.position.x));
                ranges.Add(currRange);
            }
            return ranges;
        }

        public static float GetDistanceInCircle(Vector2 firstPt, Vector2 secondPt)
        {
            float firstAngle = PointToAngle(firstPt);
            float secondAngle = PointToAngle(secondPt);
            float radius = Mathf.Sqrt((firstPt.x * firstPt.x) + (firstPt.y * firstPt.y));
            float length = radius * 2 * Mathf.PI;

            return length / 360 * AngleDifference(firstAngle, secondAngle);
        }

    }
}
