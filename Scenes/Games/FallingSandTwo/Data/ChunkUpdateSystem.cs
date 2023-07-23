namespace TeleBox.Scenes.Games.FallingSandTwo.Data;

public class ChunkUpdateSystem
{
    private List<PixelRect> _dirtyRects;
    
    public ChunkUpdateSystem()
    {
        _dirtyRects = new List<PixelRect>();
    }

    public void Update(Particle[] particles, int width, int height)
    {
        //Last Pass - Set Dirty Rects

        PixelRect currentRect = new PixelRect();
        
        /*for (uint x = 0; x < width; x++)
        {
            for(uint y = 0; y < height; y++)
            {
                var index = GridUtils.Index(x, y, width);
                var particle = particles[index];
                if (particle.hasBeenUpdated)
                {
                    if (x < currentRect.x && y < currentRect.y)
                    {
                        currentRect.x = x;
                        currentRect.y = y;
                    }

                    if (x > currentRect.xMax && y > currentRect.yMax)
                    {
                        currentRect.width = x - currentRect.x;
                        currentRect.height = y - currentRect.y;
                    }
                }

                particle.hasBeenUpdated = false;


            }
        }*/
    }
}