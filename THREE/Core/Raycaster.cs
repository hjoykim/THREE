using System;
using System.Collections;
using System.Collections.Generic;
using THREE;

namespace THREE
{
    public class Intersection
    {
        public float distance;
        public float distanceToRay;
        public Vector3 point;
        public int index;
	    public Face3 face;
        public int faceIndex;
	    public Object3D object3D;
	    public Vector2 uv;
		public Vector2 uv2;
        public int instanceId;
    }
    public class RaycasterParameters
    {
		public Hashtable Mesh = new Hashtable();
		public Hashtable Line = new Hashtable();
		public Hashtable LOD = new Hashtable();
		public Hashtable Points = new Hashtable();
		public Hashtable Sprite = new Hashtable();
    }
    public class Raycaster
    {
        public Ray ray;
        public float near = 0;
        public float far = float.PositiveInfinity;
        public Camera camera;
        public Layers layers;
        public RaycasterParameters parameters;

		public Hashtable PointCloud
        {
			get
            {
				return parameters.Points;
            }
        }
		public Raycaster(Vector3 origin=null,Vector3 direction=null,float? near=null,float? far=null)
        {
			this.ray = new Ray(origin, direction);
			this.near = near != null ? near.Value : 0;
			this.far = far != null ? far.Value : float.PositiveInfinity;
			this.camera = null;
			this.layers = new Layers();
		
        }

		/**
		 * Updates the ray with a new origin and direction.
		 * @param origin The origin vector where the ray casts from.
		 * @param direction The normalized direction vector that gives direction to the ray.
		 */
		public void Set(Vector3 origin, Vector3 direction)
        {
			this.ray.Set(origin, direction);
		}

		/**
		 * Updates the ray with a new origin and direction.
		 * @param coords 2D coordinates of the mouse, in normalized device coordinates (NDC)---X and Y components should be between -1 and 1.
		 * @param camera camera from which the ray should originate
		 */
		public void SetFromCamera(Vector2 coords, Camera camera)
        {
			if ((camera!=null && camera is PerspectiveCamera))
			{

				this.ray.origin.SetFromMatrixPosition(camera.MatrixWorld);
				this.ray.direction.Set(coords.X, coords.Y, 0.5f).UnProject(camera).Sub(this.ray.origin).Normalize();
				this.camera = camera;

			}
			else if ((camera!=null && camera is OrthographicCamera))
			{

				this.ray.origin.Set(coords.X, coords.Y, (camera.Near + camera.Far) / (camera.Near - camera.Far)).UnProject(camera); // set origin in plane of camera
				this.ray.direction.Set(0, 0, -1).TransformDirection(camera.MatrixWorld);
				this.camera = camera;

			}
			else
			{

				throw new SystemException("THREE.Raycaster: Unsupported camera type.");

			}
		}
		private void IntersectObject(Object3D object3D,Raycaster raycaster,List<Intersection> intersects,bool recursive=false)
        {
			if (object3D.Layers.Test(raycaster.layers))
			{

				object3D.Raycast(raycaster, intersects);

			}

			if (recursive == true)
			{

				var children = object3D.Children;

				for (var i = 0;i< children.Count; i++)
				{

					IntersectObject(children[i], raycaster, intersects, true);

				}

			}
		}
		/**
		 * Checks all intersection between the ray and the object with or without the descendants. Intersections are returned sorted by distance, closest first.
		 * @param object The object to check for intersection with the ray.
		 * @param recursive If true, it also checks all descendants. Otherwise it only checks intersecton with the object. Default is false.
		 * @param optionalTarget (optional) target to set the result. Otherwise a new Array is instantiated. If set, you must clear this array prior to each call (i.e., array.length = 0;).
		 */
		public List<Intersection> IntersectObject(Object3D object3D,bool? recursive=null,List<Intersection> optionalTarget = null)
        {
			var intersects = optionalTarget!=null ? optionalTarget : new List<Intersection>();

			IntersectObject(object3D, this, intersects, recursive!=null? recursive.Value:false);
			//Sort(delegate (RenderItem a, RenderItem b)

			intersects.Sort(delegate(Intersection a,Intersection b) 
			{
				return (int)(a.distance - b.distance);
			});

			return intersects;
		}
	

		/**
		 * Checks all intersection between the ray and the objects with or without the descendants. Intersections are returned sorted by distance, closest first. Intersections are of the same form as those returned by .intersectObject.
		 * @param objects The objects to check for intersection with the ray.
		 * @param recursive If true, it also checks all descendants of the objects. Otherwise it only checks intersecton with the objects. Default is false.
		 * @param optionalTarget (optional) target to set the result. Otherwise a new Array is instantiated. If set, you must clear this array prior to each call (i.e., array.length = 0;).
		 */
		public List<Intersection> IntersectObjects(List<Object3D> objects,bool? recursive=null,List<Intersection> optionalTarget = null)
        {
			var intersects = optionalTarget != null ? optionalTarget : new List<Intersection>();				

			for (int i = 0;i<objects.Count; i++)
			{

				IntersectObject(objects[i], this, intersects, recursive!=null?recursive.Value:false);

			}

			intersects.Sort(delegate(Intersection a,Intersection b) 
			{
				return (int)(a.distance - b.distance);
			});

			return intersects;
		}
		
    }
}
