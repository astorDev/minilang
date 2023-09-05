public record FileData(string Name, string[] Content)
{
    public static FileData[] Read(string directoryPath, string extension) {
        var fileNames = Directory.GetFiles(directoryPath, "*." + extension);
        return fileNames.Select(rawFilename => {
            var filename = rawFilename.Replace(directoryPath, "").Replace($".{extension}", "").TrimStart('/');
            return new FileData(filename, File.ReadAllLines(rawFilename));
        }).ToArray();
    }
}