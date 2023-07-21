using System.Diagnostics;
using Microsoft.VisualBasic.CompilerServices;
using SFML.Graphics;
using SFML.Window;
using TeleBox.Engine.Data.Graphics;
using TeleBox.Engine.Utility;
using TeleBox.Scenes.Materials;
using TeleBox.UI;

namespace TeleBox.Scenes.Games.FallingSand;

public sealed class World : Drawable
{
    private readonly RWTexture _canvas;
    private readonly Sprite _sprite;
    private readonly Shader _shader;
    private readonly CellularMatrix _matrix;
    private float _selectionRadius;
    private DateTime _lastTime;
    private long _framesRendered;
    private int _prevMouseX, _prevMouseY;
    private int _curMouseX, _curMouseY;
    private Mouse.Button _mouseButton;

    public World(int width, int height)
    {
        Width = width;
        Height = height;

        _matrix = new CellularMatrix(Width, Height);
        _matrix.MatrixUpdated += OnMatrixUpdated;

        _canvas = new RWTexture(Width, Height);
        _sprite = new Sprite();
        _sprite.Texture = _canvas.Texture;
        var fragmentShaderFile = Properties.Resources.simpleShader;
        _shader = new Shader(null, null, new MemoryStream(fragmentShaderFile));
        _shader.SetUniform("texture", Shader.CurrentTexture);

        Init();

        IsUsed = false;
    }

    public int Width { get; }
    public int Height { get; }
    public long FPS { get; private set; }
    public bool IsUsed { get; private set; }
    public MaterialType SelectedMaterial { get; private set; }

    public float SelectionRadius
    {
        get => _selectionRadius;
        private set
        {
            if (value > 0 &&
                value <= MaterialConstants.MaxRadius)
            {
                _selectionRadius = value;
            }
        }
    }

    private void OnMatrixUpdated(object sender, MatrixUpdatedEventArgs e)
    {
        _canvas.DrawPoint(e.X, e.Y, e.Color);
    }

    private void Init()
    {
        _canvas.Clear(MaterialDB.GetColor(MaterialType.Empty, 0));
        _matrix.Initialize();
    }

    public void Clear()
    {
        _canvas.Clear(MaterialDB.GetColor(MaterialType.Empty, 0));
        _canvas.Update();
        _matrix.Clear();
    }

    public void Load(MaterialType[,] newMatrix)
    {
        for (int i = 0; i < newMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < newMatrix.GetLength(1); j++)
            {
                _matrix.Set(newMatrix[i, j], i, j);
            }
        }

        _canvas.Update();
    }

    public MaterialType[,] GetMatrix()
    {
        return _matrix.GetMatrix();
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        Widgets.Label($"Test [{Time1}ms] [{Time2}ms]", new Rect(5,5, 100, 24));
        
        if (Shader.IsAvailable)
        {
            states = new RenderStates(states)
            {
                Shader = _shader
            };
            UIRoot.Window.Draw(_sprite, states);
            //target.Draw(_sprite, states);
        }
    }
    
    public  double Time1 { get; set; }
    public  double Time2 { get; set; }

    public void Update()
    {
        Stopwatch watch = new Stopwatch();
        watch.Start();
        _matrix.StepAll();
        Time1 = watch.ElapsedMilliseconds;
        watch.Reset();
        _matrix.ResetFrame();
        _canvas.Update();
        Time2 = watch.ElapsedMilliseconds;

        watch.Stop();
        CountFPS();
    }

    public void StartInput(Mouse.Button button, int x, int y)
    {
        IsUsed = true;

        _mouseButton = button;
        _prevMouseX = _curMouseX - 1;
        _prevMouseY = _curMouseY - 1;
        _curMouseX = x;
        _curMouseY = y;
    }

    public void StopInput()
    {
        IsUsed = false;
    }

    public void Input()
    {
        if (!IsUsed)
        {
            return;
        }

        if (_mouseButton == Mouse.Button.Left &&
            SelectedMaterial != MaterialType.Empty)
        {
            BrushAddMaterial();
        }

        if (_mouseButton == Mouse.Button.Right)
        {
            BrushEraseMaterial();
        }

        _canvas.Update();
    }

    public void UpdateMousePosition(int x, int y)
    {
        _prevMouseX = _curMouseX;
        _prevMouseY = _curMouseY;
        _curMouseX = x;
        _curMouseY = y;
    }

    private void CountFPS()
    {
        _framesRendered++;
        if ((DateTime.Now - _lastTime).TotalSeconds >= 1)
        {
            FPS = _framesRendered;
            _framesRendered = 0;
            _lastTime = DateTime.Now;
        }
    }

    private void BrushAddMaterial()
    {
        var deltaX = _curMouseX - _prevMouseX;
        var deltaY = _curMouseY - _prevMouseY;
        var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

        var numDraws = Convert.ToSingle(distance);
        distance = Convert.ToSingle(Math.Floor(distance));

        for (var i = 0; i < (int)distance; i++)
        {
            var ipos = Vector2.Lerp(new Vector2(_prevMouseX, _prevMouseY), new Vector2(_curMouseX, _curMouseY),
                i / numDraws);

            AddMaterialToWorld((int)ipos.x, (int)ipos.y);
        }
    }

    private void AddMaterialToWorld(int x, int y)
    {
        if (SelectedMaterial == MaterialType.Stone ||
            SelectedMaterial == MaterialType.Wood ||
            SelectedMaterial == MaterialType.Titan ||
            SelectedMaterial == MaterialType.Obsidian ||
            SelectedMaterial == MaterialType.Ice ||
            SelectedMaterial == MaterialType.Plant)
        {
            var R = (int)SelectionRadius;
            for (var i = -R; i <= R; i++)
            {
                for (var j = -R; j <= R; j++)
                {
                    if ((i * i + j * j) <= R * R)
                    {
                        _matrix.Add(SelectedMaterial, x + i, y + j, new Vector2(0, 0));
                    }
                }
            }

            return;
        }

        var count = Rand.Range(1, 1000);
        for (int i = 0; i < count; i++)
        {
            var r = SelectionRadius * Rand.NextDouble();
            var theta = 2.0 * Math.PI * Rand.NextDouble();
            var rx = Math.Cos(theta) * r;
            var ry = Math.Sin(theta) * r;

            var deviation = Rand.Next(0, 100) > 50 ? -2 : 2;
            var velocity = new Vector2(deviation, Rand.Range(-2, 5));

            _matrix.Add(SelectedMaterial, x + (int)rx, y + (int)ry, velocity);
        }
    }

    private void BrushEraseMaterial()
    {
        var deltaX = _curMouseX - _prevMouseX;
        var deltaY = _curMouseY - _prevMouseY;
        var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

        var numDraws = Convert.ToSingle(distance);
        distance = Convert.ToSingle(Math.Floor(distance));

        for (var i = 0; i < (int)distance; i++)
        {
            var ipos = Vector2.Lerp(new Vector2(_prevMouseX, _prevMouseY), new Vector2(_curMouseX, _curMouseY),
                i / numDraws);

            EraseMaterial((int)ipos.x, (int)ipos.y);
        }
    }

    private void EraseMaterial(int x, int y)
    {
        var R = (int)SelectionRadius;
        for (var i = -R; i <= R; i++)
        {
            for (var j = -R; j <= R; j++)
            {
                if ((i * i + j * j) <= R * R)
                {
                    _matrix.Erase(x + i, y + j);
                }
            }
        }
    }

    public void SetMaterial(MaterialType material)
    {
        SelectedMaterial = material;
    }

    public void SetRadius(float radius)
    {
        SelectionRadius = radius;
    }
}