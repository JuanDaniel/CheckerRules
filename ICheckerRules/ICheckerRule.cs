using Autodesk.Revit.DB;
using System.Data;

namespace BBI.JD
{
    public interface ICheckerRule
    {
        /// <summary>
        /// String ID to identify the rule, it is suggested to use the GUI format
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Rule name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Group in which the rule will be placed
        /// </summary>
        string Group { get; }

        /// <summary>
        /// Short description about the name of the rule
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Logic of the rule to be executed
        /// </summary>
        /// <param name="document">Revit document</param>
        /// <returns>A <c>DataTable</c> that will compose the resulting excel</returns>
        DataTable Execute(Document document);
    }
}