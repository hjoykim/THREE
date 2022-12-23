using System.Collections;

namespace THREE
{
    public class GLMaterials
    {
        private GLProperties properties;

        public GLMaterials(GLProperties properties)
        {
            this.properties = properties;
        }
        private void RefreshUniformsNormal(GLUniforms uniforms, MeshNormalMaterial material)
        {
            if (material.BumpMap != null)
            {

                (uniforms["bumpMap"] as Hashtable)["value"] = material.BumpMap;
                (uniforms["bumpScale"] as Hashtable)["value"] = material.BumpScale;
                if (material.Side == Constants.BackSide)
                {
                    float value = (float)(uniforms["bumpScale"] as Hashtable)["value"];
                    (uniforms["bumpScale"] as Hashtable)["value"] = -1 * value;
                }

            }

            if (material.NormalMap != null)
            {

                (uniforms["normalMap"] as Hashtable)["value"] = material.NormalMap;
                (uniforms["normalScale"] as Hashtable)["value"] = material.NormalScale;
                if (material.Side == Constants.BackSide)
                {
                    Vector2 value = (Vector2)(uniforms["normalScale"] as Hashtable)["value"];
                    (uniforms["normalScale"] as Hashtable)["value"] = value * -1.0f;
                }


            }

            if (material.DisplacementMap != null)
            {

                (uniforms["displacementMap"] as Hashtable)["value"] = material.DisplacementMap;
                (uniforms["displacementScale"] as Hashtable)["value"] = material.DisplacementScale;
                (uniforms["displacementBias"] as Hashtable)["value"] = material.DisplacementBias;

            }

        }
        public void RefreshMaterialUniforms(GLUniforms m_uniforms, Material material,float pixelRatio,float height)
         {
            if (material is MeshBasicMaterial)
            {

                RefreshUniformsCommon(m_uniforms, material);

            }
            else if (material is MeshLambertMaterial)
            {

                RefreshUniformsCommon(m_uniforms, material);
                RefreshUniformsLambert(m_uniforms, (MeshLambertMaterial)material);

            }
            else if (material is MeshToonMaterial)
            {
                RefreshUniformsCommon(m_uniforms, material);
                RefreshUniformsToon(m_uniforms, (MeshToonMaterial)material);
            }
            else if (material is MeshPhongMaterial)
            {

                RefreshUniformsCommon(m_uniforms, material);
                RefreshUniformsPhong(m_uniforms, (MeshPhongMaterial)material);
            }
            else if (material is MeshStandardMaterial)
            {

                RefreshUniformsCommon(m_uniforms, material);

                if (material is MeshPhysicalMaterial)
                {

                    RefreshUniformsPhysical(m_uniforms, (MeshPhysicalMaterial)material);

                }
                else
                {

                    RefreshUniformsStandard(m_uniforms, (MeshStandardMaterial)material);

                }

            }
            else if (material is MeshMatcapMaterial)
            {

                RefreshUniformsCommon(m_uniforms, material);

                RefreshUniformsMatcap(m_uniforms, (MeshMatcapMaterial)material);

            }
            else if (material is MeshDepthMaterial)
            {

                RefreshUniformsCommon(m_uniforms, material);
                RefreshUniformsDepth(m_uniforms, (MeshDepthMaterial)material);

            }
            else if (material is MeshDistanceMaterial)
            {

                RefreshUniformsCommon(m_uniforms, material);
                RefreshUniformsDistance(m_uniforms, (MeshDistanceMaterial)material);

            }
            else if (material is MeshNormalMaterial)
            {

                RefreshUniformsCommon(m_uniforms, material);
                RefreshUniformsNormal(m_uniforms, (MeshNormalMaterial)material);

            }
            else if (material is LineBasicMaterial)
            {

                RefreshUniformsLine(m_uniforms, (LineBasicMaterial)material);

                if (material is LineDashedMaterial)
                {

                    RefreshUniformsDash(m_uniforms, (LineDashedMaterial)material);

                }

            }
            else if (material is PointsMaterial)
            {

                RefreshUniformsPoints(m_uniforms, (PointsMaterial)material,pixelRatio,height);

            }
            else if (material is SpriteMaterial)
            {

                RefreshUniformsSprites(m_uniforms, material);

            }
            else if (material is ShadowMaterial)
            {

                (m_uniforms["color"] as Hashtable)["value"] = (material as ShadowMaterial).Color;
                (m_uniforms["opacity"] as Hashtable)["value"] = (material as ShadowMaterial).Opacity;

            }
            else if (material is ShaderMaterial)
            {

                (material as ShaderMaterial).UniformsNeedUpdate = false; // #15581

            }
        }

                 

