using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;

namespace BBI.JD
{
    public interface ICheckerRule
    {
        string Id { get; }
        string Name { get; }
        string Group { get; }
        List<Dictionary<string, string>> Execute(Document document);
    }
}