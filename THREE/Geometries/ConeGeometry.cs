namespace THREE
{
    //float radiusTop, float radiusBottom ,float height, int? radialSegments=null, int? heightSegments=null, bool? openEnded = null, float? thetaStart = null, float? thetaLength = null
    public class ConeGeometry : CylinderGeometry
    {
        public ConeGeometry(float radius, float height, int? radialSegments = null, int? heightSegments = null, bool? openEnded = null, float? thetaStart = null, float? thetaLength=null) :
            base(0,radius,height,radialSegments,heightSegments,openEnded,thetaStart,thetaLength)
        {

        }        
    }
    
    public class ConeBufferGeometry : CylinderBufferGeometry
    {
        public ConeBufferGeometry(float radius, float height, int? radialSegments = null, int? heightSegments = null, bool? openEnded = null, float? thetaStart = null, float? thetaLength = null):
            base(0, radius, height, radialSegments, heightSegments, openEnded, thetaStart, thetaLength)
        {

        }
    }
}