        public void RefreshFogUniforms(GLUniforms uniforms,Fog fog)
        {
            (uniforms["fogColor"] as Hashtable)["value"] = fog.Color;

            if (fog is Fog)
            {

                (uniforms["fogNear"] as Hashtable)["value"] = fog.Near;
                (uniforms["fogFar"] as Hashtable)["value"] = fog.Far;

            }
            else if (fog is FogExp2)
            {

                (uniforms["fogDensity"] as Hashtable)["value"] = (fog as FogExp2).Density;

            }

        }
        private void RefreshUniformsLambert(GLUniforms uniforms, MeshLambertMaterial material)
        {
            if (material.EmissiveMap != null)
            {

                (uniforms["emissiveMap"] as Hashtable)["value"] = material.EmissiveMap;

            }
        }

        private void RefreshUniformsToon(GLUniforms uniforms, MeshToonMaterial material)
        {
            (uniforms["specular"] as Hashtable)["value"] = material.Specular;
            (uniforms["shininess"] as Hashtable)["value"] = (float)System.Math.Max(material.Shininess, 1e-4); // to prevent pow( 0.0, 0.0 )

            if (material.GradientMap != null)
            {

                (uniforms["gradientMap"] as Hashtable)["value"] = material.GradientMap;

            }

            if (material.EmissiveMap != null)
            {

                (uniforms["emissiveMap"] as Hashtable)["value"] = material.EmissiveMap;

            }

            if (material.BumpMap != null)
            {

                (uniforms["bumpMap"] as Hashtable)["value"] = material.BumpMap;
                (uniforms["bumpScale"] as Hashtable)["value"] = material.BumpScale;
                if (material.Side == Constants.BackSide)
                {
                    float value = (float)(uniforms["bumpScale"] as Hashtable)["value"];
                    (uniforms["bumpScale"] as Hashtable)["value"] = -1 * value;
                }

            }

            if (material.NormalMap != null)
            {

                (uniforms["normalMap"] as Hashtable)["value"] = material.NormalMap;
                (uniforms["normalScale"] as Hashtable)["value"] = material.NormalScale;
                if (material.Side == Constants.BackSide)
                {
                    Vector2 value = (Vector2)(uniforms["normalScale"] as Hashtable)["value"];
                    (uniforms["normalScale"] as Hashtable)["value"] = value * -1;
                }

            }

            if (material.DisplacementMap != null)
            {

                (uniforms["displacementMap"] as Hashtable)["value"] = material.DisplacementMap;
                (uniforms["displacementScale"] as Hashtable)["value"] = material.DisplacementScale;
                (uniforms["displacementBias"] as Hashtable)["value"] = material.DisplacementBias;

            }
        }

        private void RefreshUniformsPhong(GLUniforms uniforms, MeshPhongMaterial material)
        {
            (uniforms["specular"] as Hashtable)["value"] = material.Specular;
            (uniforms["shininess"] as Hashtable)["value"] = (float)System.Math.Max(material.Shininess, 1e-4); // to prevent pow( 0.0, 0.0 )

            if (material.EmissiveMap != null)
            {

                (uniforms["emissiveMap"] as Hashtable)["value"] = material.EmissiveMap;

            }

            if (material.BumpMap != null)
            {

                (uniforms["bumpMap"] as Hashtable)["value"] = material.BumpMap;
                (uniforms["bumpScale"] as Hashtable)["value"] = material.BumpScale;
                if (material.Side == Constants.BackSide)
                {
                    float value = (float)(uniforms["bumpScale"] as Hashtable)["value"];
                    (uniforms["bumpScale"] as Hashtable)["value"] = -1 * value;
                }

            }

            if (material.NormalMap != null)
            {

                (uniforms["normalMap"] as Hashtable)["value"] = material.NormalMap;
                (uniforms["normalScale"] as Hashtable)["value"] = material.NormalScale;
                if (material.Side == Constants.BackSide)
                {
                    Vector2 value = (Vector2)(uniforms["normalScale"] as Hashtable)["value"];
                    (uniforms["normalScale"] as Hashtable)["value"] = value * -1;
                }

            }

            if (material.DisplacementMap != null)
            {

                (uniforms["displacementMap"] as Hashtable)["value"] = material.DisplacementMap;
                (uniforms["displacementScale"] as Hashtable)["value"] = material.DisplacementScale;
                (uniforms["displacementBias"] as Hashtable)["value"] = material.DisplacementBias;

            }
        }

