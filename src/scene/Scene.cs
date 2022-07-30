using System;
using System.Collections.Generic;
namespace RayTracer
{
    /// <summary>
    /// Class to represent a ray traced scene, including the objects,
    /// light sources, and associated rendering logic.
    /// </summary>
    public class Scene
    {
        private SceneOptions options;
        private ISet<SceneEntity> entities;
        private ISet<PointLight> lights;
        private ISet<SceneEntity> emissiveObjects;
        public const double PointOffset = 0.0000001f;

        /// <summary>
        /// Construct a new scene with provided options.
        /// </summary>
        /// <param name="options">Options data</param>
        public Scene(SceneOptions options = new SceneOptions())
        {
            this.options = options;
            this.entities = new HashSet<SceneEntity>();
            this.lights = new HashSet<PointLight>();
        }

        /// <summary>
        /// Add an entity to the scene that should be rendered.
        /// </summary>
        /// <param name="entity">Entity object</param>
        public void AddEntity(SceneEntity entity)
        {
            this.entities.Add(entity);
        }

        /// <summary>
        /// Add a point light to the scene that should be computed.
        /// </summary>
        /// <param name="light">Light structure</param>
        public void AddPointLight(PointLight light)
        {
            this.lights.Add(light);
        }

        /// <summary>
        /// Render the scene to an output image. This is where the bulk
        /// of your ray tracing logic should go... though you may wish to
        /// break it down into multiple functions as it gets more complex!
        /// </summary>
        /// <param name="outputImage">Image to store render output</param>
        public void Render(Image outputImage)
        {
            double fov = 60;
            double aspectRatio = outputImage.Width/outputImage.Height;
            double aaMult = this.options.AAMultiplier;
            double aaMultSq = aaMult*aaMult;
            Vector3 cameraPos = this.options.CameraPosition;
            Vector3 rayOrigin = cameraPos;
            emissiveObjects = GetEmissiveObjects();
            for (int i = 0; i < outputImage.Width; i++)
            {
                for (int j = 0; j < outputImage.Height; j++)
                {
                    Color finalPixelColor = new Color(0,0,0);
                    for (int widthCount = 0; widthCount < aaMult; widthCount++)
                    {
                        double widthOffset = widthCount/aaMult;
                        if(aaMult == 1) widthOffset=1/2;
                        for (int lengthCount = 0; lengthCount < aaMult; lengthCount++)
                        {
                            double lengthOffset = lengthCount/aaMult;
                            if(aaMult == 1) lengthOffset = 1/2;
                            double pixel_loc_x = (i+widthOffset)/outputImage.Width;
                            double pixel_loc_y = (j+lengthOffset)/outputImage.Height;
                            double x_pos = (pixel_loc_x * 2) - 1;
                            double y_pos = 1 - (pixel_loc_y * 2);
                            double half_fov_radians = fov/2*Math.PI/180;
                            double z_pos = 1.0f;
                            x_pos = x_pos * Math.Tan(half_fov_radians);
                            y_pos = y_pos * Math.Tan(half_fov_radians)/aspectRatio;
                            Vector3 rayDirection = new Vector3(x_pos,y_pos,z_pos);
                            Ray ray = new Ray(rayOrigin,rayDirection.Normalized());
                            finalPixelColor += CastRay(ray);
                        }
                    }
                    /**
                    for(int rayCount = 0; rayCount < aaMultSq; rayCount++)
                    {
                        double pixelOffset = (rayCount+1)/aaMultSq;
                        if(aaMultSq == 1)
                        {
                            pixelOffset = 1/2;
                        }
                        double pixel_loc_x = (i+pixelOffset)/outputImage.Width;
                        double pixel_loc_y = (j+pixelOffset)/outputImage.Height;
                        double x_pos = (pixel_loc_x * 2) - 1;
                        double y_pos = 1 - (pixel_loc_y * 2);
                        double half_fov_radians = fov/2*Math.PI/180;
                        double z_pos = 1.0f;
                        x_pos = x_pos * Math.Tan(half_fov_radians);
                        y_pos = y_pos * Math.Tan(half_fov_radians)/aspectRatio;
                        Vector3 rayDirection = new Vector3(x_pos,y_pos,z_pos);
                        Ray ray = new Ray(rayOrigin,rayDirection.Normalized());
                        finalPixelColor += CastRay(ray);
                    }
                    **/
                    finalPixelColor /= aaMultSq;
                    finalPixelColor = finalPixelColor.scaledColor();
                    outputImage.SetPixel(i,j,finalPixelColor);
                }
            }
        }


