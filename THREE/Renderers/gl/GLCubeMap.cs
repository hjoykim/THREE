using System.Collections;

namespace THREE
{
    public class GLCubeMap
    {
        public GLRenderer renderer;
        public Hashtable Cubemaps = new Hashtable();
        public GLCubeMap(GLRenderer renderer)
        {
            this.renderer = renderer;
        }
        private Texture mapTextureMapping(Texture texture, int mapping )
        {

            if (mapping == Constants.EquirectangularReflectionMapping)
            {

                texture.Mapping = Constants.CubeReflectionMapping;

            }
            else if (mapping == Constants.EquirectangularRefractionMapping)
            {

                texture.Mapping = Constants.CubeRefractionMapping;

            }

            return texture;

        }
        public Texture Get(Texture texture)
        {
            if (texture!=null && texture is Texture)
            {

                var mapping = texture.Mapping;

                if (mapping == Constants.EquirectangularReflectionMapping || mapping == Constants.EquirectangularRefractionMapping)
                {

                    if (Cubemaps.Contains(texture))
                    {

                        var cubemap = Cubemaps[texture] as Texture;
                        return mapTextureMapping(cubemap, texture.Mapping);

                    }
                    else
                    {

                        var image = texture.Image;

                        if (image!=null && image.Height > 0)
                        {

                            var currentRenderList = renderer.GetRenderList();
                            var currentRenderTarget = renderer.GetRenderTarget();
                            var currentRenderState = renderer.GetRenderState();

                            GLCubeRenderTarget renderTarget = new GLCubeRenderTarget(image.Height / 2);
                            renderTarget.FromEquirectangularTexture(renderer, texture);
                            Cubemaps.Add(texture, renderTarget);

                            renderer.SetRenderTarget(currentRenderTarget);
                            renderer.SetRenderList(currentRenderList);
                            renderer.SetRenderState(currentRenderState);

                            return mapTextureMapping(renderTarget.Texture, texture.Mapping);

                        }
                        else
                        {

                            // image not yet ready. try the conversion next frame

                            return null;

                        }

                    }

                }

            }
            return texture;
        }
    }
}
