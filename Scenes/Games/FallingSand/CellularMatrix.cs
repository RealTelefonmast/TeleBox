using Microsoft.VisualBasic.CompilerServices;
using TeleBox.Engine.Utility;
using TeleBox.Scenes.Games.FallingSand.Data.Materials;
using TeleBox.Scenes.Materials;
using TeleBox.UI;

namespace TeleBox.Scenes;

public sealed class CellularMatrix
{
    private readonly Material[,] _matrix;

    public CellularMatrix(int width, int height)
    {
        Width = width;
        Height = height;

        _matrix = new Material[Width, Height];
    }

    public event EventHandler<MatrixUpdatedEventArgs> MatrixUpdated;

    public int Width { get; }
    public int Height { get; }

    public Material this[int i, int j]
    {
        get => _matrix[i, j];
        private set => _matrix[i, j] = value;
    }
    
    public void Initialize()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                this[i, j] = new Material(this, MaterialType.Empty);
            }
        }
    }

    public void Clear()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                ChangeMaterial(i, j, MaterialType.Empty);
            }
        }
    }

    public void ChangeMaterial(int x, int y, MaterialType type)
    {
        if (IsWithihBounds(x, y))
        {
            this[x, y].ChangeType(type);
        }
    }

    public bool IsWithihBounds(int i, int j)
    {
        return i >= 0 && i < Width && j >= 0 && j < Height;
    }

    public bool IsFree(int i, int j)
    {
        return IsWithihBounds(i, j) && this[i, j].Type == MaterialType.Empty;
    }

    public bool IsOccupied(int i, int j)
    {
        return IsWithihBounds(i, j) && this[i, j].Type != MaterialType.Empty;
    }

    public bool IsWater(int i, int j)
    {
        return IsWithihBounds(i, j) && this[i, j].Type == MaterialType.Water;
    }

    public bool IsFire(int i, int j)
    {
        return IsWithihBounds(i, j) && this[i, j].Type == MaterialType.Fire;
    }

    public bool IsSmoke(int i, int j)
    {
        return IsWithihBounds(i, j) && this[i, j].Type == MaterialType.Smoke;
    }

    public bool IsSteam(int i, int j)
    {
        return IsWithihBounds(i, j) && this[i, j].Type == MaterialType.Steam;
    }

    public bool IsOil(int i, int j)
    {
        return IsWithihBounds(i, j) && this[i, j].Type == MaterialType.Oil;
    }

    public bool IsAcid(int i, int j)
    {
        return IsWithihBounds(i, j) && this[i, j].Type == MaterialType.Acid;
    }

    public bool IsLava(int i, int j)
    {
        return IsWithihBounds(i, j) && this[i, j].Type == MaterialType.Lava;
    }

    public bool IsAsh(int i, int j)
    {
        return IsWithihBounds(i, j) && this[i, j].Type == MaterialType.Ash;
    }

    public bool IsLiquid(int i, int j)
    {
        return IsWithihBounds(i, j) && MaterialProperties.Properties[this[i, j].Type].HasFlag(MaterialAttributes.Liquid);
    }

    public bool IsMovableSolid(int i, int j)
    {
        return IsWithihBounds(i, j) &&
               (this[i, j].Type == MaterialType.Sand ||
                this[i, j].Type == MaterialType.Coal ||
                this[i, j].Type == MaterialType.Ash ||
                this[i, j].Type == MaterialType.Ember ||
                this[i, j].Type == MaterialType.Fire ||
                this[i, j].Type == MaterialType.Virus);
    }

    public bool IsLiquidNearby(int i, int j, out int iNew, out int jNew)
    {
        var h = Rand.NextBoolean() ? -1 : 1;
        var v = Rand.NextBoolean() ? -1 : 1;

        if (IsLiquid(i + h, j - v))
        {
            iNew = i + h;
            jNew = j - v;

            return true;
        }

        if (IsLiquid(i - h, j + v))
        {
            iNew = i - h;
            jNew = j + v;

            return true;
        }

        if (IsLiquid(i - h, j - v))
        {
            iNew = i - h;
            jNew = j - v;

            return true;
        }

        if (IsLiquid(i + h, j + v))
        {
            iNew = i + h;
            jNew = j + v;

            return true;
        }

        if (IsLiquid(i + h, j))
        {
            iNew = i + h;
            jNew = j;

            return true;
        }

        if (IsLiquid(i, j + v))
        {
            iNew = i;
            jNew = j + v;

            return true;
        }

        if (IsLiquid(i - h, j))
        {
            iNew = i - h;
            jNew = j;

            return true;
        }

        if (IsLiquid(i, j - v))
        {
            iNew = i;
            jNew = j - v;

            return true;
        }

        iNew = i;
        jNew = j;

        return false;
    }

    public bool IsElementNearby(int i, int j, MaterialType type, out int iNew, out int jNew)
    {
        var h = Rand.NextBoolean() ? 1 : -1;
        var v = Rand.NextBoolean() ? -1 : 1;

        if (IsWithihBounds(i + h, j + v) && this[i + h, j + v].Type == type)
        {
            iNew = i + h;
            jNew = j + v;

            return true;
        }

        if (IsWithihBounds(i - h, j - v) && this[i - h, j - v].Type == type)
        {
            iNew = i - h;
            jNew = j - v;

            return true;
        }

        if (IsWithihBounds(i + h, j - v) && this[i + h, j - v].Type == type)
        {
            iNew = i + h;
            jNew = j - v;

            return true;
        }

        if (IsWithihBounds(i - h, j + v) && this[i - h, j + v].Type == type)
        {
            iNew = i - h;
            jNew = j + v;

            return true;
        }

        if (IsWithihBounds(i + h, j) && this[i + h, j].Type == type)
        {
            iNew = i + h;
            jNew = j;

            return true;
        }

        if (IsWithihBounds(i, j + v) && this[i, j + v].Type == type)
        {
            iNew = i;
            jNew = j + v;

            return true;
        }

        if (IsWithihBounds(i - h, j) && this[i - h, j].Type == type)
        {
            iNew = i - h;
            jNew = j;

            return true;
        }

        if (IsWithihBounds(i, j - v) && this[i, j - v].Type == type)
        {
            iNew = i;
            jNew = j - v;

            return true;
        }

        iNew = i;
        jNew = j;

        return false;
    }

    public bool AnyElementNearby(int i, int j, out int iNew, out int jNew)
    {
        var h = Rand.NextBoolean() ? 1 : -1;
        var v = Rand.NextBoolean() ? -1 : 1;

        if (IsOccupied(i + h, j + v))
        {
            iNew = i + h;
            jNew = j + v;

            return true;
        }

        if (IsOccupied(i - h, j - v))
        {
            iNew = i - h;
            jNew = j - v;

            return true;
        }

        if (IsOccupied(i + h, j - v))
        {
            iNew = i + h;
            jNew = j - v;

            return true;
        }

        if (IsOccupied(i - h, j + v))
        {
            iNew = i - h;
            jNew = j + v;

            return true;
        }

        if (IsOccupied(i + h, j))
        {
            iNew = i + h;
            jNew = j;

            return true;
        }

        if (IsOccupied(i, j + v))
        {
            iNew = i;
            jNew = j + v;

            return true;
        }

        if (IsOccupied(i - h, j))
        {
            iNew = i - h;
            jNew = j;

            return true;
        }

        if (IsOccupied(i, j - v))
        {
            iNew = i;
            jNew = j - v;

            return true;
        }

        iNew = i;
        jNew = j;

        return false;
    }

    public bool IsCompletelySurrounded(int i, int j)
    {
        var h = Rand.NextBoolean() ? -1 : 1;
        var v = Rand.NextBoolean() ? -1 : 1;

        if (IsWithihBounds(i + h, j + v) && this[i + h, j + v].Type == MaterialType.Empty)
        {
            return false;
        }

        if (IsWithihBounds(i - h, j - v) && this[i - h, j - v].Type == MaterialType.Empty)
        {
            return false;
        }

        if (IsWithihBounds(i + h, j - v) && this[i + h, j - v].Type == MaterialType.Empty)
        {
            return false;
        }

        if (IsWithihBounds(i - h, j + v) && this[i - h, j + v].Type == MaterialType.Empty)
        {
            return false;
        }

        if (IsWithihBounds(i + h, j) && this[i + h, j].Type == MaterialType.Empty)
        {
            return false;
        }

        if (IsWithihBounds(i, j + v) && this[i, j + v].Type == MaterialType.Empty)
        {
            return false;
        }

        if (IsWithihBounds(i - h, j) && this[i - h, j].Type == MaterialType.Empty)
        {
            return false;
        }

        if (IsWithihBounds(i, j - v) && this[i, j - v].Type == MaterialType.Empty)
        {
            return false;
        }

        return true;
    }

    public int CountNeighborElements(int i, int j, MaterialType type)
    {
        var count = 0;

        var h = Rand.NextBoolean() ? -1 : 1;
        var v = Rand.NextBoolean() ? -1 : 1;

        if (IsWithihBounds(i + h, j + v) && this[i + h, j + v].Type == type)
        {
            count += 1;
        }

        if (IsWithihBounds(i - h, j - v) && this[i - h, j - v].Type == type)
        {
            count += 1;
        }

        if (IsWithihBounds(i + h, j - v) && this[i + h, j - v].Type == type)
        {
            count += 1;
        }

        if (IsWithihBounds(i - h, j + v) && this[i - h, j + v].Type == type)
        {
            count += 1;
        }

        if (IsWithihBounds(i + h, j) && this[i + h, j].Type == type)
        {
            count += 1;
        }

        if (IsWithihBounds(i, j + v) && this[i, j + v].Type == type)
        {
            count += 1;
        }

        if (IsWithihBounds(i - h, j) && this[i - h, j].Type == type)
        {
            count += 1;
        }

        if (IsWithihBounds(i, j - v) && this[i, j - v].Type == type)
        {
            count += 1;
        }

        return count;
    }

    public void Swap(int i1, int j1, int i2, int j2)
    {
        (this[i1, j1], this[i2, j2]) = (this[i2, j2], this[i1, j1]);

        MatrixUpdated?.Invoke(this, new MatrixUpdatedEventArgs(i1, j1, this[i1, j1].Color));
        MatrixUpdated?.Invoke(this, new MatrixUpdatedEventArgs(i2, j2, this[i2, j2].Color));
    }

    public void StepAll()
    {
        for (int j = Height - 1; j >= 0; j--)
        {
            for (int i = 0; i < Width; i++)
            {
                this[i, j].Step(i, j);
            }
        }
    }

    public void Add(MaterialType type, int i, int j, Vector2 velocity)
    {
        if (type == MaterialType.Empty)
        {
            return;
        }

        if (IsFree(i, j))
        {
            ChangeMaterial(i, j, type);
            this[i, j].Velocity = velocity;

            MatrixUpdated?.Invoke(this, new MatrixUpdatedEventArgs(i, j, this[i, j].Color));
        }
    }

    public void Add(MaterialType type, int i, int j)
    {
        if (type == MaterialType.Empty)
        {
            return;
        }

        if (IsFree(i, j))
        {
            ChangeMaterial(i, j, type);
            this[i, j].Velocity = Vector2.Zero;

            MatrixUpdated?.Invoke(this, new MatrixUpdatedEventArgs(i, j, this[i, j].Color));
        }
    }

    public void Erase(int i, int j)
    {
        if (IsWithihBounds(i, j))
        {
            ChangeMaterial(i, j, MaterialType.Empty);
            MatrixUpdated?.Invoke(this, new MatrixUpdatedEventArgs(i, j, this[i, j].Color));
        }
    }

    public void Set(MaterialType type, int i, int j)
    {
        if (IsWithihBounds(i, j))
        {
            ChangeMaterial(i, j, type);
            MatrixUpdated?.Invoke(this, new MatrixUpdatedEventArgs(i, j, this[i, j].Color));
        }
    }

    public void ResetFrame()
    {
    }

    public MaterialType[,] GetMatrix()
    {
        var result = new MaterialType[Width, Height];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                result[i, j] = this[i, j].Type;
            }
        }

        return result;
    }
}