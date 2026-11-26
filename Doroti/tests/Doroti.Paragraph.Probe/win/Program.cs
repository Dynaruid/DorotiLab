if (args.Length != 2)
{
    Console.Error.WriteLine("Pass the fixture font directory and output PNG path.");
    return 2;
}

var fontDirectory = args[0];
var output = args[1];
byte[] ReadFont(string name) => File.ReadAllBytes(Path.Combine(fontDirectory, name + ".ttf"));
#if DOROTI_SHAPED_PARAGRAPHS
var result = await ProductParagraphProbe.RunAsync(ReadFont);
#else
var result = ParagraphProbe.Run(ReadFont);
#endif
File.WriteAllBytes(output, result.Png);
Console.WriteLine(result.Receipt);
return 0;
