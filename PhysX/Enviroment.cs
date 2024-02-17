namespace PhysX
{
    public class Enviroment
    {
        /// <summary>
        /// The interval in seconds from the last frame to the current one
        /// </summary>
        public static decimal DeltaTime { get; set; } = 1;
        /// <summary>
        /// Number of decimal places
        /// </summary>
        public static int RoundAmount => 24;

        /// <summary>
        /// All objects on the scene used in calculations
        /// </summary>
        public static List<PhysicsObject> Objects { get; set; } = new();

        private static string? _text;
        private static string? _jsonFilename;
        private static string? _filename;

        public Enviroment(string filename = "Result.txt", string jsonFilename = "Result.json",
            decimal deltaTime = 1)
        {
            _filename = filename;
            _jsonFilename = jsonFilename;
            DeltaTime = deltaTime;
        }

        public static void AddObjects(params PhysicsObject[] objects)
        {
            foreach (var @object in objects) Objects.Add(@object);
        }

        /// <summary>
        /// Adds message to log file
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="consoleOutput">Do log message to console?</param>
        public static void Log(string message, bool consoleOutput = true)
        {
            if (consoleOutput)
                Console.WriteLine(message);
            if (_filename != null)
            {
                _text += message + "\n";
                using StreamWriter writer = new(_filename);
                writer.WriteLine(_text);
            }
        }
        /// <summary>
        /// Adds physics object information to log file
        /// </summary>
        /// <param name="physicsObject">Object to be logged</param>
        public static void Log(PhysicsObject physicsObject) => Log(physicsObject.ToString());

        /// <summary>
        /// Adds physics object information to JSON file
        /// </summary>
        public static void LogJson()
        {
            if (_jsonFilename != null)
            {
                using StreamWriter writer = new(_jsonFilename);
                foreach (var @object in Objects)
                {
                    writer.WriteLine(@object.ToJson());
                }
            }
        }

        /// <summary>
        /// Sets the properties of physics objects from a JSON file
        /// <param name="filename">
        /// Filename/path of JSON file that's properties of physics objects
        /// </param>
        /// </summary>
        public static void LoadJson(string filename)
        {
            using StreamReader reader = new(filename);
            string fullJson = reader.ReadToEnd();
            List<PhysicsObject> objects = new();
            foreach (var item in fullJson.Split('{'))
            {
                if (string.IsNullOrEmpty(item)) continue;
                string json = item.Insert(0, "{");
                objects.Add(PhysicsObject.FromJson(json));
            }
            Objects = objects;
        }

        public override string ToString()
        {
            string[] objects = new string[Objects.Count];
            for (int i = 0; i < Objects.Count; i++)
                if (Objects[i].Name != null) objects[i] = Objects[i]!.Name;

            return
                $"Enviroment;\n" +
                $"Delta time: {DeltaTime};\n" +
                $"Objects: {string.Join(", ", objects)};";
        }
    }
}
