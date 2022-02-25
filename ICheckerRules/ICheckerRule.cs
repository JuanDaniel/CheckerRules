using Autodesk.Revit.DB;
using System.Data;

namespace BBI.JD
{
    public interface ICheckerRule
    {
        string Id { get; }
        string Name { get; }
        string Group { get; }
        string Description { get; }
        DataTable Execute(Document document);
    }
}