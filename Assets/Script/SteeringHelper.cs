using UnityEngine;

public static class SteeringHelper
{
    public static Vector2 ApplySteering(
        Transform self,
        Vector2 desiredDirection,
        float avoidDistance,
        float steeringStrength,
        LayerMask obstacleMask)
    {
        Vector2 adjusted = desiredDirection;
        float[] angles = new float[] { -60f, -30f, -15f, 15f, 30f, 60f };

        foreach (float angle in angles)
        {
            Vector2 offsetDir = Quaternion.Euler(0, 0, angle) * desiredDirection;
            RaycastHit2D hit = Physics2D.Raycast(self.position, offsetDir, avoidDistance, obstacleMask);

            if (hit.collider != null)
            {
                float dist = hit.distance;
                float weight = Mathf.Clamp01(1f - (dist / avoidDistance));

                Vector2 pushDir = Vector2.Perpendicular(offsetDir).normalized;
                float side = Vector2.SignedAngle(desiredDirection, offsetDir);
                if (side > 0) pushDir *= -1;

                adjusted += pushDir * weight * steeringStrength;
            }
        }

        return adjusted.normalized;
    }

    public static bool IsStuck(
        Transform self,
        Vector2 desiredDir,
        float avoidDistance,
        LayerMask obstacleMask,
        int threshold = 3)
    {
        float[] angles = new float[] { -60f, -30f, -15f, 15f, 30f, 60f };
        int obstacleHits = 0;

        foreach (float angle in angles)
        {
            Vector2 offsetDir = Quaternion.Euler(0, 0, angle) * desiredDir;
            RaycastHit2D hit = Physics2D.Raycast(self.position, offsetDir, avoidDistance, obstacleMask);

            if (hit.collider != null)
                obstacleHits++;
        }

        return obstacleHits >= threshold;
    }
}