        private void RefreshUniformsPhysical(GLUniforms uniforms, MeshPhysicalMaterial material)
        {
            RefreshUniformsStandard(uniforms, material);

            (uniforms["reflectivity"] as Hashtable)["value"] = material.Reflectivity; // also part of uniforms common

            (uniforms["clearcoat"] as Hashtable)["value"] = material.Clearcoat;
            (uniforms["clearcoatRoughness"] as Hashtable)["value"] = material.ClearcoatRoughness;
            if (material.Sheen != null) (uniforms["sheen"] as Hashtable)["value"] = material.Sheen;

            if (material.ClearcoatNormalMap != null)
            {

                (uniforms["clearcoatNormalScale"] as Hashtable)["value"] = material.ClearcoatNormalScale;
                (uniforms["clearcoatNormalMap"] as Hashtable)["value"] = material.ClearcoatNormalMap;

                if (material.Side == Constants.BackSide)
                {
                    Vector2 value = (Vector2)(uniforms["clearcoatNormalScale"] as Hashtable)["value"];
                    (uniforms["clearcoatNormalScale"] as Hashtable)["value"] = value * -1;

                }

            }

            (uniforms["transparency"] as Hashtable)["value"] = material.Transparency;
        }

        private void RefreshUniformsStandard(GLUniforms uniforms, MeshStandardMaterial material)
        {
            (uniforms["roughness"] as Hashtable)["value"] = material.Roughness;
            (uniforms["metalness"] as Hashtable)["value"] = material.Metalness;

            if (material.RoughnessMap != null)
            {

                (uniforms["roughnessMap"] as Hashtable)["value"] = material.RoughnessMap;

            }

            if (material.MetalnessMap != null)
            {

                (uniforms["metalnessMap"] as Hashtable)["value"] = material.MetalnessMap;

            }

            if (material.EmissiveMap != null)
            {

                (uniforms["emissiveMap"] as Hashtable)["value"] = material.EmissiveMap;

            }

            if (material.BumpMap != null)
            {

                (uniforms["bumpMap"] as Hashtable)["value"] = material.BumpMap;
                (uniforms["bumpScale"] as Hashtable)["value"] = material.BumpScale;
                if (material.Side == Constants.BackSide)
                {
                    float value = (float)(uniforms["bumpScale"] as Hashtable)["value"];
                    (uniforms["bumpScale"] as Hashtable)["value"] = -1 * value;
                }

            }

            if (material.NormalMap != null)
            {

                (uniforms["normalMap"] as Hashtable)["value"] = material.NormalMap;
                (uniforms["normalScale"] as Hashtable)["value"] = material.NormalScale;
                if (material.Side == Constants.BackSide)
                {
                    Vector2 value = (Vector2)(uniforms["normalScale"] as Hashtable)["value"];
                    (uniforms["normalScale"] as Hashtable)["value"] = value * -1;
                }

            }

            if (material.DisplacementMap != null)
            {

                (uniforms["displacementMap"] as Hashtable)["value"] = material.DisplacementMap;
                (uniforms["displacementScale"] as Hashtable)["value"] = material.DisplacementScale;
                (uniforms["displacementBias"] as Hashtable)["value"] = material.DisplacementBias;

            }

            if (material.EnvMap != null)
            {

                //(uniforms["envMap"] as Hashtable)["value"] = material.envMap; // part of uniforms common
                (uniforms["envMapIntensity"] as Hashtable)["value"] = material.EnvMapIntensity;

            }
        }

        private void RefreshUniformsMatcap(GLUniforms uniforms, MeshMatcapMaterial material)
        {
            if (material.Matcap != null)
            {

                (uniforms["matcap"] as Hashtable)["value"] = material.Matcap;

            }

            if (material.BumpMap != null)
            {

                (uniforms["bumpMap"] as Hashtable)["value"] = material.BumpMap;
                (uniforms["bumpScale"] as Hashtable)["value"] = material.BumpScale;
                if (material.Side == Constants.BackSide)
                {
                    float value = (float)(uniforms["bumpScale"] as Hashtable)["value"];
                    (uniforms["bumpScale"] as Hashtable)["value"] = -1 * value;
                }

            }

            if (material.NormalMap != null)
            {

                (uniforms["normalMap"] as Hashtable)["value"] = material.NormalMap;
                (uniforms["normalScale"] as Hashtable)["value"] = material.NormalScale;
                if (material.Side == Constants.BackSide)
                {
                    Vector2 value = (Vector2)(uniforms["normalScale"] as Hashtable)["value"];
                    (uniforms["normalScale"] as Hashtable)["value"] = value * -1;
                }


            }

            if (material.DisplacementMap != null)
            {

                (uniforms["displacementMap"] as Hashtable)["value"] = material.DisplacementMap;
                (uniforms["displacementScale"] as Hashtable)["value"] = material.DisplacementScale;
                (uniforms["displacementBias"] as Hashtable)["value"] = material.DisplacementBias;

            }

        }

