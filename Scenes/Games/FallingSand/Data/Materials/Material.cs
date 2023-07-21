using Microsoft.VisualBasic.CompilerServices;
using TeleBox.Engine.Utility;
using TeleBox.Scenes.Materials;
using TeleBox.UI;
using Color = SFML.Graphics.Color;

namespace TeleBox.Scenes.Games.FallingSand.Data.Materials;

  public sealed class Material
    {
        private readonly List<IntVec2> _trajectory;

        public Material(CellularMatrix matrix, MaterialType type)
        {
            _trajectory = new List<IntVec2>();
            Matrix = matrix;
            Type = type;
            LifeTime = 0;
            Color = MaterialDB.GetColor(Type, LifeTime);
            Velocity = new Vector2(0, 0);
            IsMovable = false;
            SpreadRate = 0;

            switch (Type)
            {
                case MaterialType.Empty:
                case MaterialType.Stone:
                case MaterialType.Wood:
                case MaterialType.Titan:
                case MaterialType.Obsidian:
                case MaterialType.Ice:
                case MaterialType.Plant:
                    break;

                case MaterialType.Ash:
                case MaterialType.Fire:
                case MaterialType.Steam:
                case MaterialType.Ember:
                case MaterialType.Coal:
                case MaterialType.Dirt:
                case MaterialType.Seed:
                case MaterialType.Virus:
                    Velocity = new Vector2(0, MaterialConstants.Gravity);
                    IsMovable = true;
                    break;

                case MaterialType.Smoke:
                    Velocity = new Vector2(0, -MaterialConstants.Gravity);
                    IsMovable = true;
                    break;

                case MaterialType.Sand:
                    Velocity = new Vector2(0, MaterialConstants.Gravity);
                    IsMovable = true;
                    SpreadRate = MaterialConstants.SandSpreadRate;
                    break;

                case MaterialType.Water:
                    Velocity = new Vector2(0, MaterialConstants.Gravity);
                    IsMovable = true;
                    SpreadRate = MaterialConstants.WaterSpreadRate;
                    break;

                case MaterialType.Oil:
                    Velocity = new Vector2(0, MaterialConstants.Gravity);
                    IsMovable = true;
                    SpreadRate = MaterialConstants.OilSpreadRate;
                    break;

                case MaterialType.Acid:
                    Velocity = new Vector2(0, MaterialConstants.Gravity);
                    IsMovable = true;
                    SpreadRate = MaterialConstants.AcidSpreadRate;
                    break;

                case MaterialType.Lava:
                    Velocity = new Vector2(0, MaterialConstants.Gravity);
                    IsMovable = true;
                    SpreadRate = MaterialConstants.LavaSpreadRate;
                    break;

                case MaterialType.Methane:
                case MaterialType.BurningGas:
                    Velocity = new Vector2(0, -MaterialConstants.Gravity);
                    IsMovable = true;
                    SpreadRate = MaterialConstants.MethaneSpreadRate;
                    break;
            }
        }

        public CellularMatrix Matrix { get; }
        public MaterialType Type { get; private set; }
        public int LifeTime { get; private set; }
        public Color Color { get; private set; }
        public Vector2 Velocity { get; set; }
        public bool IsMovable { get; private set; }
        public int SpreadRate { get; private set; }

        public void ChangeType(MaterialType newType)
        {
            Type = newType;
            LifeTime = 0;
            Color = MaterialDB.GetColor(Type, LifeTime);

            switch (Type)
            {
                case MaterialType.Empty:
                case MaterialType.Stone:
                case MaterialType.Wood:
                case MaterialType.Titan:
                case MaterialType.Obsidian:
                case MaterialType.Ice:
                case MaterialType.Plant:
                    Velocity = new Vector2(0, 0);
                    IsMovable = false;
                    SpreadRate = 0;
                    break;

                case MaterialType.Sand:
                    Velocity = new Vector2(0, MaterialConstants.Gravity);
                    IsMovable = true;
                    SpreadRate = MaterialConstants.SandSpreadRate;
                    break;

                case MaterialType.Water:
                    Velocity = new Vector2(0, MaterialConstants.Gravity);
                    IsMovable = true;
                    SpreadRate = MaterialConstants.WaterSpreadRate;
                    break;

                case MaterialType.Oil:
                    Velocity = new Vector2(0, MaterialConstants.Gravity);
                    IsMovable = true;
                    SpreadRate = MaterialConstants.OilSpreadRate;
                    break;

                case MaterialType.Acid:
                    Velocity = new Vector2(0, MaterialConstants.Gravity);
                    IsMovable = true;
                    SpreadRate = MaterialConstants.AcidSpreadRate;
                    break;

                case MaterialType.Lava:
                    Velocity = new Vector2(0, MaterialConstants.Gravity);
                    IsMovable = true;
                    SpreadRate = MaterialConstants.LavaSpreadRate;
                    break;

                case MaterialType.Fire:
                case MaterialType.Virus:
                    Velocity = new Vector2(0, MaterialConstants.Gravity);
                    IsMovable = true;
                    break;

                case MaterialType.Steam:
                case MaterialType.Smoke:
                    Velocity = new Vector2(0, -MaterialConstants.Gravity);
                    IsMovable = true;
                    SpreadRate = 0;
                    break;

                case MaterialType.Ember:
                case MaterialType.Coal:
                case MaterialType.Ash:
                case MaterialType.Dirt:
                case MaterialType.Seed:
                    Velocity = new Vector2(0, MaterialConstants.Gravity);
                    IsMovable = true;
                    SpreadRate = 0;
                    break;

                case MaterialType.Methane:
                case MaterialType.BurningGas:
                    Velocity = new Vector2(0, -MaterialConstants.Gravity);
                    IsMovable = true;
                    SpreadRate = MaterialConstants.MethaneSpreadRate;
                    break;
            }
        }

        public void Step(int i, int j)
        {
            if (Type == MaterialType.Empty)
            {
                return;
            }

            LifeTime += 1;
            if (LifeTime == int.MaxValue)
            {
                LifeTime = 0;
            }

            // if (Matrix.IsCellUpdatedThisFrame(i, j))
            // {
            //     return;
            // }

            if (IsMovable)
            {
                Velocity *= new Vector2(0.9f, 0);

                if (Type == MaterialType.Steam ||
                    Type == MaterialType.Smoke)
                {
                    Velocity += new Vector2(0,MaterialConstants.GasGravity);
                }
                else 
                if (Type == MaterialType.Methane ||
                    Type == MaterialType.BurningGas)
                {
                    Velocity += new Vector2(0,MaterialConstants.MethaneGravity);
                }
                else
                {
                    Velocity += new Vector2(0,MaterialConstants.Gravity);
                }
            }

            switch (Type)
            {
                case MaterialType.Sand:
                    UpdateSand(i, j);
                    break;

                case MaterialType.Water:
                    UpdateWater(i, j);
                    break;

                case MaterialType.Oil:
                    UpdateOil(i, j);
                    break;

                case MaterialType.Fire:
                    UpdateFire(i, j);
                    break;

                case MaterialType.Steam:
                    UpdateSteam(i, j);
                    break;

                case MaterialType.Smoke:
                    UpdateSmoke(i, j);
                    break;

                case MaterialType.Ember:
                    UpdateEmber(i, j);
                    break;

                case MaterialType.Coal:
                    UpdateCoal(i, j);
                    break;

                case MaterialType.Acid:
                    UpdateAcid(i, j);
                    break;

                case MaterialType.Lava:
                    UpdateLava(i, j);
                    break;

                case MaterialType.Ash:
                    UpdateAsh(i, j);
                    break;

                case MaterialType.Methane:
                    UpdateMethane(i, j);
                    break;

                case MaterialType.BurningGas:
                    UpdateBurningGas(i, j);
                    break;

                case MaterialType.Ice:
                    UpdateIce(i, j);
                    break;

                case MaterialType.Dirt:
                    UpdateDirt(i, j);
                    break;

                case MaterialType.Seed:
                    UpdateSeed(i, j);
                    break;

                case MaterialType.Plant:
                    UpdatePlant(i, j);
                    break;

                case MaterialType.Virus:
                    UpdateVirus(i, j);
                    break;
            }

            //Matrix.UpdateCell(i, j);
        }

        private void CalculateTrajectory(int x0, int y0, int x1, int y1)
        {
            _trajectory.Clear();

            var dx = Math.Abs(x1 - x0);
            var dy = Math.Abs(y1 - y0);
            var sx = (x0 < x1) ? 1 : -1;
            var sy = (y0 < y1) ? 1 : -1;
            var err = dx - dy;

            while (true)
            {
                _trajectory.Add(new IntVec2(x0, y0));

                if ((x0 == x1) && (y0 == y1))
                {
                    break;
                }

                var e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy; 
                    x0 += sx;
                }

                if (e2 < dx)
                {
                    err += dx; 
                    y0 += sy;
                }
            }
        }

        private void UpdateSand(int i, int j)
        {
            if (float.IsInfinity(Velocity.x) || float.IsNaN(Velocity.x))
            {
                Velocity = new Vector2(0, Velocity.y);
            }
            if(float.IsInfinity(Velocity.y) || float.IsNaN(Velocity.y))
            {
                Velocity = new Vector2(Velocity.x, 0);
            }
            
            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);
            
            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsWater(point.x, point.y) ||
                    Matrix.IsAcid(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var spreadRate = Rand.NextBoolean() ? SpreadRate : -SpreadRate;

            CalculateTrajectory(i, j, i + spreadRate, j + 1);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsWater(point.x, point.y) ||
                    Matrix.IsAcid(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var random = Rand.Next(0, 100);
            var direction = random < 50 ? Direction.Right : Direction.Left;
            var cellBelow = new IntVec2(direction == Direction.Right ? (i + 1) : direction == Direction.Left ? (i - 1) : i, j + 1);

            if (Matrix.IsFree(cellBelow.x, cellBelow.y) ||
                Matrix.IsWater(cellBelow.x, cellBelow.y) ||
                Matrix.IsAcid(cellBelow.x, cellBelow.y))
            {
                Matrix.Swap(cellBelow.x, cellBelow.y, i, j);

                return;
            }

            if (Matrix.IsLiquidNearby(i, j, out int iLiquid, out int jLiquid) &&
                Rand.Range(0, 10) == 0)
            {
                Matrix.Swap(iLiquid, jLiquid, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdateWater(int i, int j)
        {
            if (Matrix.IsElementNearby(i, j, MaterialType.Plant, out _, out _) &&
                Rand.Range(0, MaterialConstants.PlantGrowthChance) == 0)
            {
                Matrix.Erase(i, j);
                Matrix.Add(MaterialType.Plant, i, j);

                return;
            }

            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);

            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsOil(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var spreadRate = Matrix.IsFree(i + 1, j) || Matrix.IsOil(i + 1, j) || Matrix.IsAcid(i + 1, j) ? SpreadRate : -SpreadRate;

            CalculateTrajectory(i, j, i + spreadRate, j + 1);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsOil(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            CalculateTrajectory(i, j, i + spreadRate, j);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsOil(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Water, out int iLiquid, out int jLiquid) &&
                Rand.Range(0, 10) == 0)
            {
                Matrix.Swap(iLiquid, jLiquid, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdateOil(int i, int j)
        {
            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);

            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var spreadRate = Matrix.IsFree(i + 1, j) || Matrix.IsAcid(i + 1, j) || Matrix.IsWater(i + 1, j) ? SpreadRate : -SpreadRate;

            CalculateTrajectory(i, j, i + spreadRate, j + 1);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            CalculateTrajectory(i, j, i + spreadRate, j);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Oil, out int iLiquid, out int jLiquid) &&
                Rand.Range(0, 20) == 0)
            {
                Matrix.Swap(iLiquid, jLiquid, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdateFire(int i, int j)
        {
            Color = MaterialDB.GetColor(Type, LifeTime);

            if (LifeTime > MaterialConstants.FireLifeTime)
            {
                Matrix.Erase(i, j);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Water, out int iWater, out int jWater))
            {
                var steamRegionHeight = Rand.Range(-MaterialConstants.SteamRegionHeight, MaterialConstants.SteamRegionHeight);
                var steamRegionWidth = Rand.Range(-MaterialConstants.SteamRegionWidth, MaterialConstants.SteamRegionWidth);
                var r1 = Rand.NextBoolean();
                var r2 = Rand.NextBoolean();
                for (int n = r1 ? steamRegionHeight : -steamRegionHeight; r1 ? n < MaterialConstants.SteamRegionHeight : n > MaterialConstants.SteamRegionHeight; n += r1 ? 1 : -1)
                {
                    for (int m = r2 ? steamRegionWidth : -steamRegionWidth; r2 ? m < MaterialConstants.SteamRegionWidth : m > MaterialConstants.SteamRegionWidth; m += r2 ? 1 : -1)
                    {
                        Matrix.Add(MaterialType.Steam, i + m, j + n);
                    }
                }

                Matrix.Erase(i, j);
                Matrix.Erase(iWater, jWater);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Acid, out int iAcid, out int jAcid))
            {
                var smokeRegionHeight = Rand.Range(-MaterialConstants.SmokeRegionWidth, MaterialConstants.SmokeRegionHeight);
                var smokeRegionWidth = Rand.Range(-MaterialConstants.SmokeRegionWidth, MaterialConstants.SmokeRegionHeight);
                var r1 = Rand.NextBoolean();
                var r2 = Rand.NextBoolean();
                for (int n = r1 ? smokeRegionHeight : -smokeRegionHeight; r1 ? n < MaterialConstants.SmokeRegionHeight : n > MaterialConstants.SmokeRegionHeight; n += r1 ? 1 : -1)
                {
                    for (int m = r2 ? smokeRegionWidth : -smokeRegionWidth; r2 ? m < MaterialConstants.SmokeRegionWidth : m > MaterialConstants.SmokeRegionWidth; m += r2 ? 1 : -1)
                    {
                        Matrix.Add(MaterialType.Smoke, i + m, j + n);
                    }
                }

                Matrix.Erase(i, j);
                Matrix.Erase(iAcid, jAcid);

                return;
            }

            if (Matrix.IsFire(i, j + 1) &&
                Matrix.IsFree(i, j - 1))
            {
                if (Rand.Range(0, 10) == 0)
                {
                    var r = Rand.NextBoolean();
                    var randomHorizontal = Rand.Range(-10, -1);
                    for (int n = randomHorizontal; n < 0; n++)
                    {
                        for (int m = r ? -SpreadRate : SpreadRate; r ? m < SpreadRate : m > -SpreadRate; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                Matrix.Swap(i + m, j + n, i, j);

                                return;
                            }
                        }
                    }
                }
            }

            if (Rand.Range(0, MaterialConstants.SmokeSpawnChance) == 0)
            {
                if (Matrix.IsFree(i, j - 1))
                {
                    Matrix.Add(MaterialType.Smoke, i, j - 1);
                }
            }

            if (Rand.Range(0, MaterialConstants.SmokeSpawnChance) == 0)
            {
                if (Matrix.IsFree(i + 1, j - 1))
                {
                    Matrix.Add(MaterialType.Smoke, i + 1, j - 1);
                }
            }

            if (Rand.Range(0, MaterialConstants.SmokeSpawnChance) == 0)
            {
                if (Matrix.IsFree(i - 1, j - 1))
                {
                    Matrix.Add(MaterialType.Smoke, i - 1, j - 1);
                }
            }

            if (Rand.Range(0, MaterialConstants.EmberSpawnChance) == 0 &&
                LifeTime < MaterialConstants.FireLifeTime / 3)
            {
                if (Matrix.IsFree(i, j - 1))
                {
                    Matrix.Add(MaterialType.Ember, i, j - 1, new Vector2(Rand.Range(-2, 2), Rand.Range(-5, 0)));
                }
            }

            if (Rand.Range(0, MaterialConstants.EmberSpawnChance) == 0 &&
                LifeTime < MaterialConstants.FireLifeTime / 3)
            {
                if (Matrix.IsFree(i + 1, j - 1))
                {
                    Matrix.Add(MaterialType.Ember, i + 1, j - 1, new Vector2(Rand.Range(-2, 2), Rand.Range(-5, 0)));
                }
            }

            if (Rand.Range(0, MaterialConstants.EmberSpawnChance) == 0 &&
                LifeTime < MaterialConstants.FireLifeTime / 3)
            {
                if (Matrix.IsFree(i - 1, j - 1))
                {
                    Matrix.Add(MaterialType.Ember, i - 1, j - 1, new Vector2(Rand.Range(-2, 2), Rand.Range(-5, 0)));
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Ice, out int iIce, out int jIce) &&
                Rand.Range(0, MaterialConstants.IceMeltsFromHeatChance) == 0)
            {
                LifeTime += 20;
                Matrix.Erase(iIce, jIce);
                Matrix.Add(MaterialType.Water, iIce, jIce);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Oil, out int iOil, out int jOil) &&
                Rand.Range(0, MaterialConstants.OilIgnitionChance) == 0)
            {
                Matrix.Erase(iOil, jOil);
                Matrix.Add(MaterialType.Fire, iOil, jOil);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Coal, out int iCoal, out int jCoal) &&
                Rand.Range(0, MaterialConstants.CoalIgnitionChance) == 0)
            {
                Matrix.Erase(iCoal, jCoal);
                Matrix.Add(MaterialType.Fire, iCoal, jCoal);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Wood, out int iWood, out int jWood) &&
                Rand.Range(0, MaterialConstants.WoodIgnitionChance) == 0)
            {
                Matrix.Erase(iWood, jWood);
                Matrix.Add(MaterialType.Fire, iWood, jWood);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Plant, out int iPlant, out int jPlant) &&
                Rand.Range(0, MaterialConstants.PlantIgnitionChance) == 0)
            {
                Matrix.Erase(iPlant, jPlant);
                Matrix.Add(MaterialType.Fire, iPlant, jPlant);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Seed, out int iSeed, out int jSeed) &&
                Rand.Range(0, MaterialConstants.PlantIgnitionChance) == 0)
            {
                Matrix.Erase(iSeed, jSeed);
                Matrix.Add(MaterialType.Fire, iSeed, jSeed);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Methane, out int iMethane, out int jMethane) &&
                Rand.Range(0, MaterialConstants.MethaneIgnitionChance) == 0)
            {
                Matrix.Erase(iMethane, jMethane);
                Matrix.Add(MaterialType.BurningGas, iMethane, jMethane);

                if (Rand.Range(0, MaterialConstants.BurningGasSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);

            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsFire(point.x, point.y) ||
                    Matrix.IsSmoke(point.x, point.y) ||
                    Matrix.IsSteam(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var random = Rand.Next(0, 100);
            var horizontalDirection = random < 50 ? Direction.Right : Direction.Left;
            random = Rand.Next(0, 100);
            var verticalDirection = random < 50 ? Direction.Up : Direction.Down;

            var neighborCell = new IntVec2(
                horizontalDirection == Direction.Right ? (i + 1) : horizontalDirection == Direction.Left ? (i - 1) : i,
                verticalDirection == Direction.Down ? (j + 1) : verticalDirection == Direction.Up ? (j - 1) : j);

            if (Matrix.IsFree(neighborCell.x, neighborCell.y) ||
                Matrix.IsSmoke(neighborCell.x, neighborCell.y) ||
                Matrix.IsWater(neighborCell.x, neighborCell.y) ||
                Matrix.IsSteam(neighborCell.x, neighborCell.y))
            {
                Matrix.Swap(neighborCell.x, neighborCell.y, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdateSteam(int i, int j)
        {
            Color = MaterialDB.GetColor(Type, LifeTime);

            if (LifeTime > MaterialConstants.SteamLifeTime)
            {
                Matrix.Erase(i, j);

                return;
            }

            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);

            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsLiquid(point.x, point.y) ||
                    Matrix.IsLava(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                    
                Velocity += new Vector2(Rand.NextBoolean() ? -MaterialConstants.SteamSpreadSpeed : MaterialConstants.SteamSpreadSpeed,0);
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var random = Rand.Next(0, 100);
            var direction = random < 50 ? Direction.Right : Direction.Left;
            var cellAbove = new IntVec2(direction == Direction.Right ? (i + 1) : direction == Direction.Left ? (i - 1) : i, j - 1);

            if (Matrix.IsFree(cellAbove.x, cellAbove.y) ||
                Matrix.IsLiquid(cellAbove.x, cellAbove.y) ||
                Matrix.IsLava(cellAbove.x, cellAbove.y))
            {
                Velocity += new Vector2(direction == Direction.Left ? -MaterialConstants.SteamSpreadSpeed : MaterialConstants.SteamSpreadSpeed, 0);
                Matrix.Swap(cellAbove.x, cellAbove.y, i, j);

                return;
            }

            if (Matrix.CountNeighborElements(i, j, MaterialType.Steam) >= 8 &&
                Rand.Range(0, MaterialConstants.SteamCondencesChance) == 0)
            {
                Matrix.Erase(i, j);
                Matrix.Add(MaterialType.Water, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdateSmoke(int i, int j)
        {
            Color = MaterialDB.GetColor(Type, LifeTime);

            if (LifeTime > MaterialConstants.SmokeLifeTime)
            {
                Matrix.Erase(i, j);

                return;
            }

            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);

            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsLiquid(point.x, point.y) ||
                    Matrix.IsLava(point.x, point.y) ||
                    Matrix.IsAsh(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Velocity += new Vector2(Rand.NextBoolean() ? -MaterialConstants.SmokeSpreadSpeed : MaterialConstants.SmokeSpreadSpeed, 0);
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var random = Rand.Next(0, 100);
            var direction = random < 50 ? Direction.Right : Direction.Left;
            var cellAbove = new IntVec2(direction == Direction.Right ? (i + 1) : direction == Direction.Left ? (i - 1) : i, j - 1);

            if (Matrix.IsFree(cellAbove.x, cellAbove.y) ||
                Matrix.IsLiquid(cellAbove.x, cellAbove.y) ||
                Matrix.IsLava(cellAbove.x, cellAbove.y) ||
                Matrix.IsAsh(cellAbove.x, cellAbove.y))
            {
                Velocity += new Vector2(direction == Direction.Left ? -MaterialConstants.SmokeSpreadSpeed : MaterialConstants.SmokeSpreadSpeed, 0);
                Matrix.Swap(cellAbove.x, cellAbove.y, i, j);

                return;
            }

            if (Matrix.CountNeighborElements(i, j, MaterialType.Smoke) >= 8 &&
                Rand.Range(0, MaterialConstants.SmokeCondencesChance) == 0)
            {
                Matrix.Erase(i, j);
                Matrix.Add(MaterialType.Ash, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdateEmber(int i, int j)
        {
            if (LifeTime > MaterialConstants.EmberLifeTime)
            {
                Matrix.Erase(i, j);

                if (Rand.Range(0, MaterialConstants.AshSpawnChance) == 0)
                {
                    Matrix.Add(MaterialType.Ash, i, j);
                }

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Water, out int iWater, out int jWater))
            {
                Matrix.Erase(iWater, jWater);
                Matrix.Erase(i, j);
                Matrix.Add(MaterialType.Steam, i, j);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Acid, out int iAcid, out int jAcid))
            {
                Matrix.Erase(iAcid, jAcid);
                Matrix.Erase(i, j);
                Matrix.Add(MaterialType.Smoke, i, j);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Ice, out int iIce, out int jIce) &&
                Rand.Range(0, MaterialConstants.IceMeltsFromHeatChance) == 0)
            {
                LifeTime += 20;
                Matrix.Erase(iIce, jIce);
                Matrix.Add(MaterialType.Water, iIce, jIce);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Wood, out int iWood, out int jWood) &&
                Rand.Range(0, MaterialConstants.WoodIgnitionChance) == 0)
            {
                Matrix.Erase(iWood, jWood);
                Matrix.Add(MaterialType.Fire, iWood, jWood);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Coal, out int iCoal, out int jCoal) &&
                Rand.Range(0, MaterialConstants.CoalIgnitionChance) == 0)
            {
                Matrix.Erase(iCoal, jCoal);
                Matrix.Add(MaterialType.Fire, iCoal, jCoal);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Oil, out int iOil, out int jOil) &&
                Rand.Range(0, MaterialConstants.OilIgnitionChance) == 0)
            {
                Matrix.Erase(iOil, jOil);
                Matrix.Add(MaterialType.Fire, iOil, jOil);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Plant, out int iPlant, out int jPlant) &&
                Rand.Range(0, MaterialConstants.PlantIgnitionChance) == 0)
            {
                Matrix.Erase(iPlant, jPlant);
                Matrix.Add(MaterialType.Fire, iPlant, jPlant);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Seed, out int iSeed, out int jSeed) &&
                Rand.Range(0, MaterialConstants.PlantIgnitionChance) == 0)
            {
                Matrix.Erase(iSeed, jSeed);
                Matrix.Add(MaterialType.Fire, iSeed, jSeed);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Methane, out int iMethane, out int jMethane) &&
                Rand.Range(0, MaterialConstants.MethaneIgnitionChance) == 0)
            {
                Matrix.Erase(iMethane, jMethane);
                Matrix.Add(MaterialType.BurningGas, iMethane, jMethane);
            }

            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);

            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsWater(point.x, point.y) ||
                    Matrix.IsFire(point.x, point.y) ||
                    Matrix.IsSmoke(point.x, point.y) ||
                    Matrix.IsSteam(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var random = Rand.Next(0, 100);
            var direction = random < 50 ? Direction.Right : Direction.Left;
            var cellAbove = new IntVec2(direction == Direction.Right ? (i + 1) : direction == Direction.Left ? (i - 1) : i, j - 1);

            if (Matrix.IsFree(cellAbove.x, cellAbove.y) ||
                Matrix.IsWater(cellAbove.x, cellAbove.y) ||
                Matrix.IsFire(cellAbove.x, cellAbove.y) ||
                Matrix.IsSmoke(cellAbove.x, cellAbove.y) ||
                Matrix.IsSteam(cellAbove.x, cellAbove.y))
            {
                Matrix.Swap(cellAbove.x, cellAbove.y, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdateCoal(int i, int j)
        {
            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);

            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsWater(point.x, point.y) ||
                    Matrix.IsOil(point.x, point.y) ||
                    Matrix.IsAcid(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            if (Matrix.IsLiquidNearby(i, j, out int iLiquid, out int jLiquid) &&
                Rand.Range(0, 10) == 0)
            {
                Matrix.Swap(iLiquid, jLiquid, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdateAcid(int i, int j)
        {
            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);

            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var spreadRate = Matrix.IsFree(i + 1, j) || Matrix.IsOil(i + 1, j) || Matrix.IsWater(i + 1, j) ? SpreadRate : -SpreadRate;

            CalculateTrajectory(i, j, i + spreadRate, j + 1);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            CalculateTrajectory(i, j, i + spreadRate, j);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Water, out _, out _) &&
                Rand.Range(0, MaterialConstants.AcidDissolvesInWaterChance) == 0)
            {
                Matrix.Erase(i, j);
                Matrix.Add(MaterialType.Water, i, j);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Stone, out int iStone, out int jStone) &&
                Rand.Range(0, MaterialConstants.AcidMeltsStoneChance) == 0)
            {
                Matrix.Erase(iStone, jStone);
                Matrix.Swap(i, j, iStone, jStone);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Wood, out int iWood, out int jWood) &&
                Rand.Range(0, MaterialConstants.AcidMeltsWoodChance) == 0)
            {
                Matrix.Erase(iWood, jWood);
                Matrix.Swap(i, j, iWood, jWood);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Dirt, out int iDirt, out int jDirt) &&
                Rand.Range(0, MaterialConstants.AcidMakesSandFromDirtChance) == 0)
            {
                Matrix.Erase(iDirt, jDirt);
                Matrix.Add(MaterialType.Sand, iDirt, jDirt);
                Matrix.Swap(i, j, iDirt, jDirt);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Plant, out int iPlant, out int jPlant) &&
                Rand.Range(0, MaterialConstants.AcidMeltsPlantChance) == 0)
            {
                Matrix.Erase(iPlant, jPlant);
                Matrix.Swap(i, j, iPlant, jPlant);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Ash, out int iAsh, out int jAsh) &&
                Rand.Range(0, MaterialConstants.AcidMeltsAshChance) == 0)
            {
                Matrix.Erase(iAsh, jAsh);
                Matrix.Swap(i, j, iAsh, jAsh);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Obsidian, out int iObsidian, out int jObsidian) &&
                Rand.Range(0, MaterialConstants.AcidMeltsObsidianChance) == 0)
            {
                Matrix.Erase(iObsidian, jObsidian);
                Matrix.Swap(i, j, iObsidian, jObsidian);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Ice, out int iIce, out int jIce) &&
                Rand.Range(0, MaterialConstants.AcidMeltsIceChance) == 0)
            {
                Matrix.Erase(iIce, jIce);
                Matrix.Add(MaterialType.Water, iIce, jIce);
                Matrix.Swap(i, j, iIce, jIce);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Oil, out int iOil, out int jOil) &&
                Rand.Range(0, MaterialConstants.AcidReactsWithOilChance) == 0)
            {
                Matrix.Erase(iOil, jOil);
                Matrix.Add(MaterialType.Coal, iOil, jOil);
                Matrix.Swap(i, j, iOil, jOil);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Acid, out int iLiquid, out int jLiquid) &&
                Rand.Range(0, 10) == 0)
            {
                Matrix.Swap(iLiquid, jLiquid, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdateLava(int i, int j)
        {
            if (LifeTime > MaterialConstants.LavaLifeTime)
            {
                Matrix.Erase(i, j);
                Matrix.Add(MaterialType.Obsidian, i, j);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Water, out int iWater, out int jWater) && 
                Rand.Range(0, 1) == 0)
            {
                var regionHeight = Rand.Range(-MaterialConstants.SteamRegionHeight, MaterialConstants.SteamRegionHeight);
                var regionWidth = Rand.Range(-MaterialConstants.SteamRegionWidth, MaterialConstants.SteamRegionWidth);
                var r1 = Rand.NextBoolean();
                var r2 = Rand.NextBoolean();
                for (int n = r1 ? regionHeight : -regionHeight; r1 ? n < MaterialConstants.SteamRegionHeight : n > MaterialConstants.SteamRegionHeight; n += r1 ? 1 : -1)
                {
                    for (int m = r2 ? regionWidth : -regionWidth; r2 ? m < MaterialConstants.SteamRegionWidth : m > MaterialConstants.SteamRegionWidth; m += r2 ? 1 : -1)
                    {
                        Matrix.Add(MaterialType.Steam, i + m, j + n);
                    }
                }

                Matrix.Erase(i, j);
                Matrix.Add(MaterialType.Steam, i, j);
                Matrix.Erase(iWater, jWater);
                Matrix.Add(MaterialType.Stone, iWater, jWater);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Acid, out int iAcid, out int jAcid) &&
                Rand.Range(0, 1) == 0)
            {
                var regionHeight = Rand.Range(-MaterialConstants.SmokeRegionHeight, MaterialConstants.SmokeRegionHeight);
                var regionWidth = Rand.Range(-MaterialConstants.SmokeRegionWidth, MaterialConstants.SmokeRegionWidth);
                var r1 = Rand.NextBoolean();
                var r2 = Rand.NextBoolean();
                for (int n = r1 ? regionHeight : -regionHeight; r1 ? n < MaterialConstants.SmokeRegionHeight : n > MaterialConstants.SmokeRegionHeight; n += r1 ? 1 : -1)
                {
                    for (int m = r2 ? regionWidth : -regionWidth; r2 ? m < MaterialConstants.SmokeRegionWidth : m > MaterialConstants.SmokeRegionWidth; m += r2 ? 1 : -1)
                    {
                        Matrix.Add(MaterialType.Smoke, i + m, j + n);
                    }
                }

                Matrix.Erase(i, j);
                Matrix.Add(MaterialType.Smoke, i, j);
                Matrix.Erase(iAcid, jAcid);
                Matrix.Add(MaterialType.Stone, iAcid, jAcid);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Ice, out int iIce, out int jIce) &&
                Rand.Range(0, MaterialConstants.IceMeltsFromHeatChance) == 0)
            {
                LifeTime += 100;
                Matrix.Erase(iIce, jIce);
                Matrix.Add(MaterialType.Water, iIce, jIce);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Stone, out int iStone, out int jStone) &&
                Rand.Range(0, MaterialConstants.LavaMeltsStoneChance) == 0)
            {
                LifeTime += 50;
                Matrix.Erase(iStone, jStone);
                Matrix.Add(MaterialType.Lava, iStone, jStone);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Sand, out int iSand, out int jSand) &&
                Rand.Range(0, MaterialConstants.LavaMeltsSandChance) == 0)
            {
                LifeTime += 20;
                Matrix.Erase(iSand, jSand);
                Matrix.Add(MaterialType.Obsidian, iSand, jSand);
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Dirt, out int iDirt, out int jDirt) &&
                Rand.Range(0, MaterialConstants.LavaMeltsDirtChance) == 0)
            {
                LifeTime += 30;
                Matrix.Erase(iDirt, jDirt);
                Matrix.Add(MaterialType.Obsidian, iDirt, jDirt);
            }

            for (int k = 0; k < Rand.Range(1, 10); k++)
            {
                if (Rand.Range(0, MaterialConstants.LavaSpawnsSmokeChance) == 0)
                {
                    if (Matrix.IsFree(i, j - 1))
                    {
                        Matrix.Add(MaterialType.Smoke, i, j - 1);
                    }
                }

                if (Rand.Range(0, MaterialConstants.LavaSpawnsSmokeChance) == 0)
                {
                    if (Matrix.IsFree(i + 1, j - 1))
                    {
                        Matrix.Add(MaterialType.Smoke, i + 1, j - 1);
                    }
                }

                if (Rand.Range(0, MaterialConstants.LavaSpawnsSmokeChance) == 0)
                {
                    if (Matrix.IsFree(i - 1, j - 1))
                    {
                        Matrix.Add(MaterialType.Smoke, i - 1, j - 1);
                    }
                }
            }
            
            if (Rand.Range(0, MaterialConstants.LavaSpawnsEmberChance) == 0 &&
                LifeTime < MaterialConstants.LavaLifeTime / 3)
            {
                if (Matrix.IsFree(i, j - 1))
                {
                    Matrix.Add(MaterialType.Ember, i, j - 1, new Vector2(0, Rand.Range(-10, 0)));
                }
            }

            if (Rand.Range(0, MaterialConstants.LavaSpawnsEmberChance) == 0 &&
                LifeTime < MaterialConstants.LavaLifeTime / 3)
            {
                if (Matrix.IsFree(i + 1, j - 1))
                {
                    Matrix.Add(MaterialType.Ember, i + 1, j - 1, new Vector2(0, Rand.Range(-10, 0)));
                }
            }

            if (Rand.Range(0, MaterialConstants.LavaSpawnsEmberChance) == 0 &&
                LifeTime < MaterialConstants.LavaLifeTime / 3)
            {
                if (Matrix.IsFree(i - 1, j - 1))
                {
                    Matrix.Add(MaterialType.Ember, i - 1, j - 1, new Vector2(0, Rand.Range(-10, 0)));
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Oil, out int iOil, out int jOil) &&
                Rand.Range(0, MaterialConstants.OilIgnitionChance) == 0)
            {
                Matrix.Erase(iOil, jOil);
                Matrix.Add(MaterialType.Fire, iOil, jOil);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Coal, out int iCoal, out int jCoal) &&
                Rand.Range(0, MaterialConstants.CoalIgnitionChance) == 0)
            {
                Matrix.Erase(iCoal, jCoal);
                Matrix.Add(MaterialType.Fire, iCoal, jCoal);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Wood, out int iWood, out int jWood) &&
                Rand.Range(0, MaterialConstants.WoodIgnitionChance) == 0)
            {
                Matrix.Erase(iWood, jWood);
                Matrix.Add(MaterialType.Fire, iWood, jWood);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Methane, out int iMethane, out int jMethane) &&
                Rand.Range(0, MaterialConstants.MethaneIgnitionChance) == 0)
            {
                Matrix.Erase(iMethane, jMethane);
                Matrix.Add(MaterialType.BurningGas, iMethane, jMethane);

                if (Rand.Range(0, MaterialConstants.BurningGasSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Plant, out int iPlant, out int jPlant) &&
                Rand.Range(0, MaterialConstants.PlantIgnitionChance) == 0)
            {
                Matrix.Erase(iPlant, jPlant);
                Matrix.Add(MaterialType.Fire, iPlant, jPlant);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Seed, out int iSeed, out int jSeed) &&
                Rand.Range(0, MaterialConstants.PlantIgnitionChance) == 0)
            {
                Matrix.Erase(iSeed, jSeed);
                Matrix.Add(MaterialType.Fire, iSeed, jSeed);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);

            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var spreadRate = Rand.NextBoolean() ? SpreadRate : -SpreadRate;

            CalculateTrajectory(i, j, i + spreadRate, j);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsOil(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Lava, out int iNew, out int jNew) &&
                Rand.Range(0, 40) == 0)
            {
                Matrix.Swap(iNew, jNew, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdateAsh(int i, int j)
        {
            var random = Rand.Next(0, 100);
            var direction = random < 50 ? Direction.Right : Direction.Left;
            var cellBelow = new IntVec2(direction == Direction.Right ? (i + 1) : direction == Direction.Left ? (i - 1) : i, j + 1);

            if (Matrix.IsFree(cellBelow.x, cellBelow.y))
            {
                Matrix.Swap(cellBelow.x, cellBelow.y, i, j);

                return;
            }

            if (Matrix.IsLiquidNearby(i, j, out int iLiquid, out int jLiquid) &&
                Rand.Range(0, 1) == 0)
            {
                Matrix.Swap(iLiquid, jLiquid, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdateMethane(int i, int j)
        {
            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);

            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsSteam(point.x, point.y) ||
                    Matrix.IsSmoke(point.x, point.y) ||
                    Matrix.IsLiquid(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var spreadRate = Rand.NextBoolean() ? SpreadRate : -SpreadRate;

            CalculateTrajectory(i, j, i + spreadRate, j);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsSteam(point.x, point.y) ||
                    Matrix.IsSmoke(point.x, point.y) ||
                    Matrix.IsLiquid(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Methane, out int iNew, out int jNew) &&
                Rand.Range(0, 1) == 0)
            {
                Matrix.Swap(iNew, jNew, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdateBurningGas(int i, int j)
        {
            Color = MaterialDB.GetColor(Type, LifeTime);

            if (LifeTime > MaterialConstants.BurningGasLifeTime)
            {
                Matrix.Erase(i, j);

                if (Rand.Range(0, MaterialConstants.SteamSpawnChance) == 0)
                {
                    Matrix.Add(MaterialType.Steam, i, j);
                }

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Oil, out int iOil, out int jOil) &&
                Rand.Range(0, MaterialConstants.OilIgnitionChance) == 0)
            {
                Matrix.Erase(iOil, jOil);
                Matrix.Add(MaterialType.Fire, iOil, jOil);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Coal, out int iCoal, out int jCoal) &&
                Rand.Range(0, MaterialConstants.CoalIgnitionChance) == 0)
            {
                Matrix.Erase(iCoal, jCoal);
                Matrix.Add(MaterialType.Fire, iCoal, jCoal);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Wood, out int iWood, out int jWood) &&
                Rand.Range(0, MaterialConstants.WoodIgnitionChance) == 0)
            {
                Matrix.Erase(iWood, jWood);
                Matrix.Add(MaterialType.Fire, iWood, jWood);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Plant, out int iPlant, out int jPlant) &&
                Rand.Range(0, MaterialConstants.PlantIgnitionChance) == 0)
            {
                Matrix.Erase(iPlant, jPlant);
                Matrix.Add(MaterialType.Fire, iPlant, jPlant);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Seed, out int iSeed, out int jSeed) &&
                Rand.Range(0, MaterialConstants.PlantIgnitionChance) == 0)
            {
                Matrix.Erase(iSeed, jSeed);
                Matrix.Add(MaterialType.Fire, iSeed, jSeed);

                if (Rand.Range(0, MaterialConstants.FireSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Methane, out int iMethane, out int jMethane) &&
                Rand.Range(0, MaterialConstants.MethaneIgnitionChance) == 0)
            {
                Matrix.Erase(iMethane, jMethane);
                Matrix.Add(MaterialType.BurningGas, iMethane, jMethane);

                if (Rand.Range(0, MaterialConstants.BurningGasSpreadChance) == 0)
                {
                    var r = Rand.NextBoolean();
                    for (var n = -3; n < 2; n++)
                    {
                        for (var m = r ? -3 : 2; r ? m < 2 : m > -3; m += r ? 1 : -1)
                        {
                            if (Matrix.IsFree(i + m, j + n))
                            {
                                LifeTime += 5;
                                Matrix.Swap(i + m, j + n, i, j);

                                break;
                            }
                        }
                    }
                }
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Ice, out int iIce, out int jIce) &&
                Rand.Range(0, MaterialConstants.IceMeltsFromHeatChance) == 0)
            {
                LifeTime += 20;
                Matrix.Erase(iIce, jIce);
                Matrix.Add(MaterialType.Water, iIce, jIce);
            }

            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);

            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsSteam(point.x, point.y) ||
                    Matrix.IsSmoke(point.x, point.y) ||
                    Matrix.IsLiquid(point.x, point.y) ||
                    Matrix.IsMovableSolid(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var spreadRate = Rand.NextBoolean() ? SpreadRate : -SpreadRate;

            CalculateTrajectory(i, j, i + spreadRate, j);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsSteam(point.x, point.y) ||
                    Matrix.IsSmoke(point.x, point.y) ||
                    Matrix.IsLiquid(point.x, point.y) ||
                    Matrix.IsMovableSolid(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Methane, out int iNew, out int jNew) &&
                Rand.Range(0, 1) == 0)
            {
                Matrix.Swap(iNew, jNew, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdateIce(int i, int j)
        {
            if (Matrix.IsElementNearby(i, j, MaterialType.Water, out int iNew, out int jNew) &&
                Rand.Range(0, MaterialConstants.IceFreezesWaterChance) == 0)
            {
                Matrix.Erase(iNew, jNew);
                Matrix.Add(MaterialType.Ice, iNew, jNew);

                return;
            }
        }

        private void UpdateDirt(int i, int j)
        {
            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);

            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsLiquid(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var random = Rand.Next(0, 100);
            var direction = random < 50 ? Direction.Right : Direction.Left;
            var cellBelow = new IntVec2(direction == Direction.Right ? (i + 1) : direction == Direction.Left ? (i - 1) : i, j + 1);

            if (Matrix.IsFree(cellBelow.x, cellBelow.y) ||
                Matrix.IsLiquid(cellBelow.x, cellBelow.y))
            {
                Matrix.Swap(cellBelow.x, cellBelow.y, i, j);

                return;
            }

            if (Matrix.IsLiquidNearby(i, j, out int iLiquid, out int jLiquid) &&
                Rand.Range(0, 10) == 0)
            {
                Matrix.Swap(iLiquid, jLiquid, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdateSeed(int i, int j)
        {
            if (Matrix.IsElementNearby(i, j, MaterialType.Dirt, out _, out _) &&
                Rand.Range(0, MaterialConstants.PlantGrowthChance) == 0)
            {
                Matrix.Erase(i, j);
                Matrix.Add(MaterialType.Plant, i, j);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Plant, out _, out _) &&
                Rand.Range(0, MaterialConstants.PlantGrowthChance) == 0)
            {
                Matrix.Erase(i, j);
                Matrix.Add(MaterialType.Plant, i, j);

                return;
            }

            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);

            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y) ||
                    Matrix.IsLiquid(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var random = Rand.Next(0, 100);
            var direction = random < 50 ? Direction.Right : Direction.Left;
            var cellBelow = new IntVec2(direction == Direction.Right ? (i + 1) : direction == Direction.Left ? (i - 1) : i, j + 1);

            if (Matrix.IsFree(cellBelow.x, cellBelow.y) ||
                Matrix.IsLiquid(cellBelow.x, cellBelow.y))
            {
                Matrix.Swap(cellBelow.x, cellBelow.y, i, j);

                return;
            }

            if (Matrix.IsLiquidNearby(i, j, out int iLiquid, out int jLiquid) &&
                Rand.Range(0, 10) == 0)
            {
                Matrix.Swap(iLiquid, jLiquid, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }

        private void UpdatePlant(int i, int j)
        {
            if (Matrix.IsElementNearby(i, j, MaterialType.Empty, out int iSpace, out int jSpace) &&
                Rand.Range(0, MaterialConstants.PlantSpontaneousGrowthChance) == 0)
            {
                Matrix.Add(MaterialType.Plant, iSpace, jSpace);

                return;
            }

            var neighboursCount = Matrix.CountNeighborElements(i, j, MaterialType.Plant);

            if (neighboursCount < 2)
            {
                Matrix.Erase(i, j);

                return;   
            }

            if (neighboursCount > 3)
            {
                Matrix.Erase(i, j);

                return;
            }

            if (Matrix.IsElementNearby(i, j, MaterialType.Empty, out int iSpace2, out int jSpace2) &&
                Rand.Range(0, 1) == 0)
            {
                Matrix.Add(MaterialType.Plant, iSpace2, jSpace2);
            }
        }

        private void UpdateVirus(int i, int j)
        {
            Color = MaterialDB.GetColor(Type, LifeTime);

            if (LifeTime > MaterialConstants.VirusLifeTime)
            {
                Matrix.Erase(i, j);
                Matrix.Add(MaterialType.BurningGas, i, j);

                return;
            }

            if (Matrix.AnyElementNearby(i, j, out int iNew, out int jNew) &&
                Matrix[iNew, jNew].Type != MaterialType.Virus &&
                Matrix[iNew, jNew].Type != MaterialType.BurningGas &&
                Rand.Range(0, MaterialConstants.VirusDevourChance) == 0)
            {
                LifeTime += 1;
                Matrix.Erase(iNew, jNew);
                Matrix.Add(MaterialType.Virus, iNew, jNew);

                return;
            }

            var vX = (int)(i + Velocity.x);
            var vY = (int)(j + Velocity.y);

            IntVec2? validPoint = null;

            CalculateTrajectory(i, j, vX, vY);
            for (int number = 0; number < _trajectory.Count; number++)
            {
                var point = _trajectory[number];

                if (point.x == i &&
                    point.y == j)
                {
                    continue;
                }

                if (Matrix.IsFree(point.x, point.y))
                {
                    validPoint = point;
                }
                else
                {
                    break;
                }
            }

            if (validPoint.HasValue)
            {
                Matrix.Swap(validPoint.Value.x, validPoint.Value.y, i, j);

                return;
            }

            var random = Rand.Next(0, 100);
            var direction = random < 50 ? Direction.Right : Direction.Left;
            var cellBelow = new IntVec2(direction == Direction.Right ? (i + 1) : direction == Direction.Left ? (i - 1) : i, j + 1);

            if (Matrix.IsFree(cellBelow.x, cellBelow.y))
            {
                Matrix.Swap(cellBelow.x, cellBelow.y, i, j);

                return;
            }

            Velocity /= new Vector2(0, 2.0f);
        }
    }