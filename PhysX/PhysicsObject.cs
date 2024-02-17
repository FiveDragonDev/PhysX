using System.Numerics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PhysX
{
    public class PhysicsObject
    {
        /// <summary>
        /// Name that will printed to log file
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Id of this object
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// The position of the object on the scene [Meters]
        /// </summary>
        public Point Position { get; set; }
        /// <summary>
        /// The rotation of the object on the scene [Degrees]
        /// </summary>
        public Point Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value % 360;
            }
        }
        /// <summary>
        /// The scales of the object on the scene [Meters]
        /// </summary>
        public Point Scale { get; set; } = new Point(1);
        /// <summary>
        /// The velocity of the physics object [Meters/Seconds]
        /// </summary>
        public Point Velocity { get; set; }

        public Bounds Bounds
        {
            set => Scale = value.Scale;
            get => GetBounds();
        }

        public decimal GravitationalAcceleration => Mass /
            (decimal)Math.Pow((double)(Bounds.Width * Bounds.Depth + Bounds.Height), 2);
        public decimal KineticEnergy => 0.5m * Mass * (Velocity * Velocity).Magnitude;
        /// <summary>
        /// The temperature of the physics object [Degrees per Kelvin]
        /// </summary>
        public decimal Temperature
        {
            get => _temperature;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(Temperature));
                _temperature = value;
            }
        }
        /// <summary>
        /// The mass of the physics object [kilograms]
        /// </summary>
        public decimal Mass
        {
            get => _mass;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(Mass));
                _mass = value;
            }
        }
        /// <summary>
        /// The volume of the physics object [meters3]
        /// </summary>
        public decimal Volume
        {
            get => _volume;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(Volume));
                _volume = value;
            }
        }
        /// <summary>
        /// The density of the physics object [kilograms/meters3]
        /// </summary>
        public decimal Density
        {
            get => _density;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(Density));
                _density = value;
            }
        }

        private static int _id;

        private Point _rotation;

        private decimal _mass;
        private decimal _volume;
        private decimal _density;
        private decimal _temperature;

        public PhysicsObject()
        {
            Id = _id++;
            Name = $"object_{Id}";
        }
        public PhysicsObject(string name) : this(name, new()) { }
        public PhysicsObject(string name, Point position)
        {
            Id = _id++;
            Name = name;
            Position = position;
        }

        public static PhysicsObject FromMass(decimal mass, decimal volume)
        {
            return new()
            {
                Mass = mass,
                Density = mass / volume,
                Volume = volume,
            };
        }
        public static PhysicsObject FromDensity(decimal density, decimal volume)
        {
            return new()
            {
                Density = density,
                Mass = density * volume,
                Volume = volume
            };
        }

        /// <summary>
        /// Returns the physical object that encounters this
        /// </summary>
        /// <returns>The physical object that encounters this</returns>
        public PhysicsObject? GetCollision()
        {
            foreach (var other in Enviroment.Objects)
            {
                if (other == this) continue;
                if (Position.X + Bounds.Width < other.Position.X ||
                    Position.X > other.Position.X + other.Bounds.Width ||
                    Position.Y + Bounds.Height < other.Position.Y ||
                    Position.Y > other.Position.Y + other.Bounds.Height ||
                    Position.Z + Bounds.Depth < other.Position.Z ||
                    Position.Z > other.Position.Z + other.Bounds.Depth)
                    continue;
                return other;
            }
            return null;
        }

        /// <summary>
        /// Adds a force to physics object
        /// </summary>
        /// <param name="force">The force you add to a physical object</param>
        /// <param name="forceMode">The force mode you add to a physical object</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void AddForce(Point force, ForceMode forceMode = ForceMode.Acceleration)
        {
            Velocity += forceMode switch
            {
                ForceMode.Impulse => force,
                ForceMode.Acceleration => force / Mass,
                _ => throw new ArgumentOutOfRangeException(forceMode.ToString()),
            };
        }

        public static Point CenterOfMasses(PhysicsObject[] objects)
        {
            decimal totalMass = 0;
            Point totalPoint = new(0);

            foreach (var obj in objects)
            {
                totalMass += obj.Mass;
                totalPoint.X += obj.Position.X * obj.Mass;
                totalPoint.Y += obj.Position.Y * obj.Mass;
                totalPoint.Z += obj.Position.Z * obj.Mass;
            }

            if (totalMass == 0) throw new DivideByZeroException(totalMass.ToString());
            return totalPoint / totalMass;
        }

        public void Tick() => Tick(Enviroment.DeltaTime);
        public void Tick(decimal deltaTime)
        {
            try
            {
                foreach (var otherObject in Enviroment.Objects)
                {
                    if (this == otherObject) continue;
                    Point direction = otherObject.Position - Position;
                    decimal distance = direction.Magnitude;
                    decimal forceMagnitude = Mass * otherObject.Mass /
                        (distance * distance);
                    forceMagnitude *= deltaTime;
                    AddForce(direction.Normalized * forceMagnitude * deltaTime);

                    decimal heatTransfer = Temperature / (distance * distance);
                    heatTransfer *= deltaTime;
                    otherObject.Temperature += heatTransfer;
                    Temperature -= heatTransfer;
                }

                Position += Velocity;
            }
            catch (Exception e)
            {
                Enviroment.Log(e.ToString() + "\n");
            }
        }

        public static PhysicsObject Clone(PhysicsObject @object)
        {
            PhysicsObject temp = new()
            {
                Scale = @object.Scale,
                Density = @object.Density,
                Mass = @object.Mass,
                Temperature = @object.Temperature,
                Volume = @object.Volume,
                Position = @object.Position,
                Rotation = @object.Rotation,
            };
            return temp;
        }
        public static PhysicsObject Clone(PhysicsObject @object, Point position, Point rotation)
        {
            PhysicsObject temp = new()
            {
                Scale = @object.Scale,
                Density = @object.Density,
                Mass = @object.Mass,
                Temperature = @object.Temperature,
                Volume = @object.Volume,
                Position = position,
                Rotation = rotation,
            };
            return temp;
        }

        public static PhysicsObject FromJsonFile(string filename)
        {
            using StreamReader reader = new(filename);
            return FromJson(reader.ReadToEnd());
        }
        public static PhysicsObject FromJson(string jsonData)
        {
            JsonDocumentOptions options = new()
            { AllowTrailingCommas = true };
            JsonNode forecastNode = JsonNode.Parse(jsonData, documentOptions: options)!;

            return new()
            {
                Name = Trim(forecastNode[nameof(Name)]!.ToJsonString()),
                Position = Point.FromJson(forecastNode[nameof(Position)]!.ToJsonString()),
                Rotation = Point.FromJson(forecastNode[nameof(Rotation)]!.ToJsonString()),
                Scale = Point.FromJson(forecastNode[nameof(Scale)]!.ToJsonString()),
                Velocity = Point.FromJson(forecastNode[nameof(Velocity)]!.ToJsonString()),
                Temperature = Convert.ToDecimal(Trim(forecastNode[nameof(Temperature)]!
                .ToJsonString())),
                Mass = Convert.ToDecimal(Trim(forecastNode[nameof(Mass)]!.ToJsonString())),
                Volume = Convert.ToDecimal(Trim(forecastNode[nameof(Volume)]!.ToJsonString())),
                Density = Convert.ToDecimal(Trim(forecastNode[nameof(Density)]!.ToJsonString())),
            };
        }

        public string ToJson()
        {
            string value =
                $"{{\n" +
                $"\t\"ID\":{Id},\n" +
                $"\t\"Name\":\"{Name}\",\n" +
                $"\t\"Position\":\"{Position.ToJson()}\",\n" +
                $"\t\"Rotation\":\"{Rotation.ToJson()}\",\n" +
                $"\t\"Scale\":\"{Scale.ToJson()}\",\n" +
                // $"\t\"Bounds\":\"{Bounds}\",\n" +
                $"\t\"Velocity\":\"{Velocity.ToJson()}\",\n" +
                $"\t\"Temperature\":\"{Temperature}\",\n" +
                $"\t\"Mass\":\"{Mass}\",\n" +
                $"\t\"Volume\":\"{Volume}\",\n" +
                $"\t\"Density\":\"{Density}\",\n" +
                $"}}";
            return value;
        }

        private Bounds GetBounds()
        {
            decimal halfLength = Scale.X / 2;
            decimal halfHeight = Scale.Z / 2;
            decimal halfWidth = Scale.Y / 2;

            Point center = new();
            Point vertexA = new Point(-halfLength, -halfHeight, -halfWidth) + center;
            Point vertexB = new Point(halfLength, -halfHeight, -halfWidth) + center;
            Point vertexC = new Point(halfLength, halfHeight, -halfWidth) + center;
            Point vertexD = new Point(-halfLength, halfHeight, -halfWidth) + center;
            Point vertexE = new Point(-halfLength, -halfHeight, halfWidth) + center;
            Point vertexF = new Point(halfLength, -halfHeight, halfWidth) + center;
            Point vertexG = new Point(halfLength, halfHeight, halfWidth) + center;
            Point vertexH = new Point(-halfLength, halfHeight, halfWidth) + center;

            decimal minX = Math.Min(vertexA.X, Math.Min(vertexB.X,
                Math.Min(vertexC.X, Math.Min(vertexD.X,
                Math.Min(vertexE.X, Math.Min(vertexF.X, Math.Min(vertexG.X, vertexH.X)))))));
            decimal maxX = Math.Max(vertexA.X, Math.Max(vertexB.X,
                Math.Max(vertexC.X, Math.Max(vertexD.X,
                Math.Max(vertexE.X, Math.Max(vertexF.X, Math.Max(vertexG.X, vertexH.X)))))));
            decimal minY = Math.Min(vertexA.Y, Math.Min(vertexB.Y,
                Math.Min(vertexC.Y, Math.Min(vertexD.Y,
                Math.Min(vertexE.Y, Math.Min(vertexF.Y, Math.Min(vertexG.Y, vertexH.Y)))))));
            decimal maxY = Math.Max(vertexA.Y, Math.Max(vertexB.Y,
                Math.Max(vertexC.Y, Math.Max(vertexD.Y,
                Math.Max(vertexE.Y, Math.Max(vertexF.Y, Math.Max(vertexG.Y, vertexH.Y)))))));
            decimal minZ = Math.Min(vertexA.Z, Math.Min(vertexB.Z,
                Math.Min(vertexC.Z, Math.Min(vertexD.Z,
                Math.Min(vertexE.Z, Math.Min(vertexF.Z, Math.Min(vertexG.Z, vertexH.Z)))))));
            decimal maxZ = Math.Max(vertexA.Z, Math.Max(vertexB.Z,
                Math.Max(vertexC.Z, Math.Max(vertexD.Z,
                Math.Max(vertexE.Z, Math.Max(vertexF.Z, Math.Max(vertexG.Z, vertexH.Z)))))));

            Point vertex1 = new(minX, minY, minZ);
            Point vertex2 = new(maxX, maxY, maxZ);

            return new(vertex1, vertex2);
        }
        private static string Trim(string text, string separator = "\"") =>
            text.Remove(0, separator.Length).Split(separator)[0];

        public static bool operator ==(PhysicsObject a, PhysicsObject b) => a.Equals(b);
        public static bool operator !=(PhysicsObject a, PhysicsObject b) => !(a == b);

        public override string ToString()
        {
            return
                $"ID: {Id}\n" +
                $"{Name};\n" +
                $"Position: {Point.Round(Position, Enviroment.RoundAmount)};\n" +
                $"Rotation: {Point.Round(Rotation, Enviroment.RoundAmount)} deg;\n" +
                $"Scale: {Point.Round(Scale, Enviroment.RoundAmount)};\n" +
                $"Bounds: {Bounds};\n" +
                $"Velocity: {Point.Round(Velocity, Enviroment.RoundAmount)} m/s;\n" +
                $"Gravitational Acceleration: " +
                $"{Math.Round(GravitationalAcceleration, Enviroment.RoundAmount)};\n" +
                $"Kinetic Energy: {Math.Round(KineticEnergy, Enviroment.RoundAmount)};\n" +
                $"Temperature: {Math.Round(Temperature, Enviroment.RoundAmount)} " +
                $"degrees per Kelvin;\n" +
                $"Mass: {Math.Round(Mass, Enviroment.RoundAmount)} kg;\n" +
                $"Volume: {Math.Round(Volume, Enviroment.RoundAmount)} m3;\n" +
                $"Density: {Math.Round(Density, Enviroment.RoundAmount)} kg/m3;\n";
        }
        public override bool Equals(object? obj) => obj is PhysicsObject @object && Id == @object.Id;
        public override int GetHashCode() => HashCode.Combine(Id);

        public enum ForceMode
        {
            /// <summary>
            /// Add an instant force, ignoring its mass.
            /// </summary>
            Impulse,
            /// <summary>
            /// Add an instant force, using its mass.
            /// </summary>
            Acceleration
        }
    }
}