        public ISet<SceneEntity> GetEmissiveObjects()
        {
            ISet<SceneEntity> emissiveObjects = new HashSet<SceneEntity>();
            foreach (SceneEntity item in this.entities)
            {
                if(item.Material.Type == Material.MaterialType.Emissive)
                {
                    emissiveObjects.Add(item);
                }
            }
            return emissiveObjects;
        }
        // Cast ray for pixel grid
        public Color CastRay(Ray ray)
        {
            RayHit closestHitpoint = null;
            foreach (SceneEntity item in this.entities)
            {
                if(item.Intersect(ray) != null)
                {
                    RayHit hitPoint = item.Intersect(ray);
                    if (closestHitpoint == null) {
                        closestHitpoint = hitPoint;
                    } else if ((hitPoint.Position-ray.Origin).LengthSq() < (closestHitpoint.Position - ray.Origin).LengthSq())
                    {
                        closestHitpoint = hitPoint;
                    }
                }
            }
            Color finalColor = CalculateColor(closestHitpoint, 20);
            return finalColor;
        }

        // Check for shadow ray intersection
        public bool ShadowHit(Ray shadowRay, double distancesq) 
        {
            
            foreach (SceneEntity item in this.entities)
            {
                RayHit intersection = item.Intersect(shadowRay);
                if(intersection != null && (intersection.Position-shadowRay.Origin).LengthSq() < distancesq)
                {
                    return true;
                }
            }
            return false;
            /**
            RayHit closestHitpoint = null;
            foreach (SceneEntity item in this.entities)
            {
                RayHit intersection = item.Intersect(shadowRay);
                if(intersection != null && (intersection.Position-shadowRay.Origin).LengthSq() < distancesq)
                {
                    if (closestHitpoint == null) {
                        closestHitpoint = intersection;
                    } else if ((intersection.Position-shadowRay.Origin).LengthSq() < (closestHitpoint.Position-shadowRay.Origin).LengthSq())
                    {
                        closestHitpoint = intersection;
                    }
                }
            }
            return closestHitpoint;
            **/
        }


        //Calculate color at intersection
        public Color CalculateColor(RayHit closestHit, int depth) 
        {
            Color finalColor = new Color(0.0f,0.0f,0.0f);
            if(closestHit == null) 
            {
                return finalColor;
            }
            else if(closestHit.Material.Type == Material.MaterialType.Diffuse)
            {
                finalColor = DiffuseColor(closestHit);
            }
            else if (closestHit.Material.Type == Material.MaterialType.Reflective)
            {
                finalColor = ReflectiveColor(closestHit, depth);
            }
            else if (closestHit.Material.Type == Material.MaterialType.Glossy)
            {
                finalColor = GlossyColor(closestHit, depth);
            }
            else if (closestHit.Material.Type == Material.MaterialType.Refractive)
            {
                finalColor = RefractiveColor(closestHit, depth);
                
            }
            else if (closestHit.Material.Type == Material.MaterialType.Emissive)
            {
                finalColor = EmissiveColor(closestHit);
            }
            return finalColor;
        }

        //Get color of diffuse material
        public Color DiffuseColor(RayHit closestHit)
        {
            Color finalColor = new Color(0.0f,0.0f,0.0f);
            foreach (PointLight lightsource in this.lights)
            {
                Vector3 lightSourceDir = lightsource.Position - closestHit.Position;
                double distancesq = lightSourceDir.LengthSq();
                lightSourceDir = lightSourceDir.Normalized();
                Ray shadowRay = new Ray(closestHit.Position+closestHit.Normal*PointOffset, lightSourceDir);
                bool isShadowHit = ShadowHit(shadowRay, distancesq);
                if(!isShadowHit)
                {
                    double factor = closestHit.Normal.Dot(lightSourceDir);
                    Color resultColor = (closestHit.Material.Color*lightsource.Color).scaledColor();
                    resultColor = (resultColor*factor).scaledColor();
                    finalColor += resultColor;
                    finalColor = finalColor.scaledColor();
                }
            }
            return finalColor; 
        }