        private void RefreshUniformsDepth(GLUniforms uniforms, MeshDepthMaterial material)
        {
            if (material.DisplacementMap != null)
            {

                (uniforms["displacementMap"] as Hashtable)["value"] = material.DisplacementMap;
                (uniforms["displacementScale"] as Hashtable)["value"] = material.DisplacementScale;
                (uniforms["displacementBias"] as Hashtable)["value"] = material.DisplacementBias;

            }
        }

        private void RefreshUniformsDistance(GLUniforms uniforms, MeshDistanceMaterial material)
        {
            if (material.DisplacementMap != null)
            {
                (uniforms["displacementMap"] as Hashtable)["value"] = material.DisplacementMap;
                (uniforms["displacementScale"] as Hashtable)["value"] = material.DisplacementScale;
                (uniforms["displacementBias"] as Hashtable)["value"] = material.DisplacementBias;
            }

            (uniforms["referencePosition"] as Hashtable)["value"] = material.ReferencePosition;
            (uniforms["nearDistance"] as Hashtable)["value"] = material.NearDistance;
            (uniforms["farDistance"] as Hashtable)["value"] = material.FarDistance;

        }

        private void RefreshUniformsCommon(GLUniforms uniforms, Material material)
        {
            (uniforms["opacity"] as Hashtable)["value"] = material.Opacity;

            if (material.Color != null)
            {

                (uniforms["diffuse"] as Hashtable)["value"] = material.Color;

            }

            if (material.Emissive != null)
            {

                Color emissiveColor = material.Emissive.Value;
                (uniforms["emissive"] as Hashtable)["value"] = emissiveColor.MultiplyScalar(material.EmissiveIntensity);

            }

            if (material.Map != null)
            {

                (uniforms["map"] as Hashtable)["value"] = material.Map;

            }

            if (material.AlphaMap != null)
            {

                (uniforms["alphaMap"] as Hashtable)["value"] = material.AlphaMap;

            }

            if (material.SpecularMap != null)
            {

                (uniforms["specularMap"] as Hashtable)["value"] = material.SpecularMap;

            }

            if (material.EnvMap != null)
            {

                (uniforms["envMap"] as Hashtable)["value"] = material.EnvMap;

                // don't flip CubeTexture envMaps, flip everything else:
                //  WebGLRenderTargetCube will be flipped for backwards compatibility
                //  WebGLRenderTargetCube.texture will be flipped because it's a Texture and NOT a CubeTexture
                // this check must be handled differently, or removed entirely, if WebGLRenderTargetCube uses a CubeTexture in the future
                (uniforms["flipEnvMap"] as Hashtable)["value"] = material.EnvMap is CubeTexture ? -1.0f : 1.0f;

                (uniforms["reflectivity"] as Hashtable)["value"] = material.Reflectivity;
                (uniforms["refractionRatio"] as Hashtable)["value"] = material.RefractionRatio;

                (uniforms["maxMipLevel"] as Hashtable)["value"] = (properties.Get(material.EnvMap) as Hashtable)["maxMipLevel"];

            }

            if (material.LightMap != null)
            {

                (uniforms["lightMap"] as Hashtable)["value"] = material.LightMap;
                (uniforms["lightMapIntensity"] as Hashtable)["value"] = material.LightMapIntensity;

            }

            if (material.AoMap != null)
            {

                (uniforms["aoMap"] as Hashtable)["value"] = material.AoMap;
                (uniforms["aoMapIntensity"] as Hashtable)["value"] = material.AoMapIntensity;

            }

            // uv repeat and offset setting priorities
            // 1. color map
            // 2. specular map
            // 3. normal map
            // 4. bump map
            // 5. alpha map
            // 6. emissive map

            Texture uvScaleMap = null;

            if (material.Map != null)
            {

                uvScaleMap = material.Map;

            }
            else if (material.SpecularMap != null)
            {

                uvScaleMap = material.SpecularMap;

            }
            else if (material.DisplacementMap != null)
            {

                uvScaleMap = material.DisplacementMap;

            }
            else if (material.NormalMap != null)
            {

                uvScaleMap = material.NormalMap;

            }
            else if (material.BumpMap != null)
            {

                uvScaleMap = material.BumpMap;

            }
            else if (material.RoughnessMap != null)
            {

                uvScaleMap = material.RoughnessMap;

            }
            else if (material.MetalnessMap != null)
            {

                uvScaleMap = material.MetalnessMap;

            }
            else if (material.AlphaMap != null)
            {

                uvScaleMap = material.AlphaMap;

            }
            else if (material.EmissiveMap != null)
            {

                uvScaleMap = material.EmissiveMap;

            }

            if (uvScaleMap != null)
            {

                // backwards compatibility
                if (uvScaleMap is GLRenderTarget)
                {

                    uvScaleMap = (uvScaleMap as GLRenderTarget).Texture;

                }

                if (uvScaleMap.MatrixAutoUpdate == true)
                {

                    uvScaleMap.UpdateMatrix();

                }

                (uniforms["uvTransform"] as Hashtable)["value"] = uvScaleMap.Matrix;

            }
        }

        

