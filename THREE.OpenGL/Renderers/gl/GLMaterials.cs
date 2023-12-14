using System.Collections;
using THREE.OpenGL.Extensions;

namespace THREE
{
    [Serializable]
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

                (uniforms["bumpMap"] as GLUniform)["value"] = material.BumpMap;
                (uniforms["bumpScale"] as GLUniform)["value"] = material.BumpScale;
                if (material.Side == Constants.BackSide)
                {
                    float value = (float)(uniforms["bumpScale"] as GLUniform)["value"];
                    (uniforms["bumpScale"] as GLUniform)["value"] = -1 * value;
                }

            }

            if (material.NormalMap != null)
            {

                (uniforms["normalMap"] as GLUniform)["value"] = material.NormalMap;
                (uniforms["normalScale"] as GLUniform)["value"] = material.NormalScale;
                if (material.Side == Constants.BackSide)
                {
                    Vector2 value = (Vector2)(uniforms["normalScale"] as GLUniform)["value"];
                    (uniforms["normalScale"] as GLUniform)["value"] = value * -1.0f;
                }


            }

            if (material.DisplacementMap != null)
            {

                (uniforms["displacementMap"] as GLUniform)["value"] = material.DisplacementMap;
                (uniforms["displacementScale"] as GLUniform)["value"] = material.DisplacementScale;
                (uniforms["displacementBias"] as GLUniform)["value"] = material.DisplacementBias;

            }

        }
        public void RefreshMaterialUniforms(GLUniforms m_uniforms, Material material, float pixelRatio, float height)
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

                RefreshUniformsPoints(m_uniforms, (PointsMaterial)material, pixelRatio, height);

            }
            else if (material is SpriteMaterial)
            {

                RefreshUniformsSprites(m_uniforms, material);

            }
            else if (material is ShadowMaterial)
            {

                (m_uniforms["color"] as GLUniform)["value"] = (material as ShadowMaterial).Color;
                (m_uniforms["opacity"] as GLUniform)["value"] = (material as ShadowMaterial).Opacity;

            }
            else if (material is ShaderMaterial)
            {
                var uniforms = (material as ShaderMaterial).Uniforms;
                m_uniforms.Merge(uniforms);
                (material as ShaderMaterial).UniformsNeedUpdate = false; // #15581

            }
        }



        public void RefreshFogUniforms(GLUniforms uniforms, Fog fog)
        {
            (uniforms["fogColor"] as GLUniform)["value"] = fog.Color;

            if (fog is Fog)
            {

                (uniforms["fogNear"] as GLUniform)["value"] = fog.Near;
                (uniforms["fogFar"] as GLUniform)["value"] = fog.Far;

            }
            else if (fog is FogExp2)
            {

                (uniforms["fogDensity"] as GLUniform)["value"] = (fog as FogExp2).Density;

            }

        }
        private void RefreshUniformsLambert(GLUniforms uniforms, MeshLambertMaterial material)
        {
            if (material.EmissiveMap != null)
            {

                (uniforms["emissiveMap"] as GLUniform)["value"] = material.EmissiveMap;

            }
        }

        private void RefreshUniformsToon(GLUniforms uniforms, MeshToonMaterial material)
        {
            (uniforms["specular"] as GLUniform)["value"] = material.Specular;
            (uniforms["shininess"] as GLUniform)["value"] = (float)System.Math.Max(material.Shininess, 1e-4); // to prevent pow( 0.0, 0.0 )

            if (material.GradientMap != null)
            {

                (uniforms["gradientMap"] as GLUniform)["value"] = material.GradientMap;

            }

            if (material.EmissiveMap != null)
            {

                (uniforms["emissiveMap"] as GLUniform)["value"] = material.EmissiveMap;

            }

            if (material.BumpMap != null)
            {

                (uniforms["bumpMap"] as GLUniform)["value"] = material.BumpMap;
                (uniforms["bumpScale"] as GLUniform)["value"] = material.BumpScale;
                if (material.Side == Constants.BackSide)
                {
                    float value = (float)(uniforms["bumpScale"] as GLUniform)["value"];
                    (uniforms["bumpScale"] as GLUniform)["value"] = -1 * value;
                }

            }

            if (material.NormalMap != null)
            {

                (uniforms["normalMap"] as GLUniform)["value"] = material.NormalMap;
                (uniforms["normalScale"] as GLUniform)["value"] = material.NormalScale;
                if (material.Side == Constants.BackSide)
                {
                    Vector2 value = (Vector2)(uniforms["normalScale"] as GLUniform)["value"];
                    (uniforms["normalScale"] as GLUniform)["value"] = value * -1;
                }

            }

            if (material.DisplacementMap != null)
            {

                (uniforms["displacementMap"] as GLUniform)["value"] = material.DisplacementMap;
                (uniforms["displacementScale"] as GLUniform)["value"] = material.DisplacementScale;
                (uniforms["displacementBias"] as GLUniform)["value"] = material.DisplacementBias;

            }
        }

        private void RefreshUniformsPhong(GLUniforms uniforms, MeshPhongMaterial material)
        {
            (uniforms["specular"] as GLUniform)["value"] = material.Specular;
            (uniforms["shininess"] as GLUniform)["value"] = (float)System.Math.Max(material.Shininess, 1e-4); // to prevent pow( 0.0, 0.0 )

            if (material.EmissiveMap != null)
            {

                (uniforms["emissiveMap"] as GLUniform)["value"] = material.EmissiveMap;

            }

            if (material.BumpMap != null)
            {

                (uniforms["bumpMap"] as GLUniform)["value"] = material.BumpMap;
                (uniforms["bumpScale"] as GLUniform)["value"] = material.BumpScale;
                if (material.Side == Constants.BackSide)
                {
                    float value = (float)(uniforms["bumpScale"] as GLUniform)["value"];
                    (uniforms["bumpScale"] as GLUniform)["value"] = -1 * value;
                }

            }

            if (material.NormalMap != null)
            {

                (uniforms["normalMap"] as GLUniform)["value"] = material.NormalMap;
                (uniforms["normalScale"] as GLUniform)["value"] = material.NormalScale;
                if (material.Side == Constants.BackSide)
                {
                    Vector2 value = (Vector2)(uniforms["normalScale"] as GLUniform)["value"];
                    (uniforms["normalScale"] as GLUniform)["value"] = value * -1;
                }

            }

            if (material.DisplacementMap != null)
            {

                (uniforms["displacementMap"] as GLUniform)["value"] = material.DisplacementMap;
                (uniforms["displacementScale"] as GLUniform)["value"] = material.DisplacementScale;
                (uniforms["displacementBias"] as GLUniform)["value"] = material.DisplacementBias;

            }
        }

        private void RefreshUniformsPhysical(GLUniforms uniforms, MeshPhysicalMaterial material)
        {
            RefreshUniformsStandard(uniforms, material);

            (uniforms["reflectivity"] as GLUniform)["value"] = material.Reflectivity; // also part of uniforms common

            (uniforms["clearcoat"] as GLUniform)["value"] = material.Clearcoat;
            (uniforms["clearcoatRoughness"] as GLUniform)["value"] = material.ClearcoatRoughness;
            if (material.Sheen != null) (uniforms["sheen"] as GLUniform)["value"] = material.Sheen;

            if (material.ClearcoatNormalMap != null)
            {

                (uniforms["clearcoatNormalScale"] as GLUniform)["value"] = material.ClearcoatNormalScale;
                (uniforms["clearcoatNormalMap"] as GLUniform)["value"] = material.ClearcoatNormalMap;

                if (material.Side == Constants.BackSide)
                {
                    Vector2 value = (Vector2)(uniforms["clearcoatNormalScale"] as GLUniform)["value"];
                    (uniforms["clearcoatNormalScale"] as GLUniform)["value"] = value * -1;

                }

            }

            (uniforms["transparency"] as GLUniform)["value"] = material.Transparency;
        }

        private void RefreshUniformsStandard(GLUniforms uniforms, MeshStandardMaterial material)
        {
            (uniforms["roughness"] as GLUniform)["value"] = material.Roughness;
            (uniforms["metalness"] as GLUniform)["value"] = material.Metalness;

            if (material.RoughnessMap != null)
            {

                (uniforms["roughnessMap"] as GLUniform)["value"] = material.RoughnessMap;

            }

            if (material.MetalnessMap != null)
            {

                (uniforms["metalnessMap"] as GLUniform)["value"] = material.MetalnessMap;

            }

            if (material.EmissiveMap != null)
            {

                (uniforms["emissiveMap"] as GLUniform)["value"] = material.EmissiveMap;

            }

            if (material.BumpMap != null)
            {

                (uniforms["bumpMap"] as GLUniform)["value"] = material.BumpMap;
                (uniforms["bumpScale"] as GLUniform)["value"] = material.BumpScale;
                if (material.Side == Constants.BackSide)
                {
                    float value = (float)(uniforms["bumpScale"] as GLUniform)["value"];
                    (uniforms["bumpScale"] as GLUniform)["value"] = -1 * value;
                }

            }

            if (material.NormalMap != null)
            {

                (uniforms["normalMap"] as GLUniform)["value"] = material.NormalMap;
                (uniforms["normalScale"] as GLUniform)["value"] = material.NormalScale;
                if (material.Side == Constants.BackSide)
                {
                    Vector2 value = (Vector2)(uniforms["normalScale"] as GLUniform)["value"];
                    (uniforms["normalScale"] as GLUniform)["value"] = value * -1;
                }

            }

            if (material.DisplacementMap != null)
            {

                (uniforms["displacementMap"] as GLUniform)["value"] = material.DisplacementMap;
                (uniforms["displacementScale"] as GLUniform)["value"] = material.DisplacementScale;
                (uniforms["displacementBias"] as GLUniform)["value"] = material.DisplacementBias;

            }

            if (material.EnvMap != null)
            {

                //(uniforms["envMap"] as GLUniform)["value"] = material.envMap; // part of uniforms common
                (uniforms["envMapIntensity"] as GLUniform)["value"] = material.EnvMapIntensity;

            }
        }

        private void RefreshUniformsMatcap(GLUniforms uniforms, MeshMatcapMaterial material)
        {
            if (material.Matcap != null)
            {

                (uniforms["matcap"] as GLUniform)["value"] = material.Matcap;

            }

            if (material.BumpMap != null)
            {

                (uniforms["bumpMap"] as GLUniform)["value"] = material.BumpMap;
                (uniforms["bumpScale"] as GLUniform)["value"] = material.BumpScale;
                if (material.Side == Constants.BackSide)
                {
                    float value = (float)(uniforms["bumpScale"] as GLUniform)["value"];
                    (uniforms["bumpScale"] as GLUniform)["value"] = -1 * value;
                }

            }

            if (material.NormalMap != null)
            {

                (uniforms["normalMap"] as GLUniform)["value"] = material.NormalMap;
                (uniforms["normalScale"] as GLUniform)["value"] = material.NormalScale;
                if (material.Side == Constants.BackSide)
                {
                    Vector2 value = (Vector2)(uniforms["normalScale"] as GLUniform)["value"];
                    (uniforms["normalScale"] as GLUniform)["value"] = value * -1;
                }


            }

            if (material.DisplacementMap != null)
            {

                (uniforms["displacementMap"] as GLUniform)["value"] = material.DisplacementMap;
                (uniforms["displacementScale"] as GLUniform)["value"] = material.DisplacementScale;
                (uniforms["displacementBias"] as GLUniform)["value"] = material.DisplacementBias;

            }

        }

        private void RefreshUniformsDepth(GLUniforms uniforms, MeshDepthMaterial material)
        {
            if (material.DisplacementMap != null)
            {

                (uniforms["displacementMap"] as GLUniform)["value"] = material.DisplacementMap;
                (uniforms["displacementScale"] as GLUniform)["value"] = material.DisplacementScale;
                (uniforms["displacementBias"] as GLUniform)["value"] = material.DisplacementBias;

            }
        }

        private void RefreshUniformsDistance(GLUniforms uniforms, MeshDistanceMaterial material)
        {
            if (material.DisplacementMap != null)
            {
                (uniforms["displacementMap"] as GLUniform)["value"] = material.DisplacementMap;
                (uniforms["displacementScale"] as GLUniform)["value"] = material.DisplacementScale;
                (uniforms["displacementBias"] as GLUniform)["value"] = material.DisplacementBias;
            }

            (uniforms["referencePosition"] as GLUniform)["value"] = material.ReferencePosition;
            (uniforms["nearDistance"] as GLUniform)["value"] = material.NearDistance;
            (uniforms["farDistance"] as GLUniform)["value"] = material.FarDistance;

        }

        private void RefreshUniformsCommon(GLUniforms uniforms, Material material)
        {
            (uniforms["opacity"] as GLUniform)["value"] = material.Opacity;

            if (material.Color != null)
            {

                (uniforms["diffuse"] as GLUniform)["value"] = material.Color;

            }

            if (material.Emissive != null)
            {

                Color emissiveColor = material.Emissive.Value;
                (uniforms["emissive"] as GLUniform)["value"] = emissiveColor.MultiplyScalar(material.EmissiveIntensity);

            }

            if (material.Map != null)
            {

                (uniforms["map"] as GLUniform)["value"] = material.Map;

            }

            if (material.AlphaMap != null)
            {

                (uniforms["alphaMap"] as GLUniform)["value"] = material.AlphaMap;

            }

            if (material.SpecularMap != null)
            {

                (uniforms["specularMap"] as GLUniform)["value"] = material.SpecularMap;

            }

            if (material.EnvMap != null)
            {

                (uniforms["envMap"] as GLUniform)["value"] = material.EnvMap;

                // don't flip CubeTexture envMaps, flip everything else:
                //  WebGLRenderTargetCube will be flipped for backwards compatibility
                //  WebGLRenderTargetCube.texture will be flipped because it's a Texture and NOT a CubeTexture
                // this check must be handled differently, or removed entirely, if WebGLRenderTargetCube uses a CubeTexture in the future
                (uniforms["flipEnvMap"] as GLUniform)["value"] = material.EnvMap is CubeTexture ? -1.0f : 1.0f;

                (uniforms["reflectivity"] as GLUniform)["value"] = material.Reflectivity;
                (uniforms["refractionRatio"] as GLUniform)["value"] = material.RefractionRatio;

                (uniforms["maxMipLevel"] as GLUniform)["value"] = (properties.Get(material.EnvMap) as Hashtable)["maxMipLevel"];

            }

            if (material.LightMap != null)
            {

                (uniforms["lightMap"] as GLUniform)["value"] = material.LightMap;
                (uniforms["lightMapIntensity"] as GLUniform)["value"] = material.LightMapIntensity;

            }

            if (material.AoMap != null)
            {

                (uniforms["aoMap"] as GLUniform)["value"] = material.AoMap;
                (uniforms["aoMapIntensity"] as GLUniform)["value"] = material.AoMapIntensity;

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

                (uniforms["uvTransform"] as GLUniform)["value"] = uvScaleMap.Matrix;

            }
        }



        private void RefreshUniformsLine(GLUniforms uniforms, LineBasicMaterial material)
        {
            (uniforms["diffuse"] as GLUniform)["value"] = material.Color;
            (uniforms["opacity"] as GLUniform)["value"] = material.Opacity;
        }

        private void RefreshUniformsDash(GLUniforms uniforms, LineDashedMaterial material)
        {
            (uniforms["dashSize"] as GLUniform)["value"] = material.DashSize;
            (uniforms["totalSize"] as GLUniform)["value"] = material.DashSize + material.GapSize;
            (uniforms["scale"] as GLUniform)["value"] = material.Scale;

        }

        private void RefreshUniformsPoints(GLUniforms uniforms, PointsMaterial material, float _pixelRatio, float height)
        {
            (uniforms["diffuse"] as GLUniform)["value"] = material.Color;
            (uniforms["opacity"] as GLUniform)["value"] = material.Opacity;
            (uniforms["size"] as GLUniform)["value"] = material.Size * _pixelRatio;
            (uniforms["scale"] as GLUniform)["value"] = height * 0.5f;

            if (material.Map != null)
            {

                (uniforms["map"] as GLUniform)["value"] = material.Map;

            }

            if (material.AlphaMap != null)
            {

                (uniforms["alphaMap"] as GLUniform)["value"] = material.AlphaMap;

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
                    (uniforms["uvTransform"] as GLUniform)["value"] = new Matrix3();
                }
                (((uniforms["uvTransform"] as GLUniform)["value"]) as Matrix3).Copy(uvScaleMap.Matrix);

            }
        }

        private void RefreshUniformsSprites(GLUniforms uniforms, Material material)
        {

            (uniforms["diffuse"] as GLUniform)["value"] = material.Color;
            (uniforms["opacity"] as GLUniform)["value"] = material.Opacity;
            (uniforms["rotation"] as GLUniform)["value"] = material.Rotation;

            if (material.Map != null)
            {

                (uniforms["map"] as GLUniform)["value"] = material.Map;

            }

            if (material.AlphaMap != null)
            {

                (uniforms["alphaMap"] as GLUniform)["value"] = material.AlphaMap;

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

                (uniforms["uvTransform"] as GLUniform)["value"] = uvScaleMap.Matrix;

            }

        }
    }
}