        // Get color of reflective material
        public Color ReflectiveColor(RayHit hit, int depth)
        {
            Color finalColor = new Color(0,0,0);
            Vector3 reflectionDir = hit.Incident - hit.Normal*2*hit.Incident.Dot(hit.Normal);
            reflectionDir = reflectionDir.Normalized();
            Ray reflectionRay = new Ray(hit.Position+hit.Normal*PointOffset, reflectionDir);
            RayHit closestHitpoint = null;
            foreach (SceneEntity item in this.entities)
            {
                if(item.Intersect(reflectionRay) != null)
                {
                    RayHit hitPoint = item.Intersect(reflectionRay);
                    if (closestHitpoint == null) {
                        closestHitpoint = hitPoint;
                    } else if ((hitPoint.Position-hit.Position).LengthSq() < (closestHitpoint.Position-hit.Position).LengthSq())
                    {
                        closestHitpoint = hitPoint;
                    }
                }
            }
            if (closestHitpoint == null) {
                return finalColor;
            } else if (depth > 0)
            {
                return finalColor = CalculateColor(closestHitpoint, depth - 1);
            } else {
                return finalColor;
            }

        }

        // Get Color of refractive material
        public Color RefractiveColor(RayHit hit, int depth)
        {
            Color finalColor = new Color(0,0,0);
            double kr;
            double n1 = 1.0f;
            double n2 = hit.Material.RefractiveIndex-0.08;
            double n = n1/n2;
            double cosI = Math.Abs(hit.Normal.Dot(hit.Incident));
            double sinT2 = n*n*(1.0f-Math.Pow(cosI,2));
            Color refractionCol = new Color(0,0,0);
            Color reflactionCol = new Color(0,0,0);
            if(sinT2 >= 1)
            {
                return finalColor;
            }
            double cosT = Math.Sqrt(1.0f-sinT2);
            double Rs = ((n2 * cosI) - (n1 * cosT)) / ((n2 * cosI) + (n1 * cosT)); 
            double Rp = ((n1 * cosI) - (n2 * cosT)) / ((n1 * cosI) + (n2 * cosT));
            kr = (Rs * Rs + Rp * Rp) / 2;
            Vector3 refractionDir = (n*hit.Incident+(n*cosI-cosT)*hit.Normal).Normalized();
            Ray refractiveRay = new Ray(hit.Position-PointOffset*hit.Normal, refractionDir);
            RayHit closestHitpoint = null;
            foreach (SceneEntity item in this.entities)
            {
                if(item.Intersect(refractiveRay) != null)
                {
                    RayHit hitPoint = item.Intersect(refractiveRay);
                    if (closestHitpoint == null) {
                        closestHitpoint = hitPoint;
                    } else if ((hitPoint.Position-hit.Position).LengthSq() < (closestHitpoint.Position-hit.Position).LengthSq())
                    {
                        closestHitpoint = hitPoint;
                    }
                }
            }
            if (closestHitpoint == null) {
                return finalColor;
            } else if (depth > 0)
            {
                refractionCol = CalculateColor(closestHitpoint,depth-1);
                reflactionCol = ReflectiveColor(hit, depth-1);
                finalColor = (refractionCol*(1-kr)).scaledColor()+(reflactionCol*kr).scaledColor();
                finalColor = new Color(finalColor.R-(hit.Material.Color.R * 0.05), finalColor.G-(hit.Material.Color.G * 0.05), finalColor.B-(hit.Material.Color.B * 0.05));
                finalColor = finalColor.scaledColor();
                return finalColor;
            } else {
                return finalColor;
            }            
        
        }

        // Get Color of glossy material
        public Color GlossyColor(RayHit hit, int depth)
        {
            Color finalColor = DiffuseColor(hit);
            finalColor += (ReflectiveColor(hit, depth))*0.5;
            finalColor = finalColor.scaledColor();
            return finalColor;
        }


        // Get Color of emissive material
        public Color EmissiveColor(RayHit hit)
        {
            return hit.Material.Color;
        }


    }
}