        private void RefreshUniformsLine(GLUniforms uniforms, LineBasicMaterial material)
        {
            (uniforms["diffuse"] as Hashtable)["value"] = material.Color;
            (uniforms["opacity"] as Hashtable)["value"] = material.Opacity;
        }

        private void RefreshUniformsDash(GLUniforms uniforms, LineDashedMaterial material)
        {
            (uniforms["dashSize"] as Hashtable)["value"] = material.DashSize;
            (uniforms["totalSize"] as Hashtable)["value"] = material.DashSize + material.GapSize;
            (uniforms["scale"] as Hashtable)["value"] = material.Scale;

        }

        private void RefreshUniformsPoints(GLUniforms uniforms, PointsMaterial material,float _pixelRatio,float height)
        {
            (uniforms["diffuse"] as Hashtable)["value"] = material.Color;
            (uniforms["opacity"] as Hashtable)["value"] = material.Opacity;
            (uniforms["size"] as Hashtable)["value"] = material.Size * _pixelRatio;
            (uniforms["scale"] as Hashtable)["value"] = height * 0.5f;

            if (material.Map != null)
            {

                (uniforms["map"] as Hashtable)["value"] = material.Map;

            }

            if (material.AlphaMap != null)
            {

                (uniforms["alphaMap"] as Hashtable)["value"] = material.AlphaMap;

            }

            // uv repeat and offset setting priorities
            // 1. color map
            // 2. alpha map

            Texture uvScaleMap = null;

            if (material.Map != null)
            {

                uvScaleMap = material.Map;

            }
            else if (material.AlphaMap != null)
            {

                uvScaleMap = material.AlphaMap;

            }

            if (uvScaleMap != null)
            {

                if (uvScaleMap.MatrixAutoUpdate == true)
                {

                    uvScaleMap.UpdateMatrix();

                }

                if (!uniforms.ContainsKey("uvTransfrom"))
                {
                    (uniforms["uvTransform"] as Hashtable)["value"] = new Matrix3();
                }
                (((uniforms["uvTransform"] as Hashtable)["value"]) as Matrix3).Copy(uvScaleMap.Matrix);

            }
        }

        private void RefreshUniformsSprites(GLUniforms uniforms, Material material)
        {

            (uniforms["diffuse"] as Hashtable)["value"] = material.Color;
            (uniforms["opacity"] as Hashtable)["value"] = material.Opacity;
            (uniforms["rotation"] as Hashtable)["value"] = material.Rotation;

            if (material.Map != null)
            {

                (uniforms["map"] as Hashtable)["value"] = material.Map;

            }

            if (material.AlphaMap != null)
            {

                (uniforms["alphaMap"] as Hashtable)["value"] = material.AlphaMap;

            }

            // uv repeat and offset setting priorities
            // 1. color map
            // 2. alpha map

            Texture uvScaleMap = null;

            if (material.Map != null)
            {

                uvScaleMap = material.Map;

            }
            else if (material.AlphaMap != null)
            {

                uvScaleMap = material.AlphaMap;

            }

            if (uvScaleMap != null)
            {

                if (uvScaleMap.MatrixAutoUpdate == true)
                {

                    uvScaleMap.UpdateMatrix();

                }

                (uniforms["uvTransform"] as Hashtable)["value"] = uvScaleMap.Matrix;

            }

        }
    }
}
