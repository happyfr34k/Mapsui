using System;

namespace Mapsui.Samples.Common.Maps.Geometries.DynamicLoadGeometries.DataFactory;

public class LabelFactory
{
    static readonly Random _random = new Random();

    public static string CreateLabel()
    {
        var words = new string[] { "Lorem", "Ipsum", "Dolor", "Sit", "Amet", "Consectetur", "Adipiscing", "Elit", "Sed", "Do", "Eiusmod", "Tempor", "Incididunt", "Labore", "Et", "Dolore",
            "Magna", "Aliqua", "Ut", "Enim", "Ad", "Minim", "Veniam", "Quis", "Nostrud", "Exercitation", "Ullamco", "Laboris", "Nisi", "Ut", "Aliquip", "Ex", "Ea", "Commodo", "Consequat",
            "Duis", "Aute", "Irure", "Dolor", "In", "Reprehenderit", "In", "Voluptate", "Velit", "Esse", "Cillum", "Dolore", "Eu", "Fugiat", "Nulla", "Pariatur", "Excepteur", "Sint", "Occaecat",
            "Cupidatat", "Non", "Proident", "Sunt", "In", "Culpa", "Qui", "Officia", "Deserunt", "Mollit", "Anim", "Id", "Est", "Laborum" };
        return words[_random.Next(words.Length)];
    }
}
