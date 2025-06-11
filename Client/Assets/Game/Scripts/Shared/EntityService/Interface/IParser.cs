namespace EntityService.Format {
    public enum FileFormat {
        CSV,

    }

    public interface IParser {
        FileFormat Format { get; }
        ESTable ParseFromFile(string filename);
        ESTable ParseFromText(string filename, string text);

    }
}
