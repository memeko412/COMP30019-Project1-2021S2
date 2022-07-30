using System;

namespace RayTracer
{
    /// <summary>
    /// Class to represent a triangle in a scene represented by three vertices.
    /// </summary>
    public class Triangle : SceneEntity
    {
        private Vector3 v0, v1, v2;
        private Material material;

        /// <summary>
        /// Construct a triangle object given three vertices.
        /// </summary>
        /// <param name="v0">First vertex position</param>
        /// <param name="v1">Second vertex position</param>
        /// <param name="v2">Third vertex position</param>
        /// <param name="material">Material assigned to the triangle</param>
        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Material material)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.material = material;
        }

        /// <summary>
        /// Determine if a ray intersects with the triangle, and if so, return hit data.
        /// </summary>
        /// <param name="ray">Ray to check</param>
        /// <returns>Hit data (or null if no intersection)</returns>
        public RayHit Intersect(Ray ray)
        {
            
            Vector3 v0v1 = v1 - v0; 
            Vector3 v0v2 = v2 - v0; 
            Vector3 N = (v0v1.Cross(v0v2)).Normalized();
            Vector3 pvec = ray.Direction.Cross(v0v2); 
            double det = v0v1.Dot(pvec); 
            if (Math.Abs(det)<0.000001) return null;
            double invDet = 1 / det; 
            Vector3 tvec = ray.Origin - v0; 
            double u = tvec.Dot(pvec) * invDet; 
            if (u < 0 || u > 1) return null; 
        
            Vector3 qvec = tvec.Cross(v0v1); 
            double v = ray.Direction.Dot(qvec) * invDet; 
            if (v < 0 || u + v > 1) return null; 
        
            double t = v0v2.Dot(qvec) * invDet; 
            if (t < 0.000001) {
                return null;
            }
            // Step 2: inside-outside test
            Vector3 C; // vector perpendicular to triangle's plane 
        
            Vector3 P = ray.Origin+ray.Direction*t;
            // edge 0
            Vector3 edge0 = v1 - v0; 
            Vector3 vp0 = P - v0; 
            C = edge0.Cross(vp0); 
            if (N.Dot(C) < 0) return null; // P is on the right side 
        
            // edge 1
            Vector3 edge1 = v2 - v1; 
            Vector3 vp1 = P - v1; 
            C = edge1.Cross(vp1); 
            if ((u = N.Dot(C)) < 0)  return null; // P is on the right side 
        
            // edge 2
            Vector3 edge2 = v0 - v2; 
            Vector3 vp2 = P - v2; 
            C = edge2.Cross(vp2); 
            if ((v = N.Dot(C)) < 0) return null; // P is on the right side; 
            RayHit hitData = new RayHit(P, N, ray.Direction, this.material);

            return hitData; 

        }

        /// <summary>
        /// The material of the triangle.
        /// </summary>
        public Material Material { get { return this.material; } }
    }

}
