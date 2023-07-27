using SFML.Graphics;
using TeleBox.Engine.Data.Primitive;
using TeleBox.UI;
using TerraFX.Interop.Windows;

namespace TeleBox;

public struct RenderArgs
{
    public RenderTarget Target { get; set; }
    public RenderStates States { get; set; }
    public Rect Rect { get; set; }
}

public abstract class Scene : Drawable
{
    public abstract string Name { get; }
    public abstract void Update(float delta);
    
    public virtual void SceneOpened()
    {
        
    }
    
    public virtual void SceneClosed()
    {
        
    }

    public virtual void HandleEvents(TEvent args)
    {
        
    }

    public virtual void DrawOnGUI()
    {
        
    }
    
    public void Draw(RenderTarget target, RenderStates states)
    {
        Draw(new RenderArgs
        {
            Target = target,
            States = states,
            Rect = UIRoot.UIRect,
        });
    }

    public virtual void Draw(RenderArgs args)
    {
        
    }
}