using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Barmetler
{
    public static class Bezier
    {
        public static Vector3 EvaluateQuadratic(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            var p0 = Vector3.Lerp(a, b, t);
            var p1 = Vector3.Lerp(b, c, t);

            return Vector3.Lerp(p0, p1, t);
        }


        public static Vector3 EvaluateCubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            var p0 = EvaluateQuadratic(a, b, c, t);
            var p1 = EvaluateQuadratic(b, c, d, t);

            return Vector3.Lerp(p0, p1, t);
        }


        public static Vector3 DeriveQuadratic(Vector3 a, Vector3 b, Vector3 c, float t)
        {
            return Vector3.Lerp(2 * (b - a), 2 * (c - b), t);
        }


        public static Vector3 DeriveCubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
        {
            return EvaluateQuadratic(3 * (b - a), 3 * (c - b), 3 * (d - c), t);
        }


        public static OrientedPoint[] GetEvenlySpacedPoints(
            IEnumerable<Vector3> points, IEnumerable<Vector3> normals, float spacing, float resolution = 1)
        {
            var b = new Bounds();

            return GetEvenlySpacedPoints(points, normals, ref b, null, spacing, resolution);
        }


        public static OrientedPoint[] GetEvenlySpacedPoints(
            IEnumerable<Vector3> points, IEnumerable<Vector3> normals, ref Bounds bounds, List<Bounds> boundingBoxes,
            float spacing, float resolution = 1)
        {
            var _points = points.ToList();
            var _normals = normals.ToList();
            var NumPoints = _points.Count;
            var NumSegments = NumPoints / 3;

            if (_normals.Count < NumSegments + 1)
            {
                throw new ArgumentException("not enough normals!");
            }


            int LoopIndex(int i)
            {
                return (i % NumPoints + NumPoints) % NumPoints;
            }


            Vector3[] GetPointsInSegment(int i)
            {
                return new[] {_points[i * 3], _points[i * 3 + 1], _points[i * 3 + 2], _points[LoopIndex(i * 3 + 3)]};
            }


            bounds.min = Vector3.positiveInfinity;
            bounds.max = Vector3.negativeInfinity;
            boundingBoxes?.Clear();

            float lineLength = 0;

            var esp = new List<OrientedPoint>();

            var previousPoint = _points[0] - (_points[1] - _points[0]).normalized * spacing;
            float dstSinceLastEvenPoint = 0;

            for (var segment = 0; segment < NumSegments; ++segment)
            {
                var segmentBounds = new Bounds
                {
                    min = Vector3.positiveInfinity,
                    max = Vector3.negativeInfinity
                };

                var p = GetPointsInSegment(segment);

                var normalOnCurve = _normals[segment];

                // Initialize bounding box
                segmentBounds.Encapsulate(p[0]);
                segmentBounds.Encapsulate(p[3]);

                var previousPointOnCurve = p[0];
                float segmentLength = 0;
                Vector3 forwardOnCurve;

                var controlNetLength = Vector3.Distance(p[0], p[1]) + Vector3.Distance(p[1], p[2]) + Vector3.Distance(p[2], p[3]);
                var estimatedCurveLength = Vector3.Distance(p[0], p[3]) + 0.5f * controlNetLength;
                var divisions = Mathf.CeilToInt(estimatedCurveLength * resolution * 10);
                var startIndex = esp.Count;
                var t = startIndex == 0 ? -1f / divisions : 0;

                while (t <= 1)
                {
                    t += 1f / divisions;
                    var pointOnCurve = EvaluateCubic(p[0], p[1], p[2], p[3], t);

                    if (t > -0.5f / divisions)
                    {
                        segmentLength += Vector3.Distance(pointOnCurve, previousPointOnCurve);
                    }

                    previousPointOnCurve = pointOnCurve;
                    forwardOnCurve = DeriveCubic(p[0], p[1], p[2], p[3], Mathf.Clamp01(t)).normalized;
                    normalOnCurve = Vector3.Cross(forwardOnCurve, Vector3.Cross(normalOnCurve, forwardOnCurve)).normalized;
                    dstSinceLastEvenPoint += Vector3.Distance(previousPoint, pointOnCurve);

                    while (dstSinceLastEvenPoint >= spacing)
                    {
                        var overshootDst = dstSinceLastEvenPoint - spacing;
                        var newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDst;

                        // Update bounding box
                        segmentBounds.Encapsulate(newEvenlySpacedPoint);

                        esp.Add(new OrientedPoint(newEvenlySpacedPoint, forwardOnCurve, normalOnCurve));

                        dstSinceLastEvenPoint = overshootDst;
                        previousPoint = newEvenlySpacedPoint;
                    }

                    previousPoint = pointOnCurve;
                }

                var endIndexExclusive = esp.Count;

                if (startIndex != endIndexExclusive)
                {
                    segmentLength += Vector3.Distance(previousPointOnCurve, p[3]);
                    lineLength += segmentLength;

                    forwardOnCurve = DeriveCubic(p[0], p[1], p[2], p[3], 1).normalized;
                    normalOnCurve = Vector3.Cross(forwardOnCurve, Vector3.Cross(normalOnCurve, forwardOnCurve)).normalized;
                    var angleError = Vector3.SignedAngle(normalOnCurve, _normals[segment + 1], forwardOnCurve);

                    // Iterate over evenly spaced points in this segment, and gradually correct angle error
                    var tStep = spacing / segmentLength;
                    var tStart = Vector3.Distance(esp[startIndex].position, p[0]) / segmentLength;

                    for (var i = startIndex; i < endIndexExclusive; ++i)
                    {
                        var t_ = (i - startIndex) * tStep + tStart;
                        // TODO: make weight non-linear, depending on handle lengths
                        var correction = t_ * angleError;
                        esp[i].normal = Quaternion.AngleAxis(correction, esp[i].forward) * esp[i].normal;
                    }
                }

                bounds.Encapsulate(segmentBounds);
                boundingBoxes?.Add(segmentBounds);
            }

            var result = esp.ToArray();
            result[0].position = _points[0];
            result[0].normal = _normals[0];
            result[0].forward = DeriveCubic(_points[0], _points[1], _points[2], _points[3], 0).normalized;
            result[result.Length - 1].position = _points[LoopIndex(-1)];
            result[result.Length - 1].normal = _normals[_normals.Count - 1];
            result[result.Length - 1].forward = DeriveCubic(_points[LoopIndex(-4)], _points[LoopIndex(-3)], _points[LoopIndex(-2)], _points[LoopIndex(-1)], 1).normalized;

            return result;
        }


        public static float AngleFromNormal(Vector3 forward, Vector3 normal)
        {
            forward = forward.normalized;
            normal = normal.normalized;
            normal = (normal - Vector3.Dot(forward, normal) * forward).normalized;
            var right = Vector3.Cross(Vector3.up, forward).normalized;
            var up = Vector3.Cross(forward, right).normalized;

            return Vector3.SignedAngle(normal, up, forward);
        }


        public static Vector3 NormalFromAngle(Vector3 forward, float angle)
        {
            var right = Vector3.Cross(Vector3.up, forward).normalized;
            var up = Vector3.Cross(forward, right).normalized;

            return Quaternion.AngleAxis(-angle, forward) * up;
        }


        /// <summary>
        ///     Position and Direction Vectors.
        /// </summary>
        public class OrientedPoint
        {
            public Vector3 position;
            public Vector3 forward;
            public Vector3 normal;


            public OrientedPoint(Vector3 p, Vector3 f, Vector3 n)
            {
                position = p;
                forward = f;
                normal = n;
            }


            public OrientedPoint ToWorldSpace(Transform transform)
            {
                var p = transform.TransformPoint(position);
                var f = transform.TransformDirection(forward);
                var n = transform.TransformDirection(normal);

                return new OrientedPoint(p, f, n);
            }


            public OrientedPoint ToLocalSpace(Transform transform)
            {
                var p = transform.InverseTransformPoint(position);
                var f = transform.InverseTransformDirection(forward);
                var n = transform.InverseTransformDirection(normal);

                return new OrientedPoint(p, f, n);
            }
        }
    }
}