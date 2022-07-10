// See https://aka.ms/new-console-template for more information
using CorrectDateOnPhoto;
using CorrectDateOnPhoto.Correctors;


foreach (var file in Directory.GetFiles(args[0]))
{
    Console.WriteLine($"Start processing file: {file}");
    using BaseImageDateCorrector corrector = new SimpleImageDateCorrector(file);

    if(!Directory.Exists( corrector.NewDirectoryName))
        Directory.CreateDirectory( corrector.NewDirectoryName );


    if (!corrector.CorrectDateFor())
    {
        Console.WriteLine($"ERROR. Correcting {Path.GetFileName(file)} file is unnessesary. The file will be moved to Error subdirectory.");
        if (file != null)
        {
            corrector.Dispose();
            File.Move(file, Path.Combine(Path.GetDirectoryName(file)!, "ERROR", Path.GetFileName(file)));
        }
    }
    Console.WriteLine($"File {file} is processed successfully.");
}